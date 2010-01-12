using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace CucumberLanguageServices.CucumberLanguageService
{
    public class GherkinIdentifier : FreeTextLiteral
    {
        public GherkinIdentifier(string name) : base(name, FreeTextOptions.AllowEof, "\n", "\r")
        {
            Priority = LowestPriority + 100;
            EditorInfo = new TokenEditorInfo(TokenType.Identifier, TokenColor.Identifier, TokenTriggers.None);
        }
    }
}
