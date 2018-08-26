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
using CtLab.SpiConnection.Interfaces;
using CtLab.SpiDirect.Interfaces;
using CtLab.BasicIntegration;

namespace CtLab.SpiDirect.Integration.Specs
{
    public abstract class QueryCommandSchedulerIntegrationSpecs
        : SpecsFor<Container>
    {
        protected Mock<ISpiSender> _spiSenderMock;

        protected override void InitializeClassUnderTest()
        {
            // Use a mock that we can query whether a method has been called.
            _spiSenderMock = GetMockFor<ISpiSender>();

            SUT = new Container (expression =>
                {
                    expression.AddRegistry<CommandsAndMessagesRegistry>();
                    expression.AddRegistry<SpiDirectRegistry>();
                    expression.For<ISpiSender>().Use(_spiSenderMock.Object);
                });
        }
    }


    public class When_sending_the_scheduled_query_commands_immediately
        : QueryCommandSchedulerIntegrationSpecs
    {
        protected override void When()
        {
            var queryCommandDictionary = SUT.GetInstance<IQueryCommandClassDictionary>();
            var queryCommand = new QueryCommandClass(new MessageChannel(11));
            queryCommandDictionary.Add(queryCommand, new CommandClassGroup());
            
            var queryCommandScheduler = SUT.With(queryCommandDictionary).GetInstance<IQueryCommandScheduler>();
            queryCommandScheduler.SendImmediately(commandClass => true);
        }

        [Test]
        public void then_underlying_SPI_sender_should_send_anything_to_the_appropriate_SPI_address()
        {
            _spiSenderMock.Verify(sender => sender.Send(11, It.IsAny<uint>()), Times.Once);
        }
    }
}
