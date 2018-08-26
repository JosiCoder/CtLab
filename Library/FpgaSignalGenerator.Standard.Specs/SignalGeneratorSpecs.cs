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
using CtLab.Messages.Interfaces;
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.FpgaSignalGenerator.Standard;
using CtLab.FpgaConnection.Interfaces;

namespace CtLab.FpgaSignalGenerator.Standard.Specs
{
    public abstract class SignalGeneratorSpecs
        : SpecsFor<SignalGenerator>
    {
        protected override void ConfigureContainer (IContainer container)
        {
            base.ConfigureContainer (container);

            GetMockFor<IFpgaConnection> ()
                .Setup (dc => dc.CreateFpgaValueSetter(It.IsAny<ushort>(), It.IsAny<CommandClassGroup>()))
                .Returns (new Mock<IFpgaValueSetter>().Object);

            GetMockFor<IFpgaConnection> ()
                .Setup (dc => dc.CreateFpgaValueGetter(It.IsAny<ushort>(), It.IsAny<CommandClassGroup>()))
                .Returns (() =>
                    {
                        var valueGetterMock = new Mock<IFpgaValueGetter>();
                        valueGetterMock
                            .Setup(vg => vg.ValueAsUInt32)
                            .Returns(120);
                        return valueGetterMock.Object;
                    });
        }
    }


    public class When_using_an_AM_modulator
        : SignalGeneratorSpecs
    {
        protected override void When()
        {
            SUT.DdsGenerators[0].AmplitudeModulationSource = ModulationAndSynchronizationSource.DdsGenerator1;
            SUT.DdsGenerators[0].Amplitude = 1000;
            SUT.DdsGenerators[1].Amplitude = 500;
        }

        [Test]
        public void then_the_SUT_should_report_the_correct_modulation_values()
        {
            SUT.DdsGeneratorsAMInformationSets[0].RelativeModulationDepth.ShouldEqual(0.5f);
        }
    }


    public class When_selecting_the_carrier_as_its_own_AM_modulator
        : SignalGeneratorSpecs
    {
        protected override void When()
        {
            SUT.DdsGenerators[0].AmplitudeModulationSource = ModulationAndSynchronizationSource.DdsGenerator0;
            SUT.DdsGenerators[0].Amplitude = 1000;
        }

        [Test]
        public void then_the_SUT_should_not_use_that_and_thus_report_zero_modulation_values()
        {
            SUT.DdsGeneratorsAMInformationSets[0].RelativeModulationDepth.ShouldEqual(0.0f);
        }
    }


    public class When_using_an_FM_modulator
        : SignalGeneratorSpecs
    {
        protected override void When()
        {
            SUT.DdsGenerators[0].FrequencyModulationSource = ModulationAndSynchronizationSource.DdsGenerator1;
            SUT.DdsGenerators[0].Frequency = 1e3;
            SUT.DdsGenerators[0].MaximumFrequencyModulationRange = 1;
            SUT.DdsGenerators[1].Amplitude = SUT.DdsGenerators[1].MaximumAmplitude;
        }

        [Test]
        public void then_the_SUT_should_report_the_correct_modulation_values()
        {
            SUT.DdsGeneratorsFMInformationSets[0].ModulationDepth
                .ShouldEqual(SUT.DdsGenerators[0].MaximumFrequencyModulationDepth);
        }
    }


    public class When_selecting_the_carrier_as_its_own_FM_modulator
        : SignalGeneratorSpecs
    {
        protected override void When()
        {
            SUT.DdsGenerators[0].FrequencyModulationSource = ModulationAndSynchronizationSource.DdsGenerator0;
            SUT.DdsGenerators[0].Frequency = 1e3;
            SUT.DdsGenerators[0].MaximumFrequencyModulationRange = 1;
            SUT.DdsGenerators[0].Amplitude = SUT.DdsGenerators[0].MaximumAmplitude;
        }

        [Test]
        public void then_the_SUT_should_not_use_that_and_thus_report_zero_modulation_values()
        {
            SUT.DdsGeneratorsFMInformationSets[0].ModulationDepth
                .ShouldEqual(0.0f);
        }
    }
}
