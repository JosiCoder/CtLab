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
using CtLab.Messages.Interfaces;
using CtLab.Messages.Standard;

namespace CtLab.Messages.Standard.Specs
{
    public abstract class SetCommandDictionarySpecs
        : SpecsFor<SetCommandClassDictionary>
    {
    }


    public abstract class SetCommandDictionaryWithSampleCommandsSpecs
        : SetCommandDictionarySpecs
    {
        protected SpecsMessageChannel[] _messageChannels = new[]
        {
            new SpecsMessageChannel(1),
            new SpecsMessageChannel(2),
            new SpecsMessageChannel(3)
        };

        protected SetCommandClass[] _setCommands;
            
        protected override void Given()
        {
            base.Given();

            _setCommands = new[]
            {
                new SetCommandClass(_messageChannels[0]),
                new SetCommandClass(_messageChannels[1]),
                new SetCommandClass(_messageChannels[2])
            };

            SUT.Add(_setCommands);
        }
    }


    public abstract class SetCommandDictionaryInteractionSpecs
        : SetCommandDictionaryWithSampleCommandsSpecs
    {
        protected Mock<ICommandSender<SetCommandClass>> _setCommandSenderMock;

        protected override void ConfigureContainer (StructureMap.IContainer container)
        {
            base.ConfigureContainer (container);

            // There is a more specific 'alias' interface available for the set command sender,
            // thus make the container use the same instance for both.
            container.Configure (expression =>
                {
                    expression.Forward
                    <
                        ICommandSender<SetCommandClass>,
                        ISetCommandSender
                    >();
                });
        }

        protected override void Given()
        {
            base.Given();

            _setCommandSenderMock = GetMockFor<ICommandSender<SetCommandClass>>();
        }
    }


    public class When_attempting_to_add_a_set_command_class_for_a_combination_of_channel_and_subchannel_that_already_exists
        : SetCommandDictionaryWithSampleCommandsSpecs
    {
        private Action _theAssertion;

        protected override void When()
        {
            _theAssertion = () => SUT.Add(new SetCommandClass(new SpecsMessageChannel(1)));
        }

        [Test]
        public void then_the_SUT_should_throw_an_exception()
        {
            _theAssertion.ShouldThrow<ArgumentException>();
        }
    }


    public class When_attempting_to_add_a_set_command_more_than_once
        : SetCommandDictionaryWithSampleCommandsSpecs
    {
        private Action _theAssertion;

        protected override void When()
        {
            _theAssertion = () => SUT.Add(_setCommands[0]);
        }

        [Test]
        public void then_the_SUT_should_throw_an_exception()
        {
            _theAssertion.ShouldThrow<ArgumentException>();
        }
    }


    public class When_sending_all_commands_for_modified_values
        : SetCommandDictionaryInteractionSpecs
    {
        protected override void When()
        {
            _setCommands[0].SetValue(2);
            SUT.SendCommandsForModifiedValues();
        }

        [Test]
        public void then_the_SUT_should_send_the_commands_for_modified_values_exactly_once_but_none_else()
        {
            _setCommandSenderMock.Verify(sender => sender.Send(It.IsAny<SetCommandClass>()), Times.Once);


            _setCommandSenderMock.Verify(sender => sender.Send(_setCommands[0]), Times.Once);
            _setCommandSenderMock.Verify(sender => sender.Send(_setCommands[1]), Times.Never);
            _setCommandSenderMock.Verify(sender => sender.Send(_setCommands[2]), Times.Never);
        }
    }


    public class When_sending_commands_for_modified_values_more_than_once
        : SetCommandDictionaryInteractionSpecs
    {
        protected override void When()
        {
            _setCommands[0].SetValue(2);
            SUT.SendCommandsForModifiedValues();
            SUT.SendCommandsForModifiedValues();
        }

        [Test]
        public void then_the_SUT_should_send_the_commands_for_modified_values_exactly_once_but_none_else()
        {
            _setCommandSenderMock.Verify(sender => sender.Send(_setCommands[0]), Times.Once);
            _setCommandSenderMock.Verify(sender => sender.Send(_setCommands[1]), Times.Never);
            _setCommandSenderMock.Verify(sender => sender.Send(_setCommands[2]), Times.Never);
        }
    }


    public class When_sending_commands_for_modified_values_after_removing_some_command_classes_from_the_command_class_dictionary
        : SetCommandDictionaryInteractionSpecs
    {
        protected override void When()
        {
            _setCommands[0].SetValue(2);
            _setCommands[1].SetValue(2);
            _setCommands[2].SetValue(2);
            SUT.RemoveChannelCommands(ch => ch.Equals(_messageChannels[0]));
            SUT.SendCommandsForModifiedValues();
        }

        [Test]
        public void then_the_SUT_should_send_the_commands_for_classes_in_the_command_class_dictionary_but_none_else()
        {
            _setCommandSenderMock.Verify(sender => sender.Send(_setCommands[0]), Times.Never);
            _setCommandSenderMock.Verify(sender => sender.Send(_setCommands[1]), Times.Once);
            _setCommandSenderMock.Verify(sender => sender.Send(_setCommands[2]), Times.Once);
        }
    }


    public class When_sending_commands_even_those_for_unmodified_values_after_removing_some_command_classes_from_the_command_class_dictionary
        : SetCommandDictionaryInteractionSpecs
    {
        protected override void When()
        {
            _setCommands[0].SetValue(2);
            _setCommands[1].SetValue(2);
            SUT.RemoveChannelCommands(ch => ch.Equals(_messageChannels[0]));
            SUT.SendCommands();
        }

        [Test]
        public void then_the_SUT_should_send_the_commands_for_classes_in_the_command_class_dictionary_but_none_else()
        {
            _setCommandSenderMock.Verify(sender => sender.Send(_setCommands[0]), Times.Never);
            _setCommandSenderMock.Verify(sender => sender.Send(_setCommands[1]), Times.Once);
            _setCommandSenderMock.Verify(sender => sender.Send(_setCommands[2]), Times.Once);
        }
    }


    public class When_sending_commands_for_modified_values_after_sending_all_commands_even_those_for_unmodified_values
        : SetCommandDictionaryInteractionSpecs
    {
        protected override void When()
        {
            _setCommands[0].SetValue(2);
            SUT.SendCommands();
            SUT.SendCommandsForModifiedValues();
        }

        [Test]
        public void then_the_SUT_should_send_all_commands_exactly_once()
        {
            _setCommandSenderMock.Verify(sender => sender.Send(_setCommands[0]), Times.Once);
            _setCommandSenderMock.Verify(sender => sender.Send(_setCommands[1]), Times.Once);
            _setCommandSenderMock.Verify(sender => sender.Send(_setCommands[2]), Times.Once);
        }
    }
}
