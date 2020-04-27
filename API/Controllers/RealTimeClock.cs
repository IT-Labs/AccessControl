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
        private readonly IGetRealTimeClock _getRealTimeClock;

        public RealTimeClockController(IGetRealTimeClock getRealTimeClock)
        {
            _getRealTimeClock = getRealTimeClock;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            DateTime rtc = await _getRealTimeClock.Execute();
            return rtc.ToString("yyyy-MM-dd HH:mm:ss");            
        }
    }
}