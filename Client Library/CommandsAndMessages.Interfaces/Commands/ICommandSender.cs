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
    /// Provides facilities to send a command to a c't Lab device.
    /// </summary>
    public interface ICommandSender<TCommandClass> where TCommandClass : CommandClass
    {
        /// <summary>
        /// Sends a set command to a c't Lab device.
        /// </summary>
        /// <param name="commandClass">The command class to send a command for.</param>
        void Send(TCommandClass commandClass);
    }

    /// <summary>
    /// Provides facilities to send a command to a c't Lab device.
    /// </summary>
    public interface ISetCommandSender : ICommandSender<SetCommandClass>
    {
        /// <summary>
        /// Sends a set command to a c't Lab device.
        /// </summary>
        /// <param name="commandClass">The command class to send a command for.</param>
        /// <param name="generateChecksum">true to append the checksum; otherwiese, false.</param>
        /// <param name="requestAcknowledge">
        /// true to request an acknowledge from the receiver; otherwiese, false.
        /// </param>
        void Send(SetCommandClass commandClass, bool generateChecksum, bool requestAcknowledge);
    }


    /// <summary>
    /// Provides facilities to send a command to a c't Lab device.
    /// </summary>
    public interface IQueryCommandSender : ICommandSender<QueryCommandClass>
    {
        /// <summary>
        /// Sends a set command to a c't Lab device.
        /// </summary>
        /// <param name="commandClass">The command class to send a command for.</param>
        /// <param name="generateChecksum">true to append the checksum; otherwiese, false.</param>
        void Send(QueryCommandClass commandClass, bool generateChecksum);
    }
}