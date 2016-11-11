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

namespace CtLab.FpgaSignalGenerator.Interfaces
{
    /// <summary>
    /// Specifies the waveforms that can be generated by the DDS generators.
    /// </summary>
    public enum Waveforms : ushort
    {
        Rectangle = 0,
        Sawtooth = 1,
        Sine = 2
    }

    /// <summary>
    /// Specifies the signal sources that can be connected to DDS generators for modulation and
    /// synchronization.
    /// </summary>
    public enum ModulationAndSynchronizationSources : ushort
    {
        DdsGenerator0 = 0,
        DdsGenerator1,
        DdsGenerator2,
        DdsGenerator3
    }

    /// <summary>
    /// Specifies the signal sources that can be connected to the outputs.
    /// </summary>
    public enum OutputSources : ushort
    {
        DdsGenerator0 = 0,
        DdsGenerator1,
        DdsGenerator2,
        DdsGenerator3,
        PulseGenerator
    }

    /// <summary>
    /// Specifies the signal sources that can be measured by the universal counter.
    /// </summary>
    public enum UniversalCounterSources : ushort
    {
        DdsGenerator0 = 0,
        DdsGenerator1,
        DdsGenerator2,
        DdsGenerator3,
        PulseGenerator,
        External
    }

    /// <summary>
    /// Specifies the measurement modes.
    /// </summary>
    public enum MeasurementModes : ushort
    {
        Frequency = 0,
        Period
    }

    /// <summary>
    /// Specifies prescaler modes used for period and frequency measurements.
    /// </summary>
    public enum PrescalerModes : ushort
    {
        GatePeriod_1s = 0,
        GatePeriod_10s,
        GatePeriod_100ms,
        CounterClock_10MHz,
        CounterClock_1MHz,
        CounterClock_100kHz,
        CounterClock_10kHz
    }
}