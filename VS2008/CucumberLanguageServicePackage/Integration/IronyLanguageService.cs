using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using CucumberLanguageServices.Integration;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Package;

using Irony.Parsing;
using Irony.Ast;

namespace CucumberLanguageServices
{
    public class IronyLanguageService : LanguageService
    {
        private GherkinGrammar GherkinGrammar;
        private Parser parser;
        private readonly StepProvider _stepProvider = new StepProvider();

        private bool _initialized;

        public IronyLanguageService()
        {
            InitParser(string.Empty);
        }

        public override void Initialize()
        {
            base.Initialize();
            if (_initialized) return;  // Initialize() is called twice...

            var solutionMonitor = new SolutionEventMonitor
                                        {
                                            StepProvider = _stepProvider,
                                            Solution = GetService(typeof(SVsSolution)) as IVsSolution
                                        };

            solutionMonitor.ProcessSolution(); // Since the current solution is already open...
            solutionMonitor.MonitorChanges();

            var codeMonitor = new CodeModelEventMonitor
                                  {
                                      DTE = GetService(typeof (DTE)) as DTE,
                                      StepProvider = _stepProvider
                                  };
            codeMonitor.MonitorChanges();
            _initialized = true;
        }

        public override Colorizer GetColorizer(IVsTextLines buffer)
        {
            Console.WriteLine();
            return base.GetColorizer(buffer);
        }

        private void InitParser(string sourceText)
        {
            if (GherkinGrammar != null && Equals(GherkinGrammar.GetLanguageFor(sourceText).Code, GherkinGrammar.Language.Code))
                return;
            GherkinGrammar = GherkinGrammar.CreateFor(sourceText);
            parser = new Parser(GherkinGrammar);
            if (scanner != null)
            {
                scanner.SetParser(GherkinGrammar);
                scanner.StepProvider = _stepProvider;
            }
            else
                scanner = new LineScanner(GherkinGrammar) { StepProvider = _stepProvider};
        }

        #region Custom Colors
        public override int GetColorableItem(int index, out IVsColorableItem item)
        {
            if (index <= Configuration.ColorableItems.Count)
            {
                item = Configuration.ColorableItems[index - 1];
                return VSConstants.S_OK;
            }
            throw new ArgumentNullException("index");
        }

        public override int GetItemCount(out int count)
        {
            count = Configuration.ColorableItems.Count;
            return VSConstants.S_OK;
        }
        #endregion

        #region MPF Accessor and Factory specialisation
        private LanguagePreferences preferences;
        public override LanguagePreferences GetLanguagePreferences()
        {
            if (preferences == null)
            {
                preferences = new LanguagePreferences(Site,
                                                        typeof(IronyLanguageService).GUID,
                                                        Name);
                preferences.Init();
                preferences.AutoOutlining = true;
                preferences.AutoListMembers = true;
            }
            return preferences;
        }

        public override Microsoft.VisualStudio.Package.Source CreateSource(IVsTextLines buffer)
        {
            return new Source(this, buffer, GetColorizer(buffer));
        }

        private LineScanner scanner;

        public override IScanner GetScanner(IVsTextLines buffer)
        {
            if (scanner == null)
                scanner = new LineScanner(GherkinGrammar);

            return scanner;
        }
        #endregion

        public override void OnIdle(bool periodic)
        {
            // from IronPythonLanguage sample
            // this appears to be necessary to get a parse request with ParseReason = Check?
            var src = (Source)GetSource(LastActiveTextView);
            if (src != null && src.LastParseTime >= Int32.MaxValue >> 12)
            {
                src.LastParseTime = 0;
            }
            base.OnIdle(periodic);
        }

