using System;
using System.Globalization;
using System.Threading.Tasks;

using Communication.Sockets;
using SoyalControllers.ControllerIdentification;

namespace SoyalControllers
{
    public interface IGetRealTimeClock
    {
        Task<DateTime> Execute();
    }

    public class GetRealTimeClock : Base, IGetRealTimeClock
    { 
        private readonly byte _head = Byte.Parse("7E", NumberStyles.HexNumber);
        private readonly byte _cmd = Byte.Parse("24", NumberStyles.HexNumber);

        public GetRealTimeClock(IControllerIdentificationService controllerIdentificationService, IAsyncClient asyncClient)
        : base (controllerIdentificationService, asyncClient)
        {
        }

        /// <summary>
        /// Return current clock time from the selected node
        /// </summary>
        /// <returns>
        /// Real Time Clock in a DateTime format
        /// </returns>
        public async Task<DateTime> Execute()
        {
            byte[] msg = new byte[6]; // head, length, destID, cmd, xor, sum

            msg[0] = _head;
            msg[1] = Convert.ToByte(msg.Length - 2); // length of stream - exclude head and length params
            msg[2] = Convert.ToByte(base._controllerAddress.NodeId);
            msg[3] = _cmd;

            byte[] XORandSUM = base.CalculateXORandSUM(msg);

            msg[msg.Length - 2] = XORandSUM[0];
            msg[msg.Length - 1] = XORandSUM[1];

            byte[] response = await base._asyncClient.StartClient(base._controllerAddress.IpAddress, base._controllerAddress.Port, msg).ConfigureAwait(false);

            if(!IsValidResponse(response))
            {
                throw new Exception("Invalid response from the card reader");
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for(int i = 0; i < response.Length; i++)
            {   
                sb.AppendLine(string.Format(
                    "byte[{0}]      hex: {1}    int: {2}",
                    (i.ToString().Length > 1) ? i.ToString() : "0" + i.ToString(),
                    response[i].ToString("X2"),
                    Convert.ToInt32(response[i])
                ));
            }

            // The return value is the Real Time Clock read from the device
            return new DateTime(
                Convert.ToInt32(response[11] + 2000),   // Year (+ 2000 since the returned year is 2 digit)
                                                        // (if this code lasts 80 years, I'll fix it then)
                Convert.ToInt32(response[10]),          // Month
                Convert.ToInt32(response[9]),           // Day
                Convert.ToInt32(response[7]),           // Hour
                Convert.ToInt32(response[6]),           // Min
                Convert.ToInt32(response[5])            // Sec
                );
        }
    }
}