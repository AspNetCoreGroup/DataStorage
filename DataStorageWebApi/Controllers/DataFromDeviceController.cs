using CommonTypeDevice;
using CommonTypeDevice.MeasurumentData;
using DataStorageCore.Models;
using DataStorageCore.Repositories;
using DataStorageWebApi.Models;
using DataStorageWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataStorageWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataFromDeviceController : ControllerBase
    {
        private readonly IRepository<Device> deviceRepository;
        private readonly IRepository<Event> eventRepository;
        private readonly IRepository<Measurement> measurementRepository;
        private readonly IRepository<Archive> archiveRepository;
        private readonly IDataFromDeviceService dataFromDeviceService;
        public DataFromDeviceController(IRepository<Device> deviceRepository,
                                        IRepository<Event> eventRepository,
                                        IRepository<Measurement> measurementRepository,
                                        IRepository<Archive> archiveRepository,
                                        IDataFromDeviceService dataFromDeviceService                                        
                                        )
        {
            this.deviceRepository = deviceRepository;
            this.eventRepository = eventRepository;
            this.dataFromDeviceService = dataFromDeviceService;
            this.measurementRepository = measurementRepository;
            this.archiveRepository = archiveRepository;
        }

        [HttpPost]
        public async Task<IActionResult> WriteDataFromDeviceAsync([FromBody] DeviceData deviceData)
        {
            await dataFromDeviceService.WriteToDbAsync(deviceData, deviceRepository, eventRepository, measurementRepository, archiveRepository);
            return Ok();
        }

        [HttpPost("DeviceArchiveById")]
        public async Task<ActionResult<List<MeasurementData>>> DeviceArchiveById([FromBody] GetMeasurementsRequest measurementsRequest)
        {
            var archives = await dataFromDeviceService.GetDeviceArchiveById(measurementsRequest, measurementRepository, archiveRepository);
            return Ok(archives);
        }

        [HttpGet("MeasurumentIdDescription")]
        public async Task<ActionResult<List<MeasurumentIdDescription>>> MeasurumentIdDescription()
        {
            var descriptions = dataFromDeviceService.GetMeasurumentIdDescriptions();
            return Ok(descriptions);
        }
    }
}
