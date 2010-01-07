using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CucumberLanguageServices;
using CucumberLanguageServices.i18n;
using Irony.Parsing;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Yaml;

namespace CucumberLanguageServicePackageTests
{
    [TestFixture]
    public class NaturalLanguageTests
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
            Assert.That(keyTerms[0].Name, Is.EqualTo(functionaliteit));
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
            Assert.That(keyTerms[0].Name, Is.EqualTo("*"));
            Assert.That(keyTerms[1].Name, Is.EqualTo("Gegeven"));
            Assert.That(keyTerms[2].Name, Is.EqualTo("Stel"));
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
            Assert.That(keyTerms[0].Name, Is.EqualTo("*"));
            Assert.That(keyTerms[1].Name, Is.EqualTo("Gegeven"));
            Assert.That(keyTerms[2].Name, Is.EqualTo("Stel"));
            Assert.That(keyTerms[3].Name, Is.EqualTo("Als"));
            Assert.That(keyTerms[4].Name, Is.EqualTo("Dan"));
            Assert.That(keyTerms[5].Name, Is.EqualTo("En"));
            Assert.That(keyTerms[6].Name, Is.EqualTo("Maar"));
        }

        [Test]
        public void Should_get_dutch_language_from_language_factory()
        {
            // Given
            var factory = new NaturalLanguageFactory();

            // When
            var language = factory.GetLanguage("nl");

            // Then
            Assert.That(language.Code, Is.EqualTo("nl"));
        }

        [Test]
        public void Should_read_yaml_file()
        {
            // Given
            var node = Node.FromFile("languages.yml");
            
            // When

            // Then
            Assert.That(true, Is.EqualTo(false));
        }
  
    }
}
