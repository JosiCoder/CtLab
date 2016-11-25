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

namespace CtLab.CommandsAndMessages.Interfaces
{
    /// <summary>
    /// Provides facilities to operate on a cache holding messages that may have been received
    /// from c't Lab devices.
    /// </summary>
    public interface IMessageCache
    {
        /// <summary>
		/// Registers a channel's subchannel for message caching and returns the message
        /// container for that subchannel.
        /// </summary>
        /// <param name="channel">
        /// The channel to register a subchannel for.
        /// </param>
        /// <param name="subchannel">
        /// The subchannel to register.
        /// </param>
        /// <returns>The message container.</returns>
        IMessageContainer Register(byte channel, ushort subchannel);

        /// <summary>
        /// Unregisters all subchannels belonging to a specific channel for message caching.
        /// </summary>
        /// <param name="channel">The channel to unregister the subchannels for.</param>
        void UnregisterSubchannelsForChannel(byte channel);
        
        /// <summary>
		/// Gets the message container for a channel's registered subchannel.
        /// </summary>
        /// <param name="channel">
		/// The channel to get a subchannel's message container for.
        /// </param>
        /// <param name="subchannel">
        /// The subchannel to get the message container for.
        /// </param>
        /// <returns>The message container.</returns>
        IMessageContainer GetMessageContainer(byte channel, ushort subchannel);
    }
}