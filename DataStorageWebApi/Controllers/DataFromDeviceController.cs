using CommonTypeDevice;
using CommonTypeDevice.MeasurumentData;
using DataStorageCore.Models;
using DataStorageCore.Repositories;
using DataStorageWebApi.Models;
using DataStorageWebApi.Services;
using DataStorageWebApi.TaskManager;
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
        private readonly IRepository<Measurement> measurementRepository;
        private readonly IRepository<Archive> archiveRepository;
        private readonly IRepository<EventDict> eventDictRepository;
        private readonly IDataFromDeviceService dataFromDeviceService;
        //private readonly ITaskManager taskManager;
        public DataFromDeviceController(IRepository<Device> deviceRepository,
                                        IRepository<DeviceType> deviceTypeRepository,
                                        IRepository<Event> eventRepository,
                                        IRepository<Measurement> measurementRepository,
                                        IRepository<Archive> archiveRepository,
                                        IRepository<EventDict> eventDictRepository,
                                        IDataFromDeviceService dataFromDeviceService,
                                        ITaskManager taskManager
                                        )
        {
            this.deviceRepository = deviceRepository;
            this.deviceTypeRepository = deviceTypeRepository;
            this.eventRepository = eventRepository;
            this.dataFromDeviceService = dataFromDeviceService;
            this.measurementRepository = measurementRepository;
            this.archiveRepository = archiveRepository;
            this.eventDictRepository = eventDictRepository; 
            // this.taskManager = taskManager;
        }

        [HttpPost]
        public async Task<IActionResult> WriteDataFromDeviceAsync([FromBody] DeviceData deviceData)
        {
            // var task = Task.Run(async () =>
            await dataFromDeviceService.WriteToDbAsync(deviceData, deviceRepository, deviceTypeRepository, eventRepository, measurementRepository, archiveRepository, eventDictRepository);
            // );
            // await task;
            //taskManager.AddTask(task);
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
