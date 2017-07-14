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
using System.Collections.Generic;
using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using Moq;
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.Frontend.ViewModels.Specs
{
    public abstract class DdsGeneratorViewModelSpecs
        : SpecsFor<DdsGeneratorViewModel>
    {
        public interface IModulationPropertyChangedSink
        {
            void ModulationPropertyChanged(object sender, EventArgs e);
        }


        protected ModulationAndSynchronizationSource _modulationAndSynchronizationSource
            = ModulationAndSynchronizationSource.DdsGenerator1;
        protected Mock<IApplianceServices> _applianceServicesMock;
        protected Mock<IDdsGenerator> _ddsGeneratorMock;
        protected Mock<IDdsGenerator> _sourceDdsGeneratorMock;
        protected Mock<IScaleInputViewModel> _frequencyScaleInputVMMock;
        protected Mock<IScaleInputViewModel> _amplitudeScaleInputVMMock;
        protected Mock<IScaleInputViewModel> _phaseScaleInputVMMock;
        protected PropertyChangedSink _propertyChangedSink;

        protected override void InitializeClassUnderTest()
        {
            // Create all dependencies.
            _applianceServicesMock = new Mock<IApplianceServices>();
            _ddsGeneratorMock = new Mock<IDdsGenerator>();
            _sourceDdsGeneratorMock = new Mock<IDdsGenerator>();
            _frequencyScaleInputVMMock = new Mock<IScaleInputViewModel>();
            _amplitudeScaleInputVMMock = new Mock<IScaleInputViewModel>();
            _phaseScaleInputVMMock = new Mock<IScaleInputViewModel>();

            var sourceDdsGenerators =
                new Dictionary<ModulationAndSynchronizationSource, IDdsGenerator> ();
            sourceDdsGenerators.Add (_modulationAndSynchronizationSource, _sourceDdsGeneratorMock.Object);

            SUT = new DdsGeneratorViewModel(
                _applianceServicesMock.Object, _ddsGeneratorMock.Object,
                sourceDdsGenerators,
                _frequencyScaleInputVMMock.Object, _amplitudeScaleInputVMMock.Object,
                _phaseScaleInputVMMock.Object);

            // Ignore calls made to mocks within the SUTs constructor.
            _applianceServicesMock.ResetCalls ();
            _ddsGeneratorMock.ResetCalls ();
            _sourceDdsGeneratorMock.ResetCalls ();
            _frequencyScaleInputVMMock.ResetCalls ();
            _amplitudeScaleInputVMMock.ResetCalls ();
            _phaseScaleInputVMMock.ResetCalls ();
        }

        protected override void Given()
        {
            base.Given();

            _propertyChangedSink = new PropertyChangedSink(SUT);
        }

        protected void VerifyAMPropertyChangedNotifications()
        {
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("AmplitudeModulationSwing");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("RelativeAmplitudeModulationSwing");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("AmplitudeModulationExceedsLimits");
        }

        protected void VerifyFMPropertyChangedNotifications()
        {
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("FrequencyModulationSwing");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("RelativeFrequencyModulationSwing");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("FrequencyModulationExceedsLimits");
        }
    }


    public class When_changing_the_amplitude_modulation_source
        : DdsGeneratorViewModelSpecs
    {
        protected override void When()
        {
            SUT.AmplitudeModulationSource = _modulationAndSynchronizationSource;
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_all_amplitude_modulation_source_properties()
        {
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("AmplitudeModulationSource");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("BindingAmplitudeModulationSource");
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_all_amplitude_modulation_properties()
        {
            VerifyAMPropertyChangedNotifications ();
        }
    }


    public class When_changing_the_frequency_modulation_source
        : DdsGeneratorViewModelSpecs
    {
        protected override void When()
        {
            SUT.FrequencyModulationSource = _modulationAndSynchronizationSource;
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_all_frequency_modulation_source_properties()
        {
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("FrequencyModulationSource");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("BindingFrequencyModulationSource");
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_all_frequency_modulation_properties()
        {
            VerifyFMPropertyChangedNotifications ();
        }
    }

    public class When_changing_a_value_of_the_amplitude_scale
        : DdsGeneratorViewModelSpecs
    {
        protected Mock<IModulationPropertyChangedSink> _modulationPropertyChangedSinkMock;

        protected override void Given()
        {
            base.Given();

            _modulationPropertyChangedSinkMock = GetMockFor<IModulationPropertyChangedSink>();

            SUT.ModulationPropertyChanged +=
                _modulationPropertyChangedSinkMock.Object.ModulationPropertyChanged;
            SUT.AmplitudeModulationSource = _modulationAndSynchronizationSource;
            _propertyChangedSink.NotifiedPropertyNames.Clear ();
        }

        protected override void When()
        {
            _amplitudeScaleInputVMMock.Raise(vm => vm.PropertyChanged += null,
                new System.ComponentModel.PropertyChangedEventArgs(""));
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_all_amplitude_modulation_properties()
        {
            VerifyAMPropertyChangedNotifications ();
        }

        [Test]
        public void then_the_SUT_should_raise_the_modulation_property_changed_event()
        {
            _modulationPropertyChangedSinkMock.Verify (sink => sink.ModulationPropertyChanged (It.IsAny<object>(), It.IsAny<EventArgs>()), Times.Once);
        }
    }

    public class When_changing_a_value_of_the_frequency_scale
        : DdsGeneratorViewModelSpecs
    {
        protected override void Given()
        {
            base.Given();

            SUT.FrequencyModulationSource = _modulationAndSynchronizationSource;
            _propertyChangedSink.NotifiedPropertyNames.Clear ();
        }

        protected override void When()
        {
            _frequencyScaleInputVMMock.Raise(vm => vm.PropertyChanged += null,
                new System.ComponentModel.PropertyChangedEventArgs(""));
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_all_frequency_modulation_properties()
        {
            VerifyFMPropertyChangedNotifications ();
        }
    }

    public class When_changing_the_frequency_modulation_range
        : DdsGeneratorViewModelSpecs
    {
        protected override void When()
        {
            SUT.MaximumFrequencyModulationRange = 5;
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_all_frequency_modulation_properties()
        {
            VerifyFMPropertyChangedNotifications ();
        }
    }

    public class When_the_viewmodel_gets_a_notification_about_a_change_in_another_object_that_could_influence_its_modulation_properties
        : DdsGeneratorViewModelSpecs
    {
        protected override void When()
        {
            SUT.OtherDdsGeneratorModulationHasChanged();
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_all_amplitude_modulation_properties()
        {
            VerifyAMPropertyChangedNotifications ();
        }

        [Test]
        public void then_the_SUT_should_raise_property_changed_events_for_all_frequency_modulation_properties()
        {
            VerifyFMPropertyChangedNotifications ();
        }
    }
}

