using System.Linq;

namespace Shared.Extensions;

public static class StringExtensions
{
    public static bool IsUnicode(this string str)
    {
        const int MaxAnsiCode = 127;
        return str.Any(c => c > MaxAnsiCode);
    }
}