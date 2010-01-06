using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using CucumberLanguageServices;

namespace CucumberLanguageServicePackageTests
{
    [TestFixture]
    public class GherkinGrammarTests : CucumberTestUtils
    {
        private const string FEATURE_LINE = "Feature: Parsing a feature line\n";
        private const string DESCRIPTION1 = "  This is a description, line 1.\n";
        private const string DESCRIPTION2 = "  This is a description, line 2.\n";
        private const string SCENARIO_ONE = "  Scenario: should return a valid parse tree.\n";
        private const string GIVEN_ONE    = "  Given     I have a parser\n";
        private const string GIVEN_AND_1  = "  And       I have a source\n";
        private const string WHEN_ONE     = "  When      I parse\n";
        private const string THEN_ONE     = "  Then      I expect a parse tree\n";
        private const string THEN_BUT_ONE = "  But       not a lexer in sight.\n";
        private const string SCENARIO_TWO = "  Scenario: should also return a valid parse tree.\n";
        private const string BACKGROUND   = "  Background: I am creating a parser.\n";

        [Test]
        public void Should_be_able_to_parse_comment_lines()
        {
            // Given
            var parser = new Parser(new GherkinGrammar());

            // When
            var tokens = parser.Parse("#comment line 1\n  #comment line 2\n").Tokens;

            // Then
            AssertNoError(tokens);
            Assert.That(tokens.Where(token => token.Category == TokenCategory.Comment).Count(), Is.EqualTo(2));
        }

        [Test]
        public void Should_be_able_to_parse_a_feature_line()
        {
            // Given
            var grammar = new GherkinGrammar();
            var parser = new Parser(grammar);

            // When
            var tokens = parser.Parse(FEATURE_LINE).Tokens;

            // Then
            AssertNoError(tokens);
            Assert.That(tokens.Where(token => token.KeyTerm == grammar.Feature).Count(), Is.EqualTo(1), "Feature");
            Assert.That(tokens.Where(token => token.Text == ":").Count(), Is.EqualTo(1), ":");
            Assert.That(tokens.Where(token => token.Terminal == grammar.Identifier).Count(), Is.EqualTo(1), "Identifier");
        }

        [Test]
        public void Should_be_able_to_parse_a_description()
        {
            // Given
            var grammar = new GherkinGrammar();
            var parser = new Parser(grammar);

            // When
            var parseTree = parser.Parse(FEATURE_LINE + DESCRIPTION1 + DESCRIPTION2);

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(tokens.Where(token => token.Terminal == grammar.DescriptiveLine).Count(), Is.EqualTo(2), "Descriptive line");
        }

        [Test]
        public void Should_be_able_to_parse_a_scenario()
        {
            // Given
            var grammar = new GherkinGrammar();
            var parser = new Parser(grammar);

            // When
            var parseTree = parser.Parse(FEATURE_LINE + DESCRIPTION1 + DESCRIPTION2 + "\n" + SCENARIO_ONE);

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(tokens.Where(token => token.KeyTerm == grammar.Scenario).Count(), Is.EqualTo(1), "Scenario");
            Assert.That(tokens.Where(token => token.Text == ":").Count(), Is.EqualTo(2), ":");
            Assert.That(tokens.Where(token => token.Terminal == grammar.Identifier).Count(), Is.EqualTo(2), "Identifier");
        }

        [Test]
        public void Should_be_able_to_parse_a_given()
        {
            // Given
            var grammar = new GherkinGrammar();
            var parser = new Parser(grammar);

            // When
            var parseTree = parser.Parse(FEATURE_LINE + DESCRIPTION1 + DESCRIPTION2 + "\n" + SCENARIO_ONE + GIVEN_ONE);

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(tokens.Where(token => token.KeyTerm == grammar.Given).Count(), Is.EqualTo(1), "Given");
            Assert.That(tokens.Where(token => token.Terminal == grammar.Identifier).Count(), Is.EqualTo(3), "Identifier");
        }

        [Test]
        public void Should_be_able_to_parse_a_when()
        {
            // Given
            var grammar = new GherkinGrammar();
            var parser = new Parser(grammar);

            // When
            var parseTree = parser.Parse(FEATURE_LINE + DESCRIPTION1 + DESCRIPTION2 + "\n" + SCENARIO_ONE + GIVEN_ONE + WHEN_ONE);

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(tokens.Where(token => token.KeyTerm == grammar.When).Count(), Is.EqualTo(1), "When");
            Assert.That(tokens.Where(token => token.Terminal == grammar.Identifier).Count(), Is.EqualTo(4), "Identifier");
        }

        [Test]
        public void Should_be_able_to_parse_a_then()
        {
            // Given
            var grammar = new GherkinGrammar();
            var parser = new Parser(grammar);

            // When
            var parseTree = parser.Parse(FEATURE_LINE + DESCRIPTION1 + DESCRIPTION2 + "\n" + SCENARIO_ONE + GIVEN_ONE + WHEN_ONE + THEN_ONE);

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(tokens.Where(token => token.KeyTerm == grammar.Then).Count(), Is.EqualTo(1), "Then");
            Assert.That(tokens.Where(token => token.Terminal == grammar.Identifier).Count(), Is.EqualTo(5), "Identifier");
        }

