using UnityEngine;

public static class Plot2D
{
    public static Texture2D PlotData(float[] x, float[] y, int textureWidth = 600, int textureHeight = 100)
    {
        if (x.Length != y.Length) return null;

        Texture2D plotTex = new Texture2D(textureWidth, textureHeight);

        float min_X = float.MaxValue, max_X = float.MinValue;
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] < min_X) min_X = x[i];
            if (x[i] > max_X) max_X = x[i];
        }

        float min_Y = float.MaxValue, max_Y = float.MinValue;
        for (int i = 0; i < y.Length; i++)
        {
            if (y[i] < min_Y) min_Y = y[i];
            if (y[i] > max_Y) max_Y = y[i];
        }

        int spacingX = 5, spacingY = 5;

        int px = 0, py = 0;

        // for (int x = 0; x < textureWidth; x++)
        // {
        //     for (int y = 0; y < textureHeight; y++)
        //     {
        //         if (x >= spacingX && x <= textureWidth - spacingX &&
        //             y >= spacingY && y <= textureHeight - spacingY)
        //         {
        //             px = x - spacingX;
        //             py = y - spacingY;

        //         }
        //     }
        // }

        return plotTex;
    }
}