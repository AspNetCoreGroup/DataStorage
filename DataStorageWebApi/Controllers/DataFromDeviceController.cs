using CommonTypeDevice;
using DataStorageCore.Models;
using DataStorageCore.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DataStorageWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataFromDeviceController : ControllerBase
    {
        private readonly IRepository<Device> deviceRepository;
        private readonly IRepository<DeviceType> deviceTypeRepository;

        public DataFromDeviceController(IRepository<Device> deviceRepository, IRepository<DeviceType> deviceTypeRepository)
        {
            this.deviceRepository = deviceRepository;
            this.deviceTypeRepository = deviceTypeRepository;
        }

        [HttpPost]
        public async Task<IActionResult> GetDataFromDeviceAsync([FromBody] DeviceData deviceData)
        {
            string devNum = "";
            int devType;
            if (deviceData != null)
            {
                foreach (var devProp in deviceData?.Properties)
                {
                    if (devProp.Name == "SN")
                    {
                        devNum = devProp.Value;
                    }

                }

                if (devNum == "")
                {
                    BadRequest("SN must not be null or empty");
                }

                var device = await deviceRepository.FirstOrDefaultAsync(x => x.SerialNumber == devNum);
                if (device == null)
                {
                    await deviceRepository.AddAsync(new Device { SerialNumber = devNum, NetAdress = "123", DeviceTypeId = 1 });
                }


            }
            return Ok();
        }
    }
}
