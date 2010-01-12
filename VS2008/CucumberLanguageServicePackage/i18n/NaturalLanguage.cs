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

        private static BnfTerm CreateTerm(string name, string tokens)
        {
            return new LanguageTokenizer { Name = name, Tokens = tokens }.CreateIronyToken();
        }

        public override string ToString()
        {
            return Code;
        }
    }
}
