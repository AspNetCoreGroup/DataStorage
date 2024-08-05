using CommonTypeDevice;
using DataStorageCore.Models;
using DataStorageCore.Repositories;
using DataStorageWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataStorageWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataFromDeviceController : ControllerBase
    {
        private readonly IRepository<Device> deviceRepository;
        private readonly IRepository<DeviceType> deviceTypeRepository;
        private readonly IRepository<Event> eventRepository;
        private readonly IDataFromDeviceService dataFromDeviceService;
        public DataFromDeviceController(IRepository<Device> deviceRepository,
                                        IRepository<DeviceType> deviceTypeRepository,
                                        IRepository<Event> eventRepository,
                                        IDataFromDeviceService dataFromDeviceService)
        {
            this.deviceRepository = deviceRepository;
            this.deviceTypeRepository = deviceTypeRepository;
            this.eventRepository = eventRepository;
            this.dataFromDeviceService = dataFromDeviceService;
        }

        [HttpPost]
        public async Task<IActionResult> WriteDataFromDeviceAsync([FromBody] DeviceData deviceData)
        {
            await dataFromDeviceService.WriteToDbAsync(deviceData, deviceRepository, eventRepository);
            return Ok();
        }
    }
}
