using CommonTypeDevice;
using DataStorageCore.Models;
using DataStorageCore.Repositories;

namespace DataStorageWebApi.Services
{
    public interface IDataFromDeviceService
    {
        public Task<bool> WriteToDbAsync(DeviceData deviceData,
                                         IRepository<Device> deviceRepository,
                                         IRepository<Event> eventRepository,
                                         IRepository<Measurement> measurementRepository,
                                         IRepository<Archive> archiveRepository);
    }
}
