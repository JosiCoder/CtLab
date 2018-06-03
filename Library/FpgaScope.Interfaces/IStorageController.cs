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

namespace CtLab.FpgaScope.Interfaces
{
    /// <summary>
    /// Provides facilities to communicate with a storage controller implemented within a c't Lab FPGA
    /// device configured as an FPGA Lab.
    /// </summary>
    public interface IStorageController
    {
        /// <summary>
        /// Gets the value read from the storage.
        /// </summary>
        int Value { get; }

        /// <summary>
        /// Prepares read access.
        /// </summary>
        /// <param name="address">The address to read from.</param>
        void PrepareReadAccess(int address);

        /// <summary>
        /// Prepares write access.
        /// </summary>
        /// <param name="address">The address to write to.</param>
        /// <param name="value">The value to write to the storage.</param>
        void PrepareWriteAccess(int address, int value);

        /// <summary>
        /// Sets the storage mode.
        /// </summary>
        void SetMode(short mode);

        /// <summary>
        /// Gets the current storage state.
        /// </summary>
        int State { get; }
    }
}