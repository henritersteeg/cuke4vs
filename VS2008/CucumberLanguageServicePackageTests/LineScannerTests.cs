using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CucumberLanguageServices;
using CucumberLanguageServices.Integration;
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
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Literal), "Identifier");
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

        [Test]
        public void Should_split_stepIdentifier_when_step_has_arguments()
        {
            // Given
            var stepProvider = new StepProviderToTest(new StepDefinition("My phone number is (\\d+) and my name is '(.*)'"));
            var scanner = new LineScanner(new GherkinGrammar()) { StepProvider = stepProvider};
            
            // When
            var tokens = ReadTokens(scanner, "Given My phone number is 555 and my name is 'Henri'");

            // Then
            Assert.That(tokens, Has.Length(6));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Keyword), "Given");
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Identifier), tokens[1].Token.ToString());
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.String), tokens[2].Token.ToString());
            Assert.That(tokens[3].Type, Is.EqualTo(TokenType.Identifier), tokens[3].Token.ToString());
            Assert.That(tokens[4].Type, Is.EqualTo(TokenType.String), tokens[4].Token.ToString());
            Assert.That(tokens[5].Type, Is.EqualTo(TokenType.Identifier), tokens[5].Token.ToString());
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
