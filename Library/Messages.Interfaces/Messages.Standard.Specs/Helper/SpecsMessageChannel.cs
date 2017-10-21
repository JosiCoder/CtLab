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

using CtLab.Messages.Interfaces;

namespace CtLab.Messages.Standard.Specs
{
    /// <summary>
    /// Specifies a channel for testing purposes. This can also be used as an example
    /// for developing own message channels.
    /// </summary>
    #pragma warning disable 0660 // operator == or != defined without Equals() override
    public class SpecsMessageChannel : MessageChannelBase<SpecsMessageChannel>
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="id">The channel identifier.</param>
        public SpecsMessageChannel(int id)
            : base((i1, i2) => CompareContents((SpecsMessageChannel)i1, (SpecsMessageChannel)i2))
        {
            Id = id;
        }

        /// <summary>
        /// Gets the channel identifier.
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether contents the specified objects are equal.
        /// </summary>
        /// <param name="item1">The first object to compare.</param>
        /// <param name="item2">The second object to compare.</param>
        /// <returns>
        /// A value indicating whether contents the specified objects are equal.
        /// </returns>
        private static bool CompareContents(SpecsMessageChannel item1, SpecsMessageChannel item2)
        {
            return item1.Id == item2.Id;
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(SpecsMessageChannel item1, SpecsMessageChannel item2)
        {
            return CompareItems(item1, item2,
                (i1, i2) => CompareContents((SpecsMessageChannel)i1, (SpecsMessageChannel)i2));
        }

        public static bool operator !=(SpecsMessageChannel item1, SpecsMessageChannel item2)
        {
            return !(item1 == item2);
        }
    }
    #pragma warning restore 0660
}

