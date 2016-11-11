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

using System.Text;

namespace CtLab.CommandsAndMessages.Standard
{
    /// <summary>
    /// Provides some extensions for c't Lab command strings.
    /// </summary>
    public static class CommandStringExtensions
    {
        /// <summary>
        /// Removes the checksum part (including the '$' sign) of a command string.
        /// </summary>
        /// <param name="commandString">The string to remove the checksum part from.</param>
        /// <returns>The string after the checksum part has been removed from.</returns>
        public static string TrimChecksum(this string commandString)
        {
            return commandString.Remove(commandString.LastIndexOf('$'));
        }

        /// <summary>
        /// Converts the command string to a sequence of bytes as expected by the
        /// c't Lab devices. 
        /// </summary>
        /// <param name="commandString">The command string to convert.</param>
        /// <returns>A sequence of bytes as expected by the c't Lab devices.</returns>
        public static byte[] GetBytes(this string commandString)
        {
            return (new ASCIIEncoding()).GetBytes(commandString);
        }
    }
}