using System;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;

namespace CucumberLanguageServices
{
    public class Resolver : CucumberLanguageServices.IASTResolver
    {
        #region IASTResolver Members


        public IList<CucumberLanguageServices.Declaration> FindCompletions(object result, int line, int col)
        {
            // Used for intellisense.
            List<CucumberLanguageServices.Declaration> declarations = new List<CucumberLanguageServices.Declaration>();

            // Add keywords defined by grammar
            foreach (KeyTerm key in Configuration.GherkinGrammar.KeyTerms.Values)
            {
                if (key.OptionIsSet(TermOptions.IsKeyword))
                {
                    declarations.Add(new Declaration("", key.Name, 206, key.Name));
                }
            }

            declarations.Sort();
            return declarations;
        }

        public IList<CucumberLanguageServices.Declaration> FindMembers(object result, int line, int col)
        {
            List<CucumberLanguageServices.Declaration> members = new List<CucumberLanguageServices.Declaration>();

            return members;
        }

        public string FindQuickInfo(object result, int line, int col)
        {
            return "unknown";
        }

        public IList<CucumberLanguageServices.Method> FindMethods(object result, int line, int col, string name)
        {
            return new List<CucumberLanguageServices.Method>();
        }

        #endregion
    }
}
