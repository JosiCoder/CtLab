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
    /// Specifies the send modes.
    /// </summary>
    public enum SendMode : ushort
    {
        Unspecified = 0,
        Periodic = 1,
        Storage = 2,
    }

    /// <summary>
    /// Provides the base functionality for command classes.
    /// </summary>
    public abstract class CommandClassBase
    {
        /// <summary>
        /// The channel the commands are sent to.
        /// </summary>
        public IMessageChannel Channel;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="channel">
        /// The channel the commands are sent to.
        /// </param>
        protected CommandClassBase(IMessageChannel channel)
        {
            Channel = channel;
        }
    }
}