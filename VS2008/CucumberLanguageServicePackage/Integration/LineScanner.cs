using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using CucumberLanguageServices.Integration;
using Microsoft.VisualStudio.Package;
using Irony.Parsing;
using TokenColor=Irony.Parsing.TokenColor;
using TokenTriggers=Irony.Parsing.TokenTriggers;
using TokenType=Irony.Parsing.TokenType;

namespace CucumberLanguageServices
{
    public class LineScanner : IScanner
    {
        private static readonly TokenEditorInfo DEFAULT_EDITOR_INFO = new TokenEditorInfo(TokenType.Text, TokenColor.Text, TokenTriggers.None);
        private GherkinGrammar _grammar;
        private Parser _parser;
        public StepProvider StepProvider { get; set; }
        private Token _previousToken;
        private Queue<TokenInfo> _queuedTokens = new Queue<TokenInfo>();

        public LineScanner(GherkinGrammar GherkinGrammar)
        {
            Debug.Print("LineScanner constructed using {0}", GherkinGrammar);
            _grammar = GherkinGrammar;
            SetParser(GherkinGrammar);
        }

        public void SetParser(GherkinGrammar GherkinGrammar)
        {
            _parser = new Parser(GherkinGrammar) {Context = {Mode = ParseMode.VsLineScan}};
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            //Debug.Print("LineScanner.ScanToken({1}) using {0}", _parser != null && _parser.Language != null ? _parser.Language.Grammar : null, state);
            // Reads each token in a source line and performs syntax coloring.  It will continue to
            // be called for the source until false is returned.
            //Debug.Print("reading token from {0}", _parser.Context != null && _parser.Context.Source != null ? _parser.Context.Source.Text : "<null>");
            if (_queuedTokens.Count > 0)
            {
                UpdateTokenInfoFromQueue(tokenInfo);
                return Returning(tokenInfo, state, _previousToken, true);
            }
            Token token = _parser.Scanner.VsReadToken(ref state);

            // !EOL and !EOF
            if (token != null && token.Terminal != GherkinGrammar.CurrentGrammar.Eof && token.Category != TokenCategory.Error && token.Length > 0)
            {
                tokenInfo.StartIndex = token.Location.Position;
                tokenInfo.EndIndex = tokenInfo.StartIndex + token.Length - 1;
                SetColorAndType(token, tokenInfo);
                SetTrigger(token, tokenInfo);
                ProcessStepIdentifiers(token, tokenInfo);
                return Returning(tokenInfo, state, token, true);
            }
            return Returning(tokenInfo, state, token, false);
        }

        private void UpdateTokenInfoFromQueue(TokenInfo tokenInfo)
        {
            var queuedToken = _queuedTokens.Dequeue();
            tokenInfo.StartIndex = queuedToken.StartIndex;
            tokenInfo.EndIndex = queuedToken.EndIndex;
            tokenInfo.Color = queuedToken.Color;
            tokenInfo.Trigger = queuedToken.Trigger;
            tokenInfo.Type = queuedToken.Type;
        }

        private bool Returning(TokenInfo tokenInfo, int state, Token token, bool returnValue)
        {
            Debug.Print("LineScanner.ScanToken({1}) => {2} ({0})", TokenInfo(tokenInfo, token), state, returnValue);
            _previousToken = token;
            return returnValue;
        }

        private void ProcessStepIdentifiers(Token token, TokenInfo tokenInfo)
        {
            if (token.Terminal != _grammar.Identifier || StepProvider == null)
                return;
            if ((_previousToken == null) || (!_grammar.Language.StepTerms.Contains(_previousToken.KeyTerm)))
                return;

            if (token.Text.Length == 1)
                tokenInfo.Trigger = Microsoft.VisualStudio.Package.TokenTriggers.MemberSelect;

            var stepDefinitions = StepProvider.FindMatchesFor(token.Text);
            if (stepDefinitions == null || stepDefinitions.Length == 0) return;
            
            var groups = stepDefinitions[0].Match(token.Text).Groups;
            if (groups.Count <= 1)
            {
                tokenInfo.Color = Microsoft.VisualStudio.Package.TokenColor.Comment;
                tokenInfo.Type = Microsoft.VisualStudio.Package.TokenType.Identifier;
                return;
            }
            var lastEndIndex = tokenInfo.StartIndex - 1;
            for (var i=1; i < groups.Count; i++)
            {
                var group = groups[i];
                var startIndex = tokenInfo.StartIndex + group.Index;
                if (startIndex > lastEndIndex + 1)
                    QueueIdentifierToken(lastEndIndex + 1, startIndex - 1);
                var captureTokenInfo = new TokenInfo
                                           {
                                               StartIndex = startIndex,
                                               EndIndex = startIndex + group.Length - 1,
                                               Color = Microsoft.VisualStudio.Package.TokenColor.Number,
                                               Type = Microsoft.VisualStudio.Package.TokenType.String
                                           };
                _queuedTokens.Enqueue(captureTokenInfo);
                lastEndIndex = captureTokenInfo.EndIndex;
            }
            if (lastEndIndex < tokenInfo.EndIndex)
                QueueIdentifierToken(lastEndIndex + 1, tokenInfo.EndIndex);
            UpdateTokenInfoFromQueue(tokenInfo);            
        }

        private void QueueIdentifierToken(int startIndex, int endIndex)
        {
            var tokenInfo = new TokenInfo
                                {
                                    StartIndex = startIndex,
                                    EndIndex = endIndex,
                                    Type = Microsoft.VisualStudio.Package.TokenType.Identifier,
                                    Color = Microsoft.VisualStudio.Package.TokenColor.Comment
                                };
            _queuedTokens.Enqueue(tokenInfo);
        }

        private static string TokenInfo(TokenInfo tokenInfo, Token token)
        {
            if (tokenInfo == null) return "<null>";
            return string.Format("TokenInfo({0}:{1} {2} '{3}' length={4})", 
                                 tokenInfo.StartIndex, 
                                 tokenInfo.EndIndex, 
                                 token != null ? (token.Terminal != null ? token.Terminal.Name : token.ValueString) : "<null>",
                                 token != null ? token.ValueString : string.Empty,
                                 token != null ? token.Length : 0
                                 );
        }

        private static void SetTrigger(Token token, TokenInfo tokenInfo)
        {
            var editorInfo = (token.KeyTerm != null ? token.KeyTerm.EditorInfo : token.EditorInfo) ?? DEFAULT_EDITOR_INFO;

            tokenInfo.Trigger =
                (Microsoft.VisualStudio.Package.TokenTriggers)editorInfo.Triggers;
        }

        private void SetColorAndType(Token token, TokenInfo tokenInfo)
        {
            var editorInfo = token.EditorInfo ?? DEFAULT_EDITOR_INFO;

            tokenInfo.Color = (Microsoft.VisualStudio.Package.TokenColor)editorInfo.Color;
            tokenInfo.Type = (Microsoft.VisualStudio.Package.TokenType)editorInfo.Type;

        }

        public void SetSource(string source, int offset)
        {
            // Stores line of source to be used by ScanTokenAndProvideInfoAboutIt.
            Debug.Print("LineScanner.SetSource({0},{1})", source, offset);
            _parser.Scanner.VsSetSource(source, offset);
        }
    }
}
