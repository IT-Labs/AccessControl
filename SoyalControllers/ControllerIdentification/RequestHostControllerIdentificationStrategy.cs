using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace SoyalControllers.ControllerIdentification
{
    /// <summary>
    /// Controller Identification interface
    /// </summary>
    public interface IControllerIdentificationService
    {
        Task<IControllerAddress> GetControllerAddress();
    }


    /// <summary>
    /// Identify the controller by the HTTP Request host headers
    /// </summary>
    public class ControllerIdentificationService : IControllerIdentificationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IControllerAddress _controllerAddress;

        public ControllerIdentificationService(IHttpContextAccessor httpContextAccessor, IControllerAddress controllerAddress)
        {
            _httpContextAccessor = httpContextAccessor;
            _controllerAddress = controllerAddress;
        }

        public async Task<IControllerAddress> GetControllerAddress()
        {
            if(_httpContextAccessor.HttpContext.Request == null)
            {
                throw new Exception("Request host is not found");
            }

            if(_httpContextAccessor.HttpContext.Request.Headers["IpAddress"].Count != 1)
            {
                throw new Exception("Request host header \"IpAddress\" is not found");
            }

            if(_httpContextAccessor.HttpContext.Request.Headers["Port"].Count != 1)
            {
                throw new Exception("Request host header \"Port\" is not found");
            }

            if(_httpContextAccessor.HttpContext.Request.Headers["NodeId"].Count != 1)
            {
                throw new Exception("Request host header \"NodeId\" is not found");
            }

            _controllerAddress.IpAddress = _httpContextAccessor.HttpContext.Request.Headers["IpAddress"];
            _controllerAddress.Port = int.Parse(_httpContextAccessor.HttpContext.Request.Headers["Port"]);
            _controllerAddress.NodeId = int.Parse(_httpContextAccessor.HttpContext.Request.Headers["NodeId"]);

            return await Task.FromResult(_controllerAddress);
        }
    }
}