using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CucumberLanguageServices.Integration
{
    public class RegionCreator : ParseTreeVisitor
    {
        public Source Source { get; set; }
        public readonly List<TextSpan> Result = new List<TextSpan>();

        public void CreateRegionsFor(params NonTerminal[] terms)
        {
            Process(terms);
        }

        protected override void Visit(ParseTreeNode node)
        {
            var span = CreateSpanFor(node.Span, Source);
            Debug.Print("RegionCreator: TextSpan({0}:{1}-{2}:{3}) created for {4}", span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex, node.Term.Name);
            Result.Add(span);
        }

    }
}
