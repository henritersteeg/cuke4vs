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
    public class RegionCreator
    {
        public Source Source { get; set; }
        public ParseTreeNode Root { get; set; }
        public readonly List<TextSpan> Result = new List<TextSpan>();

        public void CreateRegionsFor(params NonTerminal[] terms)
        {
            foreach (var term in terms)
            {
                CreateSpansFor(term, Root);
            }
        }

        private void CreateSpansFor(NonTerminal term, ParseTreeNode node)
        {
            if (node.Term == term)
            {
                CreateSpanFor(node);
                return;
            }
            foreach (var childNode in node.ChildNodes)
            {
                CreateSpansFor(term, childNode);
            }
        }

        private void CreateSpanFor(ParseTreeNode node)
        {
            var span = new TextSpan
                           {
                               iStartLine = node.Span.Location.Line,
                               iStartIndex = node.Span.Location.Column,
                           };
            if (Source != null)
                Source.GetLineIndexOfPosition(node.Span.EndPosition, out span.iEndLine, out span.iEndIndex);
            Debug.Print("RegionCreator: TextSpan({0}:{1}-{2}:{3}) created for {4}", span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex, node.Term.Name);
            Result.Add(span);
        }
    }
}
