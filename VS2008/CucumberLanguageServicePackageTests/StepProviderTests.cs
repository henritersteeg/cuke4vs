using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CucumberLanguageServices.Integration;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CucumberLanguageServicePackageTests
{
    [TestFixture]
    public class StepProviderTests
    {
        [Test]
        public void Should_find_matching_step_definitions()
        {
            // Given
            var stepProvider = new StepProviderToTest
                                   (
                                        new StepDefinition("^nothing$"),
                                        new StepDefinition("^match\\d$"),
                                        new StepDefinition("^nomatch\\d$")
                                   );

            // When
            var matches = stepProvider.FindMatchesFor("match1");

            // Then
            Assert.That(matches, Has.Length(1));
            Assert.That(matches[0].Value, Is.EqualTo("^match\\d$"));
        }

    }

    public class StepProviderToTest : StepProvider
    {
        public StepProviderToTest(params StepDefinition[] stepDefinitions)
        {
            _stepDefinitions.AddRange(stepDefinitions);
        }
    }
}
