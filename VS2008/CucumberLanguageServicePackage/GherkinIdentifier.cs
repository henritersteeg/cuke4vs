using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace CucumberLanguageServices.CucumberLanguageService
{
    public class GherkinIdentifier : FreeTextLiteral
    {
        public GherkinIdentifier(string name) : base(name, "\n", "\r")
        {
            Priority = LowestPriority;
        }
    }
}
