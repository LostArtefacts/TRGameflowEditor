using System.Text;

namespace TRGE.Core;

public static class StringExtensions
{
    public static string ToLowerSnake(this string str)
    {
        return Convert(str, '_');
    }

    public static string ToLowerKebab(this string str)
    {
        return Convert(str, '-');
    }

    private static string Convert(string str, char separator)
    {
        StringBuilder sb = new();
        bool prev = false;
        for (int i = 0; i < str.Length; i++)
        {
            if (!prev)
            {
                if (char.IsNumber(str[i]) && i < str.Length - 1 && char.IsLower(str[i + 1]))
                {
                    sb.Append(separator);
                }
                else if (i > 0 && char.IsUpper(str[i]))
                {
                    sb.Append(separator);
                }
            }
            sb.Append(char.ToLower(str[i]));
            prev = char.IsUpper(str[i]);
        }
        return sb.ToString();
    }
}