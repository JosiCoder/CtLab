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
    /// Provides facilities to access cached messages.
    /// </summary>
    public interface IMessageCache
    {
        /// <summary>
        /// Registers a message channel for caching and returns the message container
        /// for that message channel.
        /// </summary>
        /// <param name="messageChannel">
        /// The message channel to register.
        /// </param>
        /// <returns>The message container for the specified message channel.</returns>
        IMessageContainer Register(IMessageChannel messageChannel);

        /// <summary>
        /// Unregisters all message channels that meet the specified predicate from caching.
        /// </summary>
        /// <param name="predicate">The predicate that must be met.</param>
        void UnregisterMessageChannels(Func<IMessageChannel, bool> predicate);

        /// <summary>
        /// Gets the message container for the specified message channel.
        /// </summary>
        /// <param name="messageChannel">
        /// The message channel to get the message container for.
        /// </param>
        /// <returns>The message container.</returns>
        IMessageContainer GetMessageContainer(IMessageChannel messageChannel);
    }
}