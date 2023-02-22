using ArtGCS.Enums;
using ArtGCS.Serializable;

namespace ArtGCS.Parsers;

public static class ParsManager
{
    private static readonly Dictionary<Resource, BaseParser> Parsers = new()
    {
        { Resource.Furaffinity, new FaParser() }
    };

    public static Profile GetProfile(string userName, Resource resource)
    {
        return Parsers[resource].ParsProfile(userName);
    }
}