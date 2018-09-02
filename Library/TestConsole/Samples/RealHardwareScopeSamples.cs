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
using System.Text;
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
        private readonly bool _spiDirect;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="spiDirect">A value indicating whether the hardware is directly accessed via the SPI interface.</param>
        public RealHardwareScopeSamples (bool spiDirect)
        {
            _spiDirect = spiDirect;
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

                var startAddress = 3u;
                var random = new Random ();
                var values = Enumerable.Range(0, 3).Select(item => (uint)random.Next (255)).ToArray();

                scope.Write(startAddress, values);

                Console.WriteLine ("========================================");

                // Read all values eagerly, then await any async 'String received' comments.
                // This is just for more beautiful output.
                var readValues = scope.Read(startAddress, values.Length).ToList();
                Thread.Sleep (100);

                Console.WriteLine ("========================================");

                Console.Write ("Written:");
                foreach (var value in values)
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
    }
}
