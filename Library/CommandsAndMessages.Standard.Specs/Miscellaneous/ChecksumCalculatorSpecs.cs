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
using CtLab.CtLabProtocol.Standard;

namespace CtLab.CtLabProtocol.Specs
{
    public abstract class ChecksumCalculatorSpecs
		: SpecsFor<ChecksumCalculator>
    {
        protected byte _calculatedChecksum;
    }


    public class When_calculating_the_checksum_of_a_string
        : ChecksumCalculatorSpecs
    {
		protected override void When()
        {
            _calculatedChecksum = SUT.Calculate("Hallo Carsten");
        }

		[Test]
        public void then_the_SUT_should_return_the_correct_checksum()
        {
			_calculatedChecksum.ShouldEqual((byte)0x3A);
        }
    }
}
