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
using System.Collections.Generic;

namespace CtLab.FpgaScope.Interfaces
{
    /// <summary>
    /// Provides facilities to communicate with a scope.
    /// </summary>
    public interface IScope : IDisposable
    {
        /// <summary>
        /// Gets or sets the input source.
        /// </summary>
        ScopeSource InputSource { get; set; }

        /// <summary>
        /// Captures values from the specified source to the storage.
        /// </summary>
        /// <param name="startAddress">The address to start writing at.</param>
        /// <param name="endAddress">The address to stop writing at.</param>
        void Capture (uint startAddress, uint endAddress);

        /// <summary>
        /// Writes the specified values to the storage.
        /// </summary>
        /// <param name="address">The address to start writing at.</param>
        /// <param name="value">The values to write to the storage.</param>
        void Write (uint startAddress, IEnumerable<uint> values);

        /// <summary>
        /// Reads the specified number of values from the storage.
        /// </summary>
        /// <param name="address">The address to start reading at.</param>
        /// <param name="numberOfValues">The number of values to read.</param>
        /// <returns>The values read.</returns>
        IEnumerable<uint> Read (uint startAddress, int numberOfValues);

        /// <summary>
        /// Resets the scope. 
        /// </summary>
        void Reset();
    }
}