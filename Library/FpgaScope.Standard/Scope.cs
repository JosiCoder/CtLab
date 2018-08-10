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

using System.Collections.Generic;
using CtLab.FpgaScope.Interfaces;
using CtLab.FpgaConnection.Interfaces;

namespace CtLab.FpgaScope.Standard
{
    /// <summary>
    /// Represents an FPGA Lab scope.
    /// </summary>
    public class Scope : IScope
    {
        private readonly IFpgaConnection _fpgaConnection;
        private readonly StorageController _storageController;

        public IStorageController StorageController
        { get { return _storageController; } }

        /// <summary>
        /// Releases all resource used by this instance.
        /// </summary>
        public void Dispose()
        {
            _fpgaConnection.Dispose();
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="deviceConnection">The connection used to access the device.</param>
        public Scope(IFpgaConnection deviceConnection)
        {
            _fpgaConnection = deviceConnection;

            _storageController = new StorageController(
                CreateFpgaValueSetter(24),
                CreateFpgaValueGetter(24),
                CreateFpgaValueSetter(25),
                CreateFpgaValueSetter(26),
                CreateFpgaValueGetter(26)
            );
        }

        /// <summary>
        /// Resets the scope. 
        /// </summary>
        public void Reset()
        {
            // e.g.:
            // StorageController.XXX = ???;
        }

        private IFpgaValueSetter CreateFpgaValueSetter(ushort registerNumber)
        {
            return _fpgaConnection.CreateFpgaValueSetter(registerNumber);
        }

        private IFpgaValueGetter CreateFpgaValueGetter(ushort registerNumber)
        {
            return _fpgaConnection.CreateFpgaValueGetter(registerNumber);
        }
    }
}