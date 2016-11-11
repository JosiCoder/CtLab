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
    public abstract class UniversalCounterConfigurationWriterSpecs
        : SubchannelWriterSpecs<UniversalCounterConfigurationWriter>
    {
    }


    public class When_setting_the_counter_configuration
        : UniversalCounterConfigurationWriterSpecs
    {
        protected override void When()
        {
            SUT.InputSource = UniversalCounterSources.DdsGenerator1;
            SUT.MeasurementMode = MeasurementModes.Period;
            SUT.PrescalerMode = PrescalerModes.CounterClock_1MHz;
            SUT.InputSource = UniversalCounterSources.DdsGenerator2;
        }

        [Test]
        public void it_should_pass_the_combined_values_to_the_command_value_setter_to_reflect_the_most_recent_changes()
        {
            _lastValueSet.ShouldEqual((uint)0x00000214);
        }
    }
}