        public override Microsoft.VisualStudio.Package.AuthoringScope ParseSource(ParseRequest req)
        {
            Debug.Print("ParseSource at ({0}:{1}), reason {2} (lang={3})", req.Line, req.Col, req.Reason, GherkinGrammar.GetLanguageFor(req.Text));
            var source = (Source)GetSource(req.FileName);
            ParseTreeNode root = null;
            switch (req.Reason)
            {
                case ParseReason.Goto:
                case ParseReason.Check:
                    // This is where you perform your syntax highlighting.
                    // Parse entire source as given in req.Text.
                    // Store results in the AuthoringScope object.
                    InitParser(req.Text);
                    var parseTree = parser.Parse(req.Text, req.FileName);
                    if (parseTree == null)
                        return null;

                    AddMessages(source, req.Sink, parseTree);

                    if (parseTree.Root == null)
                        return null;

                    root = parseTree.Root;
                    var node = (AstNode)parseTree.Root.AstNode;
                    source.ParseResult = node;

                    if (parser.Context.CurrentParseTree.ParserMessages.Count > 0)
                    {
                        foreach (ParserMessage error in parser.Context.CurrentParseTree.ParserMessages)
                        {
                            var span = new TextSpan();
                            span.iStartLine = span.iEndLine = error.Location.Line;
                            span.iStartIndex = error.Location.Column;
                            span.iEndIndex = error.Location.Position;
                            req.Sink.AddError(req.FileName, error.Message, span, Severity.Error);
                        }
                    }
                    CreateHiddenRegions(req.Sink, parseTree, source);
                    CreateWarningsForUnknownSteps(req.FileName, req.Sink, parseTree, source);
                    break;

                case ParseReason.DisplayMemberList:
                    // Parse the line specified in req.Line for the two
                    // tokens just before req.Col to obtain the identifier
                    // and the member connector symbol.
                    // Examine existing parse tree for members of the identifer
                    // and return a list of members in your version of the
                    // Declarations class as stored in the AuthoringScope
                    // object.
                    break;

                case ParseReason.MethodTip:
                    // Parse the line specified in req.Line for the token
                    // just before req.Col to obtain the name of the method
                    // being entered.
                    // Examine the existing parse tree for all method signatures
                    // with the same name and return a list of those signatures
                    // in your version of the Methods class as stored in the
                    // AuthoringScope object.
                    break;

                case ParseReason.HighlightBraces:
                case ParseReason.MemberSelectAndHighlightBraces:
                    if (source.Braces != null)
                    {
                        foreach (TextSpan[] brace in source.Braces)
                        {
                            if (brace.Length == 2)
                                req.Sink.MatchPair(brace[0], brace[1], 1);
                            else if (brace.Length >= 3)
                                req.Sink.MatchTriple(brace[0], brace[1], brace[2], 1);
                        }
                    }
                    break;
            }
            return new AuthoringScope(source.ParseResult) { Grammar = GherkinGrammar, Root = root, StepProvider = _stepProvider};
        }

        private void CreateWarningsForUnknownSteps(string path, AuthoringSink sink, ParseTree parseTree, Source source)
        {
            var warningCreator = new WarningCreator
                                     {
                                         Grammar = GherkinGrammar,
                                         Root = parseTree.Root,
                                         Source = source,
                                         StepProvider = _stepProvider
                                     };
            warningCreator.CreateSpansForUndefinedSteps();

            foreach (var textSpan in warningCreator.Result)
                sink.AddError(path, "Step not (yet) defined.", textSpan, Severity.Warning);
        }

        private void AddMessages(Source source, AuthoringSink sink, ParseTree parseTree)
        {
            var messages = parseTree.ParserMessages;
            foreach (var message in messages)
            {
                var line = source.GetLine(message.Location.Line);
                var nextSpace = line.Length;
                
                if (line.Length > message.Location.Column)
                    nextSpace = line.IndexOfAny(" \t\n\r".ToCharArray(), message.Location.Column + 1);
                
                if (nextSpace == -1)
                    nextSpace = line.Length;

                var span = new TextSpan
                                {
                                    iStartLine = message.Location.Line,
                                    iStartIndex = message.Location.Column,
                                    iEndLine = message.Location.Line,
                                    iEndIndex = nextSpace
                                };
                
                var severity = Severity.Hint;
                switch (message.Level)
                {
                    case ParserErrorLevel.Info:
                        severity = Severity.Hint;
                        break;

                    case ParserErrorLevel.Warning:
                        severity = Severity.Warning;
                        break;

                    case ParserErrorLevel.Error:
                        severity = Severity.Error;
                        break;
                }
                sink.AddError(source.GetFilePath(), message.Message, span, severity);
            }
        }

