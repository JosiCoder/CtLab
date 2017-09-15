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

using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using CtLab.Messages.Interfaces;
using CtLab.CommandsAndMessages.Interfaces;

namespace CtLab.CommandsAndMessages.Standard
{
    /// <summary>
    /// Parses message strings that may have been received from c't Lab devices.
    /// </summary>
    public class MessageParser : IMessageParser
    {
        private Regex regularExpression =
            new Regex(
                @"^#(?<channel>\d+)\s*:\s*(?<subchannel>\d+)\s*=\s*(?<value>\d+)\s*(\[(?<description>.*)\])?\s*$",
                RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// Parses a message string that may have been received from one or more c't Lab
        /// devices. A message string might contain multiple lines from the same or different
        /// devices.
		/// Each line is expected to look like "#7:255=33 [OK]", where '7' is the device's
		/// channel, '255' is the command's subchannel, '33' is the value and 'DSCR' is
        /// a description. The description is optional.
        /// </summary>
        /// <param name="messageString">The message string to be parsed.</param>
        /// <returns>A list of messages, each per line in the message string.</returns>
        public Message<CtLabMessageChannel>[] Parse(string messageString)
        {
            var messageList = new List<Message<CtLabMessageChannel>>();
            foreach (Match match in regularExpression.Matches(messageString))
            {
                var groups = match.Groups;
                var message = new Message<CtLabMessageChannel>
                {
                    Channel = new CtLabMessageChannel
                    {
                        Channel = byte.Parse(groups["channel"].Value, CultureInfo.InvariantCulture),
                        Subchannel = ushort.Parse(groups["subchannel"].Value, CultureInfo.InvariantCulture),
                    },
                    RawValue = groups["value"].Value,
                    Description = groups["description"].Value,
                };
                messageList.Add(message);
            }
            return messageList.ToArray();
        }
    }
}