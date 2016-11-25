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
using CtLab.Utilities;
using CtLab.Connection.Interfaces;

namespace CtLab.Connection.Dummy
{
    /// <summary>
    /// Provides a dummy string receiver that can be used for tests and samples that don't need real
    /// c't Lab hardware.
    /// </summary>
    public class DummyStringReceiver : IStringReceiver
    {
        /// <summary>
        /// Occurs when a string has been injected into the dummy string receiver as if it has been
        /// received from a c't Lab device.
        /// </summary>
        public event EventHandler<StringReceivedEventArgs> StringReceived;

        /// <summary>
        /// Injects a string into the dummy string receiver as if it has been received from a c't
        /// Lab device.
        /// </summary>
        /// <param name="receivedString">The string to be injected.</param>
        public void InjectReceivedString(string receivedString)
        {
            Console.WriteLine("String injected into dummy string receiver: {0}", receivedString);
            StringReceived.Raise(this, new StringReceivedEventArgs(receivedString));
        }
    }
}