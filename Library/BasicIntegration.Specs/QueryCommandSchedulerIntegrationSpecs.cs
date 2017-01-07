﻿//------------------------------------------------------------------------------
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
using CtLab.Connection.Interfaces;
using CtLab.CommandsAndMessages.Interfaces;

namespace CtLab.BasicIntegration.Specs
{
    public abstract class QueryCommandSchedulerIntegrationSpecs
        : SpecsFor<object>
    {
        protected Container _container;
        protected Mock<IStringSender> _stringSenderMock;

        protected override void Given()
        {
            base.Given ();

            // Use a mock that we can query whether a method has been called.
            _stringSenderMock = GetMockFor<IStringSender>();

            _container = new Container (expression =>
                {
                    expression.AddRegistry<CommandsAndMessagesRegistry>();
                    expression.For<IStringSender>().Use(_stringSenderMock.Object);
                });
        }
    }


    public class When_getting_the_query_command_scheduler_more_than_once
        : QueryCommandSchedulerIntegrationSpecs
    {
        protected override void When()
        {
        }

        [Test]
        public void it_should_get_the_same_instance()
        {
            var instance1 = _container.GetInstance<IQueryCommandScheduler>();
            var instance2 = _container.GetInstance<IQueryCommandScheduler>();
            instance2.ShouldBeSameAs(instance1);
        }
    }


    public class When_sending_the_scheduled_query_commands_immediately
        : QueryCommandSchedulerIntegrationSpecs
    {
        protected override void When()
        {
            var queryCommandDictionary = _container.GetInstance<IQueryCommandClassDictionary>();
            var queryCommand = new QueryCommandClass(1, 11);
            queryCommandDictionary.Add(queryCommand);
            
            var queryCommandScheduler = _container.With(queryCommandDictionary).GetInstance<IQueryCommandScheduler>();
            queryCommandScheduler.SendImmediately();
        }

        [Test]
        public void it_should_tell_the_underlying_string_sender_to_send_the_command_strings()
        {
            _stringSenderMock.Verify(sender => sender.Send("1:11?$34"), Times.Once);
        }
    }
}