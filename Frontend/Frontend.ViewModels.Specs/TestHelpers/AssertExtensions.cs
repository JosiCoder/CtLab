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
using Should;
using SpecsFor.ShouldExtensions;

namespace CtLab.Frontend.ViewModels.Specs
{
    public static class AssertExtensions
    {
        private const double DefaultRelativeRange = 1e-10;

        public static void ShouldBeAround<T>(this T actual, T expected, double relativeRange)
        {
            var actualAsDouble = (double)Convert.ChangeType(actual, typeof(double));
            var expectedAsDouble = (double)Convert.ChangeType(expected, typeof(double));

            var maxDeviation = Math.Abs(expectedAsDouble * relativeRange);
            var low = expectedAsDouble - maxDeviation;
            var high = expectedAsDouble + maxDeviation;
            actualAsDouble.ShouldBeInRange (low, high);
        }

        public static void ShouldBeAround<T>(this T actual, T expected)
        {
            actual.ShouldBeAround (expected, DefaultRelativeRange);
        }
    }
}

