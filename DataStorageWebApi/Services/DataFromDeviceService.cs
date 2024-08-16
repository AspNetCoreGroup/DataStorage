using CommonTypeDevice;
using CommonTypeDevice.MeasurumentData;
using DataStorageCore.Models;
using DataStorageCore.Repositories;
using DataStorageWebApi.Models;

namespace DataStorageWebApi.Services
{
    public class DataFromDeviceService : IDataFromDeviceService
    {

        public async Task<bool> WriteToDbAsync(DeviceData deviceData,
                                                IRepository<Device> deviceRepository,
                                                IRepository<Event> eventRepository,
                                                IRepository<Measurement> measurementRepository,
                                                IRepository<Archive> archiveRepository)
        {
            Device? device = await CheckDeviceAsync(deviceData, deviceRepository);

            if (device != null)
            {
                await EventParseAsync(deviceData, device, eventRepository);
                await MeasureParseAsync(deviceData, device, measurementRepository, archiveRepository);
                return true;
            }
            return false;
        }

        public async Task<Device?> CheckDeviceAsync(DeviceData deviceData, IRepository<Device> deviceRepository)
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
                }
            }

            return device;
        }


        public async Task<bool> EventParseAsync(DeviceData deviceData, Device device, IRepository<Event> eventRepository)
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
                        }

                    }
                }
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
