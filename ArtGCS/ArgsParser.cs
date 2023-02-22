namespace ArtGCS;

public class ArgsParser
{
    public string[] FileNames { get; }
    private readonly Option[] _options;

    private static readonly Dictionary<char, OptionType>
        OptionsKeys = new() { };

    private static readonly Dictionary<char, OptionType>
        OptionsNoIgnoreCaseKeys = new() { };

    private static readonly Dictionary<string, OptionType>
        OptionsWords = new()
        {
            { "--help", OptionType.Help },
            { "--version", OptionType.Version }
        };

    private static readonly Dictionary<string, OptionType>
        OptionsWithParams = new() { };

    private readonly List<string> _fileNamesList = new();
    private readonly List<Option> _optionsList = new();

    public ArgsParser(string[] args)
    {
        var i = 0;
        for (; i < args.Length; i++)
        {
            if (args[i].StartsWith('-'))
            {
                AddOption(args[i]);
            }
            else
            {
                break;
            }
        }

        for (; i < args.Length; i++)
        {
            _fileNamesList.Add(args[i]);
        }

        FileNames = _fileNamesList.ToArray();
        _options = _optionsList.ToArray();
    }

    public bool ContainOption(OptionType optionType)
    {
        return _options.Any(opt => opt.OptionType == optionType);
    }

    public bool ContainOption(OptionType optionType, out string? param)
    {
        foreach (var opt in _options)
        {
            if (opt.OptionType != optionType) continue;

            param = opt.Param;
            return true;
        }

        param = default;
        return false;
    }

    private void AddOption(string arg)
    {
        if (arg[1] == '-')
        {
            foreach (var option in OptionsWithParams)
            {
                if (!arg.StartsWith(option.Key)) continue;

                if (arg.Length > option.Key.Length)
                {
                    _optionsList.Add(new Option(option.Value, arg[option.Key.Length..]));
                    return;
                }
            }

            foreach (var option in OptionsWords)
            {
                if (IsCommand(option.Key, option.Value))
                {
                    _fileNamesList.Add(arg);
                    return;
                }
            }
        }
        else
        {
            foreach (var option in OptionsKeys)
            {
                ArgContains(option.Key, option.Value);
            }

            foreach (var option in OptionsNoIgnoreCaseKeys)
            {
                ArgContains(option.Key, option.Value, false);
            }
        }

        bool IsCommand(string command, OptionType option)
        {
            if (arg != command) return false;

            _optionsList.Add(new Option(option));
            return true;
        }

        void ArgContains(char commandKey, OptionType option, bool ignoreCase = true)
        {
            if (ignoreCase)
            {
                if (arg.Contains(commandKey, StringComparison.OrdinalIgnoreCase))
                {
                    _optionsList.Add(new Option(option));
                }
            }
            else
            {
                if (arg.Contains(commandKey, StringComparison.Ordinal))
                {
                    _optionsList.Add(new Option(option));
                }
            }
        }
    }
}