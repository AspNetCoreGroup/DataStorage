using CommonTypeDevice;
using DataStorageCore.Models;
using DataStorageCore.Repositories;

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
                foreach (var deviceMeasurement in deviceData.Measurements)
                {
                    var measureFromDB = await measurementRepository.FirstOrDefaultAsync(x => x.DeviceId == device.Id && x.MeasurumentDictId == deviceMeasurement.MeasurumentId);
                    if (measureFromDB == null)
                    {
                        await measurementRepository.AddAsync(new Measurement
                        {
                            DeviceId = device.Id,
                            MeasurumentDictId = deviceMeasurement.MeasurumentId
                        });
                        measureFromDB = await measurementRepository.FirstOrDefaultAsync(x => x.DeviceId == device.Id && x.MeasurumentDictId == deviceMeasurement.MeasurumentId);
                    }

                    if (measureFromDB != null)
                    {
                        await archiveRepository.AddAsync(new Archive
                        {
                            MeasurumentId = measureFromDB.Id,
                            Dt = deviceMeasurement.DateTime,
                            Value = deviceMeasurement.Value
                        });
                    }

                }
            }

            return true;
        }
    }
}
