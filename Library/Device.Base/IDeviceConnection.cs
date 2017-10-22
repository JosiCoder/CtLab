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
using CtLab.Messages.Interfaces;
using CtLab.CtLabProtocol.Interfaces;
//using CtLab.SpiDirect.Interfaces;

namespace CtLab.Device.Base
{
    /// <summary>
    /// Provides facilities to access a c't Lab device.
    /// </summary>
    public interface IDeviceConnection : IDisposable
    {
        /// <summary>
        /// Builds a set command class and adds it to the dictionary.
        /// </summary>
        /// <param name="registerNumber">
        /// The FPGA register number corresponding to the commands sent.
        /// </param>
        /// <returns>The set command class.</returns>
        SetCommandClass BuildAndRegisterSetCommandClass(ushort registerNumber);

        /// <summary>
        /// Builds a query command class and adds it to the dictionary.
        /// </summary>
        /// <param name="registerNumber">
        /// The FPGA register number corresponding to the commands sent.
        /// </param>
        /// <returns>The query command class.</returns>
        QueryCommandClass BuildAndRegisterQueryCommandClass(ushort registerNumber);

        /// <summary>
        /// Registers an FPGA register for message caching and returns the message
        /// container for that FPGA register.
        /// </summary>
        /// <param name="registerNumber">
        /// The number of the FPGA register to register.
        /// </param>
        /// <returns>The message container.</returns>
        IMessageContainer RegisterMessage(ushort registerNumber);
    }
}