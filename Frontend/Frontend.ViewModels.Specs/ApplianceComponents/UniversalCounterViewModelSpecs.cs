//------------------------------------------------------------------------------
// Copyright (C) 2017 Josi Coder

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
using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using Moq;
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.Frontend.ViewModels.Specs
{
    public abstract class UniversalCounterViewModelSpecs
        : SpecsFor<UniversalCounterViewModel>
    {
        protected Mock<IApplianceServices> _applianceServicesMock;
        protected Mock<IUniversalCounter> _universalCounterMock;
        protected Mock<IMeasurementValueReadoutViewModel> _frequencyReadoutVMMock;
        protected Mock<IMeasurementValueReadoutViewModel> _periodReadoutVMMock;
        protected PropertyChangedSink _propertyChangedSink;

        protected override void InitializeClassUnderTest()
        {
            // Create all dependencies.
            _applianceServicesMock = new Mock<IApplianceServices>();
            _universalCounterMock = new Mock<IUniversalCounter>();
            _frequencyReadoutVMMock = new Mock<IMeasurementValueReadoutViewModel>();
            _periodReadoutVMMock = new Mock<IMeasurementValueReadoutViewModel>();

            SUT = new UniversalCounterViewModel(
                _applianceServicesMock.Object, _universalCounterMock.Object,
                _frequencyReadoutVMMock.Object, _periodReadoutVMMock.Object);

            // Ignore calls made to mocks within the SUTs constructor.
            _applianceServicesMock.ResetCalls ();
            _universalCounterMock.ResetCalls ();
            _frequencyReadoutVMMock.ResetCalls ();
            _periodReadoutVMMock.ResetCalls ();
        }

        protected override void Given()
        {
            base.Given();

            _propertyChangedSink = new PropertyChangedSink(SUT);
        }

        protected void AssertConfigureReadoutVM(Mock<IMeasurementValueReadoutViewModel> readoutVMMock)
        {
            readoutVMMock.VerifySet(readoutVM => readoutVM.LeastSignificantDigitExponentProvider = It.IsAny<Func<int>>(), Times.AtLeastOnce);
            readoutVMMock.VerifySet(readoutVM => readoutVM.ScaleExponentProvider = It.IsAny<Func<int>>(), Times.AtLeastOnce);
            readoutVMMock.VerifySet(readoutVM => readoutVM.ValueWasDerived = It.IsAny<bool>(), Times.AtLeastOnce);
        }
    }


    public class When_the_attached_universal_counter_raises_a_value_changed_event
        : UniversalCounterViewModelSpecs
    {
        protected override void When()
        {
            _universalCounterMock.Raise(counter => counter.ValueChanged += null,
                new ValueChangedEventArgs(999));
        }

        [Test]
        public void then_the_SUT_should_update_the_measurement_value_readout_viewmodels_accordingly()
        {
            _frequencyReadoutVMMock.VerifySet(readoutVM => readoutVM.Value = It.IsAny<double>(), Times.AtLeastOnce);
            _periodReadoutVMMock.VerifySet(readoutVM => readoutVM.Value = It.IsAny<double>(), Times.AtLeastOnce);
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_the_value_and_the_overflow_indicator()
        {
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("Value");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("Overflow");
        }
    }


    public class When_the_attached_universal_counter_raises_an_input_signal_activity_event
        : UniversalCounterViewModelSpecs
    {
        protected override void When()
        {
            _universalCounterMock.Raise(counter => counter.InputSignalActiveChanged += null,
                new InputSignalActiveChangedEventArgs(true));
        }

        [Test]
        public void then_the_SUT_should_raise_a_property_changed_event_for_the_signal_activity_indicator()
        {
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("InputSignalActive");
        }
    }


    public class When_setting_the_input_source
        : UniversalCounterViewModelSpecs
    {
        protected override void When()
        {
            SUT.InputSource = UniversalCounterSource.DdsGenerator0;
        }

        [Test]
        public void then_the_SUT_should_call_the_flush_action_passed_to_it()
        {
            _applianceServicesMock.Verify(flush => flush.Flush(), Times.AtLeastOnce);
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_the_input_source()
        {
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("InputSource");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("BindingInputSource");
        }
    }


    public class When_setting_the_prescaler_mode
        : UniversalCounterViewModelSpecs
    {
        protected override void When()
        {
            SUT.PrescalerMode = PrescalerMode.CounterClock_100kHz;
        }

        [Test]
        public void then_the_SUT_should_call_the_flush_action_passed_to_it()
        {
            _applianceServicesMock.Verify(flush => flush.Flush(), Times.AtLeastOnce);
        }

        [Test]
        public void then_the_SUT_should_update_the_measurement_value_readout_viewmodels_accordingly()
        {
            AssertConfigureReadoutVM (_frequencyReadoutVMMock);
            AssertConfigureReadoutVM (_periodReadoutVMMock);
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_the_prescaler_mode()
        {
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("PrescalerMode");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("BindingPrescalerMode");
        }
    }


    public class When_setting_the_scale_exponent
        : UniversalCounterViewModelSpecs
    {
        protected override void When()
        {
            SUT.ScaleExponent = 3;
        }

        [Test]
        public void then_the_SUT_should_update_the_measurement_value_readout_viewmodels_accordingly()
        {
            AssertConfigureReadoutVM (_frequencyReadoutVMMock);
            AssertConfigureReadoutVM (_periodReadoutVMMock);
        }

        [Test]
        public void then_the_SUT_should_raise_a_property_changed_events_for_the_scale_exponent()
        {
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("ScaleExponent");
        }
    }
}

