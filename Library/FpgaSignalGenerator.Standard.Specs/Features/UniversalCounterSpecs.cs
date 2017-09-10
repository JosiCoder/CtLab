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
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.FpgaSignalGenerator.Standard;
using CtLab.FpgaConnection.Interfaces;

namespace CtLab.FpgaSignalGenerator.Standard.Specs
{
    public abstract class UniversalCounterSpecs : FpgaWriterSpecs<UniversalCounter>
    {
    }


    public abstract class UniversalCounterValueSpecs
        : UniversalCounterSpecs
    {
        protected override void ConfigureContainer (IContainer container)
        {
            base.ConfigureContainer (container);

            var rawValueGetterMock = new Mock<IFpgaValueGetter>();
            rawValueGetterMock.Setup (rvg => rvg.ValueAsUInt32).Returns (120);

            container.Configure (expression =>
                {
                    expression.ForConcreteType<UniversalCounter>()
                        .Configure.Ctor<IFpgaValueGetter>("rawValueGetter")
                        .Is(rawValueGetterMock.Object);
                }
            );
        }
    }


    public class When_setting_the_prescaler_mode_to_a_mode_related_to_frequency_measurements
        : UniversalCounterSpecs
    {
        protected override void When()
        {
            SUT.PrescalerMode = PrescalerMode.GatePeriod_10s;
        }

        [Test]
        public void then_the_SUT_should_pass_the_combined_values_to_the_FPGA_writer_to_reflect_the_most_recent_changes()
        {
            var expectedValue =
                ((uint)MeasurementMode.Frequency) << 4
                | ((uint)PrescalerMode.GatePeriod_10s);

            _lastValueSet.ShouldEqual(expectedValue);
        }
    }


    public class When_setting_the_prescaler_mode_to_a_mode_related_to_period_measurements
        : UniversalCounterSpecs
    {
        protected override void When()
        {
            SUT.PrescalerMode = PrescalerMode.CounterClock_1MHz;
        }

        [Test]
        public void then_the_SUT_should_pass_the_combined_values_to_the_FPGA_writer_to_reflect_the_most_recent_changes()
        {
            var expectedValue =
                ((uint)MeasurementMode.Period) << 4
                | ((uint)PrescalerMode.CounterClock_1MHz);

            _lastValueSet.ShouldEqual(expectedValue);
        }
    }


    public class When_using_a_period_of_one_second
        : UniversalCounterValueSpecs
    {
        protected override void When()
        {
            SUT.PrescalerMode = PrescalerMode.GatePeriod_1s;
        }

        [Test]
        public void then_the_SUT_should_return_the_raw_value_directly()
        {
            SUT.Value.ShouldEqual(120);
        }

        [Test]
        public void then_the_SUT_should_return_a_least_significant_digit_exponent_of_0()
        {
            SUT.LeastSignificantDigitExponent.ShouldEqual(0);
        }
    }


    public class When_using_a_period_of_ten_seconds
        : UniversalCounterValueSpecs
    {
        protected override void When()
        {
            SUT.PrescalerMode = PrescalerMode.GatePeriod_10s;
        }

        [Test]
        public void then_the_SUT_should_return_the_raw_value_divided_by_ten()
        {
            SUT.Value.ShouldEqual(12);
        }

        [Test]
        public void then_the_SUT_should_return_a_least_significant_digit_exponent_of_minus_1()
        {
            SUT.LeastSignificantDigitExponent.ShouldEqual(-1);
        }
    }


    public class When_using_a_counter_clock_of_ten_kilohertz
        : UniversalCounterValueSpecs
    {
        protected override void When()
        {
            SUT.PrescalerMode = PrescalerMode.CounterClock_10kHz;
        }

        [Test]
        public void then_the_SUT_should_return_the_raw_value_divided_by_ten_thousand()
        {
            SUT.Value.ShouldEqual(0.012);
        }

        [Test]
        public void then_the_SUT_should_return_a_least_significant_digit_exponent_of_minus_4()
        {
            SUT.LeastSignificantDigitExponent.ShouldEqual(-4);
        }
    }
}
