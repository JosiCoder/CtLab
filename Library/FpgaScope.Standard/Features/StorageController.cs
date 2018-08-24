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
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using CtLab.Messages.Interfaces;
using CtLab.Utilities;
using CtLab.FpgaScope.Interfaces;
using CtLab.FpgaConnection.Interfaces;

namespace CtLab.FpgaScope.Standard
{
    /// <summary>
    /// Specfies several parameters describing how to access the storage.
    /// </summary>
    public class StorageHardwareSettings
    {
        public bool WriteWithHandshake; // usually not needed
        public bool ReadWithHandshake; // needed for c't Lab protocol, not needed for SPI
        public int MillisecondsToWaitForAsynchronousReads; // 10 or more needed for c't Lab protocol, not needed for SPI
    }

    /// <summary>
    /// Communicates with a storage controller implemented within a c't Lab FPGA device configured as
    /// an FPGA Lab.
    /// </summary>
    internal class StorageController
    {
        private readonly StorageHardwareSettings _hardwareSettings;
        private readonly IFpgaConnection _deviceConnection;
        private readonly IFpgaValueSetter _valueSetter;
        private readonly IFpgaValueGetter _valueGetter;
        private readonly IFpgaValueSetter _addressSetter;
        private readonly IFpgaValueSetter _modeSetter;
        private readonly IFpgaValueGetter _stateGetter;
        private readonly IFpgaValuesAccessor _fpgaValuesAccessor;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="hardwareSettings">
        /// The hardware settings used to access the storage.
        /// </param>
        /// <param name="deviceConnection">The connection used to access the device.</param>
        /// <param name="fpgaValuesAccessor">The accessor used to control access to FPGA values.</param>
        /// <param name="valueSetter">
        /// The setter used to set the value to be written to the storage.
        /// </param>
        /// <param name="valueGetter">
        /// The getter used to get the value read from the storage.
        /// </param>
        /// <param name="addressSetter">
        /// The setter used to set the address to write to ar read from.
        /// </param>
        /// <param name="modeSetter">
        /// The setter used to set the storage mode.
        /// </param>
        /// <param name="stateGetter">
        /// The getter used to get the storage state.
        /// </param>
        public StorageController(
            StorageHardwareSettings hardwareSettings,
            IFpgaConnection deviceConnection,
            IFpgaValuesAccessor fpgaValuesAccessor
        )
        {
            _hardwareSettings = hardwareSettings;
            _deviceConnection = deviceConnection;
            _fpgaValuesAccessor = fpgaValuesAccessor;

            _valueSetter = CreateFpgaValueSetter(24);
            _valueGetter = CreateFpgaValueGetter(24);
            _addressSetter = CreateFpgaValueSetter(25);
            _modeSetter = CreateFpgaValueSetter(26);
            _stateGetter = CreateFpgaValueGetter(26);
        }


        private IFpgaValueSetter CreateFpgaValueSetter(ushort registerNumber)
        {
            return _deviceConnection.CreateFpgaValueSetter(registerNumber);
        }

        private IFpgaValueGetter CreateFpgaValueGetter(ushort registerNumber)
        {
            return _deviceConnection.CreateFpgaValueGetter(registerNumber, SendMode.Storage);
        }

        /// <summary>
        /// Writes to the storage using the low-level SRAM controller protocol.
        /// </summary>
        /// <param name="address">The address to start writing to.</param>
        /// <param name="value">The values to write to the storage.</param>
        public void Write(int startAddress, IEnumerable<int> values)
        {
            foreach (var value in values)
            {
                Write(startAddress++, value);
            }
        }

        /// <summary>
        /// Reads from the storage using the low-level SRAM controller protocol.
        /// </summary>
        /// <param name="address">The address to start reading from.</param>
        /// <param name="numberOfValues">The number of values to read.</param>
        /// <returns>The values read.</returns>
        public IEnumerable<int> Read(int startAddress, int numberOfValues)
        {
            while (numberOfValues-- > 0)
            {
                yield return Read(startAddress++);
            }
        }

        /// <summary>
        /// Gets the value read from the storage.
        /// </summary>
        private int Value
        {
            get { return _valueGetter.ValueAsInt32; }
        }

        /// <summary>
        /// Gets the current storage state.
        /// </summary>
        private StorageState State
        {
            get { return (StorageState)_stateGetter.ValueAsInt32; }
        }

        /// <summary>
        /// Writes to the storage using the low-level SRAM controller protocol.
        /// </summary>
        private void Write(int address, int value)
        {
            Debug.WriteLine ("** Writing {0}={1} **", address, value);

            var withHandshake = _hardwareSettings.WriteWithHandshake;

            // Finish any pending access.
            SetMode(StorageMode.Idle);
            if (withHandshake) AwaitStateAndValue(state => state != StorageState.Writing, "any non-'writing' state");

            // Set address and value.
            PrepareWriteAccess(address, value);

            // Start writing.
            SetMode(StorageMode.Write);
            if (withHandshake) AwaitStateAndValue(StorageState.Writing);

            // Finish access.
            SetMode(StorageMode.Idle);
            if (withHandshake) AwaitStateAndValue(StorageState.Ready);

            Debug.WriteLine ("------------------------------");
        }

        /// <summary>
        /// Reads from the storage using the low-level SRAM controller protocol.
        /// </summary>
        private int Read(int address)
        {
            Debug.WriteLine ("------------------------------");

            var withHandshake = _hardwareSettings.ReadWithHandshake;

            // Set address.
            PrepareReadAccess(address);

            // Start reading.
            SetMode(StorageMode.Read);
            if (withHandshake) AwaitStateAndValue(StorageState.Reading);

            // Finish access.
            if (withHandshake)
            {
                SetMode(StorageMode.Idle);
                AwaitStateAndValue(StorageState.Ready);
            }
            else
            {
                GetStateAndValue();
            }

            // Get value.
            var value = Value;
            Debug.WriteLine ("** Read {0}={1} **", address, value);
            return value;
        }

        /// <summary>
        /// Prepares read access.
        /// </summary>
        private void PrepareReadAccess(int address)
        {
            Debug.WriteLine("=> Prepare read access...");
            _addressSetter.SetValue (address);
            _fpgaValuesAccessor.FlushSetters();
        }

        /// <summary>
        /// Prepares write access.
        /// </summary>
        private void PrepareWriteAccess(int address, int value)
        {
            Debug.WriteLine("=> Prepare write access...");
            _addressSetter.SetValue (address);
            _valueSetter.SetValue (value);
            _fpgaValuesAccessor.FlushSetters();
        }

        /// <summary>
        /// Sets the storage mode.
        /// </summary>
        private void SetMode(StorageMode mode)
        {
            Debug.WriteLine("=> Set mode to {0}...", mode);
            _modeSetter.SetValue ((ushort)mode);
            _fpgaValuesAccessor.FlushSetters();
        }

        /// <summary>
        /// Waits until the specified state is achieved. The value is supposed to be available as
        /// soon as the state matches.
        /// </summary>
        private void AwaitStateAndValue(StorageState state)
        {
            AwaitStateAndValue(st => st == state, string.Format("{0} state", state.ToString()));
        }

        /// <summary>
        /// Waits until the state satisfies the specified predicate. The value is supposed to be available as
        /// soon as the state matches.
        /// </summary>
        private void AwaitStateAndValue(Predicate<StorageState> statePredicate, string statePredicateCaption)
        {
            Debug.WriteLine("=> Waiting for {0}...", statePredicateCaption);
            int i = 0;
            while (!statePredicate(State))
            {
                QueryStateAndValue();
                // When using the c't Lab protocol, state and value are returned asynchronously. Wait a little
                // before polling another time.
                // When using direct SPI access, as it is synchronous, waiting is not necessary at all.
                if (_hardwareSettings.MillisecondsToWaitForAsynchronousReads != 0)
                {
                    Thread.Sleep (_hardwareSettings.MillisecondsToWaitForAsynchronousReads);
                }
                i++;
            }
            Debug.WriteLine("=> Achieved {0} after polling {1} times.", statePredicateCaption, i);
        }

        /// <summary>
        /// Read value directly.
        /// </summary>
        private void GetStateAndValue()
        {
            Debug.WriteLine("=> Waiting for value availability...");
            QueryStateAndValue();
        }

        /// <summary>
        /// Initiates transfer of the storage state and the value read from the storage.
        /// </summary>
        private void QueryStateAndValue()
        {
            _fpgaValuesAccessor.RefreshGetters();
        }
    }
}