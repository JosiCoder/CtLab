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
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Gtk;
using CtLab.Frontend.ViewModels;
using CtLab.Frontend.Views;

namespace CtLab.Frontend
{
    /// <summary>
    /// Provides the entry point of the application and its main loop.
    /// </summary>
    class Program
    {
        private static readonly string _executableName;
        private static readonly string _applicationName;
        private static readonly string _applicationDataPath;
        private static readonly string _applicationSettingsFilePath;
        private static readonly string _applicationLogFilePath;

        /// <summary>
        /// Initializes the class.
        /// </summary>
        static Program()
        {
            _executableName = typeof(Program).Assembly.GetName().Name;
            _applicationName = _executableName;

            _applicationDataPath = Path.Combine
                (System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), _applicationName);
            _applicationSettingsFilePath =
                Path.Combine(_applicationDataPath, string.Format (@"{0}.xml", _applicationName));
            _applicationLogFilePath =
                Path.Combine(_applicationDataPath, string.Format (@"{0}.log", _applicationName));
        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main (string[] args)
        {
            CreateApplicationDataDirectory ();

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = (Exception) e.ExceptionObject;
                WriteLogMessage(ex.Message);
            };

            GLib.ExceptionManager.UnhandledException += (e) => 
            {
                var ex = (Exception) e.ExceptionObject;
                WriteLogMessage(ex.Message);
            };

            var applicationSettingsReaderWriter = new ApplicationSettingsReaderWriter (_applicationSettingsFilePath);
            var applicationSettings = applicationSettingsReaderWriter.Read ();

            // Initialize the UI toolkit.
            Application.Init();

            // Create the viewmodel hierarchy and show the view hierarchy.
            using(var mainViewModel = new MainViewModel(new ApplianceFactory(),
                new DialogServiceViewModelFactory(), applicationSettingsReaderWriter))
            {
                mainViewModel.InitializeAppliances(applicationSettings);

                // Create and show the main window.
                var mainWindowView = MainWindowView.Create(mainViewModel, HandleCloseRequest);
                mainWindowView.Show();

                // Debug only: By creating a second view referring to the same viewmodel,
                // data binding can be easily tested (each view must immediately reflect
                // changes made in the other one).
                #if DEBUG
                //var mainWindowView1 = MainWindowView.Create(mainViewModel, HandleCloseRequest);
                //mainWindowView1.Show();
                #endif

                // Run the message loop.
                Application.Run();
            }
        }

        /// <summary>
        /// Quits the application.
        /// </summary>
        private static bool HandleCloseRequest()
        {
            // If the last window was closed, quit the application.
            Application.Quit();

            // Accept the close request, i.e. close the window.
            return true;
        }

        /// <summary>
        /// Creates the application data directory unless already existing.
        /// </summary>
        private static void CreateApplicationDataDirectory()
        {
            try
            {
                Directory.CreateDirectory (_applicationDataPath);
            }
            catch {}
        }

        /// <summary>
        /// Writes a message to the application log.
        /// </summary>
        private static void WriteLogMessage(string logMessage)
        {
            try
            {
                Console.WriteLine(logMessage);
                using (var writer = new StreamWriter(_applicationLogFilePath, true))
                {
                    writer.WriteLine ("{0:o}: {1}",
                        DateTime.Now, logMessage);
                }
            }
            catch {}
        }
    }
}
