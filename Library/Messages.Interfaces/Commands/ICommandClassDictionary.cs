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

namespace CtLab.Messages.Interfaces
{
    /// <summary>
    /// Provides facilities to operate on a dictionary holding command classes.
    /// </summary>
    public interface ICommandClassDictionary<TCommandClass> where TCommandClass : CommandClassBase
    {
        /// <summary>
        /// Adds a command class to the dictionary unless there is already a matching one.
        /// </summary>
        /// <param name="commandClass">The command class to add to the cache.</param>
        void Add(TCommandClass commandClass);

        /// <summary>
        /// For a list of command classes, adds each one to the dictionary unless there is already
        /// a matching one.
        /// </summary>
        /// <param name="commandClasses">A list of command classes to add to the cache.</param>
        void Add(TCommandClass[] commandClasses);

        /// <summary>
        /// Removes the commands for all channels that meet the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate that must be met.</param>
        void RemoveChannelCommands(Func<IMessageChannel, bool> predicate);

        /// <summary>
        /// Sends all commands.
        /// </summary>
        /// <param name="predicate">The predicate that must be met.</param>
        void SendCommands(Predicate<TCommandClass> predicate);
    }
}