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

using StructureMap;
using CtLab.CommandsAndMessages.Interfaces;
using CtLab.CommandsAndMessages.Standard;

namespace CtLab.BasicIntegration
{
    /// <summary>
    /// Registers required classes with the dependency injection container.
    /// </summary>
    public class CommandsAndMessagesRegistry : Registry
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        public CommandsAndMessagesRegistry()
        {
            // === Set commands ===

            For<ISetCommandClassDictionary>()
                .Singleton()
                .Use<SetCommandClassDictionary>();

            For<ISetCommandSender>()
                .Singleton()
                .Use<SetCommandSender>();

            For<ISetCommandStringBuilder>()
                .Singleton()
                .Use<CommandStringBuilder>();


            // === Query commands ===

            For<IQueryCommandScheduler>()
                .Singleton()
                .Use<QueryCommandScheduler>();

            For<IQueryCommandClassDictionary>()
                .Singleton()
                .Use<QueryCommandClassDictionary>();
            
            For<IQueryCommandSender>()
                .Singleton()
                .Use<QueryCommandSender>();

            For<IQueryCommandStringBuilder>()
                .Singleton()
                .Use<CommandStringBuilder>();


            // === Received messages ===

            For<IMessageCache>()
                .Singleton()
                .Use<ReceivedMessagesCache>();

            For<IMessageReceiver>()
                .Singleton()
                .Use<MessageReceiver>();

            For<IMessageParser>()
                .Singleton()
                .Use<MessageParser>();


            // === Miscellaneous ===

            For<IChecksumCalculator>()
                .Use<ChecksumCalculator>();
        }
    }
}
