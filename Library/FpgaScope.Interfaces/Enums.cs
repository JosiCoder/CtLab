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
    /// Specifies the access mode of the storage.
    /// </summary>
    [Flags]
    public enum StorageMode : byte
    {
        // These values must correspond to the FPGA implementation.
        Idle = 0,
        Read = 1,
        Write = 2
    }

    /// <summary>
    /// Specifies the state of the storage.
    /// </summary>
    [Flags]
    public enum StorageState : byte
    {
        // These values must correspond to the FPGA implementation.
        Reading = 1,
        Writing = 2,
        Ready = 4,
    }
}