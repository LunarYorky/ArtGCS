using ArtGCS.Parsers.Settings;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ArtGCS.Parsers;

public static class ParserFactory
{
    private static readonly List<ParserTypeFaSettings> ParserTypeFaSettingsList = new();

    static ParserFactory()
    {
        foreach (var fileName in Directory.GetFiles(Constants.ParsersConfigs))
        {
            try
            {
                ParserTypeFaSettingsList.Add(
                    JsonSerializer.Deserialize<ParserTypeFaSettings>(File.ReadAllText(fileName)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        Console.WriteLine($"Parsers settings count: {ParserTypeFaSettingsList.Count}");
        foreach (var settings in ParserTypeFaSettingsList)
        {
            Console.WriteLine(settings.Host.ToString());
        }
        Console.WriteLine("List end");
    }

    public static Parser Create(ParsHandler parsHandler, Uri uri)
    {
        Console.WriteLine(uri.Host.ToString());
        Console.WriteLine(ParserTypeFaSettingsList.First().Host.ToString());
        var qwe = ParserTypeFaSettingsList.Find((x => x.Host == uri.Host));
        Console.WriteLine(qwe.Host);
        return new ParserTypeFa(parsHandler, qwe); //TODO types switch
    }
}