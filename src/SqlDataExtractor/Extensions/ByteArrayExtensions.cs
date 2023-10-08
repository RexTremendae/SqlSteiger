using System.Text;

namespace SqlDX;

public static class ByteArrayExtensions
{
    public static string ToStringRepresentation(this byte[] bytes)
    {
        var builder = new StringBuilder("0x");

        foreach (var d in bytes)
        {
            var i = (int)d;
            builder.Append(HexRepresentation(i/16));
            builder.Append(HexRepresentation(i%16));
        }

        return builder.ToString();
    }

    private static string HexRepresentation(int d)
    {
        return d < 10
            ? d.ToString()
            : ((char)('A' + (d - 10))).ToString();
    }
}