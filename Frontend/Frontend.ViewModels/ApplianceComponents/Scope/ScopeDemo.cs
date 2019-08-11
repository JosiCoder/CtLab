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
    // TODO: Remove demo parts? Comment?
    public class ScopeDemo
    {
        private const int demoTriggerChannelIndex = 0;

        private const char _channelCaptionBaseSymbol = '\u278A';// one of '\u2460', '\u2776', '\u278A';

        private readonly Color _baseColor = new Color(0.5, 0.8, 1.0);
        private readonly Color[] _channelColors = new Color[]
        {
            new Color(1, 1, 0),
            new Color(0, 1, 0),
        };

        /// <summary>
        /// Configures the main scope screen viewmodel.
        /// </summary>
        public void ConfigureMainScopeScreenVM (IScopeScreenViewModel scopeScreenVM,
            IEnumerable<Func<SampleSequence>> sampleSequenceGenerators)
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
            var triggerChannelIndex = demoTriggerChannelIndex;

            graphbaseVM.TriggerVM =
                new LevelTriggerViewModel(trigger, channelVMs[triggerChannelIndex]);
            graphbaseVM.MeasurementCursor1VM.Visible = true;
            graphbaseVM.MeasurementCursor2VM.Visible = true;
            graphbaseVM.MeasurementCursor1VM.Value = 2.0;
            graphbaseVM.MeasurementCursor2VM.Value = 3.0;
            scopeScreenVM.GraphbaseVM = graphbaseVM;

            // === Sample Sequences ===

            BuildMainSampleSequenceProviders(scopeScreenVM, sampleSequenceGenerators);
        }

        /// <summary>
        /// Builds a sequence provider for the main scope screen.
        /// </summary>
        private void BuildMainSampleSequenceProviders(
            IScopeScreenViewModel scopeScreenVM,
            IEnumerable<Func<SampleSequence>> sampleSequenceGenerators)
        {
            var triggerChannelIndex = demoTriggerChannelIndex;

            var sampleSequenceProviders = sampleSequenceGenerators.Select(ssg =>
            {
                return new Func<SampleSequence>(() => ssg());
            });
            var trigger = scopeScreenVM.GraphbaseVM.TriggerVM.Trigger;
            var sampler = new Sampler(sampleSequenceProviders, trigger, triggerChannelIndex);
            scopeScreenVM.SampleSequenceProviders = sampler.SampleSequenceProviders;
        }

        /// <summary>
        /// Configures the FFT scope screen viewmodel.
        /// </summary>
        public void ConfigureFFTScopeScreenVM (IScopeScreenViewModel scopeScreenVM,
            IEnumerable<Func<SampleSequence>> sampleSequenceGenerators)
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

            BuildFFTSampleSequenceProviders(scopeScreenVM, sampleSequenceGenerators);
        }

        /// <summary>
        /// Builds a sequence provider for the FFT scope screen.
        /// </summary>
        private void BuildFFTSampleSequenceProviders(
            IScopeScreenViewModel scopeScreenVM,
            IEnumerable<Func<SampleSequence>> sampleSequenceGenerators)
        {
            var sampleSequenceProviders = sampleSequenceGenerators.Select(ssg =>
            {
                SampleSequence fftSamples;
                try
                {
                    fftSamples = DoFourierTransform(ssg());
                }
                catch
                {
                    fftSamples = new SampleSequence(1, new double[0]);
                }
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
    }
}

