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

namespace CtLab.Messages.Interfaces
{
    /// <summary>
    /// Provides facilities to send a command.
    /// </summary>
    /// <typeparam name="TMessageChannel">The type of the message channel.</typeparam>
    public interface ICommandSender<TCommandClass, TMessageChannel> where TCommandClass : CommandClassBase<TMessageChannel>
    {
        /// <summary>
        /// Sends a set command.
        /// </summary>
        /// <param name="commandClass">The command class to send a command for.</param>
        void Send(TCommandClass commandClass);
    }


    /// <summary>
    /// Provides facilities to send a command.
    /// </summary>
    /// <typeparam name="TMessageChannel">The type of the message channel.</typeparam>
    public interface ISetCommandSender<TMessageChannel> : ICommandSender<SetCommandClass<TMessageChannel>, TMessageChannel>
    {
    }


    /// <summary>
    /// Provides facilities to send a command.
    /// </summary>
    /// <typeparam name="TMessageChannel">The type of the message channel.</typeparam>
    public interface IQueryCommandSender<TMessageChannel> : ICommandSender<QueryCommandClass<TMessageChannel>, TMessageChannel>
    {
    }
}