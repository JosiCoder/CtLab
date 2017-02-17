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
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides a base implementation for viewmodels.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private readonly TaskScheduler _scheduler;
        private readonly Thread _uiThread;
        private readonly IApplianceServices _applianceServices;

        /// <summary>
        /// Occurs when a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="applianceServices">The services provided by the appliance.</param>
        public ViewModelBase(IApplianceServices applianceServices)
        {
            _uiThread = Thread.CurrentThread;
            try
            {
                // Sometimes we don't have a proper synchronization context (e.g. during unit tests).
                _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            }
            catch
            {
                ; // intentionally left empty
            }
            _applianceServices = applianceServices;
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        public ViewModelBase()
            : this(null)
        {
        }

        /// <summary>
        /// Flush all pending modifications.
        /// </summary>
        protected void Flush()
        {
            if (_applianceServices != null)
            {
                _applianceServices.Flush ();
            }
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (Thread.CurrentThread != _uiThread)
            {
                System.Diagnostics.Debug.WriteLine (
                    "RaisePropertyChanged for {0} was called on thread id {1}, ignored.",
                    propertyName, Thread.CurrentThread.ManagedThreadId);
                return;
            }

            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
            {
                try
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
                catch
                {
                    
                }
            }
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property that has a new value</typeparam>
        /// <param name="propertyExpression">A lambda expression representing the property that has a new value.</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            string propertyName = ExtractPropertyName(propertyExpression);
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Extracts the property name from a lambda expression referencing that property.
        /// </summary>
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            var propertyInfo = memberExpression != null ? memberExpression.Member as PropertyInfo : null;
            var getMethod = propertyInfo != null ? propertyInfo.GetGetMethod(true) : null;
            if( getMethod == null || getMethod.IsStatic )
            {
                throw new ArgumentException("Invalid expression", "propertyExpression");
            }
            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Schedules the specified action using the task scheduler obtained from the
        /// synchronization context of the thread this object was created on. This causes
        /// the action to be dispatched on the UI thread if this object was created on the
        /// UI thread).
        /// The action is called directly if no task scheduler could be obtained from
        /// the synchronization context.
        /// </summary>
        /// <param name="action">The action to dispatch.</param>
        protected virtual void DispatchOnUIThread(System.Action action)
        {
            if (_scheduler != null)
            {
                Task.Factory.StartNew (() => { action (); },
                    Task.Factory.CancellationToken, TaskCreationOptions.None, _scheduler);
            }
            else
            {
                action ();
            }
        }
    }
}