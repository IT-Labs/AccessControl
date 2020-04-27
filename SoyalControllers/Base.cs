using System;
using System.Globalization;
using System.Text;

using SoyalControllers.ControllerIdentification;
using Communication.Sockets;

namespace SoyalControllers
{
    public abstract class Base
    {
        protected readonly IControllerAddress _controllerAddress;
        protected readonly IAsyncClient _asyncClient;

        protected Base (IControllerIdentificationService controllerIdentificationService, IAsyncClient asyncClient)
        {
            _controllerAddress = controllerIdentificationService.GetControllerAddress().GetAwaiter().GetResult();
            _asyncClient = asyncClient;
        }

        /// <summary>
        /// Calculates:
        /// XOR - to XOR each byte from Destination ID to Data with 0xFF as first argument
        /// SUM - to SUM each byte from Destination ID to XOR. If the SUM is greater than 0xFF, it should keep the low byte.
        /// </summary>
        /// <param name="input">The complete byte[]</param>
        /// <returns>byte[]{XOR, SUM}</returns>
        internal byte[] CalculateXORandSUM(byte[] input)
        {
            byte zero = Byte.Parse("FF", NumberStyles.HexNumber);

            // Exclude first 2 elements (head and length) from the toCalculate array
            byte xor = (byte)(zero ^ input[2]);
            byte sum = input[2];

            // Exclude last 2 elements as they are reserved for XOR and SUM
            for(int i = 3; i < input.Length - 2; i++)
            {
                xor ^= input[i];
                sum += input[i];
            }

            sum += xor;

            return new byte[]{xor, sum};
        }


        /// <summary>
        /// Validates whether the response returned from the card reader is valid or not
        /// </summary>
        /// <param name="response">The response as byte[]</param>
        /// <returns></returns>
        internal bool IsValidResponse(byte[] response)
        {
            // First 2 bytes are ommited (head and length)
            // Last 2 bytes are XOR and SUM
            // The response shoud be validated against last 2 bytes XOR and SUM

            byte[] calculatedXORandSUM = CalculateXORandSUM(response);

            // Compare calculated XOR and SUM vs retunerd XOR and SUM
            return (
                response[response.Length - 2] == calculatedXORandSUM[0] && // Compare XORs
                response[response.Length - 1] == calculatedXORandSUM[1] // Compare SUMs
                );
        }

        /// <summary>
        /// HELPER FUNCTION
        /// Converts byte[] to string in format"
        /// byte[index]     hex: {hex value of byte}    int: {int value of byte}
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>multiline string</returns>
        internal string ConvertToString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.AppendLine(string.Format(
                    "byte[{0}]      hex: {1}    int: {2}",
                    (i.ToString().Length > 1) ? i.ToString() : "0" + i.ToString(),
                    bytes[i].ToString("X2"),
                    Convert.ToInt32(bytes[i])
                ));
            }

            return sb.ToString();
        }
    }
}