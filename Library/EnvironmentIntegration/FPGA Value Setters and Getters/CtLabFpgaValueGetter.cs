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
using CtLab.CommandsAndMessages.Interfaces;
using CtLab.FpgaSignalGenerator.Standard;

namespace CtLab.EnvironmentIntegration
{
    /// <summary>
    /// Gets Fpga values by sending according c't Lab query commands
    /// and evaluating according c't Lab messages.
    /// </summary>
    public class CtLabFpgaValueGetter : IFpgaValueGetter
    {
        /// <summary>
        /// Occurs when a value has been updated.
        /// </summary>
        public event EventHandler<EventArgs> ValueUpdated;

        private IMessageContainer _messageContainer;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="queryCommandClass">
        /// The command class representing the query commands sent to the c't Lab device.
        /// </param>
        /// <param name="messageContainer">
        /// The message container used to receive messages from the c't Lab device.
        /// </param>
        public CtLabFpgaValueGetter(QueryCommandClass queryCommandClass,
            IMessageContainer messageContainer)
        {
            _messageContainer = messageContainer;

            messageContainer.MessageUpdated += (sender, e) =>
            {
                var handler = ValueUpdated;
                if (handler != null)
                {
                    handler(sender, e);
                }
            };
        }

        /// <summary>
        /// Gets the value as an integer.
        /// </summary>
        public int ValueAsInt32
        {
            get
            {
                return _messageContainer.Message.ValueToInt32();
            }
        }

        /// <summary>
        /// Gets the value as an unsigned integer.
        /// </summary>
        public uint ValueAsUInt32
        {
            get
            {
                return _messageContainer.Message.ValueToUInt32();
            }
        }

        /// <summary>
        /// Gets the value as a floating point number.
        /// </summary>
        public double ValueAsDouble
        {
            get
            {
                return _messageContainer.Message.ValueToDouble();
            }
        }

        /// <summary>
        /// Gets the value as a boolean value.
        /// </summary>
        public bool ValueAsBoolean
        {
            get
            {
                return _messageContainer.Message.ValueToBoolean();
            }
        }
    }
}