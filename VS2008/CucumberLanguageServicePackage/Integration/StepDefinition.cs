using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EnvDTE;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CucumberLanguageServices.Integration
{
    public class StepDefinition
    {
        public string Value { get; private set; }
        public ProjectItem ProjectItem { get; set; }
        public TextPoint StartPoint { get; set; }
        public TextPoint EndPoint { get; set; }
        public CodeFunction Function { get; set; }
        public string ClassName { get; set; }

        public bool IsPending
        {
            get
            {
                if (Function == null) return true;
                foreach (CodeAttribute attribute in Function.Attributes)
                {
                    if (attribute == null || attribute.FullName == null)
                        continue;
                    if (attribute.FullName.Equals(StepProvider.PENDING_ATTRIBUTE))
                        return true;
                }
                return false;
            }
        }

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
                                 StartPoint.AbsoluteCharOffset);
        }

        public bool Matches(string stepIdentifier)
        {
            if (_valueRegex == null)
                _valueRegex = new Regex(Value);
            return _valueRegex.IsMatch(stepIdentifier);
        }

        public Match Match(string stepIdentifier)
        {
            return _valueRegex.Match(stepIdentifier);
        }

    }
}
