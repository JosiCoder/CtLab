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
using CtLab.SpiDirect.Interfaces;

namespace CtLab.SpiDirect.Standard.Specs
{
    public abstract class MessageSpecs
        : SpecsFor<Message>
    {
        protected override void InitializeClassUnderTest ()
        {
            SUT = new Message (GetMockFor<IMessageChannel>().Object, 0);
        }
    }


    public class When_converting_the_message_value_to_an_integer
        : MessageSpecs
    {
        protected override void When()
        {
            SUT.RawValue = 33;
        }

        [Test]
        public void then_the_SUT_should_return_the_correct_value()
        {
            SUT.ValueToInt32().ShouldEqual(33);
        }
    }
}
