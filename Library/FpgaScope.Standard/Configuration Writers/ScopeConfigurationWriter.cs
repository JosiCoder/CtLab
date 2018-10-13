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

using CtLab.FpgaScope.Interfaces;
using CtLab.FpgaConnection.Interfaces;

namespace CtLab.FpgaScope.Standard
{
    //TODO add more stuff similar to the UniversalCounterConfigurationWriter class.

    /// <summary>
    /// Writes the scope configuration by setting the according value of an
    /// FPGA device configured as an FPGA Lab.
    /// </summary>
    public class ScopeConfigurationWriter
    {
        private readonly IFpgaValueSetter _valueSetter;
        private ScopeSource _inputSource;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="valueSetter">
        /// The setter used to set the FPGA's value.
        /// </param>
        public ScopeConfigurationWriter (IFpgaValueSetter valueSetter)
        {
            _valueSetter = valueSetter;
        }

        /// <summary>
        /// Gets or sets the signal source.
        /// </summary>
        public ScopeSource InputSource
        {
            get { return _inputSource; }
            set
            {
                _inputSource = value;
                SetCommandValue();
            }
        }

        private void SetCommandValue()
        {
            // TODO adjust to FPGA implementation
            var combinedValue =
                ((uint)_inputSource) << 8;
            _valueSetter.SetValue(combinedValue);
        }
    }
}

