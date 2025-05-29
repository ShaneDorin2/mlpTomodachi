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
    [SerializeField] Color HairBase;
    [SerializeField] Color SFHairStripe1;
    [SerializeField] Color SFHairStripe2;
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
        HairStripe1 = SFHairStripe1.a == 0 ? null : SFHairStripe1;
        HairStripe2 = SFHairStripe2.a == 0 ? null : SFHairStripe2;

        colorChanger = GetComponent<ColorChanger>();
        allColors = new Color[7];
    }

    public void Update()
    {
        //HairStripe1 = SFHairStripe1.a == 0 ? null : SFHairStripe1;
        //HairStripe2 = SFHairStripe2.a == 0 ? null : SFHairStripe2;

        //colorChanger = GetComponent<ColorChanger>();
        //allColors = new Color[7];

        //Start();

        // Skin
        if (colorChanger == null) Start();

        colorChanger.SetSkinColor(SkinBase);
        if (LockLineColorToBaseColorS) colorChanger.SetSkinLineColor(AutoGenerateLineColor(SkinBase)); 
        else colorChanger.SetSkinLineColor(SkinLineart);

        colorChanger.SetBackLegsColor(GenerateBackLegsColor());

        // Eyes
        colorChanger.SetEyeColor(EyeColor);

        // Hair
        colorChanger.SetHairColor(HairBase);
        if (LockLineColorToBaseColorH) { colorChanger.SetHairLineColor(AutoGenerateLineColor(HairBase)); }
        else colorChanger.SetHairLineColor(HairBase);

        if (HairStripe1 != null) colorChanger.SetHairStripe1Color((Color)HairStripe1);
        else colorChanger.SetHairStripe1Color(new Color(0, 0, 0, 0));

        if (HairStripe2 != null) colorChanger.SetHairStripe2Color((Color)HairStripe2);
        else colorChanger.SetHairStripe2Color(new Color(0, 0, 0, 0));
    }

    public void SetNewColors(PonyColorsStruct colors)
    {
        SkinBase = colors.skinColor;
        EyeColor = colors.eyeColor;
        HairBase = colors.mainColor;
        HairStripe1 = colors.hairStripeAColor;
        HairStripe2 = colors.hairStripeBColor;

        Update();
    }

    public PonyColorsStruct GetCurrentColors()
    {
        if (HairStripe1 == null)
        {
            HairStripe1 = SFHairStripe1.a == 0 ? null : SFHairStripe1;
            HairStripe2 = SFHairStripe2.a == 0 ? null : SFHairStripe2;
        }
        return new PonyColorsStruct(SkinBase, HairBase, EyeColor, HairStripe1, HairStripe2);
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
