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
using CtLab.Messages.Interfaces;
using CtLab.FpgaConnection.Interfaces;

namespace CtLab.FpgaScope.Standard
{
    //TODO add more stuff similar to the UniversalCounter class.

    /// <summary>
    /// Represents an FPGA Lab scope.
    /// </summary>
    public class Scope : IScope
    {
        private ScopeConfigurationWriter _configurationWriter;

        private readonly IFpgaConnection _fpgaConnection;
        private readonly StorageController _storageController;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="deviceConnection">The connection used to access the device.</param>
        /// <param name="hardwareSettings">The hardware settings to use for the storage.</param>
        /// <param name="fpgaValuesAccessor">The accessor used to control access to FPGA values.</param>
        public Scope(IFpgaConnection deviceConnection, StorageHardwareSettings hardwareSettings,
            IFpgaValuesAccessor fpgaValuesAccessor)
        {
            _fpgaConnection = deviceConnection;
            // TODO adjust to FPGA implementation
            _configurationWriter = new ScopeConfigurationWriter(CreateFpgaValueSetter(99));

            _storageController = new StorageController(hardwareSettings, _fpgaConnection, fpgaValuesAccessor);
        }

        /// <summary>
        /// Gets or sets the input source.
        /// </summary>
        public ScopeSource InputSource
        {
            get { return _configurationWriter.InputSource; }
            set { _configurationWriter.InputSource = value; }
        }

        /// <summary>
        /// Captures values from the specified source to the storage.
        /// </summary>
        /// <param name="startAddress">The address to start writing at.</param>
        /// <param name="endAddress">The address to stop writing at.</param>
        public void Capture (uint startAddress, uint endAddress)
        {
            _storageController.Capture (startAddress, endAddress);
        }

        /// <summary>
        /// Writes the specified values to the storage.
        /// </summary>
        /// <param name="address">The address to start writing at.</param>
        /// <param name="value">The values to write to the storage.</param>
        public void Write (uint startAddress, IEnumerable<uint> values)
        {
            _storageController.Write (startAddress, values);
        }

        /// <summary>
        /// Reads the specified number of values from the storage.
        /// </summary>
        /// <param name="address">The address to start reading at.</param>
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
        /// Resets the scope. 
        /// </summary>
        public void Reset()
        {
            // e.g.:
            // StorageController.XXX = ???;
        }

        private IFpgaValueSetter CreateFpgaValueSetter(ushort registerNumber)
        {
            return _fpgaConnection.CreateFpgaValueSetter(registerNumber, new CommandClassGroup());
        }
    }
}