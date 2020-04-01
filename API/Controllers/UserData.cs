using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SoyalControllers;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserDataController : ControllerBase
    {
        private IControllerAddress _controllerAddress; 
        private readonly IGetUserData _getUserData;       
        private readonly ILogger<UserDataController> _logger;
        public UserDataController(
            IControllerAddress controllerAddress,
            IGetUserData getUserData,
            ILogger<UserDataController> logger
            )
        {
            _controllerAddress = controllerAddress;
            _getUserData = getUserData;
            _logger = logger;
        }

        [HttpGet]
        public async Task<string> Get([FromQuery] ControllerAddress ctrAddress)
        {
            // _controllerAddress = ctrAddress;

            _controllerAddress.IpAddress = "192.168.5.26";
            _controllerAddress.Port = 1621;
            _controllerAddress.NodeId = 3;

            int[,] cards = new int[,]{
                {00082, 24864}, // ile
                {00082, 24734}, // spase
                {00202, 57228}  // mise
            };

            return await _getUserData.Execute(cards);
        }
    }
}