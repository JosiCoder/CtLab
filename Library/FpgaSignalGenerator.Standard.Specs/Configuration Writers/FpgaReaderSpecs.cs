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
using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using Moq;
using CtLab.FpgaSignalGenerator.Standard;
using CtLab.FpgaConnection.Interfaces;

namespace CtLab.FpgaSignalGenerator.Standard.Specs
{
    public abstract class FpgaReaderSpecs<TSystemUnderTest>
        : SpecsFor<TSystemUnderTest>
        where TSystemUnderTest : class
    {
        protected Mock<IFpgaValueGetter> _valueGetterMock;

        protected override void Given()
        {
            base.Given();

            _valueGetterMock = GetMockFor<IFpgaValueGetter>();
        }
    }
}