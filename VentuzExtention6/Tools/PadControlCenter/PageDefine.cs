using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class PageButton
{
    public string id = null;
    public string name = null;
    public string style = "normal";
    public string command = null;
    public int[] rect = null;
}

[Serializable]
public class PageText
{
    public string id = null;
    public string text = "";
    public string aligment = "left";
    public int[] color = { 255, 255, 255 };
    public int[] rect = null;
}

[Serializable]
public class PageImage
{
    public string id = null;
    public string image = null;
    public int[] color = { 255, 255, 255 };
    public int[] rect = null;
}

[Serializable]
public class PageSlider
{
    public string id = null;
    public string style = null;
    public string command = null;
    public int[] rect = null;
}

[Serializable]
public class PageList
{
    public string id = null;
    public string style = null;
    public string command = null;
    public int[] rect = null;
}

[Serializable]
public class PageLine
{
    public string style = "normal";
    public int[] begcolor = { 255, 255, 255 };
    public int[] endcolor = { 255, 255, 255 };
    public float begwidth = 0.3f;
    public float endwidth = 0.3f;
    public int[] points = { };
}

[Serializable]
public class SlideCommand
{
    public string left = null;
    public string right = null;
    public string top = null;
    public string bottom = null;
}

[Serializable]
public class PageDefine
{
    public string title = null;
    public string bg = null;
    public int xoffset = 0;
    public int yoffset = 0;
    public SlideCommand slide = null;
    public PageButton[] buttons = null;
    public PageText[] texts = null;
    public PageLine[] lines = null;
    public PageSlider[] slider = null;
    public PageList[] lists = null;
    public PageImage[] images = null;

    public void OffsetPatch()
    {
        if(xoffset == 0 && yoffset == 0)
        {
            return;
        }

        if(buttons != null)
        {
            foreach (var item in buttons)
            {
                item.rect[0] += xoffset;
                item.rect[1] += yoffset;
            }
        }

        if(texts != null)
        {
            foreach (var item in texts)
            {
                item.rect[0] += xoffset;
                item.rect[1] += yoffset;
            }
        }

        if(lines != null)
        {
            foreach (var item in lines)
            {
                for (int i = 0; i < item.points.Length; ++i)
                {
                    if (i % 2 == 0)
                    {
                        item.points[i] += xoffset;
                    }
                    else
                    {
                        item.points[i] += yoffset;
                    }
                }
            }
        }

        if(slider != null)
        {
            foreach (var item in slider)
            {
                item.rect[0] += xoffset;
                item.rect[1] += yoffset;
            }
        }

        if(lists != null)
        {
            foreach (var item in lists)
            {
                item.rect[0] += xoffset;
                item.rect[1] += yoffset;
            }
        }

        if(images != null)
        {
            foreach (var item in images)
            {
                item.rect[0] += xoffset;
                item.rect[1] += yoffset;
            }
        }

        xoffset = 0;
        yoffset = 0;
    }
}

