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
using System.Threading;
using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using Moq;
using CtLab.CommandsAndMessages.Interfaces;
using CtLab.CommandsAndMessages.Standard;

namespace CtLab.CommandsAndMessages.Specs
{
    public abstract class QueryCommandSchedulerSpecs
        : SpecsFor<QueryCommandScheduler>
    {
    }


    public abstract class QueryCommandSchedulerInteractionSpecs
        : QueryCommandSchedulerSpecs
    {
        protected Mock<IQueryCommandClassDictionary> _queryCommandDictionaryMock;

        protected override void Given()
        {
            base.Given();

            _queryCommandDictionaryMock = GetMockFor<IQueryCommandClassDictionary> ();                
        }
    }


    public class When_sending_the_scheduled_commands_immediately
        : QueryCommandSchedulerInteractionSpecs
    {
        protected override void When()
        {
            SUT.SendImmediately();
        }

        [Test]
        public void then_the_SUT_should_immediately_tell_the_associated_dictionary_exactly_once_to_send_the_commands()
        {
            _queryCommandDictionaryMock.Verify(sender => sender.SendCommands(), Times.Once);
        }
    }


    public class When_sending_the_scheduled_commands_periodically
        : QueryCommandSchedulerInteractionSpecs
    {
        private const int _period = 100; // ms
        private DateTime _startTime;

        protected override void When()
        {
            _startTime = DateTime.Now;
            SUT.StartSending(_period);
        }

        [Test]
        public void then_the_SUT_should_tell_the_associated_dictionary_exactly_once_almost_immediately_to_send_the_commands()
        {
            Thread.Sleep(_period/2);
            _queryCommandDictionaryMock.Verify(sender => sender.SendCommands(), Times.Once);
        }


        [Test]
        public void then_the_SUT_should_tell_the_associated_dictionary_once_per_period_to_send_the_commands()
        {
            while ((DateTime.Now - _startTime).TotalMilliseconds < 5 * _period)
            {
                Thread.Sleep(_period/5);
            }
            _queryCommandDictionaryMock.Verify(sender => sender.SendCommands(), Times.Between(5, 6, Range.Inclusive));
        }
    }
}
