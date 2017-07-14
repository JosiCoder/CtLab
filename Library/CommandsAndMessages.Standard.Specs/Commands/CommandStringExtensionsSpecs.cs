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
using CtLab.CommandsAndMessages.Standard;

namespace CtLab.CommandsAndMessages.Specs
{
    public abstract class CommandStringExtensionsSpecs
        : SpecsFor<object>
    {
        protected string _sampleString;
    }


    public class When_getting_the_bytes_from_a_command_string
        : CommandStringExtensionsSpecs
    {
        protected override void When()
        {
            _sampleString = "123 ABC abc";
        }

        [Test]
        public void then_the_SUT_should_return_the_correct_bytes()
        {
            _sampleString.GetBytes().ShouldEqual(new byte[] { 0x31, 0x32, 0x33, 0x20, 0x41, 0x42, 0x43, 0x20, 0x61, 0x62, 0x63 });
        }
    }


    public class When_trimming_the_checksum_from_a_command_string
        : CommandStringExtensionsSpecs
    {
        protected override void When()
        {
            _sampleString = "command$sum";
        }

        [Test]
        public void then_the_SUT_should_return_the_correctly_trimmed_command_string()
        {
            _sampleString.TrimChecksum().ShouldEqual("command");
        }
    }
}
