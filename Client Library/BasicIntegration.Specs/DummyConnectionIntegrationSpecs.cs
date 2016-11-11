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
using CtLab.Connection.Interfaces;
using CtLab.Connection.Dummy;

namespace CtLab.BasicIntegration.Specs
{
    public abstract class DummyConnectionIntegrationSpecs
        : SpecsFor<object>
    {
        protected Container _container;

        protected override void Given()
        {
            base.Given();

            _container = new Container (expression =>
                {
                    expression.AddRegistry<DummyConnectionRegistry>();
                });
        }
    }

    public class When_getting_the_dummy_connection_multiple_times
        : DummyConnectionIntegrationSpecs
    {
        protected override void When()
        {
        }

        [Test]
        public void it_should_get_the_same_instance()
        {
            var instance1 = _container.GetInstance<IConnection>();
            var instance2 = _container.GetInstance<IConnection>();
            instance2.ShouldBeSameAs(instance1);
        }
    }

    public class When_getting_the_dummy_connection_in_a_generic_and_in_a_specific_way
        : DummyConnectionIntegrationSpecs
    {
        protected override void When()
        {
        }

        [Test]
        public void it_should_get_the_same_instance()
        {
            var instance1 = _container.GetInstance<IConnection>();
            var instance2 = _container.GetInstance<DummyConnection>();
            instance2.ShouldBeSameAs(instance1);
        }
    }

    public class When_getting_dummy_string_sender_and_receiver_multiple_times
        : DummyConnectionIntegrationSpecs
    {
        protected override void When()
        {
        }

        [Test]
        public void it_should_get_the_same_instance_for_the_sender()
        {
            var instance1 = _container.GetInstance<IStringSender>();
            var instance2 = _container.GetInstance<IStringSender>();
            instance2.ShouldBeSameAs(instance1);
        }

        [Test]
        public void it_should_get_the_same_instance_for_the_receiver()
        {
            var instance1 = _container.GetInstance<IStringReceiver>();
            var instance2 = _container.GetInstance<IStringReceiver>();
            instance2.ShouldBeSameAs(instance1);
        }
    }
}
