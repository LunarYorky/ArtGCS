namespace ArtGCS;

public class Option
{
    public Option(OptionType optionType, string? param)
    {
        OptionType = optionType;
        Param = param;
    }

    public Option(OptionType optionType)
    {
        OptionType = optionType;
    }

    public OptionType OptionType { get; }
    public string? Param { get; }
}