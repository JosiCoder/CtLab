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

namespace CtLab.Frontend.ViewModels.Specs
{
    public abstract class UnitHelperSpecs
        : SpecsFor<UnitHelper>
    {
    }


    public class When_getting_the_scaling_information_for_a_certain_scale_factor
        : UnitHelperSpecs
    {
        [Test]
        [TestCase(3, "k", "")]
        [TestCase(4, "k", "e1")]
        [TestCase(8, "M", "e2")]
        [TestCase(10, "G", "e1")]
        [TestCase(-3, "m", "")]
        [TestCase(-4, "μ", "e2")]
        [TestCase(-8, "n", "e1")]
        public void then_the_SUT_should_return_the_according_unit_prefix_and_value_suffix (
            int scaleFactorExponent, string unitPrefix, string valueSuffix)
        {
            var scaleFactorInfo = UnitHelper.GetScaleFactorInfo(scaleFactorExponent);
            scaleFactorInfo.UnitPrefix.ShouldEqual(unitPrefix);
            scaleFactorInfo.ValueSuffix.ShouldEqual(valueSuffix);
        }
    }
}

