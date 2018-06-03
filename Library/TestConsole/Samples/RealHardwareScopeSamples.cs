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
using System.Threading;
using StructureMap;
using CtLab.Connection.Interfaces;
using CtLab.Connection.Serial;
using CtLab.FpgaScope.Interfaces;
using CtLab.Environment;
using CtLab.EnvironmentIntegration;

namespace CtLab.TestConsole
{
    /// <summary>
    /// Provides some samples using real c´t Lab hardware connected via a (physical or emulated) serial
    /// port. For these tests, a proper configuration supporting a scope must be loaded into
    /// the FPGA.
    /// </summary>
    public static class RealHardwareScopeSamples
    {
        private const int _queryCommandSendPeriod = 500; // ms

        /// <summary>
        /// Writes sample values to the storage and reads them.
        /// TODO: The current protocol is a very low-level implementation, this might be changed.
        /// </summary>
        public static void WriteAndReadStorageValues()
        {
            Utilities.WriteHeader();

            using (var appliance = new ApplianceFactory().CreateTestAppliance())
            {
                // Get the scope and reset the hardware to cancel settings from previous
                // configurations.
                var scope = appliance.Scope;
                scope.Reset();

                var random = new Random ();
                DoWrite(appliance, 1, random.Next(255));
                DoWrite(appliance, 2, random.Next(255));
                DoWrite(appliance, 3, random.Next(255));

                Console.WriteLine ("========================================");

                DoRead(appliance, 1);
                DoRead(appliance, 2);
                DoRead(appliance, 3);
           }

            Utilities.WriteFooterAndWaitForKeyPress();
        }

        /// <summary>
        /// Writes to the memory using the low-level SRAM controller protocol.
        /// </summary>
        private static void DoWrite(Appliance appliance, int address, int value)
        {
            Console.WriteLine ("Writing {0}={1}", address, value);

            var scope = appliance.Scope;

            // Finish any pending access, wait until mode becomes 'non-writing', i.e. 'reading' or 'ready'.
            SetModeAndWaitForState(appliance, StorageMode.Idle, state => state != StorageState.Writing, "non-writing");

            // Set address and value.
            scope.StorageController.PrepareWriteAccess(address, value);
            appliance.ApplianceConnection.SendSetCommandsForModifiedValues();

            // Start writing, wait until mode becomes 'writing'.
            SetModeAndWaitForState(appliance, StorageMode.Write, state => state == StorageState.Writing, "writing");

            // Finish access, wait until mode becomes 'ready' (MSB set, all other bits reset).
            SetModeAndWaitForState(appliance, StorageMode.Idle, state => state == StorageState.Ready, "ready");

            Console.WriteLine ("------------------------------");
        }

        /// <summary>
        /// Reads from the memory the low-level SRAM controller protocol.
        /// </summary>
        private static int DoRead(Appliance appliance, int address)
        {
            Console.WriteLine ("------------------------------");

            var scope = appliance.Scope;

            // Set address.
            scope.StorageController.PrepareReadAccess(address);
            appliance.ApplianceConnection.SendSetCommandsForModifiedValues();

            // Start reading, wait until mode becomes 'reading'.
            SetModeAndWaitForState(appliance, StorageMode.Read, state => state == StorageState.Reading, "reading");

            // Finish access, wait until mode becomes 'ready' (MSB set, all other bits reset).
            SetModeAndWaitForState(appliance, StorageMode.Idle, state => state == StorageState.Ready, "ready");

            // Get value.
            var value = scope.StorageController.Value;
            Console.WriteLine ("Read {0}={1}", address, value);
            return value;
        }

        /// <summary>
        /// Sets the storage mode and waits until the state satisfies the specified predicate.
        /// </summary>
        private static void SetModeAndWaitForState(Appliance appliance, StorageMode mode, Predicate<StorageState> statePredicate, string statePredicateCaption)
        {
            var scope = appliance.Scope;

            scope.StorageController.SetMode(mode);
            appliance.ApplianceConnection.SendSetCommandsForModifiedValues();

            int i = 0;
            while (!statePredicate(scope.StorageController.State))
            {
                appliance.ApplianceConnection.SendQueryCommandsImmediately();
                Thread.Sleep (10);
                i++;
            }
            Console.WriteLine("Polled {0} times while waiting for '{1}' state", i, statePredicateCaption);
        }
    }
}
