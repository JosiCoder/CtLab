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
using CtLab.CommandsAndMessages.Interfaces;

namespace CtLab.BasicIntegration.Specs
{
    public abstract class SetCommandDictionaryIntegrationSpecs
        : SpecsFor<Container>
    {
        protected Mock<IStringSender> _stringSenderMock;

        protected override void InitializeClassUnderTest()
        {
            base.Given ();

            // Use a mock that we can query whether a method has been called.
            _stringSenderMock = GetMockFor<IStringSender>();

            SUT = new Container (expression =>
                {
                    expression.AddRegistry<CommandsAndMessagesRegistry>();
                    expression.For<IStringSender>().Use(_stringSenderMock.Object);
                });
        }
    }


    public class When_getting_the_set_command_cache_more_than_once
        : SetCommandDictionaryIntegrationSpecs
    {
        protected override void When()
        {
        }

        [Test]
        public void then_the_SUT_should_get_the_same_instance()
        {
            var instance1 = SUT.GetInstance<ISetCommandClassDictionary<MessageChannel>>();
            var instance2 = SUT.GetInstance<ISetCommandClassDictionary<MessageChannel>>();
            instance2.ShouldBeSameAs(instance1);
        }
    }


    public class When_sending_a_command_for_a_set_command_class_in_the_dictionary
        : SetCommandDictionaryIntegrationSpecs
    {
        protected override void When()
        {
            var setCommandCLassDict = SUT.GetInstance<ISetCommandClassDictionary<MessageChannel>>();
            var setCommandClass = new SetCommandClass<MessageChannel>(new MessageChannel(1, 11));
            setCommandCLassDict.Add(setCommandClass);
            setCommandClass.SetValue(15);
            setCommandCLassDict.SendCommandsForModifiedValues();
        }

        [Test]
        public void then_the_SUT_should_send_the_command_string_including_the_checksum_but_without_an_acknowledge_request()
        {
            _stringSenderMock.Verify(sender => sender.Send("1:11=15$32"), Times.Once);
        }
    }
}
