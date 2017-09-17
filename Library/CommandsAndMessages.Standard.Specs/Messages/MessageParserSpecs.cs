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
using ExpectedObjects;
using SpecsFor.ShouldExtensions;
using CtLab.Messages.Interfaces;
using CtLab.CommandsAndMessages.Interfaces;
using CtLab.CommandsAndMessages.Standard;

namespace CtLab.CommandsAndMessages.Specs
{
    public abstract class MessageParserSpecs
        : SpecsFor<MessageParser>
    {
        protected Message<MessageChannel>[] _message;
    }

    public class When_parsing_a_single_line_message_string
        : MessageParserSpecs
    {
        protected override void When()
        {
            _message = SUT.Parse("#7:255=33 [DSCR]");
        }

        [Test]
        public void then_the_SUT_should_return_the_correctly_parsed_response()
        {
            _message.ShouldLookLike(new[]
            {
                new Message<MessageChannel>  
                {
                    Channel = new MessageChannel
                    {
                        Main = 7,
                        Sub = 255,
                    },
                    RawValue = "33",
                    Description = "DSCR"
                }
            });
        }
    }


    public class When_parsing_a_multiple_lines_message_string
        : MessageParserSpecs
    {
        protected override void When()
        {
            _message = SUT.Parse("#7:255=33 [DSCR]\n#6:254=22 [NEXT]");
        }

        [Test]
        public void then_the_SUT_should_return_the_correctly_parsed_response()
        {
            _message.ShouldLookLike(new[]
            {
                new Message<MessageChannel>  
                {
                    Channel = new MessageChannel
                    {
                        Main = 7,
                        Sub = 255,
                    },
                    RawValue = "33",
                    Description = "DSCR"
                },
                new Message<MessageChannel>  
                {
                    Channel = new MessageChannel
                    {
                        Main = 6,
                        Sub = 254,
                    },
                    RawValue = "22",
                    Description = "NEXT"
                }
            });
        }
    }
}
