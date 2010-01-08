require 'rubygems'
require 'cucumber'
require 'builder'

$KCODE = 'UTF8'

class LanguageFactoryGenerator

  def generate
    File.open("cuke4vs.resx", "w") do |io|
      xml = Builder::XmlMarkup.new(:target => io, :indent => 2)
      xml.instruct!
      xml.root do |root|
        Cucumber::LANGUAGES.each do |code, keywords|
          generate_language(root, code, keywords)
        end
      end
    end
  end

  def generate_language(root, code, keywords)
    write_data root, code, 'name', keywords
    write_data root, code, 'native', keywords
    write_data root, code, 'feature', keywords
    write_data root, code, 'background', keywords
    write_data root, code, 'scenario', keywords
    write_data root, code, 'scenario_outline', keywords
    write_data root, code, 'examples', keywords
    write_data root, code, 'given', keywords
    write_data root, code, 'when', keywords
    write_data root, code, 'then', keywords
    write_data root, code, 'and', keywords
    write_data root, code, 'but', keywords
  end

  def write_data(root, code, keyword, values)
    root.data :name =>"#{code}_#{keyword}", "xml:space" => "preserve" do |data|
      data.value values[keyword]
    end
  end

end

LanguageFactoryGenerator.new.generate