        private void CreateHiddenRegions(AuthoringSink sink, ParseTree parseTree, Source source)
        {
            var regionCreator = new RegionCreator
                                    {
                                        Root = parseTree.Root,
                                        Source = source
                                    };
            regionCreator.CreateRegionsFor(GherkinGrammar.Description, GherkinGrammar.GivenWhenThenClause);

            sink.ProcessHiddenRegions = true;
            foreach (var textSpan in regionCreator.Result)
                sink.AddHiddenRegion(textSpan);
        }

        /// <summary>
        /// Called to determine if the given location can have a breakpoint applied to it. 
        /// </summary>
        /// <param name="buffer">The IVsTextBuffer object containing the source file.</param>
        /// <param name="line">The line number where the breakpoint is to be set.</param>
        /// <param name="col">The offset into the line where the breakpoint is to be set.</param>
        /// <param name="pCodeSpan">
        /// Returns the TextSpan giving the extent of the code affected by the breakpoint if the 
        /// breakpoint can be set.
        /// </param>
        /// <returns>
        /// If successful, returns S_OK; otherwise returns S_FALSE if there is no code at the given 
        /// position or returns an error code (the validation is deferred until the debug engine is loaded). 
        /// </returns>
        /// <remarks>
        /// <para>
        /// CAUTION: Even if you do not intend to support the ValidateBreakpointLocation but your language 
        /// does support breakpoints, you must override the ValidateBreakpointLocation method and return a 
        /// span that contains the specified line and column; otherwise, breakpoints cannot be set anywhere 
        /// except line 1. You can return E_NOTIMPL to indicate that you do not otherwise support this 
        /// method but the span must always be set. The example shows how this can be done.
        /// </para>
        /// <para>
        /// Since the language service parses the code, it generally knows what is considered code and what 
        /// is not. Normally, the debug engine is loaded and the pending breakpoints are bound to the source. It is at this time the breakpoint location is validated. This method is a fast way to determine if a breakpoint can be set at a particular location without loading the debug engine.
        /// </para>
        /// <para>
        /// You can implement this method to call the ParseSource method with the parse reason of CodeSpan. 
        /// The parser examines the specified location and returns a span identifying the code at that 
        /// location. If there is code at the location, the span identifying that code should be passed to 
        /// your implementation of the CodeSpan method in your version of the AuthoringSink class. Then your 
        /// implementation of the ValidateBreakpointLocation method retrieves that span from your version of 
        /// the AuthoringSink class and returns that span in the pCodeSpan argument.
        /// </para>
        /// <para>
        /// The base method returns E_NOTIMPL.
        /// </para>
        /// </remarks>
        public override int ValidateBreakpointLocation(IVsTextBuffer buffer, int line, int col, TextSpan[] pCodeSpan)
        {
            // TODO: Add code to not allow breakpoints to be placed on non-code lines.
            // TODO: Refactor to allow breakpoint locations to span multiple lines.
            if (pCodeSpan != null)
            {
                pCodeSpan[0].iStartLine = line;
                pCodeSpan[0].iStartIndex = col;
                pCodeSpan[0].iEndLine = line;
                pCodeSpan[0].iEndIndex = col;
                if (buffer != null)
                {
                    int length;
                    buffer.GetLengthOfLine(line, out length);
                    pCodeSpan[0].iStartIndex = 0;
                    pCodeSpan[0].iEndIndex = length;
                }
                return VSConstants.S_OK;
            }
            return VSConstants.S_FALSE;
        }

        public override ViewFilter CreateViewFilter(CodeWindowManager mgr, IVsTextView newView)
        {
            return new IronyViewFilter(mgr, newView);
        }

        public override string Name
        {
            get { return Configuration.Name; }
        }

        public override string GetFormatFilterList()
        {
            return Configuration.FormatList;
        }
    }
}