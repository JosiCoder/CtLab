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
using System.Collections.Generic;
using System.Linq;
using CtLab.Messages.Interfaces;

namespace CtLab.Messages.Standard
{
    /// <summary>
    /// Provides the base functionality for command class dictionaries.
    /// </summary>
    /// <typeparam name="TCommandClass">The type of the command classes in the dictionary.</typeparam>
    public abstract class CommandClassDictionaryBase<TCommandClass> : ICommandClassDictionary<TCommandClass>
        where TCommandClass : CommandClassBase
    {
        protected struct CommandClassKey
        {
            public CommandClassKey(CommandClassBase commandClass)
            {
                Channel = commandClass.Channel;
            }
            public readonly IMessageChannel Channel;
        }

        public class CommandClassContainer
        {
            public CommandClassContainer(TCommandClass commandClass, SendMode sendMode)
            {
                CommandClass = commandClass;
                SendMode = sendMode;
            }
            public readonly TCommandClass CommandClass;
            public readonly SendMode SendMode;
        }

        protected readonly ICommandSender<TCommandClass> _commandSender;
        protected readonly Dictionary<CommandClassKey, CommandClassContainer> _containerDictionary
            = new Dictionary<CommandClassKey, CommandClassContainer>();

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="commandSender">The command sender used to send the commands.</param>
        protected CommandClassDictionaryBase(ICommandSender<TCommandClass> commandSender)
        {
            _commandSender = commandSender;
        }

        /// <summary>
		/// Adds a command class to the dictionary unless there is already one for the command
        /// classes' combination of channel and subchannel.
        /// </summary>
        /// <param name="sendMode">
        /// The send mode used.
        /// </param>
        /// <param name="commandClass">The command class to add to the dictionary.</param>
        public void Add(TCommandClass commandClass, SendMode sendMode)
        {
            _containerDictionary.Add(new CommandClassKey(commandClass),
                new CommandClassContainer(commandClass, sendMode));
        }

        /// <summary>
        /// For a list of command classes, adds each one to the dictionary unless there is
        /// already one for the command classes' combination of channel and subchannel.
        /// </summary>
        /// <param name="sendMode">
        /// The send mode used.
        /// </param>
        /// <param name="commandClasses">A list of command classes to add to the dictionary.</param>
        public void Add(TCommandClass[] commandClasses, SendMode sendMode)
        {
            foreach (var command in commandClasses)
            {
                Add(command, sendMode);
            }
        }

        /// <summary>
        /// Removes the commands for all channels that meet the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate that must be met.</param>
        public void RemoveChannelCommands(Func<IMessageChannel, bool> predicate)
        {
            var affectedKeys = (from key in _containerDictionary.Keys
                                where predicate(key.Channel)
                                select key
                           ).ToArray();

            foreach (var key in affectedKeys)
            {
                _containerDictionary.Remove(key);
            }
        }

        /// <summary>
        /// Sends commands for all command classes.
        /// </summary>
        /// <param name="predicate">The predicate that must be met.</param>
        public void SendCommands(Predicate<SendMode> predicate)
        {
            foreach (var container in _containerDictionary.Values
                .Where(container => predicate(container.SendMode)))
            {
                var commandClass = container.CommandClass;
                _commandSender.Send(commandClass);
                PostSendCommandHook(commandClass);
            }
        }

        protected virtual void PostSendCommandHook(TCommandClass commandClass)
        {
        }
    }
}