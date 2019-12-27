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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ScopeLib.Utilities;
using ScopeLib.Sampling;
using ScopeLib.Display.ViewModels;

namespace CtLab.Frontend.ViewModels.SampleSequencesSources
{
    /// <summary>
    /// Provides sample sequences used to demonstrate scope features.
    /// </summary>
    public class DemoSampleSequencesSource
    {
        /// <summary>
        /// Returns some sample sequences used to demonstrate scope features.
        /// </summary>
        public IEnumerable<SampleSequence> GetSampleSequences()
        {
            var duration = 4.000000001; // ensure that the last point is included.

            yield return CreateDemoSampleSequence(duration, 64);

            //var interpolator = new LinearInterpolator();
            var interpolator = new SincInterpolator();
            yield return CreateDemoSampleSequence(duration, 8, interpolator, 64);
        }

        /// <summary>
        /// Creates a sample sequence used to demonstrate scope features.
        /// </summary>
        private SampleSequence CreateDemoSampleSequence(double duration, int sampleRate)
        {
            return CreateDemoSampleSequence(duration, sampleRate, null, 0);
        }

        /// <summary>
        /// Creates a sample sequence used to demonstrate scope features.
        /// </summary>
        private SampleSequence CreateDemoSampleSequence(double duration, int sampleRate,
            IInterpolator interpolator, int interpolatedSampleRate)
        {
            var values1 = FunctionValueGenerator.GenerateSineValuesForFrequency (1, sampleRate,
                duration, (x, y) => y);
            var values3 = FunctionValueGenerator.GenerateSineValuesForFrequency (3, sampleRate,
                duration, (x, y) => y/2);

            var values = CollectionUtilities.Zip(
                objects => ((double)objects[0]) + ((double)objects[1]),
                values1,
                values3);

            if (interpolator != null)
            {
                values = interpolator.Interpolate(values, 0, duration,
                    sampleRate, interpolatedSampleRate);

                sampleRate = interpolatedSampleRate;
            }

            return new SampleSequence(1f/sampleRate, values);
        }
    }
}

