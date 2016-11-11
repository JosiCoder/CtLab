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

using CtLab.CommandsAndMessages.Interfaces;

namespace CtLab.SubchannelAccess
{
    /// <summary>
	/// Provides the base functionality for classes that write a value to a c't Lab device's
    /// subchannel by setting the value of a set command that can be sent to that c't Lab device.
    /// </summary>
    public abstract class SubchannelWriterBase
    {
        protected readonly ISubchannelValueSetter _subchannelValueSetter;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="subchannelValueSetter">
		/// The setter used to set the subchannel's value.
        /// </param>
        protected SubchannelWriterBase(ISubchannelValueSetter subchannelValueSetter)
        {
            _subchannelValueSetter = subchannelValueSetter;
        }
    }
}