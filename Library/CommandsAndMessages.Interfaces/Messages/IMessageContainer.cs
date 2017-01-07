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

namespace CtLab.CommandsAndMessages.Interfaces
{
    /// <summary>
    /// Provides facilities to hold and return a message that may have been received from a
    /// c't Lab device.
    /// </summary>
    public interface IMessageContainer
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <returns>A message.</returns>
        Message Message {get;}

        /// <summary>
        /// Occurs when the message has been updated.
        /// </summary>
        event EventHandler<EventArgs> MessageUpdated;
    }
}