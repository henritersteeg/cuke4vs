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

        [Test]
        public void Should_not_unescape_string_if_it_start_with_an_at_sign()
        {
            // Given
            const string string_with_at_sign = "@\"c:\\bla\"";

            // When
            var result = StepProvider.Unescape(string_with_at_sign);

            // Then
            Assert.That(result, Is.EqualTo("c:\\bla"));
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
