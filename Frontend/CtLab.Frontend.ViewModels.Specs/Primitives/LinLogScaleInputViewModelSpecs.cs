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
    public abstract class LinLogScaleInputViewModelSpecs
        : SpecsFor<LinLogScaleInputViewModel<uint>>
    {
        protected const double InternalValueUnit = 1e-8;
        protected const double InternalValueStepWidth = InternalValueUnit;
        protected const double MaxBaseValueUpper = 1e4;

        protected PropertyChangedSink _propertyChangedSink;
        protected uint modelValue;

        protected override void InitializeClassUnderTest()
        {
            SUT = new LinLogScaleInputViewModel<uint>(
                new Mock<IApplianceServices>().Object, value => { modelValue = value; }, () => modelValue,
                InternalValueUnit, uint.MaxValue, InternalValueStepWidth, MaxBaseValueUpper, 0);
        }

        protected override void Given()
        {
            base.Given();

            _propertyChangedSink = new PropertyChangedSink(SUT);
        }
    }


    public class When_setting_the_internal_value_and_the_scale_exponent_of_a_linlog_scale_input
        : LinLogScaleInputViewModelSpecs
    {
        [Test]
        [TestCase((uint)3, 0, 3 * InternalValueUnit)]
        [TestCase((uint)3, -3, 3e3 * InternalValueUnit)]
        [TestCase((uint)5e8, 0, 5e8 * InternalValueUnit)]
        [TestCase((uint)5e8, -3, 5e11 * InternalValueUnit)]
        [TestCase((uint)5e8, 4, 5e4 * InternalValueUnit)]
        public void it_should_return_the_according_base_and_overview_values (
            uint internalValue, int scaleExponent, double baseValue)
        {
            var overviewValue = Math.Log10 (baseValue);

            SUT.ScaleExponent = scaleExponent;
            SUT.InternalValue = internalValue;

            SUT.BaseValue.ShouldBeAround(baseValue);
            SUT.OverviewValue.ShouldBeAround(overviewValue);
        }
    }


    public class When_setting_the_base_value_and_the_scale_exponent_of_a_linlog_scale_input
        : LinLogScaleInputViewModelSpecs
    {
        [Test]
        [TestCase((uint)3, 0, 3 * InternalValueUnit)]
        [TestCase((uint)3, -3, 3e3 * InternalValueUnit)]
        [TestCase((uint)5e8, 0, 5e8 * InternalValueUnit)]
        [TestCase((uint)5e8, -3, 5e11 * InternalValueUnit)]
        [TestCase((uint)5e8, 4, 5e4 * InternalValueUnit)]
        public void it_should_return_the_according_internal_and_overview_values (
            uint internalValue, int scaleExponent, double baseValue)
        {
            var overviewValue = Math.Log10 (baseValue);

            SUT.ScaleExponent = scaleExponent;
            SUT.BaseValue = baseValue;

            SUT.InternalValue.ShouldBeAround(internalValue);
            SUT.OverviewValue.ShouldBeAround(overviewValue);
        }
    }


    public class When_setting_the_overview_value_and_the_scale_exponent_of_a_linlog_scale_input
        : LinLogScaleInputViewModelSpecs
    {
        [Test]
        [TestCase((uint)3, 0, 3 * InternalValueUnit)]
        [TestCase((uint)3, -3, 3e3 * InternalValueUnit)]
        [TestCase((uint)5e8, 0, 5e8 * InternalValueUnit)]
        [TestCase((uint)5e8, -3, 5e11 * InternalValueUnit)]
        [TestCase((uint)5e8, 4, 5e4 * InternalValueUnit)]
        public void it_should_return_the_according_internal_and_base_values (
            uint internalValue, int scaleExponent, double baseValue)
        {
            var overviewValue = Math.Log10 (baseValue);

            SUT.ScaleExponent = scaleExponent;
            SUT.OverviewValue = overviewValue;

            SUT.InternalValue.ShouldBeAround(internalValue);
            SUT.BaseValue.ShouldBeAround(baseValue);
        }
    }
}

