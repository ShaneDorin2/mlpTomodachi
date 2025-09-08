using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

public struct PonyColorsStruct
{
    public Color skinColor;
    public Color[] mainColorStripes;
    public Color eyeColor;
    public Dictionary<int, Color> streaks;

    public List<float> hues;
    public List<float> saturations;
    public List<float> brightness;


    public PonyColorsStruct(Color skinColor, Color[] mainColorStripes, Color eyeColor, Dictionary<int, Color> streaks = null) : this()
    {
        this.skinColor = skinColor;
        this.mainColorStripes = mainColorStripes;
        this.eyeColor = eyeColor;
        if (streaks != null)
        {
            foreach (int i in streaks.Keys)
            {
                if (i >= 0 && i <= 6)
                {
                    this.streaks.Add(i, streaks[i]);
                }
            }
        }


        hues = new List<float>();
        saturations = new List<float>();
        brightness = new List<float>();

        float h, s, v;
        Color.RGBToHSV(skinColor, out h, out s, out v);
        hues.Add(h);
        saturations.Add(s);
        brightness.Add(v);
        Color.RGBToHSV(mainColorStripes[0], out h, out s, out v);
        hues.Add(h);
        brightness.Add(v);
        saturations.Add(s);

    }
}
