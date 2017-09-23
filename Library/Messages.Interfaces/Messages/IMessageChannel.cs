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

namespace CtLab.Messages.Interfaces
{
    /// <summary>
    /// Provides facilities to access a message channel.
    /// </summary>
    public interface IMessageChannel
    {
    }

    /// <summary>
    /// Provides the base functionality for message channels.
    /// </summary>
    /// <typeparam name="TMessageChannel">
    /// The type of the derived class.
    /// </typeparam>
    #pragma warning disable 0659 // Equals() overridden without GetHashCode()
    public abstract class MessageChannelBase<TMessageChannel> : IMessageChannel
        where TMessageChannel : class
    {
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>
        /// A value indicating whether the specified object is equal to the current object.
        /// </returns>
        public override bool Equals(object other)
        {
            return Equals(other as TMessageChannel);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>
        /// A value indicating whether the specified object is equal to the current object.
        /// </returns>
        public virtual bool Equals(TMessageChannel other)
        {
            if (object.ReferenceEquals(this, other)) { return true; }
            if (other == null) { return false; }
            return CompareContents(other);
        }

        /// <summary>
        /// Determines whether the references of the specified objects are equal.
        /// </summary>
        /// <param name="item1">The first object to compare.</param>
        /// <param name="item2">The second object to compare.</param>
        /// <returns>
        /// A value indicating whether the references of the specified objects are equal.
        /// </returns>
        protected static bool CompareReferences(TMessageChannel item1, TMessageChannel item2)
        {
            if (object.ReferenceEquals(item1, item2)) { return true; }
            if ((object)item1 == null || (object)item2 == null) { return false; }
            return true;
        }

        /// <summary>
        /// Determines whether contents the specified object are equal to contents of the
        /// current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>
        /// A value indicating whether contents the specified object are equal to contents
        /// of the current object.
        /// </returns>
        protected abstract bool CompareContents (TMessageChannel other);
    }
    #pragma warning restore 0659
}