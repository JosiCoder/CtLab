﻿//------------------------------------------------------------------------------
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
using CtLab.FpgaScope.Interfaces;
using CtLab.Environment;

namespace CtLab.TestConsole
{
    /// <summary>
    /// Provides some samples using real c´t Lab hardware connected via a (physical or emulated) serial
    /// port or a direct SPI connection.
    /// For these tests, a proper configuration supporting a scope must be loaded into the FPGA.
    /// </summary>
    public class RealHardwareScopeSamples
    {
        public struct HardwareSettings
        {
            public bool WriteWithHandshake; // usually not needed
            public bool ReadWithHandshake; // needed for c't Lab protocol, not needed for SPI
            public int MillisecondsToWaitForAsynchronousReads; // 10 or more needed for c't Lab protocol, not needed for SPI
        }

        private readonly bool _spiDirect;
        private readonly HardwareSettings _hardwareSettings;

        /// <summary>
        /// Initializes an instance of class.
        /// </summary>
        public RealHardwareScopeSamples (bool spiDirect, HardwareSettings hardwareSettings)
        {
            _spiDirect = spiDirect;
            _hardwareSettings = hardwareSettings;
        }

        /// <summary>
        /// Writes sample values to the storage and reads them using the low-level SRAM controller protocol.
        /// Note: No handshake is usually necessary for writing as the storage controller (VHDL) is much faster
        /// than this software. For reading, the entire roundtrip time necessary to provide the read value has
        /// to be considered, especially when using the c't Lab protocol. 
        /// </summary>
        public void WriteAndReadStorageValues()
        {
            Utilities.WriteHeader();

            using (var appliance = new ApplianceFactory(_spiDirect).CreateTestAppliance())
            {
                // Get the scope and reset the hardware to cancel settings from previous configurations.
                var scope = appliance.Scope;
                scope.Reset();

                var adresses = new [] {3, 4, 5};
                var random = new Random ();
                var items = adresses.ToDictionary(address => address, address => random.Next (255));

                foreach (var storageItem in items)
                {
                    DoWrite(appliance, storageItem.Key, storageItem.Value, _hardwareSettings.WriteWithHandshake);
                }

                Console.WriteLine ("========================================");

                // Trigger all reads immediately.
                var readValues = items.Select(item => DoRead(appliance, item.Key, _hardwareSettings.ReadWithHandshake)).ToList();

                // Await any async 'String received' comments, just for more beautiful output.
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
        private void DoWrite(Appliance appliance, int address, int value, bool withHandshake)
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
        private int DoRead(Appliance appliance, int address, bool readWithHandshake)
        {
            Console.WriteLine ("------------------------------");

            // Set address.
            PrepareReadAccess(appliance, address);

            // Start reading.
            SetMode(appliance, StorageMode.Read);
            if (readWithHandshake) AwaitState(appliance, StorageState.Reading);

            // Finish access.
            if (readWithHandshake)
            {
                SetMode(appliance, StorageMode.Idle);
                AwaitState(appliance, StorageState.Ready);
            }
            else
            {
                GetStateAndValue(appliance);
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
        private void AwaitState(Appliance appliance, StorageState state)
        {
            AwaitState(appliance, st => st == state, string.Format("{0} state", state.ToString()));
        }

        /// <summary>
        /// Waits until the state satisfies the specified predicate. The value is supposed to be available as
        /// soon as the state matches.
        /// </summary>
        private void AwaitState(Appliance appliance, Predicate<StorageState> statePredicate, string statePredicateCaption)
        {
            Console.WriteLine("=> Waiting for {0}...", statePredicateCaption);
            int i = 0;
            while (!statePredicate(appliance.Scope.StorageController.State))
            {
                QueryStateAndValue(appliance);
                // When using the c't Lab protocol, state and value are returned asynchronously. Wait a little
                // before polling another time.
                // When using direct SPI access, as it is synchronous, waiting is not necessary at all.
                if (_hardwareSettings.MillisecondsToWaitForAsynchronousReads != 0)
                {
                    Thread.Sleep (_hardwareSettings.MillisecondsToWaitForAsynchronousReads);
                }
                i++;
            }
            Console.WriteLine("=> Achieved {0} after polling {1} times.", statePredicateCaption, i);
        }

        /// <summary>
        /// Read value directly.
        /// </summary>
        private static void GetStateAndValue(Appliance appliance)
        {
            Console.WriteLine("=> Waiting for value availability...");
            QueryStateAndValue(appliance);
        }

        /// <summary>
        /// Initiates transfer of the storage state and the value read from the storage.
        /// </summary>
        private static void QueryStateAndValue(Appliance appliance)
        {
            appliance.ApplianceConnection.SendStorageQueryCommands();
        }
    }
}
