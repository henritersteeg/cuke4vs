using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using CucumberLanguageServices.CucumberLanguageService;
using CucumberLanguageServices.i18n;
using Irony.Parsing;

namespace CucumberLanguageServices
{
    [Language("Cucumber", "1.0", "Cucumber feature language")]
    public class GherkinGrammar : Grammar
    {
        public NaturalLanguage Language { get; private set; }

        private static readonly CommentTerminal LineComment = new CommentTerminal("line-comment", "#", "\r", "\n", "\u2085", "\u2028", "\u2029");

        private const string IdentifierFirstChars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789[]{}<>?/\\|\"\';`~!@#$%^&*(),.-_+=";

        private readonly NonTerminal Behavior = new NonTerminal("behavior");
        private readonly NonTerminal Tags = new NonTerminal("tags");
        private readonly NonTerminal FeatureClause = new NonTerminal("feature-clause");
        private readonly NonTerminal FeatureLine = new NonTerminal("feature-line");
        public readonly NonTerminal Description = new NonTerminal("description");
        private readonly NonTerminal BackgroundClause = new NonTerminal("background-clause");
        private readonly NonTerminal BackgroundLine = new NonTerminal("background-line");
        private readonly NonTerminal Scenarios = new NonTerminal("scenarios");
        private readonly NonTerminal ScenarioWithTags = new NonTerminal("scenario-with-tags");
        private readonly NonTerminal ScenarioClause = new NonTerminal("scenario-clause");
        private readonly NonTerminal ScenarioLine = new NonTerminal("scenario-line");
        private readonly NonTerminal ScenarioOutlineWithTags = new NonTerminal("scenario-outline-with-tags");
        private readonly NonTerminal ScenarioOutlineClause = new NonTerminal("scenario-outline-clause");
        private readonly NonTerminal ScenarioOutlineLine = new NonTerminal("scenario-outline-line");
        private readonly NonTerminal ExamplesClause = new NonTerminal("examples-clause");
        public readonly NonTerminal GivenWhenThenClause = new NonTerminal("given-when-then-clause");
        private readonly NonTerminal GivenWhenThenLine = new NonTerminal("given-when-then-line");
        public readonly NonTerminal StepIdentifierWithOptionalArgument = new NonTerminal("step-identifier-with-optional-argument");
        private readonly NonTerminal Identifiers = new NonTerminal("identifiers");
        private readonly NonTerminal MultilineArg = new NonTerminal("multiline-arg");

        public readonly StringLiteral PyString = new StringLiteral("py-string", "\"\"\"", StringFlags.AllowsAllEscapes | StringFlags.AllowsLineBreak);
        private readonly NonTerminal Table = new NonTerminal("table");
        private readonly NonTerminal TableHeader = new NonTerminal("table-header");
        private readonly NonTerminal ColumnNames = new NonTerminal("column-names");
        private readonly NonTerminal TableRows = new NonTerminal("table-rows");
        private readonly NonTerminal TableRow = new NonTerminal("table-row");
        private readonly NonTerminal TableCells = new NonTerminal("table-cells");
        private readonly Terminal LastPipe = new RegexBasedTerminal("last-pipe", "\\|\\s*[\\r\\n]+", "|");
        public readonly Terminal DescriptiveLine = new GherkinIdentifier("descriptive-line") { Priority = Terminal.LowestPriority};

        public IdentifierTerminal Tag { get; private set; }
        public GherkinIdentifier Identifier { get; private set; }

        public readonly FreeTextLiteral ColumnName = new FreeTextLiteral("column-name", "|");
        public readonly FreeTextLiteral TableCell = new FreeTextLiteral("table-cell", "|");

        public static readonly Regex LanguageRegex = new Regex("^#\\s*language:\\s*([^\\s]+)\\s*\\n");

        private static int _instanceCounter = 0;
        private int _instanceNo;

        public static GherkinGrammar CreateFor(string sourceText)
        {
            var language = GetLanguageFor(sourceText);
            return new GherkinGrammar(language);
        }

        public static NaturalLanguage GetLanguageFor(string sourceText)
        {
            var match = LanguageRegex.Match(sourceText);
            return match.Success ? NaturalLanguageFactory.GetLanguage(match.Groups[1].Value) : NaturalLanguageFactory.DEFAULT_LANGUAGE;
        }

        public GherkinGrammar()
            : this(NaturalLanguageFactory.DEFAULT_LANGUAGE)
        {
        }

        public GherkinGrammar(NaturalLanguage language)
        {
            _instanceNo = ++_instanceCounter;
            InitGrammar(language);
            Debug.Print("{0} constructed.", this);
        }

        private void InitGrammar(NaturalLanguage language)
        {
            Language = language ?? NaturalLanguageFactory.DEFAULT_LANGUAGE;

            if (CurrentGrammar == null)
                new GherkinGrammar(Language); // Hack!

            DeclareKeyTerms();
            DeclareTerminals();
            DeclareNonTerminals();
            DefineRules();
            DefineKeywords();
        }

        private void DeclareKeyTerms()
        {
            KeyTerms.Clear();
            foreach (var keyTerm in Language.KeyTerms)
            {
                KeyTerms.Add(keyTerm.Text, keyTerm);
            }
        }

        public void SetLanguageFor(string sourceCode)
        {
            InitGrammar(GetLanguageFor(sourceCode));
        }

        private void DeclareTerminals()
        {
            Identifier = new GherkinIdentifier("identifier");
            Tag = new IdentifierTerminal("tag")
            {
                AllFirstChars = "@",
                AllChars = IdentifierFirstChars,
                EditorInfo = new TokenEditorInfo(TokenType.Identifier, TokenColor.Number, TokenTriggers.None)
            };
            NonGrammarTerminals.Add(LineComment);
        }

        private void DeclareNonTerminals()
        {

        }

        private void DefineRules()
        {
            Root = Behavior;
            Behavior.Rule = Tags + FeatureClause +
                            (Scenarios | BackgroundClause + Scenarios);

            Tags.Rule = MakeStarRule(Tags, Tag);

            FeatureClause.Rule = FeatureLine | FeatureLine + Description;

            FeatureLine.Rule = Language.Feature + Identifier;

            Description.Rule = MakePlusRule(Description, DescriptiveLine);

            BackgroundClause.Rule = BackgroundLine + GivenWhenThenClause;

            BackgroundLine.Rule = Language.Background + Identifiers;

            Scenarios.Rule = MakeStarRule(Scenarios, (ScenarioWithTags | ScenarioOutlineClause));

            ScenarioOutlineWithTags.Rule = Tags + ScenarioOutlineClause;

            ScenarioOutlineClause.Rule = ScenarioOutlineLine + Identifiers + GivenWhenThenClause + ExamplesClause;

            ScenarioOutlineLine.Rule = Language.ScenarioOutline + Identifier;

            ExamplesClause.Rule = Language.Examples + Table;

            ScenarioWithTags.Rule = Tags + ScenarioClause;

            ScenarioClause.Rule = ScenarioLine + Identifiers + GivenWhenThenClause;

            ScenarioLine.Rule = Language.Scenario + Identifier;

            GivenWhenThenClause.Rule = MakeStarRule(GivenWhenThenClause, GivenWhenThenLine);

            GivenWhenThenLine.Rule = Language.Steps + StepIdentifierWithOptionalArgument;
            
            StepIdentifierWithOptionalArgument.Rule = Identifier | Identifier + MultilineArg;

            MultilineArg.Rule = Table | PyString;

            Table.Rule = TableHeader + TableRows;

            TableHeader.Rule = ToTerm("|") + ColumnNames + LastPipe;

            ColumnNames.Rule = MakeStarRule(ColumnNames, ToTerm("|"), ColumnName);

            TableRows.Rule = MakeStarRule(TableRows, TableRow);

            TableRow.Rule = ToTerm("|") + TableCells + LastPipe;

            TableCells.Rule = MakeStarRule(TableCells, ToTerm("|"), TableCell);

            Identifiers.Rule = MakeStarRule(Identifiers, Identifier);
        }

        private void DefineKeywords()
        {
        }

        public override string ToString()
        {
            return string.Format("GherkinGrammer({0}, {1})", _instanceNo, Language != null ? Language.Code : "<null>");
        }
    }
}
