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

namespace CtLab.Messages.Interfaces
{
    /// <summary>
    /// Provides facilities to schedule query commands contained in a query command
    /// dictionary, i.e. to send them periodically.
    /// </summary>
    public interface IQueryCommandScheduler
    {
        /// <summary>
        /// Gets an object that can be used to synchronize access to the current object
        /// and its internals.
        /// </summary>
        object SyncRoot { get; }

        /// <summary>
        /// Gets the underlying dictionary holding the command classes to send commands for.
        /// When scheduling is activated, this dictionary will be periodically accessed
        /// asynchronously, i.e. from a different thread.
        /// </summary>
        IQueryCommandClassDictionary CommandClassDictionary { get ; }

        /// <summary>
        /// Sends all scheduled commands immediately.
        /// </summary>
        /// <param name="predicate">The predicate that must be met.</param>
        void SendImmediately(Predicate<QueryMode> predicate);

        /// <summary>
        /// Starts sending the scheduled commands periodically using the specified period.
        /// This will periodically access the underlying command dictionary asynchronously,
        /// i.e. from a different thread.
        /// </summary>
        /// <param name="predicate">The predicate that must be met.</param>
        /// <param name="period">The period in milliseconds.</param>
        void StartSending(Predicate<QueryMode> predicate, long period);


        /// <summary>
        /// Stops sending the scheduled commands periodically.
        /// </summary>
        void StopSending();
    }
}