using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace CucumberLanguageServices.i18n
{
    public class LanguageTokenizer
    {
        public string Name { get; set; }
        public string Tokens { get; set; }
        public KeyTerm[] KeyTerms { get; set; }

        public KeyTerm[] CreateKeyTerms()
        {
            KeyTerms = Tokens != null ? Tokens.Split('|').Distinct().Select(token => CreateKeyTerm(token)).ToArray() : new KeyTerm[] { };
            return KeyTerms;
        }

        private KeyTerm CreateKeyTerm(string token)
        {
            return new KeyTerm(token, Name)
                                {
                                    Options = TermOptions.IsReservedWord,
                                    EditorInfo = new TokenEditorInfo(TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None)
                                };
        }

        public NonTerminal CreateIronyToken()
        {
            var result = new NonTerminal(Name);
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
