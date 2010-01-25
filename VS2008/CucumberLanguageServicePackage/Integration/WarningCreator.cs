using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CucumberLanguageServices.Integration
{
    public class WarningCreator : ParseTreeVisitor
    {
        public GherkinGrammar Grammar { get; set; }
        public Source Source { get; set; }
        public StepProvider StepProvider { get; set; }

        public readonly List<TextSpan> Result = new List<TextSpan>();

        public void CreateSpansForUndefinedSteps()
        {
            Process(Grammar.StepIdentifierWithOptionalArgument);
        }
        protected override void Visit(ParseTreeNode node)
        {
            var stepIdentifier = node.FindToken();

            if (stepIdentifier == null)
                return;

            if (StepProvider.HasMatchFor(stepIdentifier.Text))
                return;

            var textspan = CreateSpanFor(stepIdentifier.Location, stepIdentifier.Location.Position + stepIdentifier.Length, Source);
            Result.Add(textspan);
        }
    }
}
