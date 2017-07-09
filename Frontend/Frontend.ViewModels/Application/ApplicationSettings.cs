//------------------------------------------------------------------------------
// Copyright (C) 2017 Josi Coder

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
//using CtLab.Environment;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides the application settings.
    /// </summary>
    public class ApplicationSettings
    {
        /// <summary>
        /// Creates a 1:1 copy of this object.
        /// </summary>
        public ApplicationSettings Clone()
        {
            // Be careful when adding any reference type properties to this class,
            // copy them explicitly!
            return (ApplicationSettings)this.MemberwiseClone();
        }

        public ApplianceConnectionType ConnectionType
        { get; set; }

        public string PortName
        { get; set; }

        public byte Channel
        { get; set; }
    }
}

