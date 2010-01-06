using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CucumberLanguageServices
{
    public static partial class Configuration
    {
        public const string Name = "Cucumber";
        public const string FormatList = "Cucumber File (*.feature)\n*.feature";

        static Configuration()
        {
            // default colors - currently, these need to be declared
            CreateColor("Keyword", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK);
            CreateColor("Comment", COLORINDEX.CI_DARKGREEN, COLORINDEX.CI_USERTEXT_BK);
            CreateColor("Identifier", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK);
            CreateColor("String", COLORINDEX.CI_MAROON, COLORINDEX.CI_USERTEXT_BK);
            CreateColor("Number", COLORINDEX.CI_RED, COLORINDEX.CI_USERTEXT_BK);
            CreateColor("Text", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK);
        }
    }
}