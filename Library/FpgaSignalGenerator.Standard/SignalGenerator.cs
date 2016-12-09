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
using CtLab.Device.Base;
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.FpgaSignalGenerator.Standard
{
    /// <summary>
    /// Represents a signal generator based on the c't Lab.
    /// </summary>
    public class SignalGenerator : DeviceBase, ISignalGenerator
    {
        public readonly DdsGenerator[] _ddsGenerators;
        public readonly OutputSourceSelector _outputSourceSelector;
        public readonly PulseGenerator _pulseGenerator;
        public readonly UniversalCounter _universalCounter;

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
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="channel">
        /// The number of the channel assigned to the FPGA Lab device controlled by this instance.
        /// </param>
        /// <param name="setCommandClassDictionary">The command class dictionary used to send the set commands.</param>
        /// <param name="queryCommandClassDictionary">The command class dictionary used to send the query commands.</param>
        /// <param name="receivedMessagesCache">The message cache used to receive the messages.</param>
        public SignalGenerator(byte channel, ISetCommandClassDictionary setCommandClassDictionary,
            IQueryCommandClassDictionary queryCommandClassDictionary, IMessageCache receivedMessagesCache)
            : base(channel, setCommandClassDictionary, queryCommandClassDictionary, receivedMessagesCache)
        {
            // Four DDS generators.
            _ddsGenerators = new DdsGenerator[4];
            _ddsGenerators[0] = BuildDdsGenerator(channel, 16);
            _ddsGenerators[1] = BuildDdsGenerator(channel, 20);
            _ddsGenerators[2] = BuildDdsGenerator(channel, 24);
            _ddsGenerators[3] = BuildDdsGenerator(channel, 28);

            // The signal selectors for two outputs.
            _outputSourceSelector
                = new OutputSourceSelector(
                    BuildAndRegisterSetCommandClass(channel, 3));

            // The pulse generator.
            _pulseGenerator
                = new PulseGenerator(
                    BuildAndRegisterSetCommandClass(channel, 15),
                    BuildAndRegisterSetCommandClass(channel, 14));

            // The universal counter.
            _universalCounter
                = new UniversalCounter(
                    BuildAndRegisterSetCommandClass(channel, 12),
                    RegisterMessage(channel, 5),
                    RegisterMessage(channel, 4)
                    );
            BuildAndRegisterQueryCommandClass(channel, 4);
            BuildAndRegisterQueryCommandClass(channel, 5);
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

        private DdsGenerator BuildDdsGenerator(byte channel, ushort baseSubchannel)
        {
            return new DdsGenerator(
                           BuildAndRegisterSetCommandClass(channel, baseSubchannel),
                           BuildAndRegisterSetCommandClass(channel, (ushort)(baseSubchannel + 1)),
                           BuildAndRegisterSetCommandClass(channel, (ushort)(baseSubchannel + 2))
                       );
        }
    }
}