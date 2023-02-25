using System.Text.Json.Serialization;

namespace ArtGCS.Serializable.Meta.Sub;

public struct IdSpace
{
    [JsonRequired] public int Start { get; set; }
    [JsonRequired] public int End { get; set; }

    public IdSpace(int start, int end)
    {
        if (start > end)
            throw new Exception("Not acceptable values for idSpace");

        Start = start;
        End = end;
    }

    public static IdSpace operator -(IdSpace a, int b)
    {
        return new IdSpace(a.Start + b, a.End);
    }
}