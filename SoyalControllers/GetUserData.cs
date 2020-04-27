using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Text;

using Communication.Sockets;
using SoyalControllers.ControllerIdentification;

namespace SoyalControllers
{
    public interface IGetUserData
    {
        Task<string> Execute(int[,] cards);
    }

    public class GetUserData : Base, IGetUserData
    {
        private readonly byte _head = Byte.Parse("7E", NumberStyles.HexNumber);
        private readonly byte _cmd = Byte.Parse("87", NumberStyles.HexNumber);

        public GetUserData(IControllerIdentificationService controllerIdentificationService, IAsyncClient asyncClient)
        : base (controllerIdentificationService, asyncClient)
        {
        }

        /// <summary>
        /// Returns card content for selected node
        /// Input values List<addressHigh, addressLow>
        /// </summary>
        /// <returns>
        /// Return content of the cards for selected node ID
        /// NodeId = "Destination ID" in the documentation
        /// </returns>
        public async Task<string> Execute(int[,] cards)
        {
            byte[] msg = new byte[7 + cards.Length * 2]; // Each card element consists of 2 bytes

            msg[0] = _head;
            msg[1] = Convert.ToByte(msg.Length - 2);
            msg[2] = Convert.ToByte(base._controllerAddress.NodeId);
            msg[3] = _cmd;

            for(int i = 0; i < cards.Length / 2; i++)
            {
                byte[] cardBytesLow = BitConverter.GetBytes(cards[i,0]);
                byte[] cardBytesHigh = BitConverter.GetBytes(cards[i,1]);

                Array.Resize(ref cardBytesLow, 2);
                Array.Resize(ref cardBytesHigh, 2);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(cardBytesLow);
                    Array.Reverse(cardBytesHigh);
                }

                int index = (i * 4) + 4;

                msg[index] = cardBytesLow[0];
                msg[index + 1] = cardBytesLow[1];

                msg[index + 2] = cardBytesHigh[0];
                msg[index + 3] = cardBytesHigh[1];
            }

            msg[msg.Length - 3] = Convert.ToByte(cards.Length / 2);

            byte[] XORandSUM = base.CalculateXORandSUM(msg);
            
            msg[msg.Length - 2] = XORandSUM[0];
            msg[msg.Length - 1] = XORandSUM[1];

            byte[] response = await base._asyncClient.StartClient(base._controllerAddress.IpAddress, base._controllerAddress.Port, msg).ConfigureAwait(false);

            if(!IsValidResponse(response))
            {
                throw new Exception("Invalid response from the card reader");
            }

            // The return value should be the Real Time Clock read from the device
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < response.Length; i++)
            {
                sb.AppendLine(string.Format(
                    "byte[{0}]      hex: {1}    int: {2}",
                    (i.ToString().Length > 1) ? i.ToString() : "0" + i.ToString(),
                    response[i].ToString("X4"),
                    Convert.ToInt32(response[i])
                ));
            }

            return sb.ToString();
        }
    }
}