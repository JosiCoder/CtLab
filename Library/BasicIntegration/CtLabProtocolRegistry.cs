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
using CtLab.Messages.Interfaces;
using CtLab.Messages.Standard;
using CtLab.CtLabProtocol.Interfaces;
using CtLab.CtLabProtocol.Standard;

namespace CtLab.BasicIntegration
{
    /// <summary>
    /// Registers required classes with the dependency injection container.
    /// </summary>
    public class CtLabProtocolRegistry : Registry
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        public CtLabProtocolRegistry()
        {
            // === Set commands ===

            For<ISetCommandClassDictionary<MessageChannel>>()
                .Singleton()
                .Use<SetCommandClassDictionary<MessageChannel>>();

            For<ISetCommandSender<MessageChannel>>()
                .Singleton()
                .Use<SetCommandSender>();

            For<ISetCommandStringBuilder<MessageChannel>>()
                .Singleton()
                .Use<CommandStringBuilder>();


            // === Query commands ===

            For<IQueryCommandScheduler<MessageChannel>>()
                .Singleton()
                .Use<QueryCommandScheduler<MessageChannel>>();

            For<IQueryCommandClassDictionary<MessageChannel>>()
                .Singleton()
                .Use<QueryCommandClassDictionary<MessageChannel>>();
            
            For<IQueryCommandSender<MessageChannel>>()
                .Singleton()
                .Use<QueryCommandSender>();

            For<IQueryCommandStringBuilder<MessageChannel>>()
                .Singleton()
                .Use<CommandStringBuilder>();


            // === Received messages ===

            For<IMessageCache<MessageChannel>>()
                .Singleton()
                .Use<ReceivedMessagesCache<MessageChannel>>();

            For<IMessageReceiver<MessageChannel>>()
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
