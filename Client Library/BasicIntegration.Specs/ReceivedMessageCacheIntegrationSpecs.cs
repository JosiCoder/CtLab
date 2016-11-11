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
using CtLab.Connection.Interfaces;
using CtLab.CommandsAndMessages.Interfaces;

namespace CtLab.BasicIntegration.Specs
{
    public interface IMessageUpdatedSink
    {
        void MessageUpdated(object sender, EventArgs e);
    }


    public abstract class ReceivedMessageCacheIntegrationSpecs
        : SpecsFor<object>
    {
        protected Container _container;
        protected Mock<IStringReceiver> _stringReceiverMock;

        protected override void Given()
        {
            base.Given ();

            // Use a mock that we can query whether a method has been called.
            _stringReceiverMock = GetMockFor<IStringReceiver>();

            _container = new Container (expression =>
                {
                    expression.AddRegistry<CommandsAndMessagesRegistry>();
                    expression.For<IStringReceiver>().Use(_stringReceiverMock.Object);
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

            _stringReceiverMock = GetMockFor<IStringReceiver>();
            _messageUpdatedSinkMocks = new []
            {
                new Mock<IMessageUpdatedSink>(),
                new Mock<IMessageUpdatedSink>(),
                new Mock<IMessageUpdatedSink>()
            };
        }
    }


    public class When_getting_the_message_cache_more_than_once
        : ReceivedMessageCacheIntegrationSpecs
    {
        protected override void When()
        {
        }

        [Test]
        public void it_should_get_the_same_instance()
        {
            var instance1 = _container.GetInstance<IMessageCache>();
            var instance2 = _container.GetInstance<IMessageCache>();
            instance2.ShouldBeSameAs(instance1);
        }
    }


    public class When_signalling_a_received_string
            : ReceivedMessageCacheIntegrationInteractionSpecs
    {
        private IMessageCache _messageCache;

        protected override void When()
        {
            _messageCache = _container.GetInstance<IMessageCache>();

            _messageCache.Register(1, 255);
            _messageCache.Register(2, 255);
            _messageCache.Register(3, 255);

            _messageCache.GetMessageContainer(1, 255).MessageUpdated += _messageUpdatedSinkMocks[0].Object.MessageUpdated;
            _messageCache.GetMessageContainer(2, 255).MessageUpdated += _messageUpdatedSinkMocks[1].Object.MessageUpdated;
            _messageCache.GetMessageContainer(3, 255).MessageUpdated += _messageUpdatedSinkMocks[2].Object.MessageUpdated;

            _stringReceiverMock.Raise(stringReceiver => stringReceiver.StringReceived += null,
                new StringReceivedEventArgs("#1:255=6 [CHKSUM]\n#2:255=7 [CHKSUM]"));
        }

        [Test]
        public void it_should_update_the_messages_in_the_message_containers()
        {
            _messageCache.GetMessageContainer(1, 255).Message.RawValue.ShouldEqual("6");
            _messageCache.GetMessageContainer(2, 255).Message.RawValue.ShouldEqual("7");
            _messageCache.GetMessageContainer(3, 255).Message.RawValue.ShouldBeNull();
        }

        [Test]
        public void it_should_raise_events_for_updated_messages_but_none_else()
        {
            _messageUpdatedSinkMocks[0].Verify(sink => sink.MessageUpdated(_messageCache.GetMessageContainer(1, 255), EventArgs.Empty), Times.Once);
            _messageUpdatedSinkMocks[1].Verify(sink => sink.MessageUpdated(_messageCache.GetMessageContainer(2, 255), EventArgs.Empty), Times.Once);
            _messageUpdatedSinkMocks[2].Verify(sink => sink.MessageUpdated(_messageCache.GetMessageContainer(3, 255), EventArgs.Empty), Times.Never);
        }

    }
}
