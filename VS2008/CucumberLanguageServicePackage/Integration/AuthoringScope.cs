using System;
using System.Collections.Generic;
using CucumberLanguageServices.Integration;
using Irony.Ast;
using Irony.Parsing;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Package;

namespace CucumberLanguageServices
{
    public class AuthoringScope : Microsoft.VisualStudio.Package.AuthoringScope
    {
        public GherkinGrammar Grammar { get; set; }
        public StepProvider StepProvider { get; set; }
        public ParseTreeNode Root { get; set; }

        public AuthoringScope(object parseResult)
        {
            this.parseResult = parseResult;

            // how should this be set?
            this.resolver = new Resolver();
        }

        object parseResult;
        IASTResolver resolver;

        // ParseReason.QuickInfo
        public override string GetDataTipText(int line, int col, out TextSpan span)
        {
            span = new TextSpan();
            return null;
        }

        // ParseReason.CompleteWord
        // ParseReason.DisplayMemberList
        // ParseReason.MemberSelect
        // ParseReason.MemberSelectAndHilightBraces
        public override Microsoft.VisualStudio.Package.Declarations GetDeclarations(IVsTextView view, int line, int col, TokenInfo info, ParseReason reason)
        {
            IList<Declaration> declarations;
            switch (reason)
            {
                case ParseReason.CompleteWord:
                    declarations = resolver.FindCompletions(StepProvider, Grammar, line, col);
                    break;
                case ParseReason.DisplayMemberList:
                case ParseReason.MemberSelect:
                case ParseReason.MemberSelectAndHighlightBraces:
                    declarations = resolver.FindMembers(StepProvider, Grammar, line, col);
                    break;
                default:
                    throw new ArgumentException("reason");
            }

            return new Declarations(declarations);
        }

        // ParseReason.GetMethods
        public override Microsoft.VisualStudio.Package.Methods GetMethods(int line, int col, string name)
        {
            return new Methods(resolver.FindMethods(parseResult, line, col, name));
        }

        // ParseReason.Goto
        public override string Goto(VSConstants.VSStd97CmdID cmd, IVsTextView textView, int line, int col, out TextSpan span)
        {
            switch(cmd)
            {
                case VSConstants.VSStd97CmdID.GotoDecl:
                case VSConstants.VSStd97CmdID.GotoDefn:
                case VSConstants.VSStd97CmdID.GotoRef:
                    var gotoCreator = new GotoCreator
                                          {
                                              Grammar = Grammar,
                                              Root = Root,
                                              SelectedLine = line,
                                              StepProvider = StepProvider
                                          };
                    gotoCreator.CreateGotoInfo();
                    span = gotoCreator.ResultTextSpan;
                    return gotoCreator.ResultFile;

                default:
                    span = new TextSpan();
                    return null;
            }
        }
    }
}