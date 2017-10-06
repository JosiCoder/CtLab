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

using System;
using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using CtLab.Messages.Interfaces;

namespace CtLab.Messages.Standard.Specs
{
    public abstract class SetCommandClassSpecs
        : SpecsFor<SetCommandClass>
    {
        protected override void InitializeClassUnderTest()
        {
            SUT = new SetCommandClass(new SpecsMessageChannel(7));
        }
    }


    public class When_converting_the_command_value_from_an_integer
        : SetCommandClassSpecs
    {
        protected override void When()
        {
            SUT.SetValue(33);
        }

        [Test]
        public void then_the_SUT_should_return_the_correct_value()
        {
            SUT.RawValue.ShouldEqual(33);
        }

        [Test]
        public void then_the_SUT_should_return_the_correct_string_representation()
        {
            SUT.RawValueAsString.ShouldEqual("33");
        }
    }


    public class When_converting_the_command_value_from_a_floating_point_number
        : SetCommandClassSpecs
    {
        protected override void When()
        {
            SUT.SetValue(33.3);
        }

        [Test]
        public void then_the_SUT_should_return_the_correct_value()
        {
            SUT.RawValue.ShouldEqual(33.3);
        }

        [Test]
        public void then_the_SUT_should_return_the_correct_string_representation()
        {
            SUT.RawValueAsString.ShouldEqual("33.3");
        }
    }


    public class When_converting_the_command_value_from_a_boolean_of_true
        : SetCommandClassSpecs
    {
        protected override void When()
        {
            SUT.SetValue(true);
        }

        [Test]
        public void then_the_SUT_should_return_the_correct_value()
        {
            SUT.RawValue.ShouldEqual(true);
        }

        [Test]
        public void then_the_SUT_should_return_the_correct_string_representation()
        {
            SUT.RawValueAsString.ShouldEqual("1");
        }
    }


    public class When_converting_the_command_value_from_a_boolean_of_false
        : SetCommandClassSpecs
    {
        protected override void When()
        {
            SUT.SetValue(false);
        }

        [Test]
        public void then_the_SUT_should_return_the_correct_value()
        {
            SUT.RawValue.ShouldEqual(false);
        }

        [Test]
        public void then_the_SUT_should_return_the_correct_string_representation()
        {
            SUT.RawValueAsString.ShouldEqual("0");
        }
    }


    public class When_changing_the_value_of_a_set_command_class
        : SetCommandClassSpecs
    {
        protected override void When()
        {
            SUT.SetValue(1);
        }

        [Test]
        public void then_the_SUT_should_be_marked_as_modified()
        {
            SUT.ValueModified.ShouldBeTrue();
        }
    }


    public class When_resetting_the_modification_identifier_of_a_set_command_class
        : SetCommandClassSpecs
    {
        protected override void When()
        {
            SUT.SetValue(1);
            SUT.ResetValueModified();
        }

        [Test]
        public void then_the_SUT_should_not_be_marked_as_modified()
        {
            SUT.ValueModified.ShouldBeFalse();
        }
    }


    public class When_changing_the_value_of_an_unmodified_set_command_class_using_the_previous_value
        : SetCommandClassSpecs
    {
        protected override void When()
        {
            SUT.SetValue(1);
            SUT.ResetValueModified();
            SUT.SetValue(1);
        }

        [Test]
        public void then_the_SUT_should_not_be_marked_as_modified()
        {
            SUT.ValueModified.ShouldBeFalse();
        }
    }


    public class When_changing_the_value_of_a_modified_set_command_class_using_the_previous_value
        : SetCommandClassSpecs
    {
        protected override void When()
        {
            SUT.SetValue(1);
            SUT.SetValue(1);
        }

        [Test]
        public void then_the_SUT_should_still_be_marked_as_modified()
        {
            SUT.ValueModified.ShouldBeTrue();
        }
    }
}
