using CommonTypeDevice;
using CommonTypeDevice.MeasurumentData;
using DataStorageCore.Models;
using DataStorageCore.Repositories;
using DataStorageWebApi.Models;

namespace DataStorageWebApi.Services
{
    public interface IDataFromDeviceService
    {
        public Task<bool> WriteToDbAsync(DeviceData deviceData,
                                         IRepository<Device> deviceRepository,
                                         IRepository<DeviceType> deviceTypeRepository,
                                         IRepository<Event> eventRepository,
                                         IRepository<Measurement> measurementRepository,
                                         IRepository<Archive> archiveRepository,
                                         IRepository<EventDict> eventDictRepository);
        public Task<List<MeasurementData>> GetDeviceArchiveById(GetMeasurementsRequest measurementsRequest,
                                                                IRepository<Measurement> measurementRepository,
                                                                IRepository<Archive> archiveRepository);
        public List<MeasurumentIdDescription> GetMeasurumentIdDescriptions();
    }
}
