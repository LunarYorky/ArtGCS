namespace ArtGCS.Parsers.Settings;

public interface IParserSettings
{
    public string ParserType { get; init; }
    public string Host { get; init; }
}