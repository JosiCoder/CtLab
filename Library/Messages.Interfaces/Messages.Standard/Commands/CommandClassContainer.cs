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
    /// Holds a command class and related metadata.
    /// </summary>
    /// <typeparam name="TCommandClass">The type of the command class in the container.</typeparam>
    public class CommandClassContainer<TCommandClass>
        where TCommandClass : CommandClassBase
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="commandClass">The command class held by this object.</param>
        /// <param name="commandClassGroup">The group the command class is related to.</param>
        public CommandClassContainer(TCommandClass commandClass, CommandClassGroup commandClassGroup)
        {
            CommandClass = commandClass;
            CommandClassGroup = commandClassGroup;
        }

        /// <summary>
        /// Gets the command class held by this object.
        /// </summary>
        public readonly TCommandClass CommandClass;

        /// <summary>
        /// Gets the group the command class is related to.
        /// </summary>
        public CommandClassGroup CommandClassGroup
        {
            get;
            private set;
        }
    }
}

