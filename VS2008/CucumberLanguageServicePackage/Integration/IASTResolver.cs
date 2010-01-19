using System;
using System.Collections.Generic;
using CucumberLanguageServices.Integration;

namespace CucumberLanguageServices
{
    interface IASTResolver
    {
        IList<Declaration> FindCompletions(StepProvider stepProvider, object result, int line, int col);
        IList<Declaration> FindMembers(StepProvider stepProvider, object result, int line, int col);
        string FindQuickInfo(object result, int line, int col);
        IList<Method> FindMethods(object result, int line, int col, string name);
    }
}