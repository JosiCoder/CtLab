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
        private const uint _dataMask = 0x000000FF;
        private const uint _addressMask = 0x1FFFFF00;
        private const uint _modeAndStateMask = 0xE0000000;
        private const int _dataLsb = 0;
        private const int _addressLsb = 8;
        private const int _modeAndStateLsb = 29;

        private readonly StorageHardwareSettings _hardwareSettings;
        private readonly IFpgaConnection _deviceConnection;
        private readonly IFpgaValueSetter _storageSetter;
        private readonly IFpgaValueGetter _storageGetter;
        private readonly IFpgaValuesAccessor _fpgaValuesAccessor;

        private readonly CommandClassGroup _queryCommandClassGroup = new CommandClassGroup();

        private uint _storageSetterData = 0;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="hardwareSettings">
        /// The hardware settings used to access the storage.
        /// </param>
        /// <param name="deviceConnection">The connection used to access the device.</param>
        /// <param name="fpgaValuesAccessor">The accessor used to control access to FPGA values.</param>
        public StorageController(
            StorageHardwareSettings hardwareSettings,
            IFpgaConnection deviceConnection,
            IFpgaValuesAccessor fpgaValuesAccessor
        )
        {
            _hardwareSettings = hardwareSettings;
            _deviceConnection = deviceConnection;
            _fpgaValuesAccessor = fpgaValuesAccessor;

            _storageSetter = CreateFpgaValueSetter(24);
            _storageGetter = CreateFpgaValueGetter(24);
        }

        /// <summary>
        /// Creates an FPGA value setter.
        /// </summary>
        private IFpgaValueSetter CreateFpgaValueSetter(ushort registerNumber)
        {
            return _deviceConnection.CreateFpgaValueSetter(registerNumber, new CommandClassGroup());
        }

        /// <summary>
        /// Creates an FPGA value getter.
        /// </summary>
        private IFpgaValueGetter CreateFpgaValueGetter(ushort registerNumber)
        {
            return _deviceConnection.CreateFpgaValueGetter(registerNumber, _queryCommandClassGroup);
        }

        /// <summary>
        /// Writes to the storage using the low-level SRAM controller protocol.
        /// </summary>
        /// <param name="address">The address to start writing to.</param>
        /// <param name="value">The values to write to the storage.</param>
        public void Write(uint startAddress, IEnumerable<uint> values)
        {
            var withHandshake = _hardwareSettings.WriteWithHandshake;

            // Finish any pending write access.
            if (GetLastMode() == StorageMode.Write)
            {
                SetMode(StorageMode.Idle);
            }
            if (withHandshake) AwaitStateAndValue(state => state != StorageState.Writing, "any non-'writing' state");

            foreach (var value in values)
            {
                Write(startAddress++, value, withHandshake);
            }
        }

        /// <summary>
        /// Reads from the storage using the low-level SRAM controller protocol.
        /// </summary>
        /// <param name="address">The address to start reading from.</param>
        /// <param name="numberOfValues">The number of values to read.</param>
        /// <returns>The values read.</returns>
        public IEnumerable<uint> Read(uint startAddress, int numberOfValues)
        {
            var withHandshake = _hardwareSettings.ReadWithHandshake;

            // Finish any pending write access.
            if (GetLastMode() == StorageMode.Write)
            {
                SetMode(StorageMode.Read); // We use Read here (instead of Idle) as we are going to use Read all the time.
            }
            if (withHandshake) AwaitStateAndValue(state => state != StorageState.Writing, "any non-'writing' state");

            while (numberOfValues-- > 0)
            {
                yield return Read(startAddress++, withHandshake);
            }
        }

        /// <summary>
        /// Writes to the storage using the low-level SRAM controller protocol. Before calling this method,
        /// the storage state must be non-writing.
        /// </summary>
        private void Write(uint address, uint value, bool withHandshake)
        {
            Debug.WriteLine ("** Writing {0}={1} **", address, value);

            // Prepare write access.
            SetAddressAndValue(address, value);

            // Start writing.
            SetMode(StorageMode.Write);
            if (withHandshake) AwaitStateAndValue(StorageState.Writing);

            // Finish access.
            SetMode(StorageMode.Idle);
            if (withHandshake) AwaitStateAndValue(StorageState.Ready);

            Debug.WriteLine ("------------------------------");
        }

        /// <summary>
        /// Reads from the storage using the low-level SRAM controller protocol. Before calling this method,
        /// the storage state must be non-writing.
        /// </summary>
        private uint Read(uint address, bool withHandshake)
        {
            Debug.WriteLine ("------------------------------");

            // Prepare read access.
            SetAddress(address);

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
        /// Gets the value read from the storage.
        /// </summary>
        private uint Value
        {
            get { return (_storageGetter.ValueAsUInt32 & _dataMask) >> _dataLsb; }
        }

        /// <summary>
        /// Gets the current storage state.
        /// </summary>
        private StorageState State
        {
            get { return (StorageState) ((_storageGetter.ValueAsUInt32 & _modeAndStateMask) >> _modeAndStateLsb); }
        }

        /// <summary>
        /// Sets the address for reading from the SRAM.
        /// </summary>
        private void SetAddress(uint address)
        {
            Debug.WriteLine("=> Set address...");
            // Replace the old address with the new one.
            var preservedBits = _storageSetterData & ~_addressMask;
            _storageSetterData = preservedBits | address << _addressLsb;
            _storageSetter.SetValue (_storageSetterData);
            _fpgaValuesAccessor.FlushSetters();
        }

        /// <summary>
        /// Sets the address and data for writing to the SRAM.
        /// </summary>
        private void SetAddressAndValue(uint address, uint value)
        {
            Debug.WriteLine("=> Set address and data...");
            // Replace the old address and data with the new ones.
            var preservedBits = _storageSetterData & ~(_addressMask | _dataMask);
            _storageSetterData = preservedBits | address << _addressLsb | value << _dataLsb;
            _storageSetter.SetValue (_storageSetterData);
            _fpgaValuesAccessor.FlushSetters();
        }

        /// <summary>
        /// Sets the storage mode.
        /// </summary>
        private void SetMode(StorageMode mode)
        {
            Debug.WriteLine("=> Set mode to {0}...", mode);
            // Replace the old mode with the new one.
            var preservedBits = _storageSetterData & ~_modeAndStateMask;
            _storageSetterData = preservedBits | ((uint)(byte)mode) << _modeAndStateLsb;
            _storageSetter.SetValue (_storageSetterData);
            _fpgaValuesAccessor.FlushSetters();
        }

        /// <summary>
        /// Returns the storage mode used most recently.
        /// </summary>
        private StorageMode GetLastMode()
        {
            return (StorageMode)(byte)((_storageSetterData & _modeAndStateMask) >> _modeAndStateLsb);
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
            Debug.WriteLine("=> Waiting for {0}...", new []{statePredicateCaption}); // enforce the correct overload
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
            _fpgaValuesAccessor.RefreshGetters(classGroup => classGroup == _queryCommandClassGroup);
        }
    }
}