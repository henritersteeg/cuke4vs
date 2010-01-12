using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CucumberLanguageServices;
using Irony.Parsing;
using Microsoft.VisualStudio.Package;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using TokenType=Microsoft.VisualStudio.Package.TokenType;

namespace CucumberLanguageServicePackageTests
{
    [TestFixture]
    public class LineScannerTests
    {
        [Test]
        public void Should_return_feature_and_identifier_tokens_for_feature_line()
        {
            // Given
            var scanner = new LineScanner(new GherkinGrammar());
            scanner.SetSource("Feature: bla", 0);

            // When
            var tokens = ReadTokens(scanner);

            // Then
            Assert.That(tokens, Has.Length(2), "Feature and Identifier expected");
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Keyword), "Feature");
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Identifier), "Identifier");
            Assert.That(tokens[0].StartIndex, Is.EqualTo(0), "Feature");
            Assert.That(tokens[1].StartIndex, Is.EqualTo(9), "Identifier");
        }

        [Test]
        public void Should_parse_an_empty_line_correctly_after_parsing_a_feature_line()
        {
            // Given
            var scanner = new LineScanner(new GherkinGrammar());
            ReadTokens(scanner, "Feature: bla");

            // When
            var tokens = ReadTokens(scanner, "");

            // Then
            Assert.That(tokens, Has.Length(0), "No tokens expected");
        }

        [Test]
        public void Should_return_scenario_and_identifier_tokens_for_scenario_line()
        {
            // Given
            var scanner = new LineScanner(new GherkinGrammar());

            // When
            var tokens = ReadTokens(scanner, "  Scenario: bla");

            // Then
            Assert.That(tokens, Has.Length(2), "Scenario and Identifier expected");
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Keyword), "Scenario");
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Identifier), "Identifier");
        }


        [Test]
        public void Should_return_scenario_and_identifier_tokens_for_scenario_line_after_other_lines()
        {
            // Given
            var scanner = new LineScanner(new GherkinGrammar());
            ReadTokens(scanner, "Feature: Addition");
            ReadTokens(scanner, "Feature: Addition"); // Simulate Visual Studio processing:
            ReadTokens(scanner, "Feature: Addition"); //     first line 3 times...
            ReadTokens(scanner, "  In order to avoid silly mistakes");
            ReadTokens(scanner, "  As a math idiot");
            ReadTokens(scanner, "  I want to be told the sum of two numbers");
            ReadTokens(scanner, "");

            // When
            var tokens = ReadTokens(scanner, "  Scenario: Add two numbers");

            // Then
            Assert.That(tokens, Has.Length(2), "Scenario and Identifier expected");
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Keyword), "Scenario");
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Identifier), "Identifier");
        }
        private static TokenInfo[] ReadTokens(IScanner scanner, string source)
        {
            scanner.SetSource(source, 0);
            return ReadTokens(scanner);
        }
        private static TokenInfo[] ReadTokens(IScanner scanner)
        {
            var state = 0;
            var result = new List<TokenInfo>();
            while (true)
            {
                var tokenInfo = new TokenInfo();
                var moreTokens = scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state);
                if (!moreTokens)
                    break;
                result.Add(tokenInfo);
            }
            return result.ToArray();
        }
    }
}
