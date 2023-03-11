using System.Text;
using ArtGCS.Parsers.Settings;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ArtGCS.Parsers;

public static class ParserFactory
{
    private static readonly List<ParserSettings> ParserTypeFaSettingsList = new();

    static ParserFactory()
    {
        foreach (var fileName in Directory.GetFiles(Constants.ParsersConfigs))
        {
            try
            {
                var settings = JsonSerializer.Deserialize<ParserSettings>(File.ReadAllText(fileName));
                if (settings?.Settings != null)
                    ParserTypeFaSettingsList.Add(settings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        Console.WriteLine($"{ParserTypeFaSettingsList.Count} parsers settings loaded. For hosts: ");

        var sb = new StringBuilder();
        foreach (var settings in ParserTypeFaSettingsList)
            sb.Append(settings.Host + '\n');
        
        Console.WriteLine(sb.ToString());
        Console.WriteLine("___");
    }

    public static Parser Create(ParsHandler parsHandler, Uri uri)
    {
        var settings = ParserTypeFaSettingsList.Find(x => x.Host == uri.Host);
        if (settings == null)
        {
            throw new Exception(
                $"Not found parser settings for \"{uri}\". Put the configuration file in a folder {Constants.ParsersConfigs}");
        }

        return settings.ParserType switch
        {
            //Don't forget to update GetSupportedTypes() when adding types

            "Fa" => new ParserTypeFa(parsHandler, new ParserTypeFaSettings(uri.Host, settings.Settings)),
            _ => throw new Exception(
                $"Not found parser type. Check the spelling of the settings file" +
                $" or update the version of the program. SupportedTypes: {GetSupportedTypes()}.")
        };
    }

    private static string GetSupportedTypes()
    {
        return "Fa";
    }
}