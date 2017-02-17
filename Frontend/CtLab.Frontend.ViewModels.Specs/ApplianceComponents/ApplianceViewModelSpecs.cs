//------------------------------------------------------------------------------
// Copyright (C) 2017 Josi Coder

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
using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using Moq;
using CtLab.Environment;

namespace CtLab.Frontend.ViewModels.Specs
{
    public abstract class ApplianceViewModelSpecs
        : SpecsFor<ApplianceViewModel>
    {
        protected Mock<IApplianceServices> _applianceServicesMock;
        protected Mock<Appliance> _applianceMock;
        protected Mock<ISignalGeneratorViewModel> _signalGeneratorVMMock;
        protected PropertyChangedSink _propertyChangedSink;

        protected override void Given()
        {
            base.Given();

            _propertyChangedSink = new PropertyChangedSink(SUT);
        }
    }

    // ApplianceViewModel has currently no dedicated functionality, thus no specs.
}

