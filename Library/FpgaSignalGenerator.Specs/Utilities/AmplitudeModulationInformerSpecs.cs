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
    public abstract class AmplitudeModulationInformerSpecs : SpecsFor<AmplitudeModulationInformer>
    {
        protected Mock<IAmplitudeModulationCarrierSource> _carrierSourceMock;
        protected Mock<IAmplitudeModulatorSource> _modulatorSourceMock;

        protected override void Given()
        {
            base.Given();

            // Use a carrier source mock with an amplitude range of +/- short.MaxValue.
            _carrierSourceMock = GetMockFor<IAmplitudeModulationCarrierSource>();
            _carrierSourceMock.Setup(source => source.MaximumAmplitude).Returns(short.MaxValue);

            // Use a modulator source mock and assign it to the SUT explicitly as that doesn't have
            // one by default.
            _modulatorSourceMock = GetMockFor<IAmplitudeModulatorSource>();
            SUT.ModulatorSource = _modulatorSourceMock.Object;
        }
    }


    public class When_no_AM_modulator_is_attached
        : AmplitudeModulationInformerSpecs
    {
        protected override void When()
        {
            SUT.ModulatorSource = null;
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_zero()
        {
            SUT.RelativeModulationDepth.ShouldEqual(0.0f);
        }

        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }


    public class When_modulating_an_AM_carrier_with_a_zero_amplitude_using_a_modulator_having_a_nonzero_amplitude
        : AmplitudeModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Amplitude).Returns (0);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(60);
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_infinity()
        {
            SUT.RelativeModulationDepth.ShouldEqual(float.PositiveInfinity);
        }

        [Test]
        public void then_the_SUT_should_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(true);
        }
    }


    public class When_modulating_an_AM_carrier_with_the_maximum_amplitude_using_a_modulator_having_a_nonzero_amplitude
        : AmplitudeModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Amplitude).Returns (SUT.CarrierSource.MaximumAmplitude);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(60);
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_infinity()
        {
            SUT.RelativeModulationDepth.ShouldEqual(float.PositiveInfinity);
        }

        [Test]
        public void then_the_SUT_should_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(true);
        }
    }


    public class When_modulating_an_AM_carrier_with_a_zero_amplitude_using_a_modulator_having_a_zero_amplitude
        : AmplitudeModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Amplitude).Returns (0);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(0);
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_zero()
        {
            SUT.RelativeModulationDepth.ShouldEqual(0.0f);
        }

        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }


    public class When_modulating_an_AM_carrier_with_the_maximum_amplitude_using_a_modulator_having_a_zero_amplitude
        : AmplitudeModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Amplitude).Returns (SUT.CarrierSource.MaximumAmplitude);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(0);
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_zero()
        {
            SUT.RelativeModulationDepth.ShouldEqual(0.0f);
        }

        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }


    public class When_modulating_an_AM_carrier_with_less_than_the_half_possible_amplitude_using_a_modulator_having_half_the_amplitude_of_the_carrier
        : AmplitudeModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Amplitude).Returns (120);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(60);
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_one_half()
        {
            SUT.RelativeModulationDepth.ShouldEqual(0.5f);
        }

        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }


    public class When_modulating_an_AM_carrier_with_more_than_the_half_possible_amplitude_using_a_modulator_having_half_the_amplitude_of_the_room_up_to_the_maximum
        : AmplitudeModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Amplitude).Returns (32767-120);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(60);
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_one_half()
        {
            SUT.RelativeModulationDepth.ShouldEqual(0.5f);
        }

        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }


    public class When_modulating_an_AM_carrier_such_that_the_elongation_reaches_zero
        : AmplitudeModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Amplitude).Returns (120);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(120);
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_100_percent()
        {
            SUT.RelativeModulationDepth.ShouldEqual(1f);
        }

        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }


    public class When_modulating_an_AM_carrier_such_that_the_elongation_reaches_its_maximum
        : AmplitudeModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Amplitude).Returns (0x7ff0);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(0xf);
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_100_percent()
        {
            SUT.RelativeModulationDepth.ShouldEqual(1f);
        }

        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }


    public class When_modulating_an_AM_carrier_such_that_the_elongation_falls_below_zero
        : AmplitudeModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Amplitude).Returns (60);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(120);
        }

        [Test]
        public void then_the_SUT_should_return_an_according_relative_modulation_depth_of_more_than_100_percent()
        {
            SUT.RelativeModulationDepth.ShouldEqual(2f);
        }

        [Test]
        public void then_the_SUT_should_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(true);
        }
    }


    public class When_modulating_an_AM_carrier_such_that_the_elongation_exeeds_its_maximum
        : AmplitudeModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Amplitude).Returns (0x7ff0);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(0x1e);
        }

        [Test]
        public void then_the_SUT_should_return_an_according_relative_modulation_depth_of_more_than_100_percent()
        {
            SUT.RelativeModulationDepth.ShouldEqual(2f);
        }

        [Test]
        public void then_the_SUT_should_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(true);
        }
    }


    public class When_using_an_inverted_AM_carrier
        : AmplitudeModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Amplitude).Returns (-120);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(60);
        }

        [Test]
        public void then_the_SUT_should_ignore_the_180_degree_phase_shift_and_still_return_a_positive_relative_modulation_depth()
        {
            SUT.RelativeModulationDepth.ShouldEqual(0.5f);
        }

        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }


    public class When_using_an_inverted_AM_modulator
        : AmplitudeModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Amplitude).Returns (120);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(-60);
        }

        [Test]
        public void then_the_SUT_should_ignore_the_180_degree_phase_shift_and_still_return_a_positive_relative_modulation_depth()
        {
            SUT.RelativeModulationDepth.ShouldEqual(0.5f);
        }

        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }
}
