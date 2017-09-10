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
    public abstract class FrequencyModulationInformerSpecs : SpecsFor<FrequencyModulationInformer>
    {
        protected Mock<IFrequencyModulationCarrierSource> _carrierSourceMock;
        protected Mock<IFrequencyModulatorSource> _modulatorSourceMock;

        protected override void Given()
        {
            base.Given();

            // Use a carrier source mock with a frequency range of 0..50MHz and a maximum
            // modulation depth of 40 MHz.
            _carrierSourceMock = GetMockFor<IFrequencyModulationCarrierSource>();
            _carrierSourceMock.Setup(source => source.MaximumFrequency).Returns(50e6);
            _carrierSourceMock.Setup(source => source.MaximumFrequencyModulationDepth).Returns(40e6);

            // Use a modulator source mock with an amplitude range of +/- 100 and assign it to the SUT
            // explicitly as that doesn't have one by default.
            _modulatorSourceMock = GetMockFor<IFrequencyModulatorSource>();
            _modulatorSourceMock.Setup(source => source.MaximumAmplitude).Returns(100);
            SUT.ModulatorSource = _modulatorSourceMock.Object;
        }
    }


    public class When_no_FM_modulator_is_attached
        : FrequencyModulationInformerSpecs
    {
        protected override void When()
        {
            SUT.ModulatorSource = null;
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_zero()
        {
            SUT.ModulationDepth.ShouldEqual(0);
            SUT.RelativeModulationDepth.ShouldEqual(0.0f);
        }

        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }


    public class When_modulating_an_FM_carrier_with_a_zero_frequency_using_a_nonzero_modulation
        : FrequencyModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Frequency).Returns (0);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(25); // 10 MHz modulation depth ((25/100)*40 MHz)
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_infinity()
        {
            SUT.ModulationDepth.ShouldEqual(10e6);
            SUT.RelativeModulationDepth.ShouldEqual(float.PositiveInfinity);
        }
    
        [Test]
        public void then_the_SUT_should_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(true);
        }
    }


    public class When_modulating_an_FM_carrier_with_the_maximum_frequency_using_a_nonzero_modulation
        : FrequencyModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Frequency).Returns (SUT.CarrierSource.MaximumFrequency);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(25); // 10 MHz modulation depth ((25/100)*40 MHz)
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_infinity()
        {
            SUT.ModulationDepth.ShouldEqual(10e6);
            SUT.RelativeModulationDepth.ShouldEqual(float.PositiveInfinity);
        }
    
        [Test]
        public void then_the_SUT_should_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(true);
        }
    }


    public class When_modulating_an_FM_carrier_with_a_zero_frequency_using_a_zero_modulation
        : FrequencyModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Frequency).Returns (0);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(0);
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_zero()
        {
            SUT.ModulationDepth.ShouldEqual(0);
            SUT.RelativeModulationDepth.ShouldEqual(0.0f);
        }

        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }


    public class When_modulating_an_FM_carrier_with_the_maximum_frequency_using_a_zero_modulation
        : FrequencyModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Frequency).Returns (SUT.CarrierSource.MaximumFrequency);
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(0);
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_zero()
        {
            SUT.ModulationDepth.ShouldEqual(0);
            SUT.RelativeModulationDepth.ShouldEqual(0.0f);
        }

        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }


    public class When_modulating_an_FM_carrier_with_less_than_the_half_possible_frequency_using_a_modulation_depth_of_the_half_of_the_distance_to_zero
        : FrequencyModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Frequency).Returns (20e6); // 20 MHz carrier
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(25); // 10 MHz modulation depth ((25/100)*40 MHz)
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_one_half()
        {
            SUT.ModulationDepth.ShouldEqual(10e6);
            SUT.RelativeModulationDepth.ShouldEqual(0.5f);
        }
    
        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }


    public class When_modulating_an_FM_carrier_with_more_than_the_half_possible_frequency_using_a_modulation_depth_of_the_half_of_the_distance_to_the_maximum_frequency
        : FrequencyModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Frequency).Returns (30e6); // 30 MHz carrier
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(25); // 10 MHz modulation depth ((25/100)*40 MHz)
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_one_half()
        {
            SUT.ModulationDepth.ShouldEqual(10e6);
            SUT.RelativeModulationDepth.ShouldEqual(0.5f);
        }

        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }


    public class When_modulating_an_FM_carrier_such_that_the_elongation_reaches_zero
        : FrequencyModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Frequency).Returns (20e6); // 20 MHz carrier
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(50); // 20 MHz modulation depth ((50/100)*40 MHz)
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_100_percent()
        {
            SUT.ModulationDepth.ShouldEqual(20e6);
            SUT.RelativeModulationDepth.ShouldEqual(1f);
        }
    
        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }


    public class When_modulating_an_FM_carrier_such_that_the_elongation_reaches_its_maximum
        : FrequencyModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Frequency).Returns (30e6); // 30 MHz carrier
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(50); // 20 MHz modulation depth ((50/100)*40 MHz)
        }

        [Test]
        public void then_the_SUT_should_return_a_relative_modulation_depth_of_100_percent()
        {
            SUT.ModulationDepth.ShouldEqual(20e6);
            SUT.RelativeModulationDepth.ShouldEqual(1f);
        }

        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }


    public class When_modulating_an_FM_carrier_such_that_the_elongation_falls_below_zero
        : FrequencyModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Frequency).Returns (10e6); // 10 MHz carrier
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(50); // 20 MHz modulation depth ((50/100)*40 MHz)
        }

        [Test]
        public void then_the_SUT_should_return_an_according_relative_modulation_depth_of_more_than_100_percent()
        {
            SUT.ModulationDepth.ShouldEqual(20e6);
            SUT.RelativeModulationDepth.ShouldEqual(2f);
        }
    
        [Test]
        public void then_the_SUT_should_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(true);
        }
    }


    public class When_modulating_an_FM_carrier_such_that_the_elongation_exeeds_its_maximum
        : FrequencyModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Frequency).Returns (40e6); // 40 MHz carrier
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(50); // 20 MHz modulation depth ((50/100)*40 MHz)
        }

        [Test]
        public void then_the_SUT_should_return_an_according_relative_modulation_depth_of_more_than_100_percent()
        {
            SUT.ModulationDepth.ShouldEqual(20e6);
            SUT.RelativeModulationDepth.ShouldEqual(2f);
        }

        [Test]
        public void then_the_SUT_should_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(true);
        }
    }


    public class When_using_an_inverted_FM_modulator
        : FrequencyModulationInformerSpecs
    {
        protected override void When()
        {
            _carrierSourceMock.Setup(source => source.Frequency).Returns (20e6); // 20 MHz carrier
            _modulatorSourceMock.Setup(source => source.Amplitude).Returns(-25); // 10 MHz modulation depth @ 180°
        }

        [Test]
        public void then_the_SUT_should_ignore_the_180_degree_phase_shift_and_still_return_positive_modulation_depths()
        {
            SUT.ModulationDepth.ShouldEqual(10e6);
            SUT.RelativeModulationDepth.ShouldEqual(0.5f);
        }

        [Test]
        public void then_the_SUT_should_not_report_overmodulation()
        {
            SUT.Overmodulated.ShouldEqual(false);
        }
    }
}
