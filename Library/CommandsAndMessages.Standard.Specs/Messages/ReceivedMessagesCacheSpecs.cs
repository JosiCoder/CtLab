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
using Moq;
using CtLab.CommandsAndMessages.Interfaces;
using CtLab.CommandsAndMessages.Standard;

namespace CtLab.CommandsAndMessages.Specs
{
    public interface IMessageUpdatedSink
    {
        void MessageUpdated(object sender, EventArgs e);
    }


    public abstract class ReceivedMessagesCacheSpecs
        : SpecsFor<ReceivedMessagesCache>
    {
    }


    public abstract class ReceivedMessagesCacheWithSampleSubchannelsSpecs
        : ReceivedMessagesCacheSpecs
    {
        protected override void Given()
        {
            base.Given();

            SUT.Register(1, 11);
            SUT.Register(2, 22);
            SUT.Register(3, 33);
        }
    }


    public abstract class ReceivedMessagesCacheInteractionSpecs
        : ReceivedMessagesCacheWithSampleSubchannelsSpecs
    {
        protected Mock<IMessageReceiver> _messageReceiverMock;
        protected Mock<IMessageUpdatedSink>[] _messageUpdatedSinkMocks;

        protected override void Given()
        {
            base.Given();

            _messageReceiverMock = GetMockFor<IMessageReceiver>();
            _messageUpdatedSinkMocks = new []
            {
                new Mock<IMessageUpdatedSink>(),
                new Mock<IMessageUpdatedSink>(),
                new Mock<IMessageUpdatedSink>()
            };

            SUT.GetMessageContainer(1, 11).MessageUpdated += _messageUpdatedSinkMocks[0].Object.MessageUpdated;
            SUT.GetMessageContainer(2, 22).MessageUpdated += _messageUpdatedSinkMocks[1].Object.MessageUpdated;
            SUT.GetMessageContainer(3, 33).MessageUpdated += _messageUpdatedSinkMocks[2].Object.MessageUpdated;
        }
    }


    public class When_registering_a_new_subchannel_for_message_caching
        : ReceivedMessagesCacheSpecs
    {
		IMessageContainer _returnedMessageContainer;

        protected override void When()
        {
			_returnedMessageContainer = SUT.Register(2, 11);
        }

		[Test]
		public void then_the_SUT_should_return_a_message_container()
		{
			_returnedMessageContainer.ShouldNotBeNull();
		}

        [Test]
		public void then_the_SUT_should_be_able_to_obtain_the_according_message_container()
        {
            var container = SUT.GetMessageContainer(2, 11);
			container.ShouldBeSameAs(_returnedMessageContainer);
        }

        [Test]
		public void then_the_SUT_should_have_an_empty_message_in_the_message_container()
        {
            var container = SUT.GetMessageContainer(2, 11);
            var message = container.Message;
            message.Channel.ShouldEqual((byte)2);
            message.Subchannel.ShouldEqual((ushort)11);
            message.RawValue.ShouldBeNull();
        }
    }


	public class When_attempting_to_register_a_subchannel_for_message_caching_that_has_already_been_registered
        : ReceivedMessagesCacheWithSampleSubchannelsSpecs
    {
        private Action _theAssertion;

        protected override void When()
        {
            _theAssertion = () => SUT.Register(1, 11);
        }

        [Test]
        public void then_the_SUT_should_throw_an_exception()
        {
            _theAssertion.ShouldThrow<ArgumentException>();
        }
    }


    public class When_signalling_received_messages
        : ReceivedMessagesCacheInteractionSpecs
    {
        protected override void When()
        {
            _messageReceiverMock.Raise (messageReceiver => messageReceiver.MessageReceived += null,
                new MessageReceivedEventArgs(new Message() { Channel = 1, Subchannel = 11, RawValue = "HI!" }));
            _messageReceiverMock.Raise (messageReceiver => messageReceiver.MessageReceived += null,
                new MessageReceivedEventArgs(new Message() { Channel = 2, Subchannel = 22, RawValue = "Hello!" }));
        }

        [Test]
        public void then_the_SUT_should_update_the_messages_in_the_message_containers()
        {
            SUT.GetMessageContainer(1, 11).Message.RawValue.ShouldEqual("HI!");
            SUT.GetMessageContainer(2, 22).Message.RawValue.ShouldEqual("Hello!");
            SUT.GetMessageContainer(3, 33).Message.RawValue.ShouldBeNull();
        }

        [Test]
        public void then_the_SUT_should_raise_events_for_updated_messages_but_none_else()
        {
            _messageUpdatedSinkMocks[0].Verify(sink => sink.MessageUpdated(SUT.GetMessageContainer(1,11), EventArgs.Empty), Times.Once);
            _messageUpdatedSinkMocks[1].Verify(sink => sink.MessageUpdated(SUT.GetMessageContainer(2, 22), EventArgs.Empty), Times.Once);
            _messageUpdatedSinkMocks[2].Verify(sink => sink.MessageUpdated(SUT.GetMessageContainer(3, 33), EventArgs.Empty), Times.Never);
        }
    }


