//------------------------------------------------------------------------------
// Copyright (C) 2016 Josi Coder

// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.

// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.

// You should have received a copy of the GNU General Public License along with
// this program. If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------

using System;
using CtLab.CommandsAndMessages.Interfaces;
using CtLab.Device.Base;
using CtLab.FpgaSignalGenerator.Standard;

namespace CtLab.EnvironmentIntegration
{
    /// <summary>
    /// Provides access to an FPGA Lab device, based on c't Lab set and query commands.
    /// </summary>
    public class CtLabFpgaLabDeviceConnection : DeviceConnectionBase, IFpgaLabDeviceConnection
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="channel">
        /// The number of the channel assigned to the c't Lab device controlled by this instance.
        /// </param>
        /// <param name="setCommandClassDictionary">The dictonary used for the set command classes.</param>
        /// <param name="queryCommandClassDictionary">The dictonary used for the query command classes.</param>
        /// <param name="receivedMessagesCache">The message cache used to receive the messages.</param>
        public CtLabFpgaLabDeviceConnection(byte channel, ISetCommandClassDictionary setCommandClassDictionary, IQueryCommandClassDictionary queryCommandClassDictionary, IMessageCache receivedMessagesCache)
            : base(channel, setCommandClassDictionary, queryCommandClassDictionary, receivedMessagesCache)
        {
        }

        /// <summary>
        /// Creates an FPGA value setter.
        /// </summary>
        /// <param name="registerNumber">
        /// The number of the FPGA register to write to.
        /// </param>
        /// <returns>The created FPGA value setter.</returns>
        public IFpgaValueSetter CreateFpgaValueSetter(ushort registerNumber)
        {
            return new CtLabSetCommandFpgaValueSetter (
                BuildAndRegisterSetCommandClass (Channel, registerNumber));
        }

        /// <summary>
        /// Creates an FPGA value getter.
        /// </summary>
        /// <param name="registerNumber">
        /// The number of the FPGA register to read from.
        /// </param>
        /// <returns>The created FPGA value getter.</returns>
        public IFpgaValueGetter CreateFpgaValueGetter(ushort registerNumber)
        {
            return new CtLabQueryCommandFpgaValueGetter (
                BuildAndRegisterQueryCommandClass (Channel, registerNumber),
                RegisterMessage(Channel, registerNumber)
            );
        }
    }
}