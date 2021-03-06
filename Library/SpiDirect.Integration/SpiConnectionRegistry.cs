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
using StructureMap;
using CtLab.SpiConnection.Interfaces;
using CtLab.SpiConnection.Standard;
using CtLab.Connection.Interfaces;
using CtLab.Connection.Dummy;

namespace CtLab.SpiDirect.Integration
{
    /// <summary>
    /// Registers required classes with the dependency injection container.
    /// </summary>
    public class SpiConnectionRegistry : Registry
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        public SpiConnectionRegistry()
        {
            For<DummyConnection>()
                .Singleton()
                .Use<DummyConnection>();
            For<IConnection>()
                .Use(c => c.GetInstance<DummyConnection>());

            For<SpiWrapper>()
                .Singleton()
                .Use<SpiWrapper>();
            For<ISpiSender>()
                .Use(c => c.GetInstance<SpiWrapper>());
            For<ISpiReceiver>()
                .Use(c => c.GetInstance<SpiWrapper>());
        }
    }
}
