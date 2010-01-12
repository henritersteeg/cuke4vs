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
        public GherkinGrammar Grammar { get; set; }
        public ParseTreeNode Root { get; set; }
        public readonly List<TextSpan> Spans = new List<TextSpan>();

        public List<TextSpan> CreateFeatureSpans()
        {
            return FindAllNodesFor(Grammar.FeatureClause, Root);
        }

        private List<TextSpan> FindAllNodesFor(NonTerminal terminal, ParseTreeNode node)
        {
            var result = new List<TextSpan>();

            if (node.Term == terminal)
            {
                TextSpan span = CreateSpanFor(node);
                result.Add(span);
                return result;
            }
            foreach (var childNode in node.ChildNodes)
            {
                result.AddRange(FindAllNodesFor(terminal, childNode));
            }
            return result;
        }

        private TextSpan CreateSpanFor(ParseTreeNode node)
        {
            var span = new TextSpan
                           {
                               iStartLine = node.Span.Location.Line + 1,
                               iStartIndex = node.Span.Location.Column,
                           };
            if (Source != null)
                Source.GetLineIndexOfPosition(node.Span.EndPosition, out span.iEndLine, out span.iEndIndex);
            Debug.Print("RegionCreator: TextSpan({0}:{1}-{2}:{3}) created for {4}", span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex, node.Term.Name);
            return span;
        }
    }
}
