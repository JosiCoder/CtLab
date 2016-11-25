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
using StructureMap;
using CtLab.CommandsAndMessages.Interfaces;
using CtLab.CommandsAndMessages.Standard;
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.FpgaSignalGenerator.Standard;

namespace CtLab.FpgaSignalGenerator.Specs
{
    public abstract class FpgaLabDeviceSpecs
        : SpecsFor<SignalGenerator>
    {
        protected override void ConfigureContainer (IContainer container)
        {
            base.ConfigureContainer (container);

            // The contained message is a value object and thus can't be mocked. It also can't
            // be modified within a mocked message container via the interface. Thus we need a
            // real message container here instead of a mock.
            var rawValueContainer = new MessageContainer(1, 11);
            rawValueContainer.UpdateMessage(new Message() { RawValue = "120" });

            container.Configure (expression =>
                {
                    expression.ForConcreteType<SignalGenerator>()
                        .Configure.Ctor<byte>("channel").Is(7);
                }
            );
        }
    }


    public class When_using_an_AM_modulator
        : FpgaLabDeviceSpecs
    {
        protected override void When()
        {
            SUT.DdsGenerators[0].AmplitudeModulationSource = ModulationAndSynchronizationSources.DdsGenerator1;
            SUT.DdsGenerators[0].Amplitude = 1000;
            SUT.DdsGenerators[1].Amplitude = 500;
        }

        [Test]
        public void it_should_report_the_correct_modulation_values()
        {
            SUT.DdsGeneratorsAMInformationSets[0].RelativeModulationDepth.ShouldEqual(0.5f);
        }
    }


    public class When_selecting_the_carrier_as_its_own_AM_modulator
        : FpgaLabDeviceSpecs
    {
        protected override void When()
        {
            SUT.DdsGenerators[0].AmplitudeModulationSource = ModulationAndSynchronizationSources.DdsGenerator0;
            SUT.DdsGenerators[0].Amplitude = 1000;
        }

        [Test]
        public void it_should_not_use_that_and_thus_report_zero_modulation_values()
        {
            SUT.DdsGeneratorsAMInformationSets[0].RelativeModulationDepth.ShouldEqual(0.0f);
        }
    }


    public class When_using_an_FM_modulator
        : FpgaLabDeviceSpecs
    {
        protected override void When()
        {
            SUT.DdsGenerators[0].FrequencyModulationSource = ModulationAndSynchronizationSources.DdsGenerator1;
            SUT.DdsGenerators[0].Frequency = 1e3;
            SUT.DdsGenerators[0].MaximumFrequencyModulationRange = 1;
            SUT.DdsGenerators[1].Amplitude = SUT.DdsGenerators[1].MaximumAmplitude;
        }

        [Test]
        public void it_should_report_the_correct_modulation_values()
        {
            SUT.DdsGeneratorsFMInformationSets[0].ModulationDepth
                .ShouldEqual(SUT.DdsGenerators[0].MaximumFrequencyModulationDepth);
        }
    }


    public class When_selecting_the_carrier_as_its_own_FM_modulator
        : FpgaLabDeviceSpecs
    {
        protected override void When()
        {
            SUT.DdsGenerators[0].FrequencyModulationSource = ModulationAndSynchronizationSources.DdsGenerator0;
            SUT.DdsGenerators[0].Frequency = 1e3;
            SUT.DdsGenerators[0].MaximumFrequencyModulationRange = 1;
            SUT.DdsGenerators[0].Amplitude = SUT.DdsGenerators[0].MaximumAmplitude;
        }

        [Test]
        public void it_should_not_use_that_and_thus_report_zero_modulation_values()
        {
            SUT.DdsGeneratorsFMInformationSets[0].ModulationDepth
                .ShouldEqual(0.0f);
        }
    }
}
