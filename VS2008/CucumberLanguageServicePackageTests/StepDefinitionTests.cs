using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CucumberLanguageServices.Integration;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CucumberLanguageServicePackageTests
{
    [TestFixture]
    public class StepDefinitionTests
    {
        [Test]
        public void Should_remove_StartOfLine_and_EndOfLine_from_replacement_text()
        {
            // Given
            var stepDefinition = new StepDefinition("^bla$");

            // When
            var replacementText = stepDefinition.Name;

            // Then
            Assert.That(replacementText, Is.EqualTo("bla"));
        }

        [Test]
        public void Should_remove_variable_definitions_with_a_sensible_replacement()
        {
            // Given
            var stepDefinition = new StepDefinition("My name is '(.*)'");

            // When
            var replacementText = stepDefinition.Name;

            // Then
            Assert.That(replacementText, Is.EqualTo("My name is '..'"));
        }
  
    }
}
