using System;
using Microsoft.VisualStudio.Package;
using Irony.Parsing;

namespace CucumberLanguageServices
{
    public class LineScanner : IScanner
    {
        private Parser parser;

        public LineScanner(GherkinGrammar GherkinGrammar)
        {
            this.parser = new Parser(GherkinGrammar);
            this.parser.Context.Mode = ParseMode.VsLineScan;
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            // Reads each token in a source line and performs syntax coloring.  It will continue to
            // be called for the source until false is returned.
            Token token = parser.Scanner.VsReadToken(ref state);

            // !EOL and !EOF
            if (token != null && token.Terminal != GherkinGrammar.CurrentGrammar.Eof && token.Category != TokenCategory.Error)
            {
                tokenInfo.StartIndex = token.Location.Position;
                tokenInfo.EndIndex = tokenInfo.StartIndex + token.Length - 1;
                tokenInfo.Color = (Microsoft.VisualStudio.Package.TokenColor)token.EditorInfo.Color;
                tokenInfo.Type = (Microsoft.VisualStudio.Package.TokenType)token.EditorInfo.Type;

                if (token.KeyTerm != null)
                {
                    tokenInfo.Trigger =
                        (Microsoft.VisualStudio.Package.TokenTriggers)token.KeyTerm.EditorInfo.Triggers;
                }
                else
                {
                    tokenInfo.Trigger =
                        (Microsoft.VisualStudio.Package.TokenTriggers)token.EditorInfo.Triggers;
                }

                return true;
            }

            return false;
        }

        public void SetSource(string source, int offset)
        {
            // Stores line of source to be used by ScanTokenAndProvideInfoAboutIt.
            parser.Scanner.VsSetSource(source, offset);
        }
    }
}
