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

namespace CtLab.Connection.Dummy
{
    /// <summary>
    /// Provides a dummy connection that actually isn't a real connection.
    /// </summary>
    public class DummyConnection : IConnection
    {
        public DummyConnection ()
        {
        }

        /// <summary>
        /// Gets a value indicating whether the connection is active.
        /// </summary>
        public bool IsActive
        { get { return true; } }

        public void Dispose()
        {
        }
    }
}

