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

using System.Collections.Generic;
using CtLab.FpgaScope.Interfaces;
using CtLab.FpgaConnection.Interfaces;

namespace CtLab.FpgaScope.Standard
{
    /// <summary>
    /// Represents an FPGA Lab scope.
    /// </summary>
    public class Scope : IScope
    {
        private readonly IFpgaConnection _fpgaConnection;
        private readonly StorageController _storageController;

        /// <summary>
        /// Writes the specified values to the storage.
        /// </summary>
        /// <param name="address">The address to start writing to.</param>
        /// <param name="value">The values to write to the storage.</param>
        public void Write (uint startAddress, IEnumerable<uint> values)
        {
            _storageController.Write (startAddress, values);
        }

        /// <summary>
        /// Reads the specified number of values from the storage.
        /// </summary>
        /// <param name="address">The address to start reading from.</param>
        /// <param name="numberOfValues">The number of values to read.</param>
        /// <returns>The values read.</returns>
        public IEnumerable<uint> Read (uint startAddress, int numberOfValues)
        {
            return _storageController.Read (startAddress, numberOfValues);
        }

        /// <summary>
        /// Releases all resource used by this instance.
        /// </summary>
        public void Dispose()
        {
            _fpgaConnection.Dispose();
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="hardwareSettings">The hardware settings to use for the storage.</param>
        /// <param name="deviceConnection">The connection used to access the device.</param>
        /// <param name="fpgaValuesAccessor">The accessor used to control access to FPGA values.</param>
        public Scope(StorageHardwareSettings hardwareSettings, IFpgaConnection deviceConnection,
            IFpgaValuesAccessor fpgaValuesAccessor)
        {
            _fpgaConnection = deviceConnection;

            _storageController = new StorageController(hardwareSettings, _fpgaConnection, fpgaValuesAccessor);
        }

        /// <summary>
        /// Resets the scope. 
        /// </summary>
        public void Reset()
        {
            // e.g.:
            // StorageController.XXX = ???;
        }
    }
}