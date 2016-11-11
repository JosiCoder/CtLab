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
using StructureMap;
using CtLab.CommandsAndMessages.Interfaces;
using CtLab.CommandsAndMessages.Standard;
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.FpgaSignalGenerator.Standard;

namespace CtLab.FpgaSignalGenerator.Specs
{
    public abstract class UniversalCounterSpecs : SubchannelWriterSpecs<UniversalCounter>
    {
    }


    public abstract class UniversalCounterValueSpecs
        : UniversalCounterSpecs
    {
        protected override void ConfigureContainer (IContainer container)
        {
            base.ConfigureContainer (container);

            // The contained message is a value object and thus can't be mocked. It also can't
            // be modified within a mocked message container via the interface. Thus we need a
            // real message container here instead of a mock.
            var rawValueContainer = new MessageContainer(1, 11);
            rawValueContainer.UpdateMessage(new Message() { RawValue = "120" });

            container.Configure (expression =>
                {
                    expression.ForConcreteType<UniversalCounter>()
                        .Configure.Ctor<IMessageContainer>("rawValueContainer")
                        .Is(rawValueContainer);
                }
            );
        }
    }


    public class When_setting_the_prescaler_mode_to_a_mode_related_to_frequency_measurements
        : UniversalCounterSpecs
    {
        protected override void When()
        {
            SUT.PrescalerMode = PrescalerModes.GatePeriod_10s;
        }

        [Test]
        public void it_should_pass_the_combined_values_to_the_command_value_setter_to_reflect_the_most_recent_changes()
        {
            var expectedValue =
                ((uint)MeasurementModes.Frequency) << 4
                | ((uint)PrescalerModes.GatePeriod_10s);

            _lastValueSet.ShouldEqual(expectedValue);
        }
    }


    public class When_setting_the_prescaler_mode_to_a_mode_related_to_period_measurements
        : UniversalCounterSpecs
    {
        protected override void When()
        {
            SUT.PrescalerMode = PrescalerModes.CounterClock_1MHz;
        }

        [Test]
        public void it_should_pass_the_combined_values_to_the_command_value_setter_to_reflect_the_most_recent_changes()
        {
            var expectedValue =
                ((uint)MeasurementModes.Period) << 4
                | ((uint)PrescalerModes.CounterClock_1MHz);

            _lastValueSet.ShouldEqual(expectedValue);
        }
    }


    public class When_using_a_period_of_one_second
        : UniversalCounterValueSpecs
    {
        protected override void When()
        {
            SUT.PrescalerMode = PrescalerModes.GatePeriod_1s;
        }

        [Test]
        public void it_should_return_the_raw_value_directly()
        {
            SUT.Value.ShouldEqual(120);
        }
    }


    public class When_using_a_period_of_ten_seconds
        : UniversalCounterValueSpecs
    {
        protected override void When()
        {
            SUT.PrescalerMode = PrescalerModes.GatePeriod_10s;
        }

        [Test]
        public void it_should_return_the_raw_value_divided_by_ten()
        {
            SUT.Value.ShouldEqual(12);
        }
    }


    public class When_using_a_couner_clock_of_ten_kilohertz
        : UniversalCounterValueSpecs
    {
        protected override void When()
        {
            SUT.PrescalerMode = PrescalerModes.CounterClock_10kHz;
        }

        [Test]
        public void it_should_return_the_raw_value_divided_by_ten_thousand()
        {
            SUT.Value.ShouldEqual(0.012);
        }
    }
}
