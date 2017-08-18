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
using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using Moq;
using CtLab.FpgaSignalGenerator.Standard;

namespace CtLab.FpgaSignalGenerator.Specs
{
    public abstract class FpgaWriterSpecs<TSystemUnderTest>
        : SpecsFor<TSystemUnderTest>
        where TSystemUnderTest : class
    {
        protected Mock<IFpgaValueSetter> _valueSetterMock;
        protected object _lastValueSet;

        protected override void Given()
        {
            base.Given();

            _valueSetterMock = GetMockFor<IFpgaValueSetter>();

            _valueSetterMock.Setup(setter => setter.SetValue(It.IsAny<uint>())).Callback<uint>(value => { _lastValueSet = value; });
            _valueSetterMock.Setup(setter => setter.SetValue(It.IsAny<int>())).Callback<int>(value => { _lastValueSet = value; });
            _valueSetterMock.Setup(setter => setter.SetValue(It.IsAny<double>())).Callback<double>(value => { _lastValueSet = value; });
            _valueSetterMock.Setup(setter => setter.SetValue(It.IsAny<bool>())).Callback<bool>(value => { _lastValueSet = value; });
        }
    }
}