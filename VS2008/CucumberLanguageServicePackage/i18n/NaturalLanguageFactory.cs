using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;

namespace CucumberLanguageServices.i18n
{
    public static class NaturalLanguageFactory
    {
        public static readonly NaturalLanguage DEFAULT_LANGUAGE = new NaturalLanguage(new NaturalLanguageText
                                                                                          {
                                                                                              Name = "English",
                                                                                              Native = "English",
                                                                                              Feature = "Feature",
                                                                                              Background = "Background",
                                                                                              Scenario = "Scenario",
                                                                                              ScenarioOutline = "Scenario Outline",
                                                                                              Examples = "Examples|Scenarios",
                                                                                              Given = "*|Given",
                                                                                              When = "*|When",
                                                                                              Then = "*|Then",
                                                                                              And = "*|And",
                                                                                              But = "*|But"
                                                                                          });

        public static NaturalLanguage GetLanguage(string key)
        {
            var languageText = NaturalLanguageText.GetTextFor(key);
            return languageText == null ? DEFAULT_LANGUAGE : new NaturalLanguage(languageText);
        }
    }
}
