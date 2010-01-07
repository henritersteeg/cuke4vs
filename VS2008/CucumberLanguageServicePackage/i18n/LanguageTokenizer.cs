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

        public KeyTerm[] CreateKeyTerms()
        {
            return Tokens.Split('|').Distinct().Select(token => new KeyTerm(token, token)).ToArray();
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
