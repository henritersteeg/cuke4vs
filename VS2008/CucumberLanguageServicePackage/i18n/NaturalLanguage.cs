using Irony.Parsing;

namespace CucumberLanguageServices.i18n
{
    public class NaturalLanguage
    {
        public string Code { get; private set; }

        public BnfTerm Feature { get; private set; }
        public BnfTerm Background { get; private set; }
        public BnfTerm Scenario { get; private set; }
        public BnfTerm ScenarioOutline { get; private set; }
        public BnfTerm Examples { get; private set; }
        public BnfTerm Steps { get; private set; }

        public readonly KeyTermList KeyTerms = new KeyTermList();
        public readonly KeyTermList StepTerms = new KeyTermList();

        public NaturalLanguage(NaturalLanguageText languageText)
        {
            Code = languageText.Code;
            Feature = CreateTerm(GherkinTerm.Feature, languageText.Feature, ":");
            Background = CreateTerm(GherkinTerm.Background, languageText.Background, ":");
            Scenario = CreateTerm(GherkinTerm.Scenario, languageText.Scenario, ":");
            ScenarioOutline = CreateTerm(GherkinTerm.ScenarioOutline, languageText.ScenarioOutline, ":");
            Examples = CreateTerm(GherkinTerm.Examples, languageText.Examples, ":");
            Steps = CreateTerm(GherkinTerm.Step, languageText.Step);
        }

        private BnfTerm CreateTerm(GherkinTerm term, string tokens)
        {
            return CreateTerm(term, tokens, null);
        }

        private BnfTerm CreateTerm(GherkinTerm term, string tokens, string postFix)
        {
            var tokenizer = new LanguageTokenizer { Term = term, Tokens = tokens, PostFix = postFix };
            var token = tokenizer.CreateIronyToken();
            KeyTerms.AddRange(tokenizer.KeyTerms);
            if (term == GherkinTerm.Step)
                StepTerms.AddRange(tokenizer.KeyTerms);
            return token;
        }

        public override string ToString()
        {
            return Code;
        }
    }
}
