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

        public NaturalLanguage(NaturalLanguageText languageText)
        {
            Code = languageText.Code;
            Feature = CreateTerm("Feature", languageText.Feature + ":");
            Background = CreateTerm("Background", languageText.Background + ":");
            Scenario = CreateTerm("Scenario", languageText.Scenario + ":");
            ScenarioOutline = CreateTerm("Scenario Outline", languageText.ScenarioOutline + ":");
            Examples = CreateTerm("Examples", languageText.Examples + ":");
            Steps = CreateTerm("Step", languageText.Step);
        }

        private BnfTerm CreateTerm(string name, string tokens)
        {
            var tokenizer = new LanguageTokenizer { Name = name, Tokens = tokens };
            var token = tokenizer.CreateIronyToken();
            KeyTerms.AddRange(tokenizer.KeyTerms);
            return token;
        }

        public override string ToString()
        {
            return Code;
        }
    }
}
