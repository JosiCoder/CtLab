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

namespace CtLab.BasicIntegration.Specs
{
    public interface IMessageUpdatedSink
    {
        void MessageUpdated(object sender, EventArgs e);
    }


    public abstract class ReceivedMessageCacheIntegrationSpecs
        : SpecsFor<Container>
    {
        protected override void InitializeClassUnderTest()
        {
            SUT = new Container (expression =>
                {
                    expression.AddRegistry<CommandsAndMessagesRegistry>();
                    expression.For<IMessageReceiver>().Use(GetMockFor<IMessageReceiver>().Object);
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


    public class When_getting_the_message_cache_more_than_once
        : ReceivedMessageCacheIntegrationSpecs
    {
        protected override void When()
        {
        }

        [Test]
        public void then_the_SUT_should_get_the_same_instance()
        {
            var instance1 = SUT.GetInstance<IMessageCache>();
            var instance2 = SUT.GetInstance<IMessageCache>();
            instance2.ShouldBeSameAs(instance1);
        }
    }
}
