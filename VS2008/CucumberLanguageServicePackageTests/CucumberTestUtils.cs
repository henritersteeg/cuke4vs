using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CucumberLanguageServices;
using Irony.Parsing;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CucumberLanguageServicePackageTests
{
    public abstract class CucumberTestUtils
    {
        protected GherkinGrammar _grammar;
        protected Parser _parser;

        protected static void AssertNoError(TokenList tokens)
        {
            var errorTokens = tokens.Where(token => token.Category == TokenCategory.Error).ToList();
            Assert.That(errorTokens, Is.Empty, tokens.ToLines());
        }


        protected int FeatureCount(TokenList tokens)
        {
            return tokens.Where(token => token.Terminal.Name == "Feature").Count();
        }

        protected int BackgroundCount(TokenList tokens)
        {
            return tokens.Where(token => token.Terminal.Name == "Background").Count();
        }

        protected int ScenarioCount(TokenList tokens)
        {
            return tokens.Where(token => token.Terminal.Name == "Scenario").Count();
        }

        protected int ScenarioOutlineCount(TokenList tokens)
        {
            return tokens.Where(token => token.Terminal.Name == "Scenario Outline").Count();
        }

        protected int ExamplesCount(TokenList tokens)
        {
            return tokens.Where(token => token.Terminal.Name == "Examples").Count();
        }

        protected int StepCount(TokenList tokens)
        {
            return tokens.Where(token => token.Terminal.Name == "Step").Count();
        }

        protected int TagCount(TokenList tokens)
        {
            return tokens.Where(token => token.Terminal == _grammar.Tag).Count();
        }

        protected int ColumnNameCount(TokenList tokens)
        {
            return tokens.Where(token => token.Terminal == _grammar.ColumnName).Count();
        }

        protected int TableCellCount(TokenList tokens)
        {
            return tokens.Where(token => token.Terminal == _grammar.TableCell).Count();
        }

        protected int CommentCount(TokenList tokens)
        {
            return tokens.Where(token => token.Category == TokenCategory.Comment).Count();
        }

        protected int PyStringCount(TokenList tokens)
        {
            return tokens.Where(token => token.Terminal == _grammar.PyString).Count();
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
