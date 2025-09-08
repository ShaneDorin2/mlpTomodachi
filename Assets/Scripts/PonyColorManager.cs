using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;
using UnityEditor;

//using Color = System.Drawing.Color; // to prevent conflic between UnityEngin.Color and System.Drawing.Color

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//[ExecuteInEditMode] // Ensures the script runs in Edit Mode
public class PonyColorManager : MonoBehaviour
{
    #region Inspector
    [Header("Hair Color")]
    [SerializeField] Color[] HairStripes; 
    [Tooltip("Tie Lineart To BaseColor (hair) ?")]
    [SerializeField] bool LockLineColorToBaseColorH = true;
    [SerializeField] Color HairLineart;

    [Header("Skin Color")]
    [SerializeField] Color SkinBase;
    [Tooltip("Tie Lineart To BaseColor (skin) ?")]
    [SerializeField] bool LockLineColorToBaseColorS = true;
    [SerializeField] Color SkinLineart;

    [Header("Eye Color")]
    [SerializeField] Color EyeColor;

    [Header("Hair Streaks")]
    [SerializeField, Tooltip("Index can only be from 0 to 5")] Dictionary<int, Color> Streaks;
    #endregion

    PonyColorsStruct _ponyColorStruct;
    ColorChanger colorChanger;

    public Color[] allColors { get; private set; } // get rid of this. 

    public void Start()
    {
        colorChanger = GetComponent<ColorChanger>();

        // get rid of this
        if (HairStripes.Length == 0) HairStripes = Enumerable.Repeat(Color.cyan, 6).ToArray();
        allColors = new Color[7];
    }

    public void Update()
    {
        if (colorChanger == null) Start();

        // Skin
        colorChanger.SetSkinColor(_ponyColorStruct.skinColor);
        if (LockLineColorToBaseColorS) colorChanger.SetSkinLineColor(AutoGenerateLineColor(_ponyColorStruct.skinColor)); 
        else colorChanger.SetSkinLineColor(SkinLineart);

        colorChanger.SetBackLegsColor(GenerateBackLegsColor());

        // Eyes
        colorChanger.SetEyeColor(_ponyColorStruct.eyeColor);

        // Hair
        colorChanger.SetHairStripesColor(_ponyColorStruct.mainColorStripes);
        if (LockLineColorToBaseColorH) { colorChanger.SetHairLineColor(AutoGenerateLineColor(_ponyColorStruct.mainColorStripes[0])); }
        else colorChanger.SetHairLineColor(HairLineart);
    }

    public void UpdateColorStructWithInspectorColors()
    {
        _ponyColorStruct = new PonyColorsStruct(SkinBase, HairStripes, EyeColor, Streaks);
    }

    public void SetNewColors(PonyColorsStruct colors)
    {
        SkinBase = colors.skinColor;
        EyeColor = colors.eyeColor;
        HairStripes = colors.mainColorStripes;
        Update();
    }

    public PonyColorsStruct GetCurrentColors()
    {
        return new PonyColorsStruct(SkinBase, HairStripes, EyeColor);
    }

    private Color GenerateBackLegsColor()
    {
        Color skin = _ponyColorStruct.skinColor;
        float severity = 0.1f;

        float r = skin.r; 
        float g = skin.g;
        float b = skin.b;

        return new Color(
            r -= severity, 
            g -= severity, 
            b -= severity, 
            1);
    }

    private Color AutoGenerateLineColor(Color baseColor)
    {
        float severity = 0.2f;

        float r = baseColor.r -= severity;
        float b = baseColor.b -= severity;
        float g = baseColor.g -= severity;

        Color newColor = new Color(r, g, b, 1);

        return newColor;
    }
}
