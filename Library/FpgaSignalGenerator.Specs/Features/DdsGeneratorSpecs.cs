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

namespace CtLab.FpgaSignalGenerator.Specs
{
    public abstract class DdsGeneratorSpecs : SpecsFor<DdsGenerator>
    {
        protected const uint _maximumPhaseIncrement = (uint.MaxValue / 2) + 1;
    }


    public class When_setting_the_frequency
        : DdsGeneratorSpecs
    {
        protected override void When()
        {
            SUT.Frequency = SUT.MaximumFrequency / 2;
        }

        [Test]
        public void it_should_have_set_the_phase_increment()
        {
            SUT.PhaseIncrement.ShouldEqual(_maximumPhaseIncrement / 2);
        }
    }


    public class When_setting_the_maximum_frequency_modulation_range
        : DdsGeneratorSpecs
    {
        protected override void When()
        {
            SUT.MaximumFrequencyModulationRange = 4;
        }

        [Test]
        public void ir_should_return_the_according_maximum_frequency_modulation_depth()
        {
            SUT.MaximumFrequencyModulationDepth.ShouldEqual(3125000);
        }
    }
}