        [Test]
        public void Should_be_able_to_parse_a_given_and()
        {
            // Given
            var grammar = new GherkinGrammar();
            var parser = new Parser(grammar);

            // When
            var parseTree = parser.Parse(FEATURE_LINE + DESCRIPTION1 + DESCRIPTION2 + "\n" + SCENARIO_ONE + GIVEN_ONE + GIVEN_AND_1 + WHEN_ONE + THEN_ONE);

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(tokens.Where(token => token.KeyTerm == grammar.Then).Count(), Is.EqualTo(1), "Then");
            Assert.That(tokens.Where(token => token.Terminal == grammar.Identifier).Count(), Is.EqualTo(6), "Identifier");
        }

        [Test]
        public void Should_be_able_to_parse_a_then_but()
        {
            // Given
            var grammar = new GherkinGrammar();
            var parser = new Parser(grammar);

            // When
            var parseTree = parser.Parse(FEATURE_LINE + DESCRIPTION1 + DESCRIPTION2 + "\n" + SCENARIO_ONE + GIVEN_ONE + WHEN_ONE + THEN_ONE + THEN_BUT_ONE);

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(tokens.Where(token => token.KeyTerm == grammar.Then).Count(), Is.EqualTo(1), "Then");
            Assert.That(tokens.Where(token => token.Terminal == grammar.Identifier).Count(), Is.EqualTo(6), "Identifier");
        }

        [Test]
        public void Should_be_able_to_parse_multiple_scenarios()
        {
            // Given
            var grammar = new GherkinGrammar();
            var parser = new Parser(grammar);

            // When
            var parseTree = parser.Parse(FEATURE_LINE + DESCRIPTION1 + DESCRIPTION2 + "\n" + 
                                         SCENARIO_ONE + GIVEN_ONE + WHEN_ONE + THEN_ONE +
                                         SCENARIO_TWO + GIVEN_ONE + WHEN_ONE + THEN_ONE);

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(tokens.Where(token => token.KeyTerm == grammar.Scenario).Count(), Is.EqualTo(2), "Scenario");
            Assert.That(tokens.Where(token => token.KeyTerm == grammar.Given).Count(), Is.EqualTo(2), "Given");
            Assert.That(tokens.Where(token => token.KeyTerm == grammar.When).Count(), Is.EqualTo(2), "When");
            Assert.That(tokens.Where(token => token.KeyTerm == grammar.Then).Count(), Is.EqualTo(2), "Then");
        }

        [Test]
        public void Should_be_able_to_parse_tags()
        {
            // Given
            var grammar = new GherkinGrammar();
            var parser = new Parser(grammar);

            // When
            var parseTree = parser.Parse("@tag1 @tag2\n"+FEATURE_LINE + DESCRIPTION1 + DESCRIPTION2 + "\n" +
                                         SCENARIO_ONE + GIVEN_ONE + WHEN_ONE + THEN_ONE);

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(tokens.Where(token => token.Terminal == grammar.Tag).Count(), Is.EqualTo(2), "Tag");
        }

        [Test]
        public void Should_be_able_to_parse_background()
        {
            // Given
            var grammar = new GherkinGrammar();
            var parser = new Parser(grammar);

            // When
            var parseTree = parser.Parse(FEATURE_LINE + DESCRIPTION1 + DESCRIPTION2 + "\n" +
                                         "Background:\n" + GIVEN_ONE + WHEN_ONE + THEN_ONE +
                                         SCENARIO_ONE + GIVEN_ONE + WHEN_ONE + THEN_ONE);

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(tokens.Where(token => token.Terminal == grammar.Background).Count(), Is.EqualTo(1), "Background");
            Assert.That(tokens.Where(token => token.Terminal == grammar.Given).Count(), Is.EqualTo(2), "2 givens");
            Assert.That(tokens.Where(token => token.Terminal == grammar.When).Count(), Is.EqualTo(2), "2 whens");
            Assert.That(tokens.Where(token => token.Terminal == grammar.Then).Count(), Is.EqualTo(2), "2 thens");
        }

        [Test]
        public void Should_be_able_to_parse_background_with_a_name()
        {
            // Given
            var grammar = new GherkinGrammar();
            var parser = new Parser(grammar);

            // When
            var parseTree = parser.Parse(FEATURE_LINE + DESCRIPTION1 + DESCRIPTION2 + "\n" +
                                         BACKGROUND + GIVEN_ONE + WHEN_ONE + THEN_ONE +
                                         SCENARIO_ONE + GIVEN_ONE + WHEN_ONE + THEN_ONE);

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(tokens.Where(token => token.Terminal == grammar.Background).Count(), Is.EqualTo(1), "Background");
            Assert.That(tokens.Where(token => token.Terminal == grammar.Given).Count(), Is.EqualTo(2), "2 givens");
            Assert.That(tokens.Where(token => token.Terminal == grammar.When).Count(), Is.EqualTo(2), "2 whens");
            Assert.That(tokens.Where(token => token.Terminal == grammar.Then).Count(), Is.EqualTo(2), "2 thens");
        }


    }
}
