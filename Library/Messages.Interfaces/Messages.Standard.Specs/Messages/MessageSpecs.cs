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
    public abstract class MessageSpecs
        : SpecsFor<object>
    {
        protected Message _message = new Message
                            {
                                Channel = new SpecsMessageChannel(7),
                                RawValue = "0",
                                Description = "DSCR"
                            };
    }


    public class When_converting_the_message_value_to_an_integer
        : MessageSpecs
    {
        protected override void When()
        {
            _message.RawValue = "33";
        }

        [Test]
        public void then_the_SUT_should_return_the_correct_value()
        {
            _message.ValueToInt32().ShouldEqual(33);
        }
    }


	public class When_converting_the_message_value_to_a_floating_point_number
        : MessageSpecs
    {
        protected override void When()
        {
            _message.RawValue = "33.3";
        }

        [Test]
		public void then_the_SUT_should_return_the_correct_value()
        {
            _message.ValueToDouble().ShouldEqual(33.3);
        }
    }


	public class When_converting_the_message_value_of_true_to_a_boolean
        : MessageSpecs
    {
        protected override void When()
        {
            _message.RawValue = "1";
        }

        [Test]
		public void then_the_SUT_should_return_the_correct_value()
        {
            _message.ValueToBoolean().ShouldEqual(true);
        }
    }


	public class When_converting_the_message_value_of_false_to_a_boolean
        : MessageSpecs
    {
        protected override void When()
        {
            _message.RawValue = "0";
        }

        [Test]
		public void then_the_SUT_should_return_the_correct_value()
        {
            _message.ValueToBoolean().ShouldEqual(false);
        }
    }


	public class When_converting_a_non_boolean_message_value_to_a_boolean
        : MessageSpecs
    {
        private Action _theAssertion;

        protected override void When()
        {
            _message.RawValue = "2";
            _theAssertion = () => _message.ValueToBoolean();
        }

        [Test]
        public void then_the_SUT_should_throw_an_exception()
        {
            _theAssertion.ShouldThrow<FormatException>();
        }
    }
}
