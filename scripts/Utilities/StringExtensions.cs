using System;
using Godot;

namespace Reactistry.scripts.Utilities;

public static class StringExtensions
{
    public static Color ColorHash(this string str)
    {
        var hash = str.GetHashCode();
        hash = Math.Abs(hash);
        var hue = (hash % 360) / 360f;
        return Color.FromHsv(hue, 0.7f, 0.9f);
    }
}
