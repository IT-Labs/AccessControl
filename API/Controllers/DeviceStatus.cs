using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;

using SoyalControllers;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceStatusController : ControllerBase
    {
        private readonly IControllerAddress _controllerAddress; 
        private readonly IGetDeviceStatus _getDeviceStatus;       
        private readonly ILogger<UserDataController> _logger;

        public DeviceStatusController(
            IControllerAddress controllerAddress,
            IGetDeviceStatus getDeviceStatus,
            ILogger<UserDataController> logger
        )
        {
            _controllerAddress = controllerAddress;
            _getDeviceStatus = getDeviceStatus;
            _logger = logger;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            _controllerAddress.IpAddress = "192.168.5.26";
            _controllerAddress.Port = 1621;
            _controllerAddress.NodeId = 3;

            byte[] response = await _getDeviceStatus.Execute();

            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < response.Length; i++)
            {   
                sb.AppendLine(string.Format(
                    "byte[{0}]      hex: {1}    int: {2}",
                    (i.ToString().Length > 1) ? i.ToString() : "0" + i.ToString(),
                    response[i].ToString("X2"),
                    Convert.ToInt32(response[i])
                ));
            }

            return sb.ToString();
        }
    }
}