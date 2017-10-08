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

using StructureMap;
using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using Moq;
using CtLab.Messages.Interfaces;
using CtLab.Connection.Interfaces;
using CtLab.CtLabProtocol.Interfaces;
using CtLab.BasicIntegration;

namespace CtLab.CtLabProtocolIntegration.Specs
{
    public abstract class QueryCommandSenderIntegrationSpecs
        : SpecsFor<Container>
    {
        protected Mock<IStringSender> _stringSenderMock;

        protected override void InitializeClassUnderTest()
        {
            // Use a mock that we can query whether a method has been called.
            _stringSenderMock = GetMockFor<IStringSender>();

            SUT = new Container (expression =>
                {
                    expression.AddRegistry<CommandsAndMessagesRegistry>();
                    expression.AddRegistry<CtLabProtocolRegistry>();
                    expression.For<IStringSender>().Use(_stringSenderMock.Object);
                });
        }
    }

    public class When_sending_a_query_command
        : QueryCommandSenderIntegrationSpecs
    {
        protected override void When()
        {
            var queryCommandSender = SUT.GetInstance<IQueryCommandSender>();
            queryCommandSender.Send(new QueryCommandClass(new MessageChannel(1, 11)));
        }

        [Test]
        public void then_the_SUT_should_send_the_command_string_including_the_checksum()
        {
            _stringSenderMock.Verify(sender => sender.Send("1:11?$34"), Times.Once);
        }
    }
}
