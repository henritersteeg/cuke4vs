using System;
using System.Diagnostics;
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
            Debug.Print("LineScanner constructed using {0}", GherkinGrammar);
            SetParser(GherkinGrammar);
        }

        public void SetParser(GherkinGrammar GherkinGrammar)
        {
            parser = new Parser(GherkinGrammar) {Context = {Mode = ParseMode.VsLineScan}};
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            Debug.Print("LineScanner.ScanToken({1}) using {0}", parser != null && parser.Language != null ? parser.Language.Grammar : null, state);
            // Reads each token in a source line and performs syntax coloring.  It will continue to
            // be called for the source until false is returned.
            Debug.Print("reading token from {0}", parser.Context != null && parser.Context.Source != null ? parser.Context.Source.Text : "<null>");
            Token token = parser.Scanner.VsReadToken(ref state);

            // !EOL and !EOF
            if (token != null && token.Terminal != GherkinGrammar.CurrentGrammar.Eof && token.Category != TokenCategory.Error && token.Length > 0)
            {
                tokenInfo.StartIndex = token.Location.Position;
                tokenInfo.EndIndex = tokenInfo.StartIndex + token.Length - 1;
                SetColorAndType(token, tokenInfo);
                SetTrigger(token, tokenInfo);
                Debug.Print("LineScanner.ScanToken({1}) => true ({0})", TokenInfo(tokenInfo, token), state);
                return true;
            }

            Debug.Print("LineScanner.ScanToken({1}) => false ({0})", TokenInfo(tokenInfo, token), state);
            return false;
        }

        private static string TokenInfo(TokenInfo tokenInfo, Token token)
        {
            if (tokenInfo == null) return "<null>";
            return string.Format("TokenInfo({0}:{1} {2})", tokenInfo.StartIndex, tokenInfo.EndIndex, token != null ? (token.Terminal != null ? token.Terminal.Name : token.ValueString) : "<null>");
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
            // Stores line of source to be used by ScanTokenAndProvideInfoAboutIt.
            Debug.Print("LineScanner.SetSource({0},{1})", source, offset);
            parser.Scanner.VsSetSource(source, offset);
        }
    }
}
