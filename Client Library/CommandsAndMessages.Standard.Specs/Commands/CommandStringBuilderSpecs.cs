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
using CtLab.CommandsAndMessages.Interfaces;
using CtLab.CommandsAndMessages.Standard;

namespace CtLab.CommandsAndMessages.Specs
{
    public abstract class CommandStringBuilderSpecs
        : SpecsFor<CommandStringBuilder>
    {
        protected string _builtCommandString;
    }


    public abstract class CommandStringBuilderInteractionSpecs
        : CommandStringBuilderSpecs
    {
        protected Mock<IChecksumCalculator> _checksumCalculatorMock;

        protected override void Given()
        {
            base.Given();

            _checksumCalculatorMock = GetMockFor<IChecksumCalculator> ();
            _checksumCalculatorMock.Setup(calc => calc.Calculate(It.IsAny<string>())).Returns(0x8);
        }
    }


    public class When_building_the_string_for_a_set_command
        : CommandStringBuilderSpecs
    {
        protected override void When()
        {
            var command = new SetCommandClass(1, 15);
            command.SetValue(2.5);
            _builtCommandString = SUT.BuildCommand(command);
        }

        [Test]
        public void it_should_return_a_standard_set_command_string()
        {
            _builtCommandString.ShouldEqual("1:15=2.5");
        }
    }


    public class When_building_the_string_for_a_set_command_including_an_acknowledge_request
        : CommandStringBuilderSpecs
    {
        protected override void When()
        {
            var command = new SetCommandClass(1, 20);
            command.SetValue(2);
            _builtCommandString = SUT.BuildCommand(command, false, true);
        }

        [Test]
        public void it_should_return_a_standard_set_command_string_including_an_exclamation_mark()
        {
            _builtCommandString.ShouldEqual("1:20=2!");
        }
    }


    public class When_building_the_string_for_a_set_command_including_a_checksum
        : CommandStringBuilderInteractionSpecs
    {
        protected override void When()
        {
            var command = new SetCommandClass(1, 20);
            command.SetValue(1);
            _builtCommandString = SUT.BuildCommand(command, true, false);
        }

        [Test]
        public void it_should_call_the_checksum_calculator_with_the_right_command_string()
        {
            _checksumCalculatorMock.Verify(calc => calc.Calculate(_builtCommandString.TrimChecksum()), Times.Once);
        }

        [Test]
        public void it_should_return_a_standard_set_command_string_including_a_checksum()
        {
            _builtCommandString.ShouldEqual("1:20=1$08");
        }
    }


    public class When_building_the_string_for_a_set_command_including_a_checksum_and_an_acknowledge_request
        : CommandStringBuilderInteractionSpecs
    {
        protected override void When()
        {
            var command = new SetCommandClass(1, 20);
            command.SetValue(1);
            _builtCommandString = SUT.BuildCommand(command, true, true);
        }

        [Test]
        public void it_should_call_the_checksum_calculator_with_the_right_command_string()
        {
            _checksumCalculatorMock.Verify(calc => calc.Calculate(_builtCommandString.TrimChecksum()), Times.Once);
        }

        [Test]
        public void it_should_return_a_standard_set_command_string_including_an_exclamation_mark_and_a_checksum()
        {
            _builtCommandString.ShouldEqual("1:20=1!$08");
        }
    }


    public class When_attempting_to_build_the_string_for_a_set_command_to_the_default_channel
        : CommandStringBuilderSpecs
    {
        protected override void When()
        {
            var command = new SetCommandClass(SUT.DefaultChannel, 20);
            command.SetValue(1);
            _builtCommandString = SUT.BuildCommand(command);
        }

        [Test]
        public void it_should_return_a_set_command_string_without_a_channel()
        {
            _builtCommandString.ShouldEqual("20=1");
        }
    }


    public class When_building_the_string_for_a_broadcast_set_command
        : CommandStringBuilderSpecs
    {
        protected override void When()
        {
            var command = new SetCommandClass(SUT.BroadcastChannel, 25);
            command.SetValue(1);
            _builtCommandString = SUT.BuildCommand(command);
        }

        [Test]
        public void it_should_return_a_broadcast_set_command_string()
        {
            _builtCommandString.ShouldEqual("*:25=1");
        }
    }


    public class When_building_the_string_for_a_query_command : CommandStringBuilderSpecs
    {
        protected override void When()
        {
            var command = new QueryCommandClass(1, 30);
            _builtCommandString = SUT.BuildCommand(command);
        }

        [Test]
        public void it_should_return_a_standard_query_command_string()
        {
            _builtCommandString.ShouldEqual("1:30?");
        }
    }


    public class When_building_the_string_for_a_query_command_including_a_checksum
        : CommandStringBuilderInteractionSpecs
    {
        protected override void When()
        {
            var command = new QueryCommandClass(1, 30);
            _builtCommandString = SUT.BuildCommand(command, true);
        }

        [Test]
        public void it_should_call_the_checksum_calculator_with_the_right_command_string()
        {
            _checksumCalculatorMock.Verify(calc => calc.Calculate(_builtCommandString.TrimChecksum()), Times.Once);
        }

        [Test]
        public void it_should_return_a_standard_query_command_string_including_a_checksum()
        {
            _builtCommandString.ShouldEqual("1:30?$08");
        }
    }


    public class When_building_the_string_for_a_query_command_to_the_default_channel
        : CommandStringBuilderSpecs
    {
        protected override void When()
        {
            var command = new QueryCommandClass(SUT.DefaultChannel, 30);
            _builtCommandString = SUT.BuildCommand(command);
        }

        [Test]
        public void it_should_return_a_query_command_string_without_a_channel()
        {
            _builtCommandString.ShouldEqual("30?");
        }
    }


    public class When_building_the_string_for_a_broadcast_query_command
        : CommandStringBuilderSpecs
    {
        protected override void When()
        {
            var command = new QueryCommandClass(SUT.BroadcastChannel, 35);
            _builtCommandString = SUT.BuildCommand(command);
        }

        [Test]
        public void it_should_return_a_broadcast_query_command_string()
        {
            _builtCommandString.ShouldEqual("*:35?");
        }
    }
}
