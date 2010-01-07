using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
