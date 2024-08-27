using CommonTypeDevice;
using CommonTypeDevice.MeasurumentData;
using CommonTypeDevice.RMQMessages;
using DataStorageCore.Models;
using DataStorageCore.Repositories;
using DataStorageWebApi.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace DataStorageWebApi.Services
{
    public class DataFromDeviceService : IDataFromDeviceService
    {
        private string RMQHost;
        private readonly ILogger logger;
        public DataFromDeviceService(ILogger<DataFromDeviceService> logger)
        {
            RMQHost = Environment.GetEnvironmentVariable("ConnectionStrings__RMQHost") ?? "localhost";


            this.logger = logger;
            logger.LogInformation("RMQHost: " + RMQHost + "!!!!!!!");
        }

        public async Task<bool> WriteToDbAsync(DeviceData deviceData,
                                                IRepository<Device> deviceRepository,
                                                IRepository<DeviceType> deviceTypeRepository,
                                                IRepository<Event> eventRepository,
                                                IRepository<Measurement> measurementRepository,
                                                IRepository<Archive> archiveRepository,
                                                IRepository<EventDict> eventDictRepository)
        {
            Device? device = await CheckDeviceAsync(deviceData, deviceRepository, deviceTypeRepository);

            if (device != null)
            {
                await EventParseAsync(deviceData, device, eventRepository, eventDictRepository);
                await MeasureParseAsync(deviceData, device, measurementRepository, archiveRepository);
                return true;
            }
            return false;
        }

        public async Task<Device?> CheckDeviceAsync(DeviceData deviceData, IRepository<Device> deviceRepository, IRepository<DeviceType> deviceTypeRepository)
        {
            string devNum = "";
            int devType = 0;
            string devAddress = "";
            Device? device = null;
            if (deviceData.Properties != null)
            {
                foreach (var devProp in deviceData.Properties)
                {
                    if (devProp.Name == "SN")
                    {
                        devNum = devProp.Value;
                        if (devNum == "")
                        {
                            throw new ArgumentException("Serial number must not be null or empty");
                        }
                    }

                    if (devProp.Name == "Address")
                    {
                        devAddress = devProp.Value;
                    }

                    if (devProp.Name == "DeviceType")
                    {
                        try
                        {
                            devType = int.Parse(devProp.Value);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("DevType parse error!");
                        }

                    }
                }

                device = await deviceRepository.FirstOrDefaultAsync(x => x.SerialNumber == devNum);
                if (device == null)
                {
                    await deviceRepository.AddAsync(new Device
                    {
                        SerialNumber = devNum,
                        NetAdress = devAddress,
                        DeviceTypeId = devType
                    });

                    device = await deviceRepository.FirstOrDefaultAsync(x => x.SerialNumber == devNum);
                    if (device != null)
                    {
                        var deviceType = await deviceTypeRepository.GetByIdAsync(device.DeviceTypeId);
                        device.DeviceType = deviceType;
                        NewDeviceMessage(device);
                    }
                }
                else
                {
                    var deviceType = await deviceTypeRepository.GetByIdAsync(device.DeviceTypeId);
                    device.DeviceType = deviceType;
                }
            }

            return device;
        }

        public bool NewDeviceMessage(Device? device)
        {
            if (device is null)
            {
                return false;
            }
            var factory = new ConnectionFactory()
            {
                HostName = RMQHost,
                Port = 5672,
                UserName = "guest",
                Password = "guest",
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                DeviceMessage deviceMessage = new DeviceMessage
                {
                    DeviceId = device.Id,
                    DeviceType = device.DeviceType.Name,
                    NetAddress = device.NetAdress,
                    SerialNumber = device.SerialNumber
                };
                string jsonString = JsonSerializer.Serialize(deviceMessage, new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                });
                channel.QueueDeclare(queue: "FromDataStorage-queue",
                                                 durable: true,
                                                 autoDelete: false,
                                                 exclusive: false,
                                                 arguments: null);

                var body = Encoding.UTF8.GetBytes(jsonString);

                channel.BasicPublish(exchange: "", routingKey: "FromDataStorage-queue", basicProperties: null, body: body);

                logger.LogInformation("New device message sended!");
            }
            return true;
        }



        public async Task<bool> EventParseAsync(DeviceData deviceData, Device device, IRepository<Event> eventRepository, IRepository<EventDict> eventDictRepository)
        {
            if (deviceData.DeviceEvents != null)
            {
                foreach (var deviceEvent in deviceData.DeviceEvents)
                {
                    if (deviceEvent.EventParameters.Count > 0)
                    {
                        foreach (var eventParam in deviceEvent.EventParameters)
                        {

                            await eventRepository.AddAsync(new Event
                            {
                                DeviceId = device.Id,
                                EventDictId = eventParam.Key,
                                Dt = deviceEvent.DateTime
                            });

                            var eventFromDb = await eventRepository.FirstOrDefaultAsync(x => x.DeviceId == device.Id &&
                                                                                        x.EventDictId == eventParam.Key &&
                                                                                        x.Dt == deviceEvent.DateTime);


                            if (eventFromDb != null)
                            {
                                var eventDict = await eventDictRepository.GetByIdAsync(eventFromDb.EventDictId);
                                if (eventDict != null)
                                {
                                    eventFromDb.Device = device;
                                    eventFromDb.EventDict = eventDict;
                                    NewEventMessage(eventFromDb);
                                }

                            }

                        }

                    }
                }
            }
            return true;
        }

        public bool NewEventMessage(Event? eventFromDb)
        {
            if (eventFromDb is null)
            {
                return false;
            }
            var factory = new ConnectionFactory()
            {
                HostName = RMQHost,
                Port = 5672,
                UserName = "guest",
                Password = "guest",
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                EventMessage eventMessage = new EventMessage
                {
                    EventName = eventFromDb.EventDict.Name,
                    Dt = eventFromDb.Dt,
                    DeviceId = eventFromDb.DeviceId,
                    DeviceSerial = eventFromDb.Device.SerialNumber,
                    DeviceType = eventFromDb.Device.DeviceType.Name



                };
                string jsonString = JsonSerializer.Serialize(eventMessage, new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                });
                channel.QueueDeclare(queue: "FromDataStorage-queue",
                                                 durable: true,
                                                 autoDelete: false,
                                                 exclusive: false,
                                                 arguments: null);

                var body = Encoding.UTF8.GetBytes(jsonString);

                channel.BasicPublish(exchange: "", routingKey: "FromDataStorage-queue", basicProperties: null, body: body);

                logger.LogInformation("New event message sended!");
            }
            return true;
        }
        public async Task<bool> MeasureParseAsync(DeviceData deviceData, Device device, IRepository<Measurement> measurementRepository, IRepository<Archive> archiveRepository)
        {
            if (deviceData.Measurements != null)
            {
                var measurementDistinctList = deviceData.Measurements.DistinctBy(x => x.MeasurumentId).ToList();
                foreach (var measurementDistinct in measurementDistinctList)
                {
                    var measureFromDB = await measurementRepository.FirstOrDefaultAsync(x => x.DeviceId == device.Id && x.MeasurumentDictId == measurementDistinct.MeasurumentId);
                    if (measureFromDB == null)
                    {
                        await measurementRepository.AddAsync(new Measurement
                        {
                            DeviceId = device.Id,
                            MeasurumentDictId = measurementDistinct.MeasurumentId
                        });
                        measureFromDB = await measurementRepository.FirstOrDefaultAsync(x => x.DeviceId == device.Id && x.MeasurumentDictId == measurementDistinct.MeasurumentId);
                    }

                    var measList = deviceData.Measurements.Where(x => x.MeasurumentId == measurementDistinct.MeasurumentId).ToList();
                    if (measList.Count > 0)
                    {
                        List<Archive> archivelist = new();
                        foreach (var meas in measList)
                        {
                            archivelist.Add(new Archive
                            {
                                MeasurumentId = measureFromDB.Id,
                                Dt = meas.DateTime,
                                Value = meas.Value
                            });
                        }

                        await archiveRepository.AddRangeAsync(archivelist);
                    }


                }


            }

            return true;
        }

        public async Task<List<MeasurementData>> GetDeviceArchiveById(GetMeasurementsRequest measurementsRequest, IRepository<Measurement> measurementRepository,
                                                                              IRepository<Archive> archiveRepository)
        {
            List<MeasurementData> measurementDatas = new();

            var measuruments = await measurementRepository.WhereAsync(x => x.DeviceId == measurementsRequest.DeviceID);
            foreach (var meas in measuruments)
            {
                var archives = await archiveRepository.WhereAsync(x => (x.MeasurumentId == meas.Id) &&
                                                                       (x.Dt < measurementsRequest.MaxDate) &&
                                                                       (x.Dt > measurementsRequest.MinDate));
                foreach (var arc in archives)
                {
                    measurementDatas.Add(new MeasurementData
                    {
                        MeasurumentId = arc.Measurument.MeasurumentDictId,
                        DateTime = arc.Dt,
                        Value = arc.Value
                    });
                }

            }
            return measurementDatas;
            //return await measurementRepository.WhereAsync(x=>x.DeviceId == id);
        }

        public List<MeasurumentIdDescription> GetMeasurumentIdDescriptions()
        {
            List<MeasurumentIdDescription> measurumentIdDescriptions = new();

            measurumentIdDescriptions.Add(new MeasurumentIdDescription
            {
                MeasurumentId = 1,
                MeasurumentShortDescription = "A+",
                MeasurumentDescription = "Энергия активная прямого направления"
            });

            measurumentIdDescriptions.Add(new MeasurumentIdDescription
            {
                MeasurumentId = 2,
                MeasurumentShortDescription = "A-",
                MeasurumentDescription = "Энергия активная обратного отправления"
            });

            measurumentIdDescriptions.Add(new MeasurumentIdDescription
            {
                MeasurumentId = 3,
                MeasurumentShortDescription = "R+",
                MeasurumentDescription = "Энергия реактивная прямого направления"
            });

            measurumentIdDescriptions.Add(new MeasurumentIdDescription
            {
                MeasurumentId = 4,
                MeasurumentShortDescription = "R-",
                MeasurumentDescription = "Энергия реактивная обратного отправления"
            });
            return measurumentIdDescriptions;
        }
    }
}
