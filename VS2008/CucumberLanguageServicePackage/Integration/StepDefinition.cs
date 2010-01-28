using System.Text.RegularExpressions;
using EnvDTE;

namespace CucumberLanguageServices.Integration
{
    public class StepDefinition
    {
        class Replacement
        {
            public string Old;
            public string New;

            public Replacement By(string newValue)
            {
                New = newValue;
                return this;
            }
        }

        private static readonly Replacement[] REPLACEMENTS = new[]
                                                        {
                                                            Replace("(.*)").By(".."),
                                                            Replace("(.+)").By(".."),
                                                            Replace("(\\d+)").By("0"),
                                                            Replace("(\\d*)").By("0"),
                                                        };
        
        private static Replacement Replace(string oldValue)
        {
            return new Replacement {Old = oldValue};
        }

        public string Value { get; private set; }
        public string Name { get; private set; }
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
            Name = CreateNameFor(value);
        }

        private static string CreateNameFor(string value)
        {
            var result = value;

            if (result.StartsWith("^"))
                result = result.Substring(1);

            if (result.EndsWith("$"))
                result = result.Substring(0, result.Length - 1);

            foreach (var replacement in REPLACEMENTS)
            {
                result = result.Replace(replacement.Old, replacement.New);
            }
            return result;
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
