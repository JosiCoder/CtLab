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
using System.Collections.Generic;
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
        private static readonly HashSet<Gtk.Window> _activeViews = new HashSet<Gtk.Window>();
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
            CreateApplicationDataDirectory();

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
                ShowMainWindow(mainViewModel);

                // Debug only: By creating a second view referring to the same viewmodel,
                // data binding can be easily tested (each view must immediately reflect
                // changes made in the other one).
                #if DEBUG
                // TODO das hier nach den Tests wieder deaktivieren
                ShowMainWindow(mainViewModel);
                #endif

                // Run the message loop.
                Application.Run();
            }
        }

        /// <summary>
        /// Shows the main window.
        /// </summary>
        private static void ShowMainWindow(MainViewModel mainViewModel)
        {
            ShowWindow(closeRequestHandler => MainWindowView.Create(mainViewModel, closeRequestHandler));
        }

        /// <summary>
        /// Shows a window.
        /// </summary>
        private static void ShowWindow(Func<Func<bool>, Gtk.Window> windowCreator)
        {
            Gtk.Window window = null;
            Func<Gtk.Window> windowProvider = () => window;
            Func<bool> closeRequestHandler = () => CloseRequestHandler(windowProvider);

            window = windowCreator(closeRequestHandler);

            window.Show();
            _activeViews.Add(window);
        }

        /// <summary>
        /// Handles a request to close a window.
        /// </summary>
        private static bool CloseRequestHandler(Func<Gtk.Window> windowProvider)
        {
            var closeAccepted = IsCloseRequestAccepted();
            if (closeAccepted)
            {
                // Remove the window, if the last one was closed, quit the application.
                _activeViews.Remove(windowProvider());
                if (_activeViews.Count == 0)
                {
                    Application.Quit();
                }
            }
            return closeAccepted;
        }

        /// <summary>
        /// Returns a value indicating whether a request to close a window is accepted.
        /// </summary>
        private static bool IsCloseRequestAccepted()
        {
            // Accept the close request.
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
