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
using CtLab.SpiConnection.Interfaces;

namespace CtLab.SpiDirect.Integration.Specs
{
    public abstract class SpiConnectionIntegrationSpecs
        : SpecsFor<Container>
    {
        protected override void InitializeClassUnderTest()
        {
            SUT = new Container (expression =>
                {
                    expression.AddRegistry<SpiConnectionRegistry>();
                });
        }
    }

    public class When_getting_the_SPI_connection_multiple_times
        : SpiConnectionIntegrationSpecs
    {
        protected override void When()
        {
        }

        [Test]
        public void then_the_SUT_should_return_the_same_instance()
        {
            var instance1 = SUT.GetInstance<IConnection>();
            var instance2 = SUT.GetInstance<IConnection>();
            instance2.ShouldBeSameAs(instance1);
        }
    }

    public class When_getting_the_SPI_connection_in_a_generic_and_in_a_specific_way
        : SpiConnectionIntegrationSpecs
    {
        protected override void When()
        {
        }

        [Test]
        public void then_the_SUT_should_return_the_same_instance()
        {
            var instance1 = SUT.GetInstance<IConnection>();
            var instance2 = SUT.GetInstance<DummyConnection>();
            instance2.ShouldBeSameAs(instance1);
        }
    }

    public class When_getting_SPI_sender_and_receiver_multiple_times
        : SpiConnectionIntegrationSpecs
    {
        protected override void When()
        {
        }

        [Test]
        public void then_the_SUT_should_return_the_same_instance_for_the_sender()
        {
            var instance1 = SUT.GetInstance<ISpiSender>();
            var instance2 = SUT.GetInstance<ISpiSender>();
            instance2.ShouldBeSameAs(instance1);
        }

        [Test]
        public void then_the_SUT_should_return_the_same_instance_for_the_receiver()
        {
            var instance1 = SUT.GetInstance<ISpiReceiver>();
            var instance2 = SUT.GetInstance<ISpiReceiver>();
            instance2.ShouldBeSameAs(instance1);
        }
    }
}
