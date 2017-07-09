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
using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using Moq;

namespace CtLab.Frontend.ViewModels.Specs
{
    public class TestViewModel : ViewModelBase
    {
        public TestViewModel (IApplianceServices applianceServices)
            : base(applianceServices)
        {}

        public void CallFlush()
        {
            Flush();
        }

        public int DirectProperty
        {
            get{ return 0; }
            set
            {
                RaisePropertyChanged ();
                RaisePropertyChanged (() => IndirectProperty);
            }
        }

        public int IndirectProperty
        {
            get;
            set;
        }
    }

    public abstract class ViewModelSpecs
        : SpecsFor<TestViewModel>
    {
        protected Mock<IApplianceServices> _applianceServicesMock;
        protected PropertyChangedSink _propertyChangedSink;

        protected override void Given()
        {
            base.Given();

            _applianceServicesMock = GetMockFor<IApplianceServices> ();
            _propertyChangedSink = new PropertyChangedSink(SUT);
        }
    }


    public class When_the_viewmodel_is_told_to_flush_pending_modifications_to_the_model
        : ViewModelSpecs
    {
        protected override void When()
        {
            SUT.CallFlush ();
        }

        [Test]
        public void it_should_call_the_flush_action_passed_to_it()
        {
            _applianceServicesMock.Verify(flush => flush.Flush(), Times.Once);
        }
    }


    public class When_a_property_is_set
        : ViewModelSpecs
    {
        protected override void When()
        {
            SUT.DirectProperty = 99;
        }

        [Test]
        public void it_should_raise_property_changed_events_for_direct_and_indirect_updates_as_specified_in_the_property_setter()
        {
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("DirectProperty");
            _propertyChangedSink.NotifiedPropertyNames.ShouldContain ("IndirectProperty");
        }
    }
}

