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
using CtLab.CommandsAndMessages.Interfaces;

namespace CtLab.CommandsAndMessages.Standard
{
    /// <summary>
    /// Maintains a unique query command class for each combination of channel and subchannel.
    /// Sends some or all set commands to get the c't Lab devices in sync.
    /// </summary>
    public class QueryCommandClassDictionary : CommandClassDictionaryBase<QueryCommandClass>, IQueryCommandClassDictionary
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="commandSender">The command sender used to send the commands.</param>
        public QueryCommandClassDictionary(IQueryCommandSender commandSender)
            : base(commandSender)
        {
        }
    }
}