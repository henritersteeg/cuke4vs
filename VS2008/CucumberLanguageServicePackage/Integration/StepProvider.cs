using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Package;
using Microsoft.Samples.LinqToCodeModel.Extensions;
using EnvDTE;

namespace CucumberLanguageServices.Integration
{
    public class StepProvider
    {
        public static readonly string[] SUPPORTED_ATTRIBUTES = {
                                                                   "Cuke4Nuke.Framework.GivenAttribute",
                                                                   "Cuke4Nuke.Framework.WhenAttribute",
                                                                   "Cuke4Nuke.Framework.ThenAttribute"
                                                               };

        private readonly List<StepDefinition> _stepDefinitions = new List<StepDefinition>();

        public void ProcessItem(ProjectItem item)
        {
            if (item == null || item.FileCodeModel == null)
                return;

            RemoveAttributesFor(item);

            foreach (var attribute in item.FileCodeModel.GetIEnumerable<CodeAttribute>())
            {
                if (!SUPPORTED_ATTRIBUTES.Contains(attribute.FullName))
                    continue;

                _stepDefinitions.Add(new StepDefinition
                                    {
                                        Value = Unescape(attribute.Value),
                                        ProjectItem = attribute.ProjectItem,
                                        Offset = attribute.StartPoint.AbsoluteCharOffset
                                    });

                Debug.Print("Attribute FullName={0}, Value={1}, Unescaped={4} at {2}:{3}",
                            attribute.FullName, attribute.Value, attribute.StartPoint.Line, attribute.StartPoint.DisplayColumn,
                            Unescape(attribute.Value));
            }
        }

        public IEnumerable<StepDefinition> StepDefinitions
        {
            get { return _stepDefinitions.OrderBy(step => step.Value); }
        }

        private void RemoveAttributesFor(ProjectItem projectItem)
        {
            _stepDefinitions.RemoveAll(step => step.ProjectItem == projectItem);
        }

        private static string Unescape(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 2)
                return value;

            if (value[0] != '"' || value[value.Length - 1] != '"')
                return value;

            return Regex.Unescape(value.Substring(1, value.Length - 2));
        }
    }
}
