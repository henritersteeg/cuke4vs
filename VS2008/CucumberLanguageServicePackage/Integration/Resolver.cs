using System;
using System.Collections.Generic;
using System.Text;
using CucumberLanguageServices.Integration;
using Irony.Parsing;

namespace CucumberLanguageServices
{
    public class Resolver : CucumberLanguageServices.IASTResolver
    {
        #region IASTResolver Members


        public IList<CucumberLanguageServices.Declaration> FindCompletions(StepProvider stepProvider, object result, int line, int col)
        {
            // Used for intellisense.
            var declarations = new List<CucumberLanguageServices.Declaration>();

            var grammar = result as Grammar;
            if (grammar == null)
                return declarations;

            // Add keywords defined by grammar
            foreach (KeyTerm key in grammar.KeyTerms.Values)
            {
                if (key.OptionIsSet(TermOptions.IsKeyword))
                {
                    declarations.Add(new Declaration(key.Name, key.Text, 206, key.Text));
                }
            }
            if (stepProvider == null)
                return declarations;

            foreach (var step in stepProvider.StepDefinitions)
            {
                declarations.Add(new Declaration(step.Value, step.Value, 18 /* Method */, step.Value));
            }
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
