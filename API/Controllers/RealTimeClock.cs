using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SoyalControllers;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RealTimeClockController : ControllerBase
    {
        private readonly IControllerAddress _controllerAddress; 
        private readonly IGetRealTimeClock _getRealTimeClock;       
        private readonly ILogger<UserDataController> _logger;

        public RealTimeClockController(
            IControllerAddress controllerAddress,
            IGetRealTimeClock getRealTimeClock,
            ILogger<UserDataController> logger
        )
        {
            _controllerAddress = controllerAddress;
            _getRealTimeClock = getRealTimeClock;
            _logger = logger;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            _controllerAddress.IpAddress = "192.168.5.26";
            _controllerAddress.Port = 1621;
            _controllerAddress.NodeId = 3;

            DateTime rtc = await _getRealTimeClock.Execute();
            return rtc.ToString("yyyy-MM-dd HH:mm:ss");            
        }
    }
}