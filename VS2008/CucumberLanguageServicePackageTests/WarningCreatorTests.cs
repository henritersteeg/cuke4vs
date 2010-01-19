using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CucumberLanguageServices;
using CucumberLanguageServices.Integration;
using Irony.Parsing;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CucumberLanguageServicePackageTests
{
    [TestFixture]
    public class WarningCreatorTests
    {
        private GherkinGrammar _grammar;
        private Parser _parser;
        private ParseTree _parseTree;

        [SetUp]
        public void SetUp()
        {
            // Given
            _grammar = new GherkinGrammar();
             _parser = new Parser(_grammar);
            _parseTree = _parser
                                .Parse("# language: en\n" +
                                       "# some other remark\n" +
                                       "Feature: bla bla\n" +
                                       "    as a ...\n" +
                                       "    i want to ...\n" +
                                       "    so that I ...\n" +
                                       " Scenario: first\n" +
                                       "    Given a\n" +
                                       "    When  b\n" +
                                       "    Then  c\n" +
                                       " Scenario: second\n" +
                                       "    Given x\n" +
                                       "    When  y\n" +
                                       "    Then  z\n");
        }

        [Test]
        public void Should_create_no_textspans_for_defined_stepIdentifiers()
        {
            // Given
            var stepProvider = new StepProviderToTest(new StepDefinition("^[abcxyz]$"));
            var warningCreator = new WarningCreator { Root = _parseTree.Root, StepProvider = stepProvider, Grammar = _grammar, Source = null };

            // When
            warningCreator.CreateSpansForUndefinedSteps();
            
            // Then
            Assert.That(warningCreator.Result, Has.Count(0));
        }

        [Test]
        public void Should_create_textspans_for_undefined_stepIdentifiers()
        {
            // Given
            var stepProvider = new StepProviderToTest(new StepDefinition("^[xyz]$"));
            var warningCreator = new WarningCreator { Root = _parseTree.Root, StepProvider = stepProvider, Grammar = _grammar, Source = null };

            // When
            warningCreator.CreateSpansForUndefinedSteps();

            // Then
            Assert.That(warningCreator.Result, Has.Count(3));
            Assert.That(warningCreator.Result[0].iStartLine, Is.EqualTo(7));
            Assert.That(warningCreator.Result[1].iStartLine, Is.EqualTo(8));
            Assert.That(warningCreator.Result[2].iStartLine, Is.EqualTo(9));
        }
    }
}
