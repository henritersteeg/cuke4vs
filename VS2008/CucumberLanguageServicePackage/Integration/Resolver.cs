using System.Collections.Generic;
using CucumberLanguageServices.Integration;
using Irony.Parsing;

namespace CucumberLanguageServices
{
    public class Resolver : IASTResolver
    {
        #region IASTResolver Members


        public IList<Declaration> FindCompletions(StepProvider stepProvider, object result, int line, int col)
        {
            // Used for intellisense.
            var declarations = new List<Declaration>();

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
            return declarations;
        }

        public IList<Declaration> FindMembers(StepProvider stepProvider, object result, int line, int col)
        {
            var members = new List<Declaration>();

            if (stepProvider == null)
                return members;

            foreach (var step in stepProvider.StepDefinitions)
            {
                members.Add(new Declaration(step.Value, step.Value, 18 /* Method */, step.Value));
            }
            return members;
        }

        public string FindQuickInfo(object result, int line, int col)
        {
            return "unknown";
        }

        public IList<Method> FindMethods(object result, int line, int col, string name)
        {
            return new List<Method>();
        }

        #endregion
    }
}
