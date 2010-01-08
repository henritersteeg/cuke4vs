using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CucumberLanguageServices;
using CucumberLanguageServices.i18n;
using Irony.Parsing;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CucumberLanguageServicePackageTests
{
    [TestFixture]
    public class NaturalLanguageTests : CucumberTestUtils
    {
        [Test]
        public void Should_create_feature_term_for_dutch_language()
        {
            // Given
            const string functionaliteit = "Functionaliteit";
            var nl = new LanguageTokenizer {Tokens = functionaliteit};

            // When
            var keyTerms = nl.CreateKeyTerms();

            // Then
            Assert.That(keyTerms, Has.Length(1));
            Assert.That(keyTerms[0].Text, Is.EqualTo(functionaliteit));
        }

        [Test]
        public void Should_create_given_terms_for_dutch_language()
        {
            // Given
            const string given = "*|Gegeven|Stel";
            var nl = new LanguageTokenizer { Tokens = given };

            // When
            var keyTerms = nl.CreateKeyTerms();

            // Then
            Assert.That(keyTerms, Has.Length(3));
            Assert.That(keyTerms[0].Text, Is.EqualTo("*"));
            Assert.That(keyTerms[1].Text, Is.EqualTo("Gegeven"));
            Assert.That(keyTerms[2].Text, Is.EqualTo("Stel"));
        }

        [Test]
        public void Should_remove_duplicates_from_terms_for_dutch_language()
        {
            // Given
            const string steps = "*|Gegeven|Stel|*|Als|*|Dan|*|En|*|Maar";
            var nl = new LanguageTokenizer { Tokens = steps };

            // When
            var keyTerms = nl.CreateKeyTerms();

            // Then
            Assert.That(keyTerms, Has.Length(7));
            Assert.That(keyTerms[0].Text, Is.EqualTo("*"));
            Assert.That(keyTerms[1].Text, Is.EqualTo("Gegeven"));
            Assert.That(keyTerms[2].Text, Is.EqualTo("Stel"));
            Assert.That(keyTerms[3].Text, Is.EqualTo("Als"));
            Assert.That(keyTerms[4].Text, Is.EqualTo("Dan"));
            Assert.That(keyTerms[5].Text, Is.EqualTo("En"));
            Assert.That(keyTerms[6].Text, Is.EqualTo("Maar"));
        }

        [Test]
        public void Should_get_dutch_language_from_language_text()
        {
            // When
            var language = NaturalLanguageText.GetTextFor("nl");

            // Then
            Assert.That(language.Code, Is.EqualTo("nl"));
            Assert.That(language.Name, Is.EqualTo("Dutch"));
            Assert.That(language.Native, Is.EqualTo("Nederlands"));
            Assert.That(language.Feature, Is.EqualTo("Functionaliteit"));
            Assert.That(language.Background, Is.EqualTo("Achtergrond"));
            Assert.That(language.Scenario, Is.EqualTo("Scenario"));
            Assert.That(language.ScenarioOutline, Is.EqualTo("Abstract Scenario"));
            Assert.That(language.Examples, Is.EqualTo("Voorbeelden"));
            Assert.That(language.Given, Is.EqualTo("*|Gegeven|Stel"));
            Assert.That(language.When, Is.EqualTo("*|Als"));
            Assert.That(language.Then, Is.EqualTo("*|Dan"));
            Assert.That(language.And, Is.EqualTo("*|En"));
            Assert.That(language.But, Is.EqualTo("*|Maar"));
        }

        [Test]
        public void Should_get_dutch_language_from_language_factory()
        {
            // Given

            // When
            var language = NaturalLanguageFactory.GetLanguage("nl");

            // Then
            Assert.That(language.Code, Is.EqualTo("nl"));
        }

        [Test]
        public void Should_parse_dutch_feature_correctly()
        {
            // Given
            _grammar = new GherkinGrammar(NaturalLanguageFactory.GetLanguage("nl"));
            _parser = new Parser(_grammar);

            // When
            var tokens = _parser.Parse("Functionaliteit: bla\nScenario: bla\nStel x\nAls y\nDan z\n").Tokens;

            // Then
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(ScenarioCount(tokens), Is.EqualTo(1), "Scenario");
            Assert.That(StepCount(tokens), Is.EqualTo(3), "Given/When/Then");
        }

        [Test]
        public void Should_create_a_language_specific_grammer()
        {
            // Given
            const string sourceText = "# language: nl\nFunctionaliteit: bla\nScenario: bla\nStel x\nAls y\nDan z\n";

            // When
            var grammer = GherkinGrammar.CreateFor(sourceText);

            // Then
            Assert.That(grammer.Language.Code, Is.EqualTo("nl"));
        }

        [Test]
        public void Should_use_default_language_when_language_specified_does_not_exist()
        {
            // Given
            const string sourceText = "# language: gronings\nFunctionaliteit: bla\nScenario: bla\nStel x\nAls y\nDan z\n";

            // When
            var grammer = GherkinGrammar.CreateFor(sourceText);

            // Then
            Assert.That(grammer.Language.Code, Is.Null);
        }

    }
}
