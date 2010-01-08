using System;
using Microsoft.VisualStudio.Package;
using Irony.Parsing;
using TokenColor=Irony.Parsing.TokenColor;
using TokenTriggers=Irony.Parsing.TokenTriggers;
using TokenType=Irony.Parsing.TokenType;

namespace CucumberLanguageServices
{
    public class LineScanner : IScanner
    {
        private static TokenEditorInfo DEFAULT_EDITOR_INFO = new TokenEditorInfo(TokenType.Text, TokenColor.Text, TokenTriggers.None); 
        private Parser parser;

        public LineScanner(GherkinGrammar GherkinGrammar)
        {
            SetParser(GherkinGrammar);
        }

        private void SetParser(GherkinGrammar GherkinGrammar)
        {
            parser = new Parser(GherkinGrammar) {Context = {Mode = ParseMode.VsLineScan}};
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            // Reads each token in a source line and performs syntax coloring.  It will continue to
            // be called for the source until false is returned.
            Token token = parser.Scanner.VsReadToken(ref state);

            // !EOL and !EOF
            if (token != null && token.Terminal != GherkinGrammar.CurrentGrammar.Eof && token.Category != TokenCategory.Error && token.Length > 0)
            {
                tokenInfo.StartIndex = token.Location.Position;
                tokenInfo.EndIndex = tokenInfo.StartIndex + token.Length - 1;
                SetColorAndType(token, tokenInfo);
                SetTrigger(token, tokenInfo);

                return true;
            }

            return false;
        }

        private static void SetTrigger(Token token, TokenInfo tokenInfo)
        {
            var editorInfo = (token.KeyTerm != null ? token.KeyTerm.EditorInfo : token.EditorInfo) ?? DEFAULT_EDITOR_INFO;

            tokenInfo.Trigger =
                (Microsoft.VisualStudio.Package.TokenTriggers)editorInfo.Triggers;
        }

        private static void SetColorAndType(Token token, TokenInfo tokenInfo)
        {
            var editorInfo = token.EditorInfo ?? DEFAULT_EDITOR_INFO;
        
            tokenInfo.Color = (Microsoft.VisualStudio.Package.TokenColor)editorInfo.Color;
            tokenInfo.Type = (Microsoft.VisualStudio.Package.TokenType)editorInfo.Type;

        }

        public void SetSource(string source, int offset)
        {
            SetParser(GherkinGrammar.CreateFor(source));
            // Stores line of source to be used by ScanTokenAndProvideInfoAboutIt.
            parser.Scanner.VsSetSource(source, offset);
        }
    }
}
