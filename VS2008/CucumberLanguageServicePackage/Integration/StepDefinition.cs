using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;

namespace CucumberLanguageServices.Integration
{
    public class StepDefinition
    {
        public string Value { get; set; }
        public ProjectItem ProjectItem { get; set; }
        public int Offset { get; set; }

        public override string ToString()
        {
            return string.Format("Step[{0}] in {1}, offset {2}", 
                                 Value, 
                                 (ProjectItem != null ? ProjectItem.Name : "<unknown>"),
                                 Offset);
        }
    }
}
