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
    public abstract class ValueConverterViewModelSpecs
        : SpecsFor<ValueConverterViewModel<int, string>>
    {
        protected PropertyChangedSink _propertyChangedSink;

        protected override void InitializeClassUnderTest()
        {
            SUT = new ValueConverterViewModel<int, string>(
                originalValue => originalValue.ToString(),
                derivedValue => int.Parse(derivedValue)
            );
        }

        protected override void Given()
        {
            base.Given();

            _propertyChangedSink = new PropertyChangedSink(SUT);
        }
    }


    public class When_setting_the_original_value
        : ValueConverterViewModelSpecs
    {
        protected override void When()
        {
            SUT.OriginalValue = 99;
        }

        [Test]
        public void it_should_return_the_correct_derived_value()
        {
            SUT.DerivedValue.ShouldEqual("99");
        }

        [Test]
        public void it_should_raise_property_changed_events_for_the_original_and_derived_values()
        {
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("OriginalValue");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("DerivedValue");
        }
    }
}

