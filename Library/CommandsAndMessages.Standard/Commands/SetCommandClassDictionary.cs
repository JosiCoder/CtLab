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
using CtLab.CommandsAndMessages.Interfaces;

namespace CtLab.CommandsAndMessages.Standard
{
    /// <summary>
    /// Maintains a unique set command class for each combination of channel and subchannel.
    /// Sends some or all set commands to get the c't Lab devices in sync.
    /// </summary>
    public class SetCommandClassDictionary : CommandClassDictionaryBase<SetCommandClass>, ISetCommandClassDictionary
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="commandSender">The command sender used to send the commands.</param>
        public SetCommandClassDictionary(ISetCommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>
        /// Sends commands for all set command classes.
        /// </summary>
        public override void SendCommands()
        {
            foreach (var command in _commandClassDictionary.Values)
            {
                _commandSender.Send(command);
                command.ResetValueModified();
            }
        }

        /// <summary>
        /// Sends commands for all set command classes that have modified values to
        /// get the c't Lab devices in sync.
        /// </summary>
        public void SendCommandsForModifiedValues()
        {
            foreach (var command in _commandClassDictionary.Values)
            {
                if (command.ValueModified)
                {
                    _commandSender.Send(command);
                    command.ResetValueModified();
                }
            }
        }
    }
}