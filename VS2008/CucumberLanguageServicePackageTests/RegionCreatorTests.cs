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
    public class RegionCreatorTests
    {
        [Test]
        public void Should_create_a_region_for_the_feature_and_description()
        {
            // Given

            var grammar = new GherkinGrammar();
            var parser = new Parser(grammar);
            var parseTree = parser
                                .Parse("# language: en\n"+
                                       "# some other remark\n"+
                                       "Feature: bla bla\n"+
                                       "    as a ...\n"+
                                       "    i want to ...\n"+
                                       "    so that I ...\n"+
                                       " Scenario: first\n"+
                                       "    Given a\n"+
                                       "    When  b\n"+
                                       "    Then  c\n"+
                                       " Scenario: second\n"+
                                       "    Given x\n"+
                                       "    When  y\n"+
                                       "    Then  z\n");
            var regionCreator = new RegionCreator { Root = parseTree.Root, Source = null};

            // When
            regionCreator.CreateRegionsFor(grammar.Description);

            // Then
            Assert.That(regionCreator.Result, Has.Count(1));
            Assert.That(regionCreator.Result[0].iStartLine, Is.EqualTo(3), "Line below 'Feature' (zero based)");
        }

        [Test]
        public void Should_create_a_region_for_the_scenarios()
        {
            // Given

            var grammar = new GherkinGrammar();
            var parser = new Parser(grammar);
            var parseTree = parser
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
            var regionCreator = new RegionCreator { Root = parseTree.Root, Source = null };

            // When
            regionCreator.CreateRegionsFor(grammar.GivenWhenThenClause);

            // Then
            Assert.That(regionCreator.Result, Has.Count(2));
            Assert.That(regionCreator.Result[0].iStartLine, Is.EqualTo(7), "Line below first 'Scenario' (zero based)");
            Assert.That(regionCreator.Result[1].iStartLine, Is.EqualTo(11), "Line below second 'Scenario' (zero based)");
        }

    }
}
