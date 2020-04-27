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
        private readonly IGetUserData _getUserData;       
        public UserDataController(IGetUserData getUserData)
        {
            _getUserData = getUserData;
        }

        [HttpGet]
        public async Task<string> Get([FromQuery] int[,] cards)
        {
            return await _getUserData.Execute(cards);
        }
    }
}