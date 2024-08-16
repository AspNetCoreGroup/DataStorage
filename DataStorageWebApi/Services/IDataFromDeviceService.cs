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
                                         IRepository<Event> eventRepository,
                                         IRepository<Measurement> measurementRepository,
                                         IRepository<Archive> archiveRepository);
        public Task<List<MeasurementData>> GetDeviceArchiveById(GetMeasurementsRequest measurementsRequest,
                                                                IRepository<Measurement> measurementRepository,
                                                                IRepository<Archive> archiveRepository);
    }
}
