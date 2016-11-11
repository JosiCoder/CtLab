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
    /// Provides facilities to build strings from set commands that can be be sent to c't Lab
    /// devices.
    /// </summary>
    public interface ISetCommandStringBuilder
    {
        /// <summary>
        /// Gets a pseudo channel number used to specify that a command is to be sent
        /// without any channel number which relies on a specific default handling by
        /// the devices.
        /// </summary>
        byte DefaultChannel { get; }

        /// <summary>
        /// Gets a pseudo channel number used to specify that a command is to be sent
        /// as a broadcast. All channels will accept the command. This isn't a real
        /// channel number.
        /// </summary>
        byte BroadcastChannel { get; }

        /// <summary>
        /// Builds a set command string using the command classes' channel and subchannel.
        /// </summary>
        /// <param name="commandClass">The command class to send a command for.</param>
        /// <returns>The built command string.</returns>
        string BuildCommand(SetCommandClass commandClass);

        /// <summary>
		/// Builds a set command string using the command classes' channel and subchannel.
        /// </summary>
        /// <param name="commandClass">The command class to send a command for.</param>
        /// <param name="generateChecksum">true to append the checksum; otherwiese, false.</param>
        /// <param name="requestAcknowledge">true to request an acknowledge from the receiver; otherwiese, false.</param>
        /// <returns>The built command string.</returns>
        string BuildCommand(SetCommandClass commandClass, bool generateChecksum, bool requestAcknowledge);
    }
}