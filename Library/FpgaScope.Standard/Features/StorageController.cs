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
using CtLab.Utilities;
using CtLab.Messages.Interfaces;
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
        public bool OptimizeSpiReading; // optional for SPI, considered only when ReadWithHandshake=false
        public int MillisecondsToWaitForAsynchronousReads; // 10 or more needed for c't Lab protocol, not needed for SPI
    }

    /// <summary>
    /// Communicates with a storage controller implemented within a c't Lab FPGA device configured as
    /// an FPGA Lab.
    /// </summary>
    internal class StorageController
    {
        private const uint _dataMask = 0x000000FF;
        private const uint _addressMask = 0x7FFFF00;
        private const uint _modeAndStateMask = 0xF8000000;
        private const int _dataLsb = 0;
        private const int _addressLsb = 8;
        private const int _modeAndStateLsb = 27;

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

            _storageSetter = CreateFpgaValueSetter((ushort)SpiRegister.StorageController);
            _storageGetter = CreateFpgaValueGetter((ushort)SpiRegister.StorageController);

            SetMode (StorageMode.Release);
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
        /// Captures to the storage using the low-level SRAM controller protocol.
        /// </summary>
        /// <param name="startAddress">The address to start writing at.</param>
        /// <param name="endAddress">The address to stop writing at.</param>
        public void Capture(uint startAddress, uint endAddress)
        {
            var withHandshake = _hardwareSettings.WriteWithHandshake;

            // Finish any pending write access.
            var lastMode = GetLastMode();
            if (IsWritingMode(lastMode))
            {
                SetMode(StorageMode.Idle);
            }
            if (withHandshake) AwaitStateAndValue(state => !IsWritingState(state), "any non-'writing' state");

            Capture(startAddress, endAddress, withHandshake);

            SetMode (StorageMode.Release);
        }

        /// <summary>
        /// Writes to the storage using the low-level SRAM controller protocol.
        /// </summary>
        /// <param name="address">The address to start writing at.</param>
        /// <param name="value">The values to write to the storage.</param>
        public void Write(uint startAddress, IEnumerable<uint> values)
        {
            var withHandshake = _hardwareSettings.WriteWithHandshake;

            // Finish any pending write access.
            var lastMode = GetLastMode();
            if (IsWritingMode(lastMode))
            {
                SetMode(StorageMode.Idle);
            }
            if (withHandshake) AwaitStateAndValue(state => !IsWritingState(state), "any non-'writing' state");

            foreach (var value in values)
            {
                Write(startAddress++, value, withHandshake);
            }

            SetMode (StorageMode.Release);
        }

        /// <summary>
        /// Reads from the storage using the low-level SRAM controller protocol.
        /// </summary>
        /// <param name="address">The address to start reading at.</param>
        /// <param name="numberOfValues">The number of values to read.</param>
        /// <returns>The values read.</returns>
        public IEnumerable<uint> Read(uint startAddress, int numberOfValues)
        {
            var withHandshake = _hardwareSettings.ReadWithHandshake;
            var optimizeSpiReading = !withHandshake && _hardwareSettings.OptimizeSpiReading;

            // Finish any pending write access.
            var lastMode = GetLastMode();
            if (IsWritingMode(lastMode))
            {
                // We use Read here already (instead of Idle) as we are going use Read afterwards.
                SetMode(StorageMode.Read);
            }
            if (withHandshake) AwaitStateAndValue(state => !IsWritingState(state), "any non-'writing' state");

            // Return all values in a well-defined order. For optimized SPI reading, skip the value that
            // we have got for the first read.
            var itemSkipped = false;
            var address = startAddress;
            while (numberOfValues-- > 0)
            {
                var readValue = Read(address++, withHandshake, optimizeSpiReading);
                if (optimizeSpiReading & !itemSkipped)
                {
                    itemSkipped = true;
                }
                else
                {
                    yield return readValue;
                }
            }

            // If the value of the first read has been skipped, get the value for the last read by
            // doing an additional read (to any arbitrary address, the start address in this case).
            if (itemSkipped)
            {
                yield return Read(startAddress, withHandshake, optimizeSpiReading);
            }

            SetMode (StorageMode.Release);
        }

        /// <summary>
        /// Captures to the storage using the low-level SRAM controller protocol. Before calling this method,
        /// the storage state must be non-writing.
        /// </summary>
        private void Capture(uint startAddress, uint endAddress, bool withHandshake)
        {
            // Prepare a random value that is used when capturing data (instead of hardware signals).
            var random = new Random ();
            var value = (uint)random.Next(255);

            Debug.WriteLine ("------------------------------");
            Debug.WriteLine ("** Capturing {0}={1} **", startAddress, value);

            // Prepare setting the end address.
            SetAddressAndValue(endAddress, value);

            // Set the end address.
            SetMode(StorageMode.Set2ndAddress);
            if (withHandshake) AwaitStateAndValue(StorageState.Setting2ndAddress);

            // Finish access.
            SetMode(StorageMode.Idle);
            if (withHandshake) AwaitStateAndValue(StorageState.Ready);

            // Prepare write access.
            SetAddressAndValue(startAddress, value);

            // Start writing (the first cell).
            SetMode(StorageMode.Write);
            if (withHandshake) AwaitStateAndValue(StorageState.Writing);

            // Initiate capture.
            SetMode(StorageMode.Capture);

            // Await end of capture.
            // This takes a while, thus waiting must be done even if no handshaking is used anywhere else.
            AwaitStateAndValue(StorageState.CapturingFinished);

            // Finish access.
            SetMode(StorageMode.Idle);
            if (withHandshake) AwaitStateAndValue(StorageState.Ready);

            Debug.WriteLine ("------------------------------");
        }

        /// <summary>
        /// Writes to the storage using the low-level SRAM controller protocol. Before calling this method,
        /// the storage state must be non-writing.
        /// </summary>
        private void Write(uint address, uint value, bool withHandshake)
        {
            Debug.WriteLine ("------------------------------");
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
        private uint Read(uint address, bool withHandshake, bool returnPreviousStateAndValue)
        {
            Debug.WriteLine ("------------------------------");

            // Prepare read access.
            // For direct SPI access, as a side effect, this also reads the current state and value from
            // the SPI slave. This can be used for optimization (see returnPreviousStateAndValue).
            SetAddress(address);

            // Start reading.
            SetMode(StorageMode.Read); // Without handshaking this is actually a no-op.
            if (withHandshake) AwaitStateAndValue(StorageState.Reading);

            // Finish access.
            if (withHandshake)
            {
                SetMode(StorageMode.Idle);
                AwaitStateAndValue(StorageState.Ready);
            }
            else if (! returnPreviousStateAndValue)
            {
                GetStateAndValue();
            }

            // Get value.
            var value = Value;

            Debug.WriteLine ("** Read {0}={1} **", address, value);
            if (returnPreviousStateAndValue)
            {
                Debug.WriteLine ("SPI optimization: delayed value, belongs to the previous access.");
            }
            Debug.WriteLine ("------------------------------");

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
        /// Sets the address for reading from the SRAM. For direct SPI access, as a side effect,
        /// this also reads the current state and value from the SPI slave.
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
        /// Sets the address and data for writing to the SRAM. For direct SPI access, as a side effect,
        /// this also reads the current state and value from the SPI slave.
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
        /// Determines whether the specified mode is a writing mode.
        /// </summary>
        private bool IsWritingMode(StorageMode mode)
        {
            return mode == StorageMode.Write || mode == StorageMode.Capture;
        }

        /// <summary>
        /// Determines whether the specified state is a writing state.
        /// </summary>
        private bool IsWritingState(StorageState state)
        {
            return state == StorageState.Writing || state == StorageState.CapturingFinished;
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