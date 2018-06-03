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
using CtLab.Utilities;
using CtLab.FpgaScope.Interfaces;
using CtLab.FpgaConnection.Interfaces;

namespace CtLab.FpgaScope.Standard
{
    /// <summary>
    /// Communicates with a storage controller implemented within a c't Lab FPGA device configured as
    /// an FPGA Lab.
    /// </summary>
    public class StorageController : IStorageController
    {
        private readonly IFpgaValueSetter _valueSetter;
        private readonly IFpgaValueGetter _valueGetter;
        private readonly IFpgaValueSetter _addressSetter;
        private readonly IFpgaValueSetter _modeSetter;
        private readonly IFpgaValueGetter _stateGetter;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="valueSetter">
        /// The setter used to set the value to be written to the storage.
        /// </param>
        /// <param name="valueGetter">
        /// The getter used to get the value read from the storage.
        /// </param>
        /// <param name="addressSetter">
        /// The setter used to set the address to write to ar read from.
        /// </param>
        /// <param name="modeSetter">
        /// The setter used to set the storage mode.
        /// </param>
        /// <param name="stateGetter">
        /// The getter used to get the storage state.
        /// </param>
        public StorageController(
            IFpgaValueSetter valueSetter,
            IFpgaValueGetter valueGetter,
            IFpgaValueSetter addressSetter,
            IFpgaValueSetter modeSetter,
            IFpgaValueGetter stateGetter)
        {
            _valueSetter = valueSetter;
            _valueGetter = valueGetter;
            _addressSetter = addressSetter;
            _modeSetter = modeSetter;
            _stateGetter = stateGetter;
        }

        /// <summary>
        /// Gets the value read from the storage.
        /// </summary>
        public int Value
        {
            get { return _valueGetter.ValueAsInt32; }
        }

        /// <summary>
        /// Prepares read access.
        /// </summary>
        /// <param name="address">The address to read from.</param>
        public void PrepareReadAccess(int address)
        {
            _addressSetter.SetValue (address);
        }

        /// <summary>
        /// Prepares write access.
        /// </summary>
        /// <param name="address">The address to write to.</param>
        /// <param name="value">The value to write to the storage.</param>
        public void PrepareWriteAccess(int address, int value)
        {
            _addressSetter.SetValue (address);
            _valueSetter.SetValue (value);
        }

        /// <summary>
        /// Sets the storage mode.
        /// </summary>
        public  void SetMode(short mode)
        {
            _modeSetter.SetValue (mode);
        }

        /// <summary>
        /// Gets the current storage state.
        /// </summary>
        public int State
        {
            get { return _stateGetter.ValueAsInt32; }
        }
    }
}