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
using CtLab.Messages.Interfaces;

namespace CtLab.Messages.Standard
{
    /// <summary>
    /// Provides the base functionality for message receivers.
    /// <typeparam name="TMessageSource">The type of the message source.</typeparam>
    public abstract class MessageReceiverBase<TMessageSource> : IMessageReceiver<TMessageSource>
    {
        /// <summary>
        /// Occurs when a message has been received from a c't Lab device. Note that
        /// this event might be called via a background thread.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<TMessageSource>> MessageReceived;

        /// <summary>
        /// Raises a MessageReceived event.
        /// </summary>
        protected void RaiseMessageReceived(object sender, MessageReceivedEventArgs<TMessageSource> eventArgs)
        {
            MessageReceived.Raise(sender, eventArgs);
        }
    }
}