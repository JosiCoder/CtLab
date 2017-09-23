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

using System.Globalization;

namespace CtLab.Messages.Interfaces
{
    /// <summary>
    /// Represents set commands that can be sent e.g. to set values or options.
    /// </summary>
    public class SetCommandClass : CommandClassBase, IChannelValueSetter
    {
        private string _rawValue;

        /// <summary>
		/// Gets or sets a value indicating whether the command value has been modified.
        /// </summary>
        public bool ValueModified { get; private set; }

        /// <summary>
		/// Resets the value indicating whether the command value has been modified.
        /// </summary>
        public void ResetValueModified()
        {
            ValueModified = false;
        }

        /// <summary>
        /// Gets or sets the raw (converted) command value.
        /// </summary>
        public string RawValue
        {
            get { return _rawValue; }
            set
            {
                if (_rawValue != value)
                {
                    ValueModified = true;
                }
                _rawValue = value;
            }
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="channel">
        /// The channel the commands are sent to.
        /// </param>
        public SetCommandClass(IMessageChannel channel)
            : base(channel)
        {
        }

        /// <summary>
        /// Sets a signed integer value.
        /// </summary>
        public void SetValue(int value)
        {
            RawValue = value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Sets an unsigned integer value.
        /// </summary>
        public void SetValue(uint value)
        {
            RawValue = value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Sets a floating point value.
        /// </summary>
        public void SetValue(double value)
        {
            RawValue = value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Sets a boolean value.
        /// </summary>
        public void SetValue(bool value)
        {
            RawValue = value ? "1" : "0";
        }
    }
}