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
using CtLab.Connection.Interfaces;
using CtLab.CommandsAndMessages.Interfaces;

namespace CtLab.Environment
{
    public interface IApplianceConnection : IDisposable
    {
        /// <summary>
        /// Gets an object that can be used to synchronize access to the underlying query
        /// command class dictionary.
        /// </summary>
        object SyncRoot
        { get; }

        /// <summary>
        /// Gets the connection used by this instance.
        /// </summary>
        IConnection Connection
        { get; }

        /// <summary>
        /// Flushes any modifications to the hardware.
        /// </summary>
        void FlushModifications();

        /// <summary>
        /// Starts polling the hardware for new values.
        /// </summary>
        /// <param name="period">The period in milliseconds.</param>
        void StartPolling(long period);

        /// <summary>
        /// Starts polling the hardware for new values.
        /// </summary>
        void StopPolling();

        /// <summary>
        /// Polls the hardware once for new values.
        /// </summary>
        void PollOnce();
    }
}