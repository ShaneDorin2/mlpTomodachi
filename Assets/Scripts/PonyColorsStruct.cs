using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Color = UnityEngine.Color;
using DrawColor = System.Drawing.Color;

public struct PonyColorsStruct
{
    public Color skinColor;
    public Color[] mainColorStripes;
    public Color eyeColor;


    public List<float> hues;
    public List<float> saturations;
    public List<float> brightness;

    public PonyColorsStruct(Color skinColor, Color[] mainColorStripes, Color eyeColor, Color? hairStripeAColor = null, Color? hairStripeBColor = null) : this()
    {
        if (mainColorStripes.Length == 0) mainColorStripes = Enumerable.Repeat(Color.cyan, 6).ToArray();

        this.skinColor = skinColor;
        this.mainColorStripes = mainColorStripes;
        this.eyeColor = eyeColor;

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
