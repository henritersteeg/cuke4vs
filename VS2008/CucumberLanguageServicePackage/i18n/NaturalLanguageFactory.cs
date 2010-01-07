using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yaml;

namespace CucumberLanguageServices.i18n
{
    public class NaturalLanguageFactory
    {
        public static readonly NaturalLanguage DEFAULT_LANGUAGE = new NaturalLanguage(new NaturalLanguageText
                                                                                          {
                                                                                              Name = "English",
                                                                                              Native = "English",
                                                                                              Feature = "Feature",
                                                                                              Background = "Background",
                                                                                              Scenario = "Scenario",
                                                                                              ScenarioOutline = "Scenario Outline",
                                                                                              Examples = "Examples",
                                                                                              Given = "*|Given",
                                                                                              When = "*|When",
                                                                                              Then = "*|Then",
                                                                                              And = "*|And",
                                                                                              But = "*|But"
                                                                                          });

        public NaturalLanguageFactory()
        {
        }

        public NaturalLanguage GetLanguage(string key)
        {

            return DEFAULT_LANGUAGE;
        }

    }
}
