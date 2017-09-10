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

using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using Moq;
using CtLab.FpgaSignalGenerator.Standard;

namespace CtLab.FpgaSignalGenerator.Standard.Specs
{
    public abstract class UniversalCounterStatusReaderSpecs
        : FpgaReaderSpecs<UniversalCounterStatusReader>
    {
    }


    public class When_getting_a_counter_status_from_a_raw_value_with_active_input_and_without_overflow
        : UniversalCounterStatusReaderSpecs
    {
        protected override void When()
        {
            _valueGetterMock.Setup(getter => getter.ValueAsUInt32).Returns(1);
        }

        [Test]
        public void then_the_SUT_should_report_an_active_signal()
        {
            SUT.InputSignalActive.ShouldEqual(true);
        }

        [Test]
        public void then_the_SUT_should_not_report_an_overflow()
        {
            SUT.Overflow.ShouldEqual(false);
        }
    }

    public class When_getting_a_counter_status_from_a_raw_value_with_active_input_and_overflow
        : UniversalCounterStatusReaderSpecs
    {
        protected override void When()
        {
            _valueGetterMock.Setup(getter => getter.ValueAsUInt32).Returns(3);
        }

        [Test]
        public void then_the_SUT_should_report_an_active_signal()
        {
            SUT.InputSignalActive.ShouldEqual(true);
        }

        [Test]
        public void then_the_SUT_should_report_an_overflow()
        {
            SUT.Overflow.ShouldEqual(true);
        }
    }
}
