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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CtLab.Environment;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides access to an application settings writer.
    /// </summary>
    public interface IApplicationSettingsWriter
    {
        /// <summary>
        /// Writes the application settings to the settings file.
        /// </summary>
        void Write (ApplicationSettings applicationSettings);
    }

    /// <summary>
    /// Reads and writes application settings from and to a settings file.
    /// </summary>
    public class ApplicationSettingsReaderWriter : IApplicationSettingsWriter
    {
        private const ApplianceConnectionType _defaultConnectionType =
            ApplianceConnectionType.Serial;
        private const string _defaultPortName = "/dev/ttyUSB0";
        private const byte _defaultChannel = 7;

        private readonly string _filePath;
        private readonly XmlSerializer _serializer;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="filePath">The path of the file to read the settings from.</param>
        public ApplicationSettingsReaderWriter (string filePath)
        {
            _filePath = filePath;
            _serializer = new XmlSerializer (typeof(ApplicationSettings));
        }

        /// <summary>
        /// Writes the application settings to the settings file.
        /// </summary>
        public void Write (ApplicationSettings applicationSettings)
        {
            try
            {
                using (var stream = new FileStream (_filePath, FileMode.Create, FileAccess.Write))
                {
                    _serializer.Serialize (stream, applicationSettings);
                }
            }
            catch {}
        }

        /// <summary>
        /// Reads the application settings from the settings file.
        /// </summary>
        public ApplicationSettings Read ()
        {
            try
            {
                using (var stream = new FileStream (_filePath, FileMode.Open, FileAccess.Read))
                {
                    return (ApplicationSettings)_serializer.Deserialize (stream);
                }
            }
            catch
            {
                return new ApplicationSettings
                {
                    ConnectionType = _defaultConnectionType,
                    PortName = _defaultPortName,
                    Channel = _defaultChannel,
                };
            }
        }
    }
}

