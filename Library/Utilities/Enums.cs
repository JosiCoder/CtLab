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

namespace CtLab.Utilities
{
    /// <summary>
    /// Specifies the SPI register numbers.
    /// </summary>
    public enum SpiRegister : ushort
    {
        // These values must correspond to the FPGA implementation.
        UniversalCounterConfiguration = 1,
        UniversalCounterStatus = 2,
        UniversalCounterRawValue = 3,
        PulseGeneratorPauseDuration = 5,
        PulseGeneratorPulseDuration = 6,
        OutputSource = 7,
        DdsGenerator1Base = 8,
        DdsGenerator2Base = 12,
        DdsGenerator3Base = 16,
        DdsGenerator4Base = 20,
        StorageController = 24,
        ScopeConfiguration = 25,
    }
}

