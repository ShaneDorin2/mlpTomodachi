using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;

//using Color = System.Drawing.Color; // to prevent conflic between UnityEngin.Color and System.Drawing.Color

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//[ExecuteInEditMode] // Ensures the script runs in Edit Mode
public class PonyColorManager : MonoBehaviour
{
    public Color[] allColors {  get; private set; }
    
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

    [Header("for fun")]
    [SerializeField] Gradient gradient;

    Color? HairStripe1;
    Color? HairStripe2;

    ColorChanger colorChanger;

    public void Start()
    {
        if (HairStripes.Length == 0) HairStripes = Enumerable.Repeat(Color.cyan, 6).ToArray();
        
        colorChanger = GetComponent<ColorChanger>();
        allColors = new Color[7];
    }

    public void Update()
    {
        // Skin
        if (colorChanger == null) Start();

        colorChanger.SetSkinColor(SkinBase);
        if (LockLineColorToBaseColorS) colorChanger.SetSkinLineColor(AutoGenerateLineColor(SkinBase)); 
        else colorChanger.SetSkinLineColor(SkinLineart);

        colorChanger.SetBackLegsColor(GenerateBackLegsColor());

        // Eyes
        colorChanger.SetEyeColor(EyeColor);

        // Hair
        colorChanger.SetHairStripesColor(HairStripes);
        if (LockLineColorToBaseColorH) { colorChanger.SetHairLineColor(AutoGenerateLineColor(HairStripes[0])); }
        else colorChanger.SetHairLineColor(HairLineart);
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
        return new PonyColorsStruct(SkinBase, HairStripes, EyeColor, HairStripe1, HairStripe2);
    }

    private Color GenerateBackLegsColor()
    {
        float severity = 0.1f;

        float r = SkinBase.r; 
        float g = SkinBase.g;
        float b = SkinBase.b;

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
