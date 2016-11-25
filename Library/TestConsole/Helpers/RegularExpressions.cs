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
using System.Text.RegularExpressions;

namespace CtLab.TestConsole
{
    /// <summary>
    /// Provides means for evaluating regular expressions.
    /// This is just for testing and developing purposes.
    /// </summary>
    public static class RegularExpressions
    {
        /// <summary>
        /// Evaluates regular expressions.
        /// </summary>
        public static void Test()
        {
            Console.WriteLine("=== Begin of regular expression evaluation. ===");

            string text = "#7:255=0[OK]\n#8:254=1 [FAIL]";
            string pat = @"^#(?<channel>\d+)\s*:\s*(?<subchannel>\d+)\s*=\s*(?<value>\d+)\s*(\[(?<description>.*)\])?\s*$";
            // Compile the regular expression.
            Regex r = new Regex(pat, RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline);
            // Match the regular expression pattern against a text string.
            Match m = r.Match(text);
            while (m.Success)
            {
                // Display the first match and its capture set.
                System.Console.WriteLine("Match=[" + m.Value + "]");
                foreach (Capture c in m.Captures)
                {
                    System.Console.WriteLine("MCapture=[" + c.Value + "]");
                }
                // Display Group1 and its capture set.
                foreach (Group group in m.Groups)
                {
                    System.Console.WriteLine("Group=[" + group.Value + "]");
                    foreach (Capture c1 in group.Captures)
                    {
                        System.Console.WriteLine("GCapture=[" + c1.Value + "]");
                    }
                }
                // Advance to the next match.
                m = m.NextMatch();
            }

            Console.WriteLine("=== End of regular expression evaluation. ===");
        }
    }
}