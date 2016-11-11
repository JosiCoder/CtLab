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
using System.Globalization;
using CtLab.CommandsAndMessages.Interfaces;

namespace CtLab.CommandsAndMessages.Standard
{
    /// <summary>
    /// Builds strings from commands that can be be sent to c't Lab devices.
    /// </summary>
    public class CommandStringBuilder : ISetCommandStringBuilder, IQueryCommandStringBuilder
    {
        private const string _setCommandWithChannelFormatString = "{0}:{1}={2}";
        private const string _setCommandWithoutChannelFormatString = "{1}={2}";
        private const string _setCommandWithBroadcastChannelFormatString = "*:{1}={2}";

        private const string _queryCommandWithChannelFormatString = "{0}:{1}?";
        private const string _queryCommandWithoutChannelFormatString = "{1}?";
        private const string _queryCommandWithBroadcastChannelFormatString = "*:{1}?";

        private const string _commandWithAcknowledgeRequestFormatString = "{0}!";
        private const string _commandWithChecksumFormatString = "{0}${1:X2}";

        /// <summary>
        /// Gets a pseudo channel number used to specify that a command is to be sent
        /// without any channel number which relies on a specific default handling by
        /// the devices.
        /// </summary>
        public byte DefaultChannel { get { return 254; } }

        /// <summary>
        /// Gets a pseudo channel number used to specify that a command is to be sent
        /// as a broadcast. All channels will accept the command. This isn't a real
        /// channel number.
        /// </summary>
        public byte BroadcastChannel { get { return 255; } }

        /// <summary>
        /// Gets the checksum calculator used.
        /// </summary>
        public readonly IChecksumCalculator ChecksumCalculator;

        /// <summary>
        /// Initialzies a new instance of this class.
        /// </summary>
        /// <param name="checksumCalculator">The checksum calculator to be used by this instance.</param>
        public CommandStringBuilder(IChecksumCalculator checksumCalculator)
        {
            ChecksumCalculator = checksumCalculator;
        }

        /// <summary>
		/// Builds a set command string using the command classes's channel and subchannel.
        /// </summary>
        /// <param name="commandClass">The command class to build the command string for.</param>
        /// <returns>The built command string.</returns>
        public string BuildCommand(SetCommandClass commandClass)
        {
            return BuildCommand(commandClass, false, false);
        }

        /// <summary>
        /// Builds a set command string using the command classes's channel and subchannel.
        /// </summary>
        /// <param name="commandClass">The command class to build the command string for.</param>
        /// <param name="generateChecksum">true to append the checksum; otherwiese, false.</param>
        /// <param name="requestAcknowledge">true to request an acknowledge from the receiver; otherwiese, false.</param>
        /// <returns>The built command string.</returns>
        public string BuildCommand(SetCommandClass commandClass, bool generateChecksum, bool requestAcknowledge)
        {
            string formatString;

            if (commandClass.Channel == DefaultChannel)
                formatString = _setCommandWithoutChannelFormatString;
            else
                formatString = (commandClass.Channel == BroadcastChannel)
                    ? _setCommandWithBroadcastChannelFormatString
                    : _setCommandWithChannelFormatString;

            // Generate basic command string.
            var commandString = String.Format(CultureInfo.InvariantCulture, formatString,
                                                 commandClass.Channel, commandClass.Subchannel, commandClass.RawValue);

            // Optionally add acknowledge request.
            commandString = requestAcknowledge
                                ? String.Format(_commandWithAcknowledgeRequestFormatString, commandString)
                                : commandString;

            // Optionally add checksum.
            commandString = generateChecksum
                                ? String.Format(_commandWithChecksumFormatString, commandString,
                                                ChecksumCalculator.Calculate(commandString))
                                : commandString;

            return commandString;
        }

        /// <summary>
        /// Builds a query command string using the command classes's channel and subchannel.
        /// </summary>
        /// <param name="commandClass">The command class to build the command string for.</param>
        /// <returns>The built command string.</returns>
        public string BuildCommand(QueryCommandClass commandClass)
        {
            return BuildCommand(commandClass, false);
        }

        /// <summary>
        /// Builds a query command string using the command classes' channel and subchannel.
        /// </summary>
        /// <param name="commandClass">The command class to build the string for.</param>
        /// <param name="generateChecksum">true to append the checksum; otherwiese, false.</param>
        /// <returns>The built command string.</returns>
        public string BuildCommand(QueryCommandClass commandClass, bool generateChecksum)
        {
            string formatString;

            if (commandClass.Channel == DefaultChannel)
                formatString = _queryCommandWithoutChannelFormatString;
            else
                formatString = (commandClass.Channel == BroadcastChannel)
                    ? _queryCommandWithBroadcastChannelFormatString
                    : _queryCommandWithChannelFormatString;

            // Generate basic command string.
            var commandString = String.Format(CultureInfo.InvariantCulture,
                formatString, commandClass.Channel, commandClass.Subchannel);

            // Optionally add checksum.
            commandString = generateChecksum
                                ? String.Format(_commandWithChecksumFormatString, commandString,
                                                ChecksumCalculator.Calculate(commandString))
                                : commandString;

            return commandString;
        }
    }
}