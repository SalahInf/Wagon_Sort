using MatrixAlgebra;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Tex2DExtension
{
    public static Texture2D Circle(this Texture2D tex, int x, int y, int r, Color color, Color backgroung,Color outlin)
    {
        float rSquared = r * r;

        for (int u = 0; u < tex.width; u++)
        {
            for (int v = 0; v < tex.height; v++)
            {
                int pix = (x - u) * (x - u) + (y - v) * (y - v);
                if (pix < rSquared) 
                    tex.SetPixel(u, v, color);
                else if(pix < rSquared + 2)
                    tex.SetPixel(u, v, outlin);
                else
                    tex.SetPixel(u, v, backgroung);
            }
        }

        return tex;
    }

    public static Texture2D Circle(this Texture2D tex, int x, int y, int r, Color color, Color backgroung)
    {
        float rSquared = r * r;

        for (int u = 0; u < tex.width; u++)
        {
            for (int v = 0; v < tex.height; v++)
            {
                int pix = (x - u) * (x - u) + (y - v) * (y - v);
                if (pix < rSquared)
                    tex.SetPixel(u, v, color);
                else
                    tex.SetPixel(u, v, backgroung);
            }
        }

        return tex;
    }

    public static Texture2D XRect(this Texture2D tex, int x, int y, int r, Color color, Color backgroung)
    {
        float rSquared = r * r;

        for (int u = 0; u < tex.width; u++)
        {
            for (int v = 0; v < tex.height; v++)
            {
                tex.SetPixel(u, v, backgroung);
            }
        }

        foreach (var item in GetPointsOnLine(x -r, y - r, x + r, y + r))
        {
            tex.SetPixel(item.Item1, item.Item2, color);
            tex.SetPixel(item.Item1 + 1, item.Item2, color);
        }

        foreach (var item in GetPointsOnLine(x - r, y + r, x + r, y - r))
        {
            tex.SetPixel(item.Item1, item.Item2, color);
            tex.SetPixel(item.Item1 + 1, item.Item2, color);
        }

        return tex;
    }

    public static Texture2D Rect(this Texture2D tex, int r, Color color, Color backgroung)
    {
        for (int u = 0; u < tex.width; u++)
        {
            for (int v = 0; v < tex.height; v++)
            {
                if(u < r || v < r || u >= tex.width - r || v >= tex.height - r)
                    tex.SetPixel(u, v, color);
                else
                    tex.SetPixel(u, v, backgroung);
            }
        }
        return tex;
    }


    public static Texture2D LoadPNG(string path)
    {
        Texture2D tex = null;
#if UNITY_EDITOR
        path = Application.dataPath + path;

        byte[] fileData;

        if (File.Exists(path))
        {
            fileData = File.ReadAllBytes(path);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
#endif
        return tex;
    }

    public static int GetPointsOnLine(int x0, int y0, int x1, int y1, ref Vector2Int[] tmpIndexs,int coutIndex = 20)
    {
        bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
        if (steep)
        {
            int t;
            t = x0; // swap x0 and y0
            x0 = y0;
            y0 = t;
            t = x1; // swap x1 and y1
            x1 = y1;
            y1 = t;
        }
        if (x0 > x1)
        {
            int t;
            t = x0; // swap x0 and x1
            x0 = x1;
            x1 = t;
            t = y0; // swap y0 and y1
            y0 = y1;
            y1 = t;
        }
        int dx = x1 - x0;
        int dy = Mathf.Abs(y1 - y0);
        int error = dx / 2;
        int ystep = (y0 < y1) ? 1 : -1;
        int y = y0;

        int count = 0;

        for (int x = x0; x <= x1 && x < coutIndex; x++)
        {
            tmpIndexs[count].x = steep ? y : x;
            tmpIndexs[count].y = steep ? x : y;
            error = error - dy;
            if (error < 0)
            {
                y += ystep;
                error += dx;
            }

            count++;
        }

        return count;
    }


    public static IEnumerable<(int, int)> GetPointsOnLine(int x0, int y0, int x1, int y1)
    {
        bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
        if (steep)
        {
            int t;
            t = x0; // swap x0 and y0
            x0 = y0;
            y0 = t;
            t = x1; // swap x1 and y1
            x1 = y1;
            y1 = t;
        }
        if (x0 > x1)
        {
            int t;
            t = x0; // swap x0 and x1
            x0 = x1;
            x1 = t;
            t = y0; // swap y0 and y1
            y0 = y1;
            y1 = t;
        }
        int dx = x1 - x0;
        int dy = Mathf.Abs(y1 - y0);
        int error = dx / 2;
        int ystep = (y0 < y1) ? 1 : -1;
        int y = y0;
        for (int x = x0; x <= x1; x++)
        {
            yield return ((steep ? y : x), (steep ? x : y));
            error = error - dy;
            if (error < 0)
            {
                y += ystep;
                error += dx;
            }
        }
        yield break;
    }
    //  public static IEnumerable<(int, int)> GetPointsOnExactLine(int x1, int y1, int x2, int y2)
    //  {
    //      int dx = x2 - x1;
    //      int dy = y2 - y1;

    //      if (dx == 0)
    //      {
    //          int step = (int)Mathf.Sign(dy);

    //          for (int y = 0; y < dy; y+= step)
    //          {
    //              yield return (x1, y);
    //          }
    //      }

    //      float m = dy / (dx + 0.0f);
    //      float b = y1 - m * x1;

    //      float step = 1.0f / (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)));



    //steps = [x * step for x in range(int(x1 / step), int(x2 / step + copysign(1, x2)), int(copysign(1, dx)))]
    //for x in steps:
    //  y = m * x + b
    //  pt = (int(round(x)), int(round(y)))
    //  points["%d,%x" % pt] = pt
    //  }
}
