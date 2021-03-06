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
using CtLab.Device.Base;
using CtLab.Messages.Interfaces;
using CtLab.FpgaConnection.Interfaces;

namespace CtLab.FpgaConnection.Standard
{
    /// <summary>
    /// Provides access to an FPGA Lab device, based on c't Lab set and query commands.
    /// </summary>
    public class FpgaConnection : IFpgaConnection
    {
        private readonly IDeviceConnection _deviceConnection;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="deviceConnection">
        /// The device connection to the c't Lab device.
        /// </param>
        public FpgaConnection(IDeviceConnection deviceConnection)
        {
            _deviceConnection = deviceConnection;
        }

        /// <summary>
        /// Creates an FPGA value setter.
        /// </summary>
        /// <param name="registerNumber">
        /// The number of the FPGA register to write to.
        /// </param>
        /// <param name="group">The group the underlying command class is related to.</param>
        /// <returns>The created FPGA value setter.</returns>
        public IFpgaValueSetter CreateFpgaValueSetter(ushort registerNumber, CommandClassGroup group)
        {
            return new FpgaValueSetter (
                _deviceConnection.BuildAndRegisterSetCommandClass (registerNumber, group));
        }

        /// <summary>
        /// Creates an FPGA value getter.
        /// </summary>
        /// <param name="registerNumber">
        /// The number of the FPGA register to read from.
        /// </param>
        /// <param name="group">The group the underlying command class is related to.</param>
        /// <returns>The created FPGA value getter.</returns>
        public IFpgaValueGetter CreateFpgaValueGetter(ushort registerNumber, CommandClassGroup group)
        {
            _deviceConnection.BuildAndRegisterQueryCommandClass (registerNumber, group);
            return new FpgaValueGetter (
                _deviceConnection.RegisterMessage(registerNumber)
            );
        }

        /// <summary>
        /// Releases all resource used by this instance.
        /// </summary>
        public void Dispose()
        {
            _deviceConnection.Dispose();
        }
    }
}