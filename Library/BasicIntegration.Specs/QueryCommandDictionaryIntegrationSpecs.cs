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

namespace CtLab.BasicIntegration.Specs
{
    public abstract class QueryCommandDictionaryIntegrationSpecs
        : SpecsFor<Container>
    {
        protected override void InitializeClassUnderTest()
        {
            SUT = new Container (expression =>
                {
                    expression.AddRegistry<CommandsAndMessagesRegistry>();
                    expression.For<IQueryCommandSender>().Use(GetMockFor<IQueryCommandSender>().Object);
                });
        }
    }


    public class When_getting_the_query_command_class_dictionary_more_than_once
        : QueryCommandDictionaryIntegrationSpecs
    {
        protected override void When()
        {
        }

        [Test]
        public void then_the_SUT_should_return_the_same_instance()
        {
            var instance1 = SUT.GetInstance<IQueryCommandClassDictionary>();
            var instance2 = SUT.GetInstance<IQueryCommandClassDictionary>();
            instance2.ShouldBeSameAs(instance1);
        }
    }
}
