using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;

using Color = UnityEngine.Color;
using DrawColor = System.Drawing.Color;

public struct PonyColorsStruct
{
    public Color skinColor;
    public Color mainColor;
    public Color eyeColor;

    public Color? hairStripeAColor;
    public Color? hairStripeBColor;

    public List<float> hues;
    public List<float> saturations;
    public List<float> brightness;

    public PonyColorsStruct(Color skinColor, Color mainColor, Color eyeColor, Color? hairStripeAColor = null, Color? hairStripeBColor = null) : this()
    {
        this.skinColor = skinColor;
        this.mainColor = mainColor;
        this.eyeColor = eyeColor;

        this.hairStripeAColor = hairStripeAColor;
        this.hairStripeBColor = hairStripeBColor;

        hues = new List<float>();
        saturations = new List<float>();
        brightness = new List<float>();

        float h, s, v;
        Color.RGBToHSV(skinColor, out h, out s, out v);
        hues.Add(h);  
        saturations.Add(s);
        brightness.Add(v);
        Color.RGBToHSV(mainColor, out h, out s, out v);
        hues.Add(h);
        brightness.Add(v);
        saturations.Add(s);
        //Color.RGBToHSV(eyeColor, out h, out s, out v);
        //hues.Add(h);
        //saturations.Add(s);
        //brightness.Add(v);
        //if (hairStripeAColor != null)
        //{
        //    Color.RGBToHSV((Color)hairStripeAColor, out h, out s, out v);
        //    if (s > 0 && v > 0) hues.Add(h);
        //    saturations.Add(s);
        //    brightness.Add(v);
        //}
        //if (hairStripeBColor != null)
        //{
        //    Color.RGBToHSV((Color)hairStripeBColor, out h, out s, out v);
        //    if (s > 0 && v > 0) hues.Add(h);
        //    saturations.Add(s);
        //    brightness.Add(v);
        //}
    }
}
