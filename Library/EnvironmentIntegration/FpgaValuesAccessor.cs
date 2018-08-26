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
using CtLab.FpgaConnection.Interfaces;
using CtLab.Messages.Interfaces;
//using CtLab.Environment;

namespace CtLab.EnvironmentIntegration
{
    /// <summary>
    /// Provides access to values within the FPGA.
    /// </summary>
    internal class FpgaValuesAccessor : IFpgaValuesAccessor
    {
        private readonly ISetCommandClassDictionary _setCommandClassDictionary;
        private readonly IQueryCommandScheduler _queryCommandScheduler;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="setCommandClassDictionary">The command class dictionary used to send the set commands.</param>
        /// <param name="queryCommandScheduler">The scheduler used to send the query commands.</param>
        public FpgaValuesAccessor (ISetCommandClassDictionary setCommandClassDictionary,
            IQueryCommandScheduler queryCommandScheduler)
        {
            _setCommandClassDictionary = setCommandClassDictionary;
            _queryCommandScheduler = queryCommandScheduler;
        }

        /// <summary>
        /// Sends modified setter values to the scope.
        /// </summary>
        public void FlushSetters()
        {
            _setCommandClassDictionary.SendCommandsForModifiedValues();
        }

        /// <summary>
        /// Refreshes getter values from the scope.
        /// </summary>
        /// <param name="predicate">The predicate the underlying command class' group must meet.</param>
        public void RefreshGetters(Predicate<CommandClassGroup> predicate)
        {
            _queryCommandScheduler.SendImmediately(predicate);
        }
    }
}

