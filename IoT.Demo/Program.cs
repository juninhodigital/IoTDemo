using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace IoT.Demo
{
    class Program
    {
        #region| Fields |

        static string iotHubUri = string.Empty;
        static string deviceId = string.Empty;
        static string deviceKey = string.Empty;
        #endregion

        #region| Constructor |

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            LoadConfiguration();
            Initialize();
        }

        #endregion

        #region| Methods |

        /// <summary>
        /// Load appsettings configuration items
        /// </summary>
        private static void LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            iotHubUri = configuration["uri"];
            deviceId  = configuration["device.id"];
            deviceKey = configuration["device.key"];
        }

        private static void Initialize()
        {
            var random = new Random();

            var deviceClient = DeviceClient.Create(iotHubUri, AuthenticationMethodFactory.CreateAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey), TransportType.Mqtt);

            try
            {
                for(int i = 0; i < 20; i++)
                {
                    int temperature = random.Next(15, 80);
                    int pressure    = random.Next(200, 300);
                    int dId         = random.Next(1, 5);

                    var info = new Event()
                    {
                        TimeStamp1  = DateTime.UtcNow,
                        DeviceId    = deviceId,
                        Temperature = temperature.ToString(),
                        Pressure    = pressure.ToString(),
                    };

                    var serializedString = JsonConvert.SerializeObject(info);
                    var data = new Message(Encoding.UTF8.GetBytes(serializedString));

                    // Enviar dados para o Hub
                    var task = Task.Run(async () => await deviceClient.SendEventAsync(data));

                    Print($@"[{info.TimeStamp1}] Device-ID: {dId.ToString()} Temperature: {info.Temperature.ToString()} deg C, Pressure: {info.Pressure} ##################################".Replace("    ", ""));

                    Thread.Sleep(3000);
                }
            }
            catch(Exception e)
            {
                var message = e.Message + e.InnerException?.Message;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ReadLine();
            }
        }

        /// <summary>
        ///  Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="message">message</param>
        private static void Print(string message)
        {
            Console.WriteLine(message);
        }

        #endregion
    }
}