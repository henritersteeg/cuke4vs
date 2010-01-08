using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using CucumberLanguageServices.i18n;
using Irony.Parsing;

namespace CucumberLanguageServices
{
    public class NaturalLanguageText
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Native { get; set; }

        public string Feature { get; set; }
        public string Background { get; set; }
        public string Scenario { get; set; }
        public string ScenarioOutline { get; set; }
        public string Examples { get; set; }
        public string Given { get; set; }
        public string When { get; set; }
        public string Then { get; set; }
        public string And { get; set; }
        public string But { get; set; }

        public string Step
        {
            get
            {
                return Given + "|" + When + "|" + Then + "|" + And + "|" + But;
            }
        }
        private static readonly ResourceManager RESOURCE = cuke4vs.ResourceManager;

        public static NaturalLanguageText GetTextFor(string key)
        {
            var name = RESOURCE.GetString(key + "_name");
            if (String.IsNullOrEmpty(name))
                return null;

            return new NaturalLanguageText
            {
                Code = key,
                Name = name,
                Native = RESOURCE.GetString(key + "_native"),
                Feature = RESOURCE.GetString(key + "_feature"),
                Background = RESOURCE.GetString(key + "_background"),
                Scenario = RESOURCE.GetString(key + "_scenario"),
                ScenarioOutline = RESOURCE.GetString(key + "_scenario_outline"),
                Examples = RESOURCE.GetString(key + "_examples"),
                Given = RESOURCE.GetString(key + "_given"),
                When = RESOURCE.GetString(key + "_when"),
                Then = RESOURCE.GetString(key + "_then"),
                And = RESOURCE.GetString(key + "_and"),
                But = RESOURCE.GetString(key + "_but")
            };
        }
    }
}
