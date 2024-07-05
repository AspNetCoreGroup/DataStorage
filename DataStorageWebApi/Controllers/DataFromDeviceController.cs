using CommonTypeDevice;
using DataStorageCore.Models;
using DataStorageCore.Repositories;
using DataStorageDataAccess.DataBaseContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataStorageWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataFromDeviceController : ControllerBase
    {
        private readonly IRepository<Device> deviceRepository;
       

        public DataFromDeviceController(IRepository<Device> deviceRepository)
        {
            this.deviceRepository = deviceRepository;

        }

        [HttpPost]
        public async  Task<IActionResult> GetDataFromDeviceAsync([FromBody] DeviceData deviceData)
        {
            var device = await deviceRepository.GetAllAsync();
            return Ok();
        }
    }
}
