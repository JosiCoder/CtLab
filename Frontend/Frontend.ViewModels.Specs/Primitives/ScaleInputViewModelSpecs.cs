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

namespace CtLab.Frontend.ViewModels.Specs
{
    public class DummyScaleInputViewModel : ScaleInputViewModelBase<int>
    {
        public DummyScaleInputViewModel(IApplianceServices applianceServices,
            Action<int> setter, Func<int> getter)
            : base(applianceServices, setter, getter, 0,0,0,0,0)
        {
        }

        public override double OverviewValue
        {
            get { return BaseValue; }
            set { BaseValue = value; }
        }

        public override bool OverviewValueIsLogarithmic
        { get { return true; } }

        public override double BaseValueLower
        { get { return 0; } }
    }

    public abstract class ScaleInputViewModelSpecs
        : SpecsFor<DummyScaleInputViewModel>
    {
        protected Mock<IGetterSetterMock<int>> _getterSetterMock;
        protected PropertyChangedSink _propertyChangedSink;

        protected override void InitializeClassUnderTest()
        {
            _getterSetterMock = new Mock<IGetterSetterMock<int>>();

            SUT = new DummyScaleInputViewModel(
                new Mock<IApplianceServices>().Object,
                _getterSetterMock.Object.Setter, _getterSetterMock.Object.Getter);
        }

        protected override void Given()
        {
            base.Given();

            _propertyChangedSink = new PropertyChangedSink(SUT);
        }
    }


    public class When_getting_the_value_of_a_scale_input
        : ScaleInputViewModelSpecs
    {
        protected override void When()
        {
            var x = SUT.InternalValue;
        }

        [Test]
        public void then_the_SUT_should_get_the_value_from_the_attached_getter()
        {
            _getterSetterMock.Verify(getterSetter => getterSetter.Getter(), Times.Once);
        }
    }


    public class When_setting_the_internal_value_of_a_scale_input
        : ScaleInputViewModelSpecs
    {
        protected override void When()
        {
            SUT.InternalValue = 99;
        }

        [Test]
        public void then_the_SUT_should_pass_the_value_to_the_attached_setter()
        {
            _getterSetterMock.Verify(getterSetter => getterSetter.Setter(99), Times.Once);
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_the_internal_value_and_derived_values()
        {
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("InternalValue");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("BaseValue");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("OverviewValue");
        }
    }


    public class When_setting_the_base_value_of_a_scale_input
        : LinLogScaleInputViewModelSpecs
    {
        protected override void When()
        {
            SUT.BaseValue = 1;
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_the_base_value_and_derived_values()
        {
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("InternalValue");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("BaseValue");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("OverviewValue");
        }
    }


    public class When_setting_the_overview_value_of_a_scale_input
        : LinLogScaleInputViewModelSpecs
    {
        protected override void When()
        {
            SUT.OverviewValue = 1;
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_the_internal_value_and_derived_values()
        {
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("InternalValue");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("BaseValue");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("OverviewValue");
        }
    }


    public class When_setting_the_scale_exponent_of_a_scale_input
        : LinLogScaleInputViewModelSpecs
    {
        protected override void When()
        {
            SUT.ScaleExponent = 3;
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_the_scale_exponent_and_all_values_depending_on_it()
        {
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("ScaleExponent");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("InternalValue");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("BaseValue");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("OverviewValue");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("BaseValueUpper");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("BaseValueStepWidth");
        }
    }


    public class When_setting_the_scale_exponent_of_a_scale_input_to_a_specific_value
        : LinLogScaleInputViewModelSpecs
    {
        [Test]
        [TestCase(0, 1e-8, uint.MaxValue * InternalValueUnit, 8)]
        [TestCase(-1, 1e-7, 1e1 * (uint.MaxValue * InternalValueUnit), 7)]
        [TestCase(-2, 1e-6, 1e2 * (uint.MaxValue * InternalValueUnit), 6)]
        [TestCase(-3, 1e-5, MaxBaseValueUpper, 5)]
        [TestCase(-8, 1, MaxBaseValueUpper, 0)]
        [TestCase(-9, 10, MaxBaseValueUpper, 0)]
        [TestCase(-10, 100, MaxBaseValueUpper, 0)]
        public void then_the_SUT_should_return_the_according_base_step_width_and_upper_value (
            int scaleExponent, double baseValueStepWidth, double baseValueUpper, int baseValueDecimalPlaces)
        {
            SUT.ScaleExponent = scaleExponent;

            SUT.BaseValueStepWidth.ShouldBeAround(baseValueStepWidth);
            SUT.BaseValueDecimalPlaces.ShouldBeAround((uint)baseValueDecimalPlaces);
            SUT.BaseValueUpper.ShouldBeAround(baseValueUpper);
        }
    }
}

