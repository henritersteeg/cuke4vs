using System;
using System.Collections.Generic;
using CucumberLanguageServices.i18n;
using CucumberLanguageServices.Integration;
using Irony.Ast;
using Irony.Parsing;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Package;
using TokenType=Irony.Parsing.TokenType;

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
                    var tokenInfo = GetTokenInfoOfFirstTokenOnLine(view, line, col);

                    if (tokenInfo.Token == (int)GherkinTerm.Step)
                        declarations = resolver.FindMembers(StepProvider, Grammar, line, col);
                    else
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

        private TokenInfo GetTokenInfoOfFirstTokenOnLine(IVsTextView view, int line, int col)
        {
            string source;
            view.GetTextStream(line, 0, line, col, out source);
            var scanner = new LineScanner(Grammar);
            scanner.SetSource(source, 0);
            var tokenInfo = new TokenInfo(0,0,Microsoft.VisualStudio.Package.TokenType.Unknown);
            int state = 0;
            scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state);
            return tokenInfo;
        }

        // ParseReason.GetMethods
        public override Microsoft.VisualStudio.Package.Methods GetMethods(int line, int col, string name)
        {
            return new Methods(resolver.FindMethods(parseResult, line, col, name));
        }

        // ParseReason.Goto
        public override string Goto(VSConstants.VSStd97CmdID cmd, IVsTextView textView, int line, int col, out TextSpan span)
        {
            switch (cmd)
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