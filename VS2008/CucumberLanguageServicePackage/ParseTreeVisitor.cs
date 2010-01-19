using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CucumberLanguageServices
{
    public abstract class ParseTreeVisitor
    {
        public ParseTreeNode Root { get; set; }

        public void Process(params BnfTerm[] terms)
        {
            foreach (var term in terms)
            {
                Process(term, Root);
            }
        }

        private void Process(BnfTerm term, ParseTreeNode node)
        {
            if (node.Term == term)
            {
                Visit(node);
                return;
            }
            foreach (var childNode in node.ChildNodes)
            {
                Process(term, childNode);
            }
        }

        protected abstract void Visit(ParseTreeNode node);

        #region Help methods

        protected TextSpan CreateSpanFor(SourceSpan span, Source source)
        {
            return CreateSpanFor(span.Location, span.EndPosition, source);
        }

        protected TextSpan CreateSpanFor(SourceLocation location, int endPosition, Source source)
        {
            var span = new TextSpan
            {
                iStartLine = location.Line,
                iStartIndex = location.Column,
            };
            if (source != null)
                source.GetLineIndexOfPosition(endPosition, out span.iEndLine, out span.iEndIndex);
            return span;
        }

        #endregion

    }
}
