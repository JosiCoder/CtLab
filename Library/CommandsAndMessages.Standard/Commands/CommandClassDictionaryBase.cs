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
using System.Linq;
using CtLab.CommandsAndMessages.Interfaces;

namespace CtLab.CommandsAndMessages.Standard
{
    /// <summary>
    /// Provides the base functionality for command class dictionaries.
    /// </summary>
    public abstract class CommandClassDictionaryBase<TCommandClass> : ICommandClassDictionary<TCommandClass>
        where TCommandClass : CommandClass
    {
        protected readonly ICommandSender<TCommandClass> _commandSender;
        protected readonly Dictionary<CommandClassKey, TCommandClass> _commandClassDictionary = new Dictionary<CommandClassKey, TCommandClass>();

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="commandSender">The command sender used to send the commands.</param>
        protected CommandClassDictionaryBase(ICommandSender<TCommandClass> commandSender)
        {
            _commandSender = commandSender;
        }

        protected struct CommandClassKey
        {
            public CommandClassKey(CommandClass commandClass)
            {
                Channel = commandClass.Channel;
                Subchannel = commandClass.Subchannel;
            }
            public byte Channel;
            public ushort Subchannel;
        }

        /// <summary>
		/// Adds a command class to the dictionary unless there is already one for the command
        /// classes' combination of channel and subchannel.
        /// </summary>
        /// <param name="commandClass">The command class to add to the dictionary.</param>
        public void Add(TCommandClass commandClass)
        {
            _commandClassDictionary.Add(new CommandClassKey(commandClass), commandClass);
        }

        /// <summary>
        /// For a list of command classes, adds each one to the dictionary unless there is
        /// already one for the command classes' combination of channel and subchannel.
        /// </summary>
        /// <param name="commandClasses">A list of command classes to add to the dictionary.</param>
        public void Add(TCommandClass[] commandClasses)
        {
            foreach (var command in commandClasses)
            {
                Add(command);
            }
        }

        /// <summary>
        /// Removes all command classes belonging to a specific channel.
        /// </summary>
        /// <param name="channel">The channel to remove the commands for.</param>
        public void RemoveCommandsForChannel(byte channel)
        {
            var affectedKeys = (from key in _commandClassDictionary.Keys
                                where key.Channel == channel
                                select key
                               ).ToArray();

            foreach (var key in affectedKeys)
            {
                _commandClassDictionary.Remove(key);
            }
        }

        /// <summary>
        /// Sends commands for all command classes.
        /// </summary>
        public virtual void SendCommands()
        {
            foreach (var commandClass in _commandClassDictionary.Values)
            {
                _commandSender.Send(commandClass);
            }
        }
    }
}