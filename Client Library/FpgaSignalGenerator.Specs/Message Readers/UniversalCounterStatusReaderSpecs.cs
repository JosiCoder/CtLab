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
using CtLab.CommandsAndMessages.Interfaces;
using CtLab.FpgaSignalGenerator.Standard;

namespace CtLab.FpgaSignalGenerator.Specs
{
    public abstract class UniversalCounterStatusReaderSpecs
        : SubchannelReaderSpecs<UniversalCounterStatusReader>
    {
    }


    public class When_getting_a_counter_status_from_a_message_with_active_input_and_without_overflow
        : UniversalCounterStatusReaderSpecs
    {
        protected override void When()
        {
            // The message is a structure and thus is a value object. It must be set
            // before passing it to the IMessageContainer mock (as its copied then).
            var messageToReturn = new Message
                                       {
                                           Channel = 7,
                                           Subchannel = 255,
                                           RawValue = "1",
                                           Description = "xxx"
                                       };

            _messageContainerMock.Setup(container => container.Message).Returns(messageToReturn);
        }

        [Test]
        public void it_should_report_an_active_signal()
        {
            SUT.InputSignalActive.ShouldEqual(true);
        }

        [Test]
        public void it_should_not_report_an_overflow()
        {
            SUT.Overflow.ShouldEqual(false);
        }
    }

    public class When_getting_a_counter_status_from_a_message_with_active_input_and_overflow
        : UniversalCounterStatusReaderSpecs
    {
        protected override void When()
        {
            // The message is a structure and thus is a value object. It must be set
            // before passing it to the IMessageContainer mock.
            var messageToReturn = new Message
            {
                Channel = 7,
                Subchannel = 255,
                RawValue = "3",
                Description = "xxx"
            };

            _messageContainerMock.Setup(container => container.Message).Returns(messageToReturn);
        }

        [Test]
        public void it_should_report_an_active_signal()
        {
            SUT.InputSignalActive.ShouldEqual(true);
        }

        [Test]
        public void it_should_report_an_overflow()
        {
            SUT.Overflow.ShouldEqual(true);
        }
    }
}
