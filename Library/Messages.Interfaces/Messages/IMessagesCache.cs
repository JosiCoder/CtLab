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
    /// Specifies a source for messages.
    /// </summary>
    public struct CtLabMessageSource
    {
        public CtLabMessageSource(byte channel, ushort subchannel)
        {
            Channel = channel;
            Subchannel = subchannel;
        }
        public byte Channel;
        public ushort Subchannel;
    }

    /// <summary>
    /// Provides facilities to access cached messages.
    /// </summary>
    /// <typeparam name="TMessageSource">The type of the message source.</typeparam>
    public interface IMessageCache<TMessageSource>
    {
        /// <summary>
        /// Registers a message source for caching and returns the message container
        /// for that message source.
        /// </summary>
        /// <param name="messageSource">
        /// The message source to register.
        /// </param>
        /// <returns>The message container for the specified message source.</returns>
        IMessageContainer<TMessageSource> Register(TMessageSource messageSource);

        /// <summary>
        /// Unregisters all message sources that meet the specified predicate from caching.
        /// </summary>
        /// <param name="predicate">The predicate that must be met.</param>
        void UnregisterSubchannelsForChannel(Func<TMessageSource, bool> predicate);

        /// <summary>
        /// Gets the message container for the specified message source.
        /// </summary>
        /// <param name="messageSource">
        /// The message source to get the message container for.
        /// </param>
        /// <returns>The message container.</returns>
        IMessageContainer<TMessageSource> GetMessageContainer(TMessageSource messageSource);
    }
}