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
    [SerializeField] EHairColorDistributionType colorDistributionType;
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

        UpdateColorStructWithInspectorColors();

        // Skin
        colorChanger.SetSkinColor(_ponyColorStruct.skinColor);
        if (LockLineColorToBaseColorS) colorChanger.SetSkinLineColor(AutoGenerateLineColor(_ponyColorStruct.skinColor));
        else colorChanger.SetSkinLineColor(SkinLineart);

        colorChanger.SetBackLegsColor(GenerateBackLegsColor());

        // Eyes
        colorChanger.SetEyeColor(_ponyColorStruct.eyeColor);

        // Hair
        colorChanger.SetHairStripesColor(MainStripeListGenerator(_ponyColorStruct.mainColorStripes, colorDistributionType));
        if (LockLineColorToBaseColorH) { colorChanger.SetHairLineColor(AutoGenerateLineColor(_ponyColorStruct.mainColorStripes[0])); }
        else colorChanger.SetHairLineColor(HairLineart);
    }

    public void UpdateColorStructWithInspectorColors()
    {
        _ponyColorStruct = new PonyColorsStruct(SkinBase, HairStripes, EyeColor, Streaks);
    }

    public void UpdateColorStructWithInheritanceTicket(InheritanceTicket ticket, PonyColorsStruct parentA, PonyColorsStruct parentB)
    {
        _ponyColorStruct = InhertanceTicketReader(ticket, parentA, parentB);
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

    enum EHairColorDistributionType
    {
        STRIPES,
        SPLITS,
        RANDOM
    }

    // up-to 6 colors and 3 streaks
    Color[] MainStripeListGenerator(Color[] colors, EHairColorDistributionType hairType, Color[] streaks = null)
    {
        Color[] outputCol = new Color[6];

        for (int i = 0; i < outputCol.Length; i++)
        {
            switch (hairType)
            {
                case EHairColorDistributionType.STRIPES:
                    outputCol[i] = colors[i % colors.Length];
                    break;
                case EHairColorDistributionType.SPLITS:
                    outputCol[i] = colors[(int)(i / (6f / colors.Length))];
                    break;
                case EHairColorDistributionType.RANDOM:
                    outputCol[i] = colors[UnityEngine.Random.Range(0, colors.Length)];
                    break;
                default:
                    break;
            }
        }
        if (streaks == null) return outputCol;
        foreach (Color strek in streaks)
        {
            outputCol[UnityEngine.Random.Range(0, 6)] = strek;
        }

        return outputCol;
    }

    private PonyColorsStruct InhertanceTicketReader(InheritanceTicket ticket, PonyColorsStruct parentA, PonyColorsStruct parentB)
    {   
        PonyColorsStruct outputStruct = new PonyColorsStruct();

        PonyColorsStruct parentHolder = ticket.hair.parent == EParent.PARENT_A ? parentA : parentB;
        switch (ticket.hair.trait)
        {
            case ETrait.HAIR:
                outputStruct.mainColorStripes = parentHolder.mainColorStripes;
                break;
            case ETrait.COAT:
                outputStruct.mainColorStripes = new Color[] { parentHolder.skinColor };
                break;
            case ETrait.EYES:
                outputStruct.mainColorStripes = new Color[] { parentHolder.eyeColor };
                break;
            case ETrait.STREAK:
                outputStruct.mainColorStripes = parentHolder.streaks.Values.ToArray();
                break;
            default:
                break;
        }

        parentHolder = ticket.coat.parent == EParent.PARENT_A ? parentA : parentB;
        switch (ticket.coat.trait)
        {
            case ETrait.HAIR:
                outputStruct.skinColor = parentHolder.mainColorStripes[0];
                break;
            case ETrait.COAT:
                outputStruct.skinColor = parentHolder.skinColor;
                break;
            case ETrait.EYES:
                outputStruct.skinColor = parentHolder.eyeColor;
                break;
            case ETrait.STREAK:
                outputStruct.skinColor = parentHolder.streaks.Values.First();
                break;
            default:
                break;
        }

        parentHolder = ticket.eyes.parent == EParent.PARENT_A ? parentA : parentB;
        switch (ticket.eyes.trait)
        {
            case ETrait.HAIR:
                outputStruct.eyeColor = parentHolder.mainColorStripes[0];
                break;
            case ETrait.COAT:
                outputStruct.eyeColor = parentHolder.skinColor;
                break;
            case ETrait.EYES:
                outputStruct.eyeColor = parentHolder.eyeColor;
                break;
            case ETrait.STREAK:
                outputStruct.eyeColor = parentHolder.streaks.Values.First();
                break;
            default:
                break;
        }

        parentHolder = ticket.streak.parent == EParent.PARENT_A ? parentA : parentB;
        switch (ticket.streak.trait)
        {
            case ETrait.HAIR:
                Dictionary<int, Color> dict = new Dictionary<int, Color>();
                for (int i = 0; i < parentHolder.mainColorStripes.Count(); i++)
                {
                    dict.Add(i, parentHolder.mainColorStripes[i]);
                }
                outputStruct.streaks = dict;
                break;
            case ETrait.COAT:
                outputStruct.streaks = new Dictionary<int, Color>() { {1, parentHolder.skinColor } };
                break;
            case ETrait.EYES:
                outputStruct.streaks = new Dictionary<int, Color>() { { 1, parentHolder.eyeColor } };
                break;
            case ETrait.STREAK:
                outputStruct.streaks = parentHolder.streaks;
                break;
            default:
                break;
        }

        return outputStruct;
    }
}
