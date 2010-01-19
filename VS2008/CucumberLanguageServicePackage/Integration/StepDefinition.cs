using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EnvDTE;

namespace CucumberLanguageServices.Integration
{
    public class StepDefinition
    {
        public string Value { get; private set; }
        public ProjectItem ProjectItem { get; set; }
        public int Offset { get; set; }

        private Regex _valueRegex;

        public StepDefinition(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("Step[{0}] in {1}, offset {2}", 
                                 Value, 
                                 (ProjectItem != null ? ProjectItem.Name : "<unknown>"),
                                 Offset);
        }

        public bool Matches(string stepIdentifier)
        {
            if (_valueRegex == null)
                _valueRegex = new Regex(Value);
            return _valueRegex.IsMatch(stepIdentifier);
        }
    }
}
