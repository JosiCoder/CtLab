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

namespace CtLab.FpgaSignalGenerator.Interfaces
{
    /// <summary>
    /// Provides facilities to communicate with a signal generator.
    /// </summary>
    public interface ISignalGenerator : IDisposable
    {
        /// <summary>
        /// Gets all the DDS generators.
        /// </summary>
        IDdsGenerator[] DdsGenerators { get; }

        /// <summary>
        /// Gets the selectors used to select the signal source connected the outputs.
        /// </summary>
        IOutputSourceSelector OutputSourceSelector { get; }

        /// <summary>
        /// Gets the pulse generator.
        /// </summary>
        IPulseGenerator PulseGenerator { get; }

        /// <summary>
        /// Gets the universal counter.
        /// </summary>
        IUniversalCounter UniversalCounter { get; }

        /// <summary>
        /// Gets the amplitude modulation settings of the DDS generators.
        /// </summary>
        AmplitudeModulationInformationSet[] DdsGeneratorsAMInformationSets { get; }

        /// <summary>
        /// Gets the frequency modulation settings of the DDS generators.
        /// </summary>
        FrequencyModulationInformationSet[] DdsGeneratorsFMInformationSets { get; }

        /// <summary>
        /// Resets the signal generator. 
        /// </summary>
        void Reset();
    }
}