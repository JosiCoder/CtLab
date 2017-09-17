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

using CtLab.CtLabProtocol.Interfaces;

namespace CtLab.CtLabProtocol.Standard
{
    /// <summary>
    /// Provides a checksum calculator used for c't Lab commands.
    /// </summary>
    public class ChecksumCalculator : IChecksumCalculator
    {
        /// <summary>
        /// Calculates a ckecksum for the payload.
        /// </summary>
        /// <param name="payload">The payload to calculate the checksum for.</param>
        /// <returns>The checksum calculated.</returns>
        public byte Calculate(string payload)
        {
            byte checksum = 0;
            foreach (var byt in payload.GetBytes())
            {
                checksum ^= byt;
            }
            return checksum;
        }
    }
}