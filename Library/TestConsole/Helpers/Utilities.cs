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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using StructureMap;
using CtLab.BasicIntegration;
using CtLab.CtLabProtocol.Integration;
using CtLab.EnvironmentIntegration;

namespace CtLab.TestConsole
{
	/// <summary>
	/// Provides some utilities for the testing and demonstration console application.
	/// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Configures and returns an IoC using the specified connection registry.
        /// </summary>
        /// <returns>The configured IoC.</returns>
        /// <typeparam name="TConnectionRegistry">The type of the registry responsible for the connections.</typeparam>
        public static Container ConfigureIoC<TConnectionRegistry>() where TConnectionRegistry : Registry, new()
        {
            // Configure the IoC container to provide specific implementations for several interfaces.
            var container = new Container(expression =>
                {
                    expression.AddRegistry<TConnectionRegistry>();
                    expression.AddRegistry<CommandsAndMessagesRegistry>();
                    expression.AddRegistry<CtLabProtocolRegistry>();
                    expression.AddRegistry<ApplianceRegistry>();
                });

            // Display the effecive IoC container configuration.
            // Note: This line is not needed for proper operation, it just provides some information
            // that helps to understand how the IoC container works. Thus this line may be deactivated.
            //Console.WriteLine(container.WhatDoIHave());

            return container;
        }

        /// <summary>
        /// Writes a header containing the name of the calling method.
        /// </summary>
        public static void WriteHeader()
        {
            Console.WriteLine("=========================================================================");
            Console.WriteLine("Sample \"{0}\" started.", new StackFrame(1).GetMethod().Name);
            Console.WriteLine("-------------------------------------------------------------------------");
        }

        /// <summary>
        /// Writes a footer containing the name of the calling method and waits for a key press.
        /// </summary>
        public static void WriteFooterAndWaitForKeyPress()
        {
            Console.WriteLine("-------------------------------------------------------------------------");
            Console.WriteLine("Sample \"{0}\" finished, press any key.", new StackFrame(1).GetMethod().Name);
            Console.WriteLine("=========================================================================");
            Console.ReadLine();
        }
    }
}
