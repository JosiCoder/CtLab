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
    public interface IMessageUpdatedSink
    {
        void MessageUpdated(object sender, EventArgs e);
    }


    public abstract class ReceivedMessageCacheIntegrationSpecs
        : SpecsFor<Container>
    {
        protected Mock<ISpiReceiver> _spiReceiverMock;

        protected override void InitializeClassUnderTest()
        {
            // Use a mock that we can query whether a method has been called.
            _spiReceiverMock = GetMockFor<ISpiReceiver>();

            SUT = new Container (expression =>
                {
                    expression.AddRegistry<CommandsAndMessagesRegistry>();
                    expression.AddRegistry<SpiDirectRegistry>();
                    expression.For<ISpiReceiver>().Use(_spiReceiverMock.Object);
                });
        }
    }


    public abstract class ReceivedMessageCacheIntegrationInteractionSpecs
        : ReceivedMessageCacheIntegrationSpecs
    {
        protected Mock<IMessageUpdatedSink>[] _messageUpdatedSinkMocks;

        protected override void Given()
        {
            base.Given();

            _messageUpdatedSinkMocks = new []
            {
                new Mock<IMessageUpdatedSink>(),
                new Mock<IMessageUpdatedSink>(),
                new Mock<IMessageUpdatedSink>()
            };
        }
    }


    public class When_signalling_a_received_string
            : ReceivedMessageCacheIntegrationInteractionSpecs
    {
        private IMessageCache _messageCache;

        protected override void When()
        {
            _messageCache = SUT.GetInstance<IMessageCache>();

            _messageCache.Register(new MessageChannel(11));
            _messageCache.Register(new MessageChannel(22));
            _messageCache.Register(new MessageChannel(33));

            _messageCache.GetMessageContainer(new MessageChannel(11)).MessageUpdated += _messageUpdatedSinkMocks[0].Object.MessageUpdated;
            _messageCache.GetMessageContainer(new MessageChannel(22)).MessageUpdated += _messageUpdatedSinkMocks[1].Object.MessageUpdated;
            _messageCache.GetMessageContainer(new MessageChannel(33)).MessageUpdated += _messageUpdatedSinkMocks[2].Object.MessageUpdated;

            _spiReceiverMock.Raise(spiReceiver => spiReceiver.ValueReceived += null, new SpiReceivedEventArgs(11, 6));
            _spiReceiverMock.Raise(spiReceiver => spiReceiver.ValueReceived += null, new SpiReceivedEventArgs(22, 7));
        }

        [Test]
        public void then_the_SUT_should_update_the_messages_in_the_message_containers()
        {
            var channel = GetMockFor<IMessageChannel>().Object;
            _messageCache.GetMessageContainer(new MessageChannel(11)).Message.ValueEquals(new Message(channel, 6)).ShouldBeTrue();
            _messageCache.GetMessageContainer(new MessageChannel(22)).Message.ValueEquals(new Message(channel, 7)).ShouldBeTrue();
            _messageCache.GetMessageContainer(new MessageChannel(33)).Message.IsEmpty.ShouldBeTrue();
        }

        [Test]
        public void then_the_SUT_should_raise_events_for_updated_messages_but_none_else()
        {
            _messageUpdatedSinkMocks[0].Verify(sink => sink.MessageUpdated(_messageCache.GetMessageContainer(new MessageChannel(11)), EventArgs.Empty), Times.Once);
            _messageUpdatedSinkMocks[1].Verify(sink => sink.MessageUpdated(_messageCache.GetMessageContainer(new MessageChannel(22)), EventArgs.Empty), Times.Once);
            _messageUpdatedSinkMocks[2].Verify(sink => sink.MessageUpdated(_messageCache.GetMessageContainer(new MessageChannel(33)), EventArgs.Empty), Times.Never);
        }

    }
}
