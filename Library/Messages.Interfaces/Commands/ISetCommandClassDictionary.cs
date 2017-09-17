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
    /// Provides facilities to operate on a cache holding set command classes.
    /// </summary>
    /// <typeparam name="TMessageChannel">The type of the message channel.</typeparam>
    public interface ISetCommandClassDictionary<TMessageChannel> : ICommandClassDictionary<SetCommandClass<TMessageChannel>, TMessageChannel>
    {
        /// <summary>
        /// Sends commands for all command classes that have modified values.
        /// </summary>
        void SendCommandsForModifiedValues();
    }
}