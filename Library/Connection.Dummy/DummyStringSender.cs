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
    /// Provides a dummy string sender that can be used for tests and samples that don't need real
    /// c't Lab hardware.
    /// </summary>
    public class DummyStringSender : IStringSender
    {
        /// <summary>
        /// Simulates sending a string to a c't Lab device.
        /// </summary>
        /// <param name="stringToSend">The string to be sent.</param>
        public void Send(string stringToSend)
        {
            Console.WriteLine("String sent via dummy string sender: {0}", stringToSend);
        }
    }
}