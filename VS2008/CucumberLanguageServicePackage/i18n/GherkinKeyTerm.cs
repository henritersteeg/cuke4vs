using Irony.Parsing;

namespace CucumberLanguageServices.i18n
{
    public class GherkinKeyTerm : KeyTerm
    {
        public GherkinTerm Term { get; private set; }
        public GherkinKeyTerm(GherkinTerm term, string text) : base(text, term.ToString())
        {
            Term = term;
        }
    }
}
