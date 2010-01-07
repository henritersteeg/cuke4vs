require 'cucumber'

class LanguageFactoryGenerator

  def generate
    Cucumber::LANGUAGES.each do |code, keywords|
       generate_language(code, keywords)
    end
  end

  def generate_language(code, keywords)
    p code, keywords
  end

end

LanguageFactoryGenerator.new.generate
