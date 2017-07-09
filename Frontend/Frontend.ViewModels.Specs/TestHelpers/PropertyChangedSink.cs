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
using System.ComponentModel;
using Moq;

namespace CtLab.Frontend.ViewModels.Specs
{
    public interface INotifyPropertyChangedSink
    {
        void PropertyChanged(object sender, PropertyChangedEventArgs e);
    }

    public class PropertyChangedSink
    {
        public readonly IList<string> NotifiedPropertyNames;
        protected readonly Mock<INotifyPropertyChangedSink> _propertyChangedSinkMock;

        public PropertyChangedSink (INotifyPropertyChanged sut)
        {
            NotifiedPropertyNames = new List<string> ();
            _propertyChangedSinkMock = new Mock<INotifyPropertyChangedSink>();
            _propertyChangedSinkMock
                .Setup(sink => sink.PropertyChanged(sut, It.IsAny<PropertyChangedEventArgs>()))
                .Callback<object, PropertyChangedEventArgs> ((sender, args) =>
                {
                    NotifiedPropertyNames.Add(args.PropertyName);
                });
            sut.PropertyChanged += PropertyChanged;
        }

        public void PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            _propertyChangedSinkMock.Object.PropertyChanged (sender, args);
        }
    }

}

