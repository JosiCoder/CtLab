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
    [Flags]
    internal enum StorageModeBits : byte
    {
        // These values must correspond to the FPGA implementation.
        Read = 1,
        Write = 2,
        //Unused = 4,
        AutoIncrementAddress = 8,
        ConnectMemory = 16,
    }

    /// <summary>
    /// Specifies the access mode of the storage.
    /// </summary>
    public enum StorageMode : byte
    {
        // These values must correspond to the FPGA implementation.
        Release = 0,
        Idle =          StorageModeBits.ConnectMemory,
        Read =          StorageModeBits.ConnectMemory | StorageModeBits.Read,
        Write =         StorageModeBits.ConnectMemory | StorageModeBits.Write,
        Set2ndAddress = StorageModeBits.ConnectMemory | StorageModeBits.Read | StorageModeBits.Write,
        Capture =       StorageModeBits.ConnectMemory | StorageModeBits.Write | StorageModeBits.AutoIncrementAddress,
    }

    [Flags]
    internal enum StorageStateBits : byte
    {
        // These values must correspond to the FPGA implementation.
        Reading = 1,
        Writing = 2,
        //Unused = 4,
        AutoIncrementAddressReached = 8,
        Ready = 16,
    }

    /// <summary>
    /// Specifies the state of the storage.
    /// </summary>
    public enum StorageState : byte
    {
        // These values must correspond to the FPGA implementation.
        Reading =           StorageStateBits.Reading,
        Writing =           StorageStateBits.Writing,
        Setting2ndAddress = StorageStateBits.Reading | StorageStateBits.Writing | StorageStateBits.Ready,
        CapturingFinished = StorageStateBits.Writing | StorageStateBits.AutoIncrementAddressReached,
        Ready =             StorageStateBits.Ready,
    }

    /// <summary>
    /// Specifies the signal/data sources that can be captured by the scope.
    /// </summary>
    public enum ScopeSource : ushort
    {
        // These values must correspond to the FPGA implementation.
        DdsGenerator0 = 0,
        DdsGenerator1 = 1,
        DdsGenerator2 = 2,
        DdsGenerator3 = 3,
        PulseGenerator = 4,
        External = 5,
        Data = 6, // mainly used for testing purposes
    }
}