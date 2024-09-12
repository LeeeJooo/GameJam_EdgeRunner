using UnityEngine;

public class Colors
{
    private static Color redColor;

    public static Color RedColor
    {
        get
        {
            if (redColor == Color.clear) ColorUtility.TryParseHtmlString("#FF4900", out redColor);
            return redColor;
        }
    }

}