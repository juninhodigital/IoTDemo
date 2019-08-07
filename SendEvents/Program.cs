using System;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace IoT.Demo.SendEvents
{
    class Program
    {
        #region| Fields |

        static string iotHubUri = string.Empty;
        static string deviceId  = string.Empty;
        static string deviceKey = string.Empty;
        #endregion

        #region| Constructor |

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
            iotHubUri = ConfigurationManager.AppSettings["IOT.HUB.URI"];
            deviceId  = ConfigurationManager.AppSettings["IOT.HUB.DEVICE.ID"];
            deviceKey = ConfigurationManager.AppSettings["IOT.HUB.DEVICE.KEY"];
        }

        private static void Initialize()
        {
            var random = new Random();

            int temp = 0,
                pressure = 0,
                dId = 0;

            // Create a device client
            var deviceClient = DeviceClient.Create(iotHubUri, AuthenticationMethodFactory.CreateAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey), TransportType.Http1);

            try
            {
                for(int i = 0; i < 20; i++)
                {
                    temp     = random.Next(35, 55);
                    pressure = random.Next(200, 300);
                    dId      = random.Next(1, 5);

                    var info = new Event()
                    {
                        TimeStamp1  = DateTime.UtcNow,
                        DeviceId    = deviceId,
                        Temperature = temp.ToString(),
                        Pressure    = pressure.ToString(),
                    };

                    Print("Enter temperature (press CTRL+Z to exit):");

                    var readtemp = Console.ReadLine();

                    if(readtemp != null)
                    {
                        info.Temperature = readtemp;
                    }

                    var serializedString = JsonConvert.SerializeObject(info);
                    var data             = new Message(Encoding.UTF8.GetBytes(serializedString));

                    // Send the metric to Event Hub
                    var task = Task.Run(async () => await deviceClient.SendEventAsync(data));

                    //Write the values to your debug console                            
                    Print($"DeviceID: {dId.ToString()}");
                    Print($"Timestamp: {info.TimeStamp1}");
                    Print($"Temperature: {info.Temperature.ToString()} deg C");
                    Print($"Pressure: {info.Pressure}");
                    Print($"------------------------------");
                    
                    //Message data = new Message(Encoding.UTF8.GetBytes("myDeviceId2,19,Ban-EGL,14804022344554"));
                    //// Send the metric to Event Hub
                    //var task = Task.Run(async () => await deviceClient.SendEventAsync(data));

                    Task.Delay(3000).Wait();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
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
