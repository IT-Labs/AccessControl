using System;
using System.Globalization;
using Communication.Sockets;
using System.Threading.Tasks;

namespace SoyalControllers
{
    public interface IGetDeviceStatus
    {
        Task<byte[]> Execute();
    }

    public class GetDeviceStatus : Base, IGetDeviceStatus
    { 
        private readonly byte _head = Byte.Parse("7E", NumberStyles.HexNumber);
        private readonly byte _cmd = Byte.Parse("18", NumberStyles.HexNumber);


        public GetDeviceStatus(IControllerAddress controllerAddress) : base (controllerAddress)
        {
        }

        /// <summary>
        /// Return device status from the selected node
        /// </summary>
        /// <returns>Device status as byte[]</returns>
        public async Task<byte[]> Execute()
        {
            byte[] msg = new byte[6]; // head, length, destID, cmd, xor, sum

            msg[0] = _head;
            msg[1] = Convert.ToByte(msg.Length - 2); // length of stream - exclude head and length params
            msg[2] = base._destID;
            msg[3] = _cmd;

            byte[] XORandSUM = base.CalculateXORandSUM(msg);

            msg[msg.Length - 2] = XORandSUM[0];
            msg[msg.Length - 1] = XORandSUM[1];

            byte[] response = await new AsyncClient().StartClient(base._ip, base._port, msg).ConfigureAwait(false);

            if(!IsValidResponse(response))
            {
                throw new Exception("Invalid response from the card reader");
            }

            // The return value from the device
            return response;
        }
    }
}