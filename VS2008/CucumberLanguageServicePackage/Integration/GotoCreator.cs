using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CucumberLanguageServices.Integration
{
    class GotoCreator : ParseTreeVisitor
    {
        public GherkinGrammar Grammar { get; set; }
        public StepProvider StepProvider { get; set; }
        public int SelectedLine { get; set; }

        public string ResultFile { get; set; }
        public TextSpan ResultTextSpan { get; private set; }

        public void CreateGotoInfo()
        {
            Process(Grammar.StepIdentifierWithOptionalArgument);
        }

        protected override void Visit(ParseTreeNode node)
        {
            var stepIdentifier = node.FindToken();
            if (stepIdentifier == null || stepIdentifier.Terminal != Grammar.Identifier)
                return;
            var stepLocation = stepIdentifier.Location;
            if (stepLocation.Line != SelectedLine)
                return;
            
            var stepDefinitions = StepProvider.FindMatchesFor(stepIdentifier.Text);
            if (stepDefinitions.Length == 0)
                return;

            var stepDefinition = stepDefinitions[0];
            ResultTextSpan = new TextSpan
                                 {
                                     iStartLine = stepDefinition.StartPoint.Line - 1,
                                     iStartIndex = stepDefinition.StartPoint.DisplayColumn - 1,
                                     iEndLine = stepDefinition.EndPoint.Line - 1,
                                     iEndIndex = stepDefinition.EndPoint.DisplayColumn - 1
                                 };
            if (stepDefinition.ProjectItem.Document != null)
                ResultFile = stepDefinition.ProjectItem.Document.FullName;
            else if (stepDefinition.ProjectItem.FileCount > 0)
                ResultFile = stepDefinition.ProjectItem.get_FileNames(0);
        }
    }
}
