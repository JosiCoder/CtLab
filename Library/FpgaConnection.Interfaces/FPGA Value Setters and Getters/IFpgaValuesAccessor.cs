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
using CtLab.Messages.Interfaces;

namespace CtLab.FpgaConnection.Interfaces
{
    /// <summary>
    /// Provides facilities to access values within the FPGA.
    /// </summary>
    public interface IFpgaValuesAccessor
    {
        /// <summary>
        /// Sends modified setter values to the scope.
        /// </summary>
        void FlushSetters();

        /// <summary>
        /// Refreshes getter values from the scope.
        /// </summary>
        /// <param name="predicate">The predicate that must be met.</param>
        void RefreshGetters(Predicate<SendMode> predicate);
    }
}