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

using System.Collections.Generic;
using CtLab.CommandsAndMessages.Interfaces;
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.FpgaSignalGenerator.Standard
{
    /// <summary>
    /// Represents an FPGA Lab signal generator.
    /// </summary>
    public class SignalGenerator : ISignalGenerator
    {
        private readonly IFpgaConnection _fpgaConnection;
        private readonly DdsGenerator[] _ddsGenerators;
        private readonly OutputSourceSelector _outputSourceSelector;
        private readonly PulseGenerator _pulseGenerator;
        private readonly UniversalCounter _universalCounter;

        public IDdsGenerator[] DdsGenerators
        { get { return (DdsGenerator[])_ddsGenerators.Clone(); } }

        public IOutputSourceSelector OutputSourceSelector
        { get { return _outputSourceSelector; } }

        public IPulseGenerator PulseGenerator
        { get { return _pulseGenerator; } }

        public IUniversalCounter UniversalCounter
        { get { return _universalCounter; } }

        /// <summary>
        /// Gets the amplitude modulation values for all DDS generators.
        /// </summary>
        public AmplitudeModulationInformationSet[] DdsGeneratorsAMInformationSets
        {
            get
            {
                var infoSetList = new List<AmplitudeModulationInformationSet>();

                foreach (var carrierGenerator in _ddsGenerators)
                {
                    // The enum is coded using the indices.
                    var index = (ushort)carrierGenerator.AmplitudeModulationSource;

                    var informer =
                        new AmplitudeModulationInformer(carrierGenerator);

                    // Use the according modulator. If the carrier references itself,
                    // don't use modulation.
                    informer.ModulatorSource =
                        carrierGenerator == _ddsGenerators[index]
                            ? null
                            : _ddsGenerators[index];

                    infoSetList.Add(informer.ModulationInformation);
                }

                return infoSetList.ToArray();
            }
        }

        /// <summary>
        /// Gets the frequency modulation values for all DDS generators.
        /// </summary>
        public FrequencyModulationInformationSet[] DdsGeneratorsFMInformationSets
        {
            get
            {
                var infoSetList = new List<FrequencyModulationInformationSet>();

                foreach (var carrierGenerator in _ddsGenerators)
                {
                    // The enum is coded using the indices.
                    var index = (ushort)carrierGenerator.FrequencyModulationSource;

                    var informer =
                        new FrequencyModulationInformer(carrierGenerator);

                    // Use the according modulator. If the carrier references itself,
                    // don't use modulation.
                    informer.ModulatorSource =
                        carrierGenerator == _ddsGenerators[index]
                            ? null
                            : _ddsGenerators[index];

                    infoSetList.Add(informer.ModulationInformation);
                }

                return infoSetList.ToArray();
            }
        }

        /// <summary>
        /// Releases all resource used by this instance.
        /// </summary>
        public void Dispose()
        {
            _fpgaConnection.Dispose();
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="deviceConnection">The connection used to access the device.</param>
        public SignalGenerator(IFpgaConnection deviceConnection)
        {
            _fpgaConnection = deviceConnection;

            // Four DDS generators.
            _ddsGenerators = new DdsGenerator[4];
            _ddsGenerators[0] = BuildDdsGenerator(16);
            _ddsGenerators[1] = BuildDdsGenerator(20);
            _ddsGenerators[2] = BuildDdsGenerator(24);
            _ddsGenerators[3] = BuildDdsGenerator(28);

            // The signal selectors for two outputs.
            _outputSourceSelector = new OutputSourceSelector(
                CreateFpgaValueSetter(3));

            // The pulse generator.
            _pulseGenerator = new PulseGenerator(
                CreateFpgaValueSetter(15),
                CreateFpgaValueSetter(14));

            // The universal counter.
            _universalCounter = new UniversalCounter(
                CreateFpgaValueSetter(12),
                CreateFpgaValueGetter(5),
                CreateFpgaValueGetter(4)
            );
        }

        /// <summary>
        /// Resets the signal generator. 
        /// </summary>
        public void Reset()
        {
            OutputSourceSelector.OutputSource0 = OutputSource.DdsGenerator0;
            OutputSourceSelector.OutputSource1 = OutputSource.DdsGenerator1;

            PulseGenerator.PulseDuration = 0;
            PulseGenerator.PauseDuration = 0;

            for (var index = 0; index < DdsGenerators.Length; index++)
            {
                var ddsGenerator = DdsGenerators[index];
                ddsGenerator.Frequency = 0;
                ddsGenerator.Amplitude = 0;
                ddsGenerator.Phase = 0;
                ddsGenerator.Waveform = Waveform.Sine;
                ddsGenerator.AmplitudeModulationSource = (ModulationAndSynchronizationSource)index;
                ddsGenerator.FrequencyModulationSource = (ModulationAndSynchronizationSource)index;
                ddsGenerator.PhaseModulationSource = (ModulationAndSynchronizationSource)index;
                ddsGenerator.SynchronizationSource = (ModulationAndSynchronizationSource)index;
                ddsGenerator.MaximumFrequencyModulationRange = 0;
            }

            UniversalCounter.InputSource = UniversalCounterSource.DdsGenerator0;
            UniversalCounter.PrescalerMode = PrescalerMode.GatePeriod_1s;
        }

        private DdsGenerator BuildDdsGenerator(ushort baseRegisterNumber)
        {
            return new DdsGenerator(
                CreateFpgaValueSetter(baseRegisterNumber),
                CreateFpgaValueSetter((ushort)(baseRegisterNumber + 1)),
                CreateFpgaValueSetter((ushort)(baseRegisterNumber + 2))
            );
        }

        private IFpgaValueSetter CreateFpgaValueSetter(ushort registerNumber)
        {
            return _fpgaConnection.CreateFpgaValueSetter(registerNumber);
        }

        private IFpgaValueGetter CreateFpgaValueGetter(ushort registerNumber)
        {
            return _fpgaConnection.CreateFpgaValueGetter(registerNumber);
        }
    }
}