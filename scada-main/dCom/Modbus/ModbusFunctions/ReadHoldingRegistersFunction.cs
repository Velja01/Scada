using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read holding registers functions/requests.
    /// </summary>
    public class ReadHoldingRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            var Command_Parameters = this.CommandParameters as ModbusReadCommandParameters;
            var message = new byte[12];
            message[0] = (byte)(Command_Parameters.TransactionId >> 8);
            message[1] = (byte)Command_Parameters.TransactionId;
            message[2] = (byte)(Command_Parameters.ProtocolId>>8);
            message[3] = (byte)Command_Parameters.ProtocolId;
            message[4] = (byte)(Command_Parameters.Length>>8);
            message[5] = (byte)Command_Parameters.Length;
            message[6] = Command_Parameters.UnitId;
            message[7] = Command_Parameters.FunctionCode;
            message[8] = (byte)(Command_Parameters.StartAddress>>8);
            message[9] = (byte)Command_Parameters.StartAddress;
            message[10] = (byte)(Command_Parameters.Quantity>>8);
            message[11] = (byte)Command_Parameters.Quantity;
            

            return message;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            var Command_Parameters = this.CommandParameters as ModbusReadCommandParameters;
            var tmp = new Dictionary<Tuple<PointType, ushort>, ushort>();
            var tuple = new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, Command_Parameters.StartAddress);
            var data  = new byte[2];
            for(int i =0; i < 2; i++)
            {
                data[i] = response[response.Length - 1 - i];
            }
            tmp.Add(tuple,BitConverter.ToUInt16(data,0));
            return tmp;
        }
    }
}