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
        /// <summary>
        /// Writes sample values to the storage and reads them using the low-level SRAM controller protocol.
        /// Note: No handshake is usually necessary for writing as the storage controller (VHDL) is much faster
        /// than this software. For reading, the entire roundtrip time necessary to provide the read value has
        /// to be considered, especially when using the c't Lab protocol. 
        /// TODO: The current protocol is very low-level, this might be changed.
        /// </summary>
        public static void WriteAndReadStorageValues()
        {
            const bool writeWithHandshake = true;
            const bool readWithHandshake = true;

            Utilities.WriteHeader();

            using (var appliance = new ApplianceFactory().CreateTestAppliance())
            {
                // Get the scope and reset the hardware to cancel settings from previous configurations.
                var scope = appliance.Scope;
                scope.Reset();

                var adresses = new [] {3, 4, 5};
                var random = new Random ();
                var items = adresses.ToDictionary(address => address, address => random.Next (255));

                foreach (var storageItem in items)
                {
                    DoWrite(appliance, storageItem.Key, storageItem.Value, writeWithHandshake);
                }

                Console.WriteLine ("========================================");

                // Trigger all reads immediately and await any async 'String received' comments.
                var readValues = items.Select(item => DoRead(appliance, item.Key, readWithHandshake)).ToList();
                Thread.Sleep (100);

                Console.WriteLine ("========================================");

                Console.Write ("Written:");
                foreach (var value in items.Values)
                {
                    Console.Write(" {0} (x{0:X2})", value);
                }
                Console.WriteLine();

                Console.Write ("Read:   ");
                foreach (var value in readValues)
                {
                    Console.Write(" {0} (x{0:X2})", value);
                }
                Console.WriteLine();
           }

            Utilities.WriteFooterAndWaitForKeyPress();
        }

        /// <summary>
        /// Writes to the storage using the low-level SRAM controller protocol.
        /// </summary>
        private static void DoWrite(Appliance appliance, int address, int value, bool withHandshake)
        {
            Console.WriteLine ("** Writing {0}={1} **", address, value);

            // Finish any pending access.
            SetMode(appliance, StorageMode.Idle);
            if (withHandshake) AwaitState(appliance, state => state != StorageState.Writing, "any non-'writing' state");

            // Set address and value.
            PrepareWriteAccess(appliance, address, value);

            // Start writing.
            SetMode(appliance, StorageMode.Write);
            if (withHandshake) AwaitState(appliance, StorageState.Writing);

            // Finish access.
            SetMode(appliance, StorageMode.Idle);
            if (withHandshake) AwaitState(appliance, StorageState.Ready);

            Console.WriteLine ("------------------------------");
        }

        /// <summary>
        /// Reads from the storage using the low-level SRAM controller protocol.
        /// </summary>
        private static int DoRead(Appliance appliance, int address, bool readWithStrictHandshake)
        {
            Console.WriteLine ("------------------------------");

            // Set address.
            PrepareReadAccess(appliance, address);

            // Start reading.
            SetMode(appliance, StorageMode.Read);
            if (readWithStrictHandshake) AwaitState(appliance, StorageState.Reading);

            // Finish access.
            if (readWithStrictHandshake)
            {
                SetMode(appliance, StorageMode.Idle);
                AwaitState(appliance, StorageState.Ready);
            }
            else
            {
                AwaitValueAvailabilityTime(appliance);
            }

            // Get value.
            var value = appliance.Scope.StorageController.Value;
            Console.WriteLine ("** Read {0}={1} **", address, value);
            return value;
        }

        /// <summary>
        /// Prepares write access.
        /// </summary>
        private static void PrepareWriteAccess(Appliance appliance, int address, int value)
        {
            Console.WriteLine("=> Prepare write access...");
            appliance.Scope.StorageController.PrepareWriteAccess(address, value);
            appliance.ApplianceConnection.SendSetCommandsForModifiedValues();
        }

        /// <summary>
        /// Prepares read access.
        /// </summary>
        private static void PrepareReadAccess(Appliance appliance, int address)
        {
            Console.WriteLine("=> Prepare read access...");
            appliance.Scope.StorageController.PrepareReadAccess(address);
            appliance.ApplianceConnection.SendSetCommandsForModifiedValues();
        }

        /// <summary>
        /// Sets the storage mode.
        /// </summary>
        private static void SetMode(Appliance appliance, StorageMode mode)
        {
            Console.WriteLine("=> Set mode to {0}...", mode);
            appliance.Scope.StorageController.SetMode(mode);
            appliance.ApplianceConnection.SendSetCommandsForModifiedValues();
        }

        /// <summary>
        /// Waits until the specified state is achieved. The value is supposed to be available as
        /// soon as the state matches.
        /// </summary>
        private static void AwaitState(Appliance appliance, StorageState state)
        {
            AwaitState(appliance, st => st == state, string.Format("{0} state", state.ToString()));
        }

        /// <summary>
        /// Waits until the state satisfies the specified predicate. The value is supposed to be available as
        /// soon as the state matches.
        /// </summary>
        private static void AwaitState(Appliance appliance, Predicate<StorageState> statePredicate, string statePredicateCaption)
        {
            Console.WriteLine("=> Waiting for {0}...", statePredicateCaption);
            int i = 0;
            while (!statePredicate(appliance.Scope.StorageController.State))
            {
                GetValue(appliance);
                Thread.Sleep (10); //TODO: Which value is needed for direct SPI access?
                i++;
            }
            Console.WriteLine("=> Achieved {0} after polling {1} times.", statePredicateCaption, i);
        }

        /// <summary>
        /// Waits until the value read from the storage is supposed to be available.
        /// </summary>
        private static void AwaitValueAvailabilityTime(Appliance appliance)
        {
            Console.WriteLine("=> Waiting for value availability...");
            GetValue(appliance);
            // Wait until the value is (hopefully) available, especially when using the c't Lab protocol.
            Thread.Sleep (100); //TODO: Which value is needed for direct SPI access?
        }

        /// <summary>
        /// Gets the value read from the storage.
        /// </summary>
        private static void GetValue(Appliance appliance)
        {
            //TODO: Improvement: Don't send all query commands here, only those for state and value.
            appliance.ApplianceConnection.SendQueryCommandsImmediately();
        }
    }
}
