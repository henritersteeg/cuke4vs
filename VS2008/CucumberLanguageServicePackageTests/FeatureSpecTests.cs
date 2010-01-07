using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CucumberLanguageServices;
using Irony.Parsing;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CucumberLanguageServicePackageTests
{
    [TestFixture]
    public class FeatureSpecTests : CucumberTestUtils
    {

        #region Comments

        [Test]
        public void Should_parse_a_file_with_only_a_one_line_comment()
        {
            // When
            var parseTree = _parser.Parse("# My comment\nFeature: hi\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(CommentCount(tokens), Is.EqualTo(1), "Comment");
        }

        [Test]
        public void Should_parse_a_file_with_only_a_multiline_comment()
        {
            // When
            var parseTree = _parser.Parse("# Hello\n# World!\nFeature: hi\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(CommentCount(tokens), Is.EqualTo(2), "Comment");
        }

        [Test]
        public void Should_parse_a_file_with_no_comments()
        {
            // When
            var parseTree = _parser.Parse("Feature: hi\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
        }

        [Test]
        public void Should_parse_a_file_with_only_a_multiline_comment_with_newlines()
        {
            // When
            var parseTree = _parser.Parse("# Hello\n\n# World!\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(CommentCount(tokens), Is.EqualTo(2), "Comment");
        }

        [Test]
        public void Should_not_consume_comments_as_part_of_a_multiline_name()
        {
            // When
            var parseTree = _parser.Parse("Feature: hi\n Scenario: test\n\n#hello\n Scenario: another\n"); // TODO: should remove last '\n' !!!

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(ScenarioCount(tokens), Is.EqualTo(2), "Scenario");
            Assert.That(CommentCount(tokens), Is.EqualTo(1), "Comment");
        }

        #endregion

        #region Tags

        [Test]
        public void Should_parse_a_file_with_tags_on_a_feature()
        {
            // When
            var parseTree = _parser.Parse("# My comment\n@hello @world Feature: hi\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(TagCount(tokens), Is.EqualTo(2), "Tag");
            Assert.That(CommentCount(tokens), Is.EqualTo(1), "Comment");
        }

        [Test]
        public void Should_not_take_the_tags_as_part_of_a_multiline_name_feature_element()
        {
            // When
            var parseTree = _parser.Parse("Feature: hi\n Scenario: test\n\n@hello Scenario: another\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(TagCount(tokens), Is.EqualTo(1), "Tag");
            Assert.That(ScenarioCount(tokens), Is.EqualTo(2), "Scenario");
        }

        [Test]
        public void Should_parse_a_file_with_tags_on_a_scenario()
        {
            // When
            var parseTree = _parser.Parse("# FC\n@ft\nFeature: hi\n\n@st1 @st2\n\nScenario: First\nGiven Pepper\n\n@st3\n@st4  @ST5 @#^%&ST6**!\nScenario: Second\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(TagCount(tokens), Is.EqualTo(7), "Tag");
            Assert.That(ScenarioCount(tokens), Is.EqualTo(2), "Scenario");
        }

        #endregion

        #region Background

        [Test]
        public void Background_should_have_steps()
        {
            // When
            var parseTree = _parser.Parse("Feature: Hi\nBackground:\nGiven I am a step\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(BackgroundCount(tokens), Is.EqualTo(1), "Background");
            Assert.That(GivenCount(tokens), Is.EqualTo(1), "Given");
        }

        [Test]
        public void Background_should_allow_multiline_names()
        {
            // When
            var parseTree = _parser.Parse("Feature: Hi\nBackground: It is my ambition to say \nin ten sentences\nwhat others say \nin a whole book.\nGiven I am a step\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(BackgroundCount(tokens), Is.EqualTo(1), "Background");
            Assert.That(GivenCount(tokens), Is.EqualTo(1), "Given");
        }

        #endregion

        #region Scenarios

        [Test]
        public void Scenario_can_be_empty()
        {
            // When
            var parseTree = _parser.Parse("Feature: Hi\n\nScenario: Hello\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(ScenarioCount(tokens), Is.EqualTo(1), "Scenario");
        }

        [Test]
        public void Scenario_should_have_steps()
        {
            // When
            var parseTree = _parser.Parse("Feature: Hi\nScenario: Hello\nGiven I am a step\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(ScenarioCount(tokens), Is.EqualTo(1), "Scenario");
            Assert.That(GivenCount(tokens), Is.EqualTo(1), "Given");
        }

        [Test]
        public void Should_allow_whitespace_lines_after_the_scenario_line()
        {
            // When
            var parseTree = _parser.Parse("Feature: Foo\n\nScenario: bar\n\nGiven baz\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(ScenarioCount(tokens), Is.EqualTo(1), "Scenario");
            Assert.That(GivenCount(tokens), Is.EqualTo(1), "Given");
        }

        [Test]
        public void Scenario_should_have_steps_with_inline_table()
        {
            // When
            var parseTree = _parser.Parse("Feature: Hi\nScenario: Hello\nGiven I have a table\n|a|b|\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(ScenarioCount(tokens), Is.EqualTo(1), "Scenario");
            Assert.That(GivenCount(tokens), Is.EqualTo(1), "Given");
            Assert.That(ColumnNameCount(tokens), Is.EqualTo(2), "columnNames");
        }

        [Test]
        public void Scenario_should_have_steps_with_inline_py_string()
        {
            // When
            var parseTree = _parser.Parse("Feature: Hi\nScenario: Hello\nGiven I have a string\n\n\"\"\"hello\nworld\"\"\"\n\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(ScenarioCount(tokens), Is.EqualTo(1), "Scenario");
            Assert.That(GivenCount(tokens), Is.EqualTo(1), "Given");
            Assert.That(PyStringCount(tokens), Is.EqualTo(1), "pyString");
        }

        [Test]
        public void Scenario_should_allow_multiline_names()
        {
            // When
            var parseTree = _parser.Parse("Feature: Hi\nScenario: It is my ambition to say \nin ten sentences\nwhat others say \nin a whole book.\nGiven I am a step\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(ScenarioCount(tokens), Is.EqualTo(1), "Scenario");
            Assert.That(GivenCount(tokens), Is.EqualTo(1), "Given");
        }

        [Test]
        public void Should_ignore_gherkin_keywords_which_are_parts_of_other_words_in_the_name()
        {
            // When
            var parseTree = _parser.Parse("Feature: Parser bug\nScenario: I have a Button\nButtons are great\nGiven I have it\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(ScenarioCount(tokens), Is.EqualTo(1), "Scenario");
            Assert.That(GivenCount(tokens), Is.EqualTo(1), "Given");
        }

        #endregion

        #region Scenario Outline

        [Test]
        public void ScenarioOutline_can_be_empty()
        {
            // When
            var parseTree = _parser.Parse("Feature: Hi\nScenario Outline: Hello\nGiven a <what> cucumber\nExamples:\n|what|\n|green|");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(ScenarioOutlineCount(tokens), Is.EqualTo(1), "ScenarioOutline");
            Assert.That(GivenCount(tokens), Is.EqualTo(1), "Given");
            Assert.That(ExamplesCount(tokens), Is.EqualTo(1), "Examples");
        }

        [Test]
        public void ScenarioOutline_should_have_line_numbered_steps_with_inline_table()
        {
            // When
            var parseTree = _parser.Parse("Feature: Hi\nScenario Outline: Hello\n\nGiven I have a table\n\n|<a>|<b>|\nExamples:\n|a|b|\n|c|d|");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(ScenarioOutlineCount(tokens), Is.EqualTo(1), "ScenarioOutline");
            Assert.That(GivenCount(tokens), Is.EqualTo(1), "Given");
            Assert.That(ExamplesCount(tokens), Is.EqualTo(1), "Examples");
            Assert.That(ColumnNameCount(tokens), Is.EqualTo(4), "Column names");
        }

        [Test]
        public void ScenarioOutline_should_have_examples()
        {
            // When
            var parseTree = _parser.Parse("Feature: Hi\nScenario Outline: Hello\n\nGiven I have a table\n\n|1|2|\nExamples:\n|x|y|\n|5|6|");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(ScenarioOutlineCount(tokens), Is.EqualTo(1), "ScenarioOutline");
            Assert.That(GivenCount(tokens), Is.EqualTo(1), "Given");
            Assert.That(ExamplesCount(tokens), Is.EqualTo(1), "Examples");
            Assert.That(ColumnNameCount(tokens), Is.EqualTo(4), "Column names");
        }

        [Test]
        public void ScenarioOutline_should_allow_multiline_names()
        {
            // When
            var parseTree = _parser.Parse("Feature: Hi\nScenario Outline: It is my ambition to say \nin ten sentences\nwhat others say \nin a whole book.\nGiven I am a step\n");

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(ScenarioOutlineCount(tokens), Is.EqualTo(1), "Scenario Outline");
            Assert.That(GivenCount(tokens), Is.EqualTo(1), "Given");
        }

        #endregion

        #region syntax

        [Test]
        public void Should_parse_empty_feature()
        {
            ParseFile("empty_feature.feature");
        }

        [Test]
        public void Should_parse_empty_scenario()
        {
            ParseFile("empty_scenario.feature");
        }

        [Test]
        public void Should_parse_empty_scenarioOutline()
        {
            ParseFile("empty_scenario_outline.feature");
        }

        [Test]
        public void Should_parse_fit_scenario()
        {
            ParseFile("multiline_steps.feature");
        }

        [Test]
        public void Should_parse_scenario_outline()
        {
            ParseFile("scenario_outline.feature");
        }

        [Test]
        public void Should_parse_comments()
        {
            ParseFile("with_comments.feature");
        }

        #endregion

        #region Tables

        [Test]
        public void Tables_should_have_newlines_to_separate_headers_from_cells()
        {
            // When
            var parseTree = _parser.Parse("Feature: Hi\nScenario: Hello\nGiven I have a table\n|a|b|\n|1|2|\n|3|4|\n");

            // Then
            var tokens = parseTree.Tokens;
            Console.WriteLine(tokens.ToLines());
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(ScenarioCount(tokens), Is.EqualTo(1), "Scenario");
            Assert.That(GivenCount(tokens), Is.EqualTo(1), "Given");
            Assert.That(ColumnNameCount(tokens), Is.EqualTo(2), "ColumnNames");
            Assert.That(TableCellCount(tokens), Is.EqualTo(4), "TableCells");
        }

        [Test]
        public void Tables_can_have_trailing_spaces()
        {
            // When
            var parseTree = _parser.Parse("Feature: Hi\nScenario: Hello\nGiven I have a table\n|a|b| \n|1|2|  \n|3|4|  \n");

            // Then
            var tokens = parseTree.Tokens;
            Console.WriteLine(tokens.ToLines());
            AssertNoError(tokens);
            Assert.That(FeatureCount(tokens), Is.EqualTo(1), "Feature");
            Assert.That(ScenarioCount(tokens), Is.EqualTo(1), "Scenario");
            Assert.That(GivenCount(tokens), Is.EqualTo(1), "Given");
            Assert.That(ColumnNameCount(tokens), Is.EqualTo(2), "ColumnNames");
            Assert.That(TableCellCount(tokens), Is.EqualTo(4), "TableCells");
        }

        #endregion

        #region Setup and private parts

        [SetUp]
        public void SetUp()
        {
            // Given
            _grammar = new GherkinGrammar();
            _parser = new Parser(_grammar);
        }

        private void ParseFile(string filename)
        {
            // Given
            var content = File.ReadAllText(filename);
            Console.WriteLine(content);

            // When
            var parseTree = _parser.Parse(content);

            // Then
            var tokens = parseTree.Tokens;
            AssertNoError(tokens);
        }
        #endregion
    }

}
