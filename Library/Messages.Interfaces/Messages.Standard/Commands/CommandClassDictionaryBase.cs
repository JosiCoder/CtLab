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

        protected readonly ICommandSender<TCommandClass> _commandSender;
        protected readonly Dictionary<CommandClassKey, CommandClassContainer<TCommandClass>> _containerDictionary
            = new Dictionary<CommandClassKey, CommandClassContainer<TCommandClass>>();

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
        /// <param name="group">The group the command class is related to.</param>
        public void Add(TCommandClass commandClass, CommandClassGroup group)
        {
            _containerDictionary.Add(new CommandClassKey(commandClass),
                new CommandClassContainer<TCommandClass>(commandClass, group));
        }

        /// <summary>
        /// For a list of command classes, adds each one to the dictionary unless there is
        /// already one for the command classes' combination of channel and subchannel.
        /// </summary>
        /// <param name="sendMode">
        /// The send mode used.
        /// </param>
        /// <param name="commandClasses">A list of command classes to add to the dictionary.</param>
        /// <param name="group">The group the command classes are related to.</param>
        public void Add(TCommandClass[] commandClasses, CommandClassGroup group)
        {
            foreach (var command in commandClasses)
            {
                Add(command, group);
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
        /// <param name="predicate">The predicate the commands' class groups must meet.</param>
        public void SendCommands(Predicate<CommandClassGroup> predicate)
        {
            foreach (var container in _containerDictionary.Values
                .Where(container => predicate(container.CommandClassGroup)))
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