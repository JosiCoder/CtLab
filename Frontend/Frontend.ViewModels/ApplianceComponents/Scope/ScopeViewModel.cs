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
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.Frontend.ViewModels
{
    // TODO: Remove demo parts?

    // TODO: Implement scope, describe like the signal generator's comment:
    /// <summary>
    /// Provides the viewmodel of a signal generator consisting of different
    /// signal-generating units and counters.
    /// </summary>

    /// <summary>
    /// Provides the viewmodel of a scope.
    /// </summary>
    public class ScopeViewModel : ViewModelBase, IScopeViewModel
    {
        private const char _channelCaptionBaseSymbol = '\u278A';// one of '\u2460', '\u2776', '\u278A';

        private readonly Color _baseColor = new Color(0.5, 0.8, 1.0);
        private readonly Color[] _channelColors = new Color[]
        {
            new Color(1, 1, 0),
            new Color(0, 1, 0),
        };

        private readonly IScopeScreenViewModel _masterScopeScreenVM = new ScopeScreenViewModel();
        private readonly IScopeScreenViewModel _slaveScopeScreenVM = new ScopeScreenViewModel();

        // TODO: Pass scope parts (e.g. storage, display,...) to the scope.
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="applianceServices">The services provided by the appliance.</param>
        /// <param name="signalGenerator">The signal generator to use.</param>
        /// <param name="universalCounterVM">The viewmodel of the universal counter.</param>
        /// <param name="pulseGeneratorVM">The viewmodel of the pulse generator.</param>
        /// <param name="ddsGeneratorVMs">The viewmodels of the DDS generators.</param>
        public ScopeViewModel (IApplianceServices applianceServices/*, ISignalGenerator signalGenerator,
            IUniversalCounterViewModel universalCounterVM, IPulseGeneratorViewModel pulseGeneratorVM,
            ObservableCollection<IDdsGeneratorViewModel> ddsGeneratorVMs*/)
            : base(applianceServices)
        {
            var sampleSequences = CreateSampleSequences();

            ConfigureMainScopeScreenVM(_masterScopeScreenVM, sampleSequences);
            ConfigureFFTScopeScreenVM(_slaveScopeScreenVM, sampleSequences);
        }

        /// <summary>
        /// Creates some sample sequences used to demonstrate scope features.
        /// </summary>
        private IEnumerable<SampleSequence> CreateSampleSequences()
        {
            var duration = 4.000000001; // ensure that the last point is included.

            yield return CreateDemoSampleSequence(duration, 64);

            //var interpolator = new LinearInterpolator();
            var interpolator = new SincInterpolator();
            yield return CreateDemoSampleSequence(duration, 8, interpolator, 64);
            //yield return CreateDemoSampleSequenceB();
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

            // LogDeferredAccess shows us some details about how the values are accessed (see there).
            return new SampleSequence(1f/sampleRate, values);
            //return new SampleSequence(1/sampleFrequency, LogDeferredAccess(values));
        }

        /// <summary>
        /// Creates a sample sequence used to demonstrate scope features.
        /// </summary>
        private SampleSequence CreateDemoSampleSequenceB()
        {
            var sampleFrequency = 1;
            var values =  new []{ -1d, 0d, 2d, 3d };
            return new SampleSequence(1f/sampleFrequency, values);
        }

        /// <summary>
        /// Configures the main scope screen viewmodel.
        /// </summary>
        private void ConfigureMainScopeScreenVM (IScopeScreenViewModel scopeScreenVM,
            IEnumerable<SampleSequence> sampleSequences)
        {
            // === Channels configuration ===

            var timeScaleFactor = 1;
            var channelVMs = new[]
            {
                new ChannelViewModel("V", new Position(0, 1), timeScaleFactor, 1,
                    BuildChannelCaptionFromIndex(0), _channelColors[0]),
                new ChannelViewModel("V", new Position(0, -2), timeScaleFactor, 1,
                    BuildChannelCaptionFromIndex(1), _channelColors[1]),
            };

            var index = 0;
            channelVMs[index].MeasurementCursor1VM.Visible = true;
            channelVMs[index].MeasurementCursor2VM.Visible = true;
            channelVMs[index].MeasurementCursor1VM.Value = 2.0;
            channelVMs[index].MeasurementCursor2VM.Value = 3.0;
            //            index++;
            //            channelVMs[index].MeasurementCursor1VM.Visible = true;
            //            channelVMs[index].MeasurementCursor2VM.Visible = true;
            //            channelVMs[index].MeasurementCursor1VM.Value = -0.5;
            //            channelVMs[index].MeasurementCursor2VM.Value = 0.5;
            scopeScreenVM.ChannelVMs = channelVMs;

            // === Graphbase configuration ===

            var graphbaseVM = new GraphbaseViewModel ("s", 1, _baseColor);

            var trigger = new LevelTrigger(LevelTriggerMode.RisingEdge, 0.5);
            var triggerChannelIndex = 0;

            graphbaseVM.TriggerVM =
                new LevelTriggerViewModel(trigger, channelVMs[triggerChannelIndex]);
            graphbaseVM.MeasurementCursor1VM.Visible = true;
            graphbaseVM.MeasurementCursor2VM.Visible = true;
            graphbaseVM.MeasurementCursor1VM.Value = 2.0;
            graphbaseVM.MeasurementCursor2VM.Value = 3.0;
            scopeScreenVM.GraphbaseVM = graphbaseVM;

            // === Sample Sequences ===

            var sampleSequenceProviders =
                sampleSequences.Select(ss => new Func<SampleSequence>(() => ss));

            var sampler = new Sampler(sampleSequenceProviders, trigger, triggerChannelIndex);
            scopeScreenVM.SampleSequenceProviders = sampler.SampleSequenceProviders;
        }

        /// <summary>
        /// Configures the FFT scope screen viewmodel.
        /// </summary>
        private void ConfigureFFTScopeScreenVM (IScopeScreenViewModel scopeScreenVM,
            IEnumerable<SampleSequence> sampleSequences)
        {
            // === Channels configuration ===

            var frequencyScaleFactor = 1;
            var channelVMs = new[]
            {
                new ChannelViewModel("dB?", new Position(0, 1), frequencyScaleFactor, 0.5,
                    BuildChannelCaptionFromIndex(0), _channelColors[0]),//TODO Unit
                new ChannelViewModel("dB?", new Position(0, -2), frequencyScaleFactor, 0.5,
                    BuildChannelCaptionFromIndex(1), _channelColors[1]),//TODO Unit
            };

            var index = 0;
            channelVMs[index].MeasurementCursor1VM.Visible = true;
            channelVMs[index].MeasurementCursor2VM.Visible = true;
            channelVMs[index].MeasurementCursor1VM.Value = -0.5;
            channelVMs[index].MeasurementCursor2VM.Value = 0.5;
            //            index++;
            //            channelVMs[index].MeasurementCursor1VM.Visible = true;
            //            channelVMs[index].MeasurementCursor2VM.Visible = true;
            //            channelVMs[index].MeasurementCursor1VM.Value = -0.5;
            //            channelVMs[index].MeasurementCursor2VM.Value = 0.5;
            scopeScreenVM.ChannelVMs = channelVMs;

            // === Graphbase configuration ===

            var graphbaseVM = new GraphbaseViewModel ("Hz", 0.2, _baseColor);

            graphbaseVM.MeasurementCursor1VM.Visible = true;
            graphbaseVM.MeasurementCursor2VM.Visible = true;
            graphbaseVM.MeasurementCursor1VM.Value = 2.0;
            graphbaseVM.MeasurementCursor2VM.Value = 3.0;
            scopeScreenVM.GraphbaseVM = graphbaseVM;

            // === Sample Sequences ===

            var sampleSequenceProviders =
                sampleSequences.Select(ss =>
                {
                    var fftSamples = DoFourierTransform(ss);
                    return new Func<SampleSequence>(() => fftSamples);
                });

            scopeScreenVM.SampleSequenceProviders = sampleSequenceProviders;
        }

        /// <summary>
        /// Transforms a time domain samples sequence to the frequency domain.
        /// </summary>
        private SampleSequence DoFourierTransform(SampleSequence samples)
        {
            var values = samples.Values.ToArray ();
            // Determine the largest possible FFT frame size, must be a power of 2.
            var numberOfSamples = values.Length;
            var fftFrameSize = (int)(0.5 + Math.Pow (2, Math.Floor (Math.Log (numberOfSamples, 2))));
            var trimmedSampleSequence = new SampleSequence (samples.SampleInterval, values.Take (fftFrameSize));
            return new Fourier().TransformForward (trimmedSampleSequence);
        }

        /// <summary>
        /// Creates a channel caption for the specified channel index.
        /// </summary>
        private string BuildChannelCaptionFromIndex(int channelIndex)
        {
            return ((char)(_channelCaptionBaseSymbol + channelIndex)).ToString();
        }

        /// <summary>
        /// Log the (deferred) access to the values in the specified enumerable. This shows
        /// us which values are accessed as well as when and how often they are accessed.
        /// </summary>
        private IEnumerable<T> LogDeferredAccess<T>(IEnumerable<T> values)
        {
            return values.ForEachDoDeferred(element => Console.WriteLine(element));
        }

        /// <summary>
        /// Gets the master scope screen viewmodel.
        /// </summary>
        public IScopeScreenViewModel MasterScopeScreenVM
        {
            get { return _masterScopeScreenVM; }
        }

        /// <summary>
        /// Gets the slave scope screen viewmodel.
        /// </summary>
        public IScopeScreenViewModel SlaveScopeScreenVM
        {
            get { return _slaveScopeScreenVM; }
        }
    }
}

