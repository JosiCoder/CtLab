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
    /// Specifies a source for c't lab messages.
    /// </summary>
    public struct CtLabMessageChannel
    {
        public CtLabMessageChannel(byte channel, ushort subchannel)
        {
            Channel = channel;
            Subchannel = subchannel;
        }
        public byte Channel;
        public ushort Subchannel;
    }
}