using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace CucumberLanguageServices.i18n
{
    public class LanguageTokenizer
    {
        public GherkinTerm Term { get; set; }
        public string Tokens { get; set; }
        public KeyTerm[] KeyTerms { get; set; }
        public string PostFix { get; set; }

        public KeyTerm[] CreateKeyTerms()
        {
            KeyTerms = Tokens != null ? Tokens.Split('|').Distinct().Select(token => CreateKeyTerm(token + PostFix)).ToArray() : new KeyTerm[] { };
            return KeyTerms;
        }

        private KeyTerm CreateKeyTerm(string token)
        {
            return new GherkinKeyTerm(Term, token)
                                {
                                    Flags = TermFlags.IsReservedWord,
                                    EditorInfo = new TokenEditorInfo(TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None) {}
                                };
        }

        public NonTerminal CreateIronyToken()
        {
            var result = new NonTerminal(Term.ToString());
            foreach (var keyTerm in CreateKeyTerms())
            {
                if (result.Rule == null)
                    result.Rule = keyTerm;
                else
                    result.Rule |= keyTerm;
            }
            return result;
        }
    }
}
