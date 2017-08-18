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
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.FpgaSignalGenerator.Standard;

namespace CtLab.FpgaSignalGenerator.Specs
{
    public abstract class OutputSourceWriterSpecs
        : FpgaWriterSpecs<OutputSourceWriter>
    {
    }


    public class When_setting_the_output_sources
        : OutputSourceWriterSpecs
    {
        protected override void When()
        {
            SUT.OutputSource0 = OutputSource.DdsGenerator1;
            SUT.OutputSource1 = OutputSource.PulseGenerator;
            SUT.OutputSource0 = OutputSource.DdsGenerator2;
        }

        [Test]
        public void then_the_SUT_should_pass_the_combined_values_to_the_FPGA_writer_to_reflect_the_most_recent_changes()
        {
            _lastValueSet.ShouldEqual((uint)0x00000042);
        }
    }
}
