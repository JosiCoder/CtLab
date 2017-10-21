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

namespace CtLab.SpiConnection.Interfaces
{
    /// <summary>
    /// Provides facilities to handle values received from an SPI slave.
    /// </summary>
    public interface ISpiReceiver
    {
        /// <summary>
        /// Occurs when a value has been received from an SPI slave. Note that
        /// this event might be called via a background thread.
        /// </summary>
        event EventHandler<SpiReceivedEventArgs> ValueReceived;
    }
}