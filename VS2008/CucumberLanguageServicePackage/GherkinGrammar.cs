using System;
using System.Collections.Generic;
using System.Linq;
using CucumberLanguageServices.CucumberLanguageService;
using Irony.Parsing;

namespace CucumberLanguageServices
{
    [Language("Cucumber", "1.0", "Cucumber feature language")]
    public class GherkinGrammar : Grammar
    {
        private static readonly CommentTerminal LineComment = new CommentTerminal("line-comment", "#", "\r", "\n", "\u2085", "\u2028", "\u2029");

        private const string IdentifierFirstChars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789[]{}<>?/\\|\"\';`~!@#$%^&*(),.-_+=";

        private const string IdentifierChars = IdentifierFirstChars + ": ";

        private readonly NonTerminal Behavior = new NonTerminal("behavior");
        private readonly NonTerminal Tags = new NonTerminal("tags");
        private readonly NonTerminal TagLine = new NonTerminal("tag-line");
        private readonly NonTerminal TagsOnLine = new NonTerminal("tags-on-line");
        private readonly NonTerminal FeatureLine = new NonTerminal("feature-line");
        private readonly NonTerminal Description = new NonTerminal("description");
        private readonly NonTerminal BackgroundClause = new NonTerminal("background-clause");
        private readonly NonTerminal BackgroundLine = new NonTerminal("background-line");
        private readonly NonTerminal Scenarios = new NonTerminal("scenarios");
        private readonly NonTerminal ScenarioClause = new NonTerminal("scenario-clause");
        private readonly NonTerminal ScenarioLine = new NonTerminal("scenario-line");
        private readonly NonTerminal ScenarioOutlineClause = new NonTerminal("scenario-outline-clause");
        private readonly NonTerminal ScenarioOutlineLine = new NonTerminal("scenario-outline-line");
        private readonly NonTerminal ExamplesClause = new NonTerminal("examples-clause");
        private readonly NonTerminal GivenWhenThenClause = new NonTerminal("given-when-then-clause");
        private readonly NonTerminal GivenWhenThenLine = new NonTerminal("given-when-then-line");
        private readonly NonTerminal Identifiers = new NonTerminal("identifiers");
        private readonly NonTerminal MultilineArg = new NonTerminal("multiline-arg");

        public readonly StringLiteral PyString = new StringLiteral("py-string", "\"\"\"", StringFlags.AllowsAllEscapes | StringFlags.AllowsLineBreak);
        private readonly NonTerminal Table = new NonTerminal("table");
        private readonly NonTerminal TableHeader = new NonTerminal("table-header");
        private readonly NonTerminal ColumnNames = new NonTerminal("column-names");
        private readonly NonTerminal TableRows = new NonTerminal("table-rows");
        private readonly NonTerminal TableRow = new NonTerminal("table-row");
        private readonly NonTerminal TableCells = new NonTerminal("table-cells");
        private readonly Terminal LastColon = new RegexBasedTerminal("last-colon", "\\|\\s*[\\r\\n]+", "|");

        public readonly Terminal DescriptiveLine = new GherkinIdentifier("descriptive-line");

        public KeyTerm Feature { get; private set; }
        public KeyTerm Background { get; private set; }
        public KeyTerm Scenario { get; private set; }
        public KeyTerm ScenarioOutline { get; private set; }
        public KeyTerm Examples { get; private set; }
        public KeyTerm Given { get; private set; }
        public KeyTerm When { get; private set; }
        public KeyTerm Then { get; private set; }
        public KeyTerm And { get; private set; }
        public KeyTerm But { get; private set; }

        public IdentifierTerminal Tag { get; private set; }
        public GherkinIdentifier Identifier { get; private set; }

        public readonly FreeTextLiteral ColumnName = new FreeTextLiteral("column-name", "|");
        public readonly FreeTextLiteral TableCell = new FreeTextLiteral("table-cell", "|");

        public GherkinGrammar()
        {
            DeclareTerminals();

            DeclareNonTerminals();

            DefineRules();

            DefineKeywords();
        }

        private void DeclareTerminals()
        {
            Feature = new KeyTerm("Feature", "Feature");
            Background = new KeyTerm("Background", "Background");
            Scenario = new KeyTerm("Scenario", "Scenario");
            ScenarioOutline = new KeyTerm("Scenario Outline", "Scenario Outline");
            Examples = new KeyTerm("Examples", "Examples");
            Given = new KeyTerm("Given", "Given");
            When = new KeyTerm("When", "When");
            Then = new KeyTerm("Then", "Then");
            And = new KeyTerm("And", "And");
            But = new KeyTerm("But", "But");
            Identifier = new GherkinIdentifier("identifier");
            Tag = new IdentifierTerminal("tag") 
            {
                AllFirstChars = "@", AllChars = IdentifierFirstChars, 
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
            Behavior.Rule = Tags + (FeatureLine | FeatureLine + Description) +
                            (Scenarios | BackgroundClause + Scenarios);

            Tags.Rule = MakeStarRule(Tags, Tag);

            FeatureLine.Rule = Feature + ":" + Identifier;

            Description.Rule = MakePlusRule(Description, DescriptiveLine);

            BackgroundClause.Rule = BackgroundLine + GivenWhenThenClause;

            BackgroundLine.Rule = Background + ":" + Identifiers;

            Scenarios.Rule = MakeStarRule(Scenarios, (ScenarioClause | ScenarioOutlineClause));

            ScenarioOutlineClause.Rule = Tags + ScenarioOutlineLine + Identifiers + GivenWhenThenClause + ExamplesClause;

            ScenarioOutlineLine.Rule = ScenarioOutline + ":" + Identifier;

            ExamplesClause.Rule = Examples + ":" + Table;

            ScenarioClause.Rule = Tags + ScenarioLine + Identifiers + GivenWhenThenClause;

            ScenarioLine.Rule = Scenario + ":" + Identifier;

            GivenWhenThenClause.Rule = MakeStarRule(GivenWhenThenClause, GivenWhenThenLine);

            GivenWhenThenLine.Rule = (Given | When | Then | And | But) + (Identifier | Identifier + MultilineArg);

            MultilineArg.Rule = Table | PyString;

            Table.Rule = TableHeader + TableRows;

            TableHeader.Rule = ToTerm("|") + ColumnNames + LastColon;

            ColumnNames.Rule = MakeStarRule(ColumnNames, ToTerm("|"), ColumnName);

            TableRows.Rule = MakeStarRule(TableRows, TableRow);

            TableRow.Rule = ToTerm("|") + TableCells + LastColon;

            TableCells.Rule = MakeStarRule(TableCells, ToTerm("|"), TableCell);

            Identifiers.Rule = MakeStarRule(Identifiers, Identifier);
        }

        private void DefineKeywords()
        {
        }


    }
}
