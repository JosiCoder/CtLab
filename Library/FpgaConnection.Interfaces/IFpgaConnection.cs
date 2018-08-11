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

namespace CtLab.FpgaConnection.Interfaces
{
    /// <summary>
    /// Provides facilities to access an FPGA Lab device.
    /// </summary>
    public interface IFpgaConnection : IDisposable
    {
        /// <summary>
        /// Creates an FPGA value setter.
        /// </summary>
        /// <param name="registerNumber">
        /// The number of the FPGA register to write to.
        /// </param>
        /// <returns>The created FPGA value setter.</returns>
        IFpgaValueSetter CreateFpgaValueSetter(ushort registerNumber);

        /// <summary>
        /// Creates an FPGA value getter.
        /// </summary>
        /// <param name="registerNumber">
        /// The number of the FPGA register to read from.
        /// </param>
        /// <param name="queryMode">
        /// The query mode used.
        /// </param>
        /// <returns>The created FPGA value getter.</returns>
        IFpgaValueGetter CreateFpgaValueGetter(ushort registerNumber, QueryMode queryMode);
    }
}