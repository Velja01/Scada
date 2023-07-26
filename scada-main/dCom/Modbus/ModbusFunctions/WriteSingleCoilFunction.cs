using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            var Command_Parameters = this.CommandParameters as ModbusWriteCommandParameters;
            var message = new byte[12];
            message[0] = (byte)(Command_Parameters.TransactionId >> 8);
            message[1] = (byte)Command_Parameters.TransactionId;
            message[2] = (byte)(Command_Parameters.ProtocolId >> 8);
            message[3] = (byte)Command_Parameters.ProtocolId;
            message[4] = (byte)(Command_Parameters.Length >> 8);
            message[5] = (byte)Command_Parameters.Length;
            message[6] = Command_Parameters.UnitId;
            message[7] = Command_Parameters.FunctionCode;
            message[8] = (byte)(Command_Parameters.OutputAddress >> 8);
            message[9] = (byte)Command_Parameters.OutputAddress;
            message[10] = (byte)(Command_Parameters.Value >> 8);
            message[11] = (byte)Command_Parameters.Value;

            return message;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            var Command_Parameters = this.CommandParameters as ModbusWriteCommandParameters;
            var tmp = new Dictionary<Tuple<PointType, ushort>, ushort>();
            var tuple = new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, Command_Parameters.OutputAddress);
            tmp.Add(tuple, response[11]);
            return tmp;
        }
    }
}