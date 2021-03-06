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

using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using Moq;
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.FpgaSignalGenerator.Standard;

namespace CtLab.FpgaSignalGenerator.Standard.Specs
{
    public abstract class WaveformWriterSpecs
        : FpgaWriterSpecs<WaveformWriter>
    {
    }


    public class When_setting_the_waveform_configuration
        : WaveformWriterSpecs
    {
        protected override void When()
        {
            SUT.Waveform = Waveform.Sawtooth;
            SUT.MaximumFrequencyModulationDepth = 10;
            SUT.SynchronizationSource = ModulationAndSynchronizationSource.DdsGenerator3;
            SUT.PhaseModulationSource = ModulationAndSynchronizationSource.DdsGenerator2;
            SUT.FrequencyModulationSource = ModulationAndSynchronizationSource.DdsGenerator1;
            SUT.AmplitudeModulationSource = ModulationAndSynchronizationSource.DdsGenerator2;
        }

        [Test]
        public void then_the_SUT_should_pass_the_combined_values_to_the_FPGA_writer_to_reflect_the_most_recent_changes()
        {
            _lastValueSet.ShouldEqual((uint)0x00010ae6);
        }
    }
}
