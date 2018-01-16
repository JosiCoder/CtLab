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
using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using Moq;
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.Frontend.ViewModels.Specs
{
    public abstract class ApplicationSettingsContextBase : IContext<MainViewModel>
    {
        protected void Initialize(ISpecs<MainViewModel> specs, string portName)
        {
            var dialogServiceViewModelFactoryMock = specs.GetMockFor<IDialogServiceViewModelFactory>();
            dialogServiceViewModelFactoryMock
                .Setup (viewModelFactory => viewModelFactory.CreateApplicationSettingsViewModel (It.IsAny<IEnumerable<string>> (), It.IsAny<ApplicationSettings> ()))
                .Callback<IEnumerable<string>, ApplicationSettings> ((portNames, appSettings) =>
                {
                    appSettings.ConnectionType = ApplianceConnectionType.Serial;    
                    appSettings.PortName = portName;
                });

        }

        public abstract void Initialize (ISpecs<MainViewModel> specs);
    }

    public class user_sets_a_valid_port : ApplicationSettingsContextBase
    {
        public override void Initialize(ISpecs<MainViewModel> specs)
        {
            base.Initialize(specs, "port1");
        }
    }

    public class user_sets_an_invalid_port : ApplicationSettingsContextBase
    {
        public override void Initialize(ISpecs<MainViewModel> specs)
        {
            base.Initialize(specs, "invalid");
        }
    }


    public abstract class ApplicationSettingsResponseContextBase : IContext<MainViewModel>
    {
        protected void Initialize(ISpecs<MainViewModel> specs, DialogResult dialogResult)
        {
            var dialogServiceMock = specs.GetMockFor<IMainViewModelDialogService>();
            dialogServiceMock
                .Setup (dialogService => dialogService.ShowAndAdjustApplicationSettings (It.IsAny<IApplicationSettingsViewModel>()))
                .Returns (dialogResult);
        }

        public abstract void Initialize (ISpecs<MainViewModel> specs);
    }

    public class user_commits_application_settings : ApplicationSettingsResponseContextBase
    {
        public override void Initialize(ISpecs<MainViewModel> specs)
        {
            base.Initialize(specs, DialogResult.Ok);
        }
    }

    public class user_cancels_application_settings : ApplicationSettingsResponseContextBase
    {
        public override void Initialize(ISpecs<MainViewModel> specs)
        {
            base.Initialize(specs, DialogResult.Cancel);
        }
    }


    public abstract class MainViewModelSpecs
        : SpecsFor<MainViewModel>
    {
        protected Mock<IApplianceFactory> _applianceFactoryMock;
        protected Mock<IDialogServiceViewModelFactory> _dialogServiceViewModelFactoryMock;
        protected Mock<IMainViewModelDialogService> _dialogServiceMock;
        protected Mock<IApplicationSettingsViewModel> _applicationSettingsViewModelMock;
        protected Mock<IApplicationSettingsWriter> _applicationSettingsWriterMock;
        protected IEnumerable<Mock<IApplianceViewModel>> _oldApplianceVMMocks;

        protected override void Given()
        {
            base.Given();

            _applianceFactoryMock = GetMockFor<IApplianceFactory>();
            _dialogServiceViewModelFactoryMock = GetMockFor<IDialogServiceViewModelFactory>();
            _dialogServiceMock = GetMockFor<IMainViewModelDialogService>();
            _applicationSettingsViewModelMock = GetMockFor<IApplicationSettingsViewModel>();
            _applicationSettingsWriterMock = GetMockFor<IApplicationSettingsWriter> ();
            _oldApplianceVMMocks = GetMockForEnumerableOf<IApplianceViewModel> (3);

            _applianceFactoryMock
                .Setup (factory => factory.AvailablePortNames)
                .Returns (new []{"port1", "port2"});

            _dialogServiceViewModelFactoryMock
                .Setup (factory => factory.CreateApplicationSettingsViewModel (It.IsAny<IEnumerable<string>> (), It.IsAny<ApplicationSettings> ()))
                .Returns (_applicationSettingsViewModelMock.Object);

            SUT.DialogService = _dialogServiceMock.Object;

            _oldApplianceVMMocks.Select (mock => mock.Object)
                .ForEach (applianceVM => SUT.ApplianceVMs.Add (applianceVM));
        }
    }


    public class When_requesting_the_adjustment_of_the_application_settings
        : MainViewModelSpecs
    {
        protected override void When()
        {
            SUT.AdjustApplicationSettings();
        }

        [Test]
        public void then_the_SUT_should_ask_the_service_viewmodel_factory_to_create_an_application_settings_viewmodel()
        {
            _dialogServiceViewModelFactoryMock.Verify(
                factory => factory.CreateApplicationSettingsViewModel(It.IsAny<IEnumerable<string>>(), It.IsAny<ApplicationSettings> ()),
                Times.Once);
        }

        [Test]
        public void then_the_SUT_should_ask_the_dialog_service_to_provide_the_according_user_dialog()
        {
            _dialogServiceMock.Verify(
                dialogService => dialogService.ShowAndAdjustApplicationSettings(It.IsAny<IApplicationSettingsViewModel>()),
                Times.Once);
        }

        [Test]
        public void then_the_SUT_should_ask_the_dialog_service_to_provide_the_according_user_dialog_and_pass_the_application_settings_viewmodel_obtained_from_the_factory()
        {
            _dialogServiceMock.Verify(
                dialogService => dialogService.ShowAndAdjustApplicationSettings(_applicationSettingsViewModelMock.Object),
                Times.Once);
        }
    }

    public class When_requesting_the_adjustment_of_the_application_settings_and_the_user_sets_a_valid_port
        : MainViewModelSpecs
    {
        protected override void Given() 
        {
            base.Given();

            Given<user_sets_a_valid_port>();
            Given<user_commits_application_settings>();
        }

        protected override void When()
        {
            SUT.AdjustApplicationSettings();
        }

        [Test]
        public void then_the_SUT_should_dispose_the_old_appliance_viewmodels()
        {
            foreach(var applianceVMMock in _oldApplianceVMMocks)
            {
                applianceVMMock.Verify(
                    appVM => appVM.Dispose (),
                    Times.Once);
            }
        }

        [Test]
        public void then_the_SUT_should_create_the_new_appliances()
        {
            // Currently, we have only one appliance, but later we might have more.
            _applianceFactoryMock.Verify(
                factory => factory.CreateSerialAppliance (It.IsAny<string>(), It.IsAny<byte>()),
                Times.Once);
        }

        [Test]
        public void then_the_SUT_should_save_the_application_settings()
        {
            _applicationSettingsWriterMock.Verify(
                writer => writer.Write(It.IsAny<ApplicationSettings>()),
                Times.Once);
        }
    }

    public class When_requesting_the_adjustment_of_the_application_settings_and_the_user_sets_an_invalid_port
        : MainViewModelSpecs
    {
        protected override void Given()
        {
            base.Given();

            Given<user_sets_an_invalid_port>();
            Given<user_commits_application_settings>();
        }

        protected override void When()
        {
            SUT.AdjustApplicationSettings();
        }

        [Test]
        public void then_the_SUT_should_dispose_the_old_appliance_viewmodels()
        {
            foreach(var applianceVMMock in _oldApplianceVMMocks)
            {
                applianceVMMock.Verify(
                    appVM => appVM.Dispose (),
                    Times.Once);
            }
        }

        [Test]
        public void then_the_SUT_should_not_create_any_appliances()
        {
            _applianceFactoryMock.Verify(
                factory => factory.CreateSerialAppliance (It.IsAny<string>(), It.IsAny<byte>()),
                Times.Never);
        }

        [Test]
        public void then_the_SUT_should_save_the_application_settings()
        {
            _applicationSettingsWriterMock.Verify(
                writer => writer.Write(It.IsAny<ApplicationSettings>()),
                Times.Once);
        }

        [Test]
        public void then_the_SUT_should_show_a_message()
        {
            _dialogServiceMock.Verify(
                dialogService => dialogService.ShowMessageAndWaitForResponse (It.IsAny<DialogType>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }
    }

    public class When_requesting_the_adjustment_of_the_application_settings_and_the_user_cancels
        : MainViewModelSpecs
    {
        protected override void Given()
        {
            base.Given();

            Given<user_cancels_application_settings>();
        }

        protected override void When()
        {
            SUT.AdjustApplicationSettings();
        }

        [Test]
        public void then_the_SUT_should_not_dispose_the_old_appliance_viewmodels()
        {
            foreach(var applianceVMMock in _oldApplianceVMMocks)
            {
                applianceVMMock.Verify(
                    appVM => appVM.Dispose (),
                    Times.Never);
            }
        }

        [Test]
        public void then_the_SUT_should_not_create_any_appliances()
        {
            _applianceFactoryMock.Verify(
                factory => factory.CreateSerialAppliance (It.IsAny<string>(), It.IsAny<byte>()),
                Times.Never);
        }

        [Test]
        public void then_the_SUT_should_not_save_the_application_settings()
        {
            _applicationSettingsWriterMock.Verify(
                writer => writer.Write(It.IsAny<ApplicationSettings>()),
                Times.Never);
        }
    }
}

