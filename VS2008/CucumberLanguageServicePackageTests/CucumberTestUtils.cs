using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CucumberLanguageServicePackageTests
{
    public abstract class CucumberTestUtils
    {

        protected static void AssertNoError(TokenList tokens)
        {
            var errorTokens = tokens.Where(token => token.Category == TokenCategory.Error).ToList();
            Assert.That(errorTokens, Is.Empty, tokens.ToLines());
        }

    }

    public static class TokenExtensions
    {
        public static string ToLines(this TokenList tokens)
        {
            var lines = string.Empty;
            if (tokens == null)
                return lines;
            foreach (var token in tokens)
            {
                lines += token + Environment.NewLine;
            }
            return lines;
        }
    }
}