    public class When_signalling_multiple_received_messages_for_each_subchannel
        : ReceivedMessagesCacheInteractionSpecs
    {
        protected override void When()
        {
            // Message with one update.
            _messageReceiverMock.Raise (messageReceiver => messageReceiver.MessageReceived += null,
                new MessageReceivedEventArgs(new Message() { Channel = 1, Subchannel = 11, RawValue = "HI!" }));
            // Message with two identical (i.e. redudant) updates.
            _messageReceiverMock.Raise (messageReceiver => messageReceiver.MessageReceived += null,
                new MessageReceivedEventArgs(new Message() { Channel = 2, Subchannel = 22, RawValue = "Hello!" }));
            _messageReceiverMock.Raise (messageReceiver => messageReceiver.MessageReceived += null,
                new MessageReceivedEventArgs(new Message() { Channel = 2, Subchannel = 22, RawValue = "Hello!" }));
            // Message with two real (i.e. non-redudant) updates.
            _messageReceiverMock.Raise (messageReceiver => messageReceiver.MessageReceived += null,
                new MessageReceivedEventArgs(new Message() { Channel = 3, Subchannel = 33, RawValue = "Hello!" }));
            _messageReceiverMock.Raise (messageReceiver => messageReceiver.MessageReceived += null,
                new MessageReceivedEventArgs(new Message() { Channel = 3, Subchannel = 33, RawValue = "Hello again!" }));
        }

        [Test]
        public void then_the_SUTs_message_containers_should_contain_the_most_recently_received_messages()
        {
            SUT.GetMessageContainer(1, 11).Message.RawValue.ShouldEqual("HI!");
            SUT.GetMessageContainer(2, 22).Message.RawValue.ShouldEqual("Hello!");
            SUT.GetMessageContainer(3, 33).Message.RawValue.ShouldEqual("Hello again!");
        }

        [Test]
        public void then_the_SUT_should_raise_events_once_per_real_update()
        {
            _messageUpdatedSinkMocks[0].Verify(sink => sink.MessageUpdated(SUT.GetMessageContainer(1,11), EventArgs.Empty), Times.Once);
            _messageUpdatedSinkMocks[1].Verify(sink => sink.MessageUpdated(SUT.GetMessageContainer(2, 22), EventArgs.Empty), Times.Once);
            _messageUpdatedSinkMocks[2].Verify(sink => sink.MessageUpdated(SUT.GetMessageContainer(3, 33), EventArgs.Empty), Times.Exactly(2));
        }
    }


    public class When_signalling_received_messages_after_unregistering_some_subchannels
        : ReceivedMessagesCacheInteractionSpecs
    {
        protected IMessageContainer _unregisteredMessageContainer;

        protected override void Given()
        {
            base.Given();

            _unregisteredMessageContainer = SUT.GetMessageContainer(1, 11);
            SUT.UnregisterSubchannelsForChannel(1);
        }

        protected override void When()
        {
            _messageReceiverMock.Raise (messageReceiver => messageReceiver.MessageReceived += null,
                new MessageReceivedEventArgs(new Message() { Channel = 1, Subchannel = 11, RawValue = "HI!" }));
            _messageReceiverMock.Raise (messageReceiver => messageReceiver.MessageReceived += null,
                new MessageReceivedEventArgs(new Message() { Channel = 2, Subchannel = 22, RawValue = "Hello!" }));
            _messageReceiverMock.Raise (messageReceiver => messageReceiver.MessageReceived += null,
                new MessageReceivedEventArgs(new Message() { Channel = 3, Subchannel = 33, RawValue = "Hello again!" }));
        }

        [Test]
        public void then_the_SUT_should_update_the_messages_in_the_message_containers_for_registered_subchannels_but_none_else()
        {
            _unregisteredMessageContainer.Message.RawValue.ShouldBeNull();
            SUT.GetMessageContainer(2, 22).Message.RawValue.ShouldEqual("Hello!");
            SUT.GetMessageContainer(3, 33).Message.RawValue.ShouldEqual("Hello again!");
        }

        [Test]
        public void then_the_SUT_should_raise_events_for_the_updated_messages_for_registered_subchannels_but_none_else()
        {
            _messageUpdatedSinkMocks[0].Verify(sink => sink.MessageUpdated(_unregisteredMessageContainer, EventArgs.Empty), Times.Never);
            _messageUpdatedSinkMocks[1].Verify(sink => sink.MessageUpdated(SUT.GetMessageContainer(2, 22), EventArgs.Empty), Times.Once);
            _messageUpdatedSinkMocks[2].Verify(sink => sink.MessageUpdated(SUT.GetMessageContainer(3, 33), EventArgs.Empty), Times.Once);
        }
    }
}
