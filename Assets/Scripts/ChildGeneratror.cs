using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using Unity.Burst;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Drawing;

using Color = UnityEngine.Color;
using DrawColor = System.Drawing.Color;

enum GeneticsAlgorythm
{
    GRADIENT,
    // Parent colors are added to a gradient and points in the gradient are chosen randomly

    KIWI,
    /* From reddit
     * Inherit hues, sats and vals seperatly
     */

    DIRECT_PASSDOWN
    /* inherit unchanged coat and main colors. 
     * Eyes and hairsteaks are dealt with seperatly
     */
}

public class ChildGeneratror : MonoBehaviour
{
    [SerializeField] GeneticsAlgorythm _chosenAlorythm;
    
    [SerializeField] GameObject parentA;
    [SerializeField] GameObject parentB;
    [SerializeField] GameObject[] children;

    [Header("Gradient")]
    [SerializeField] Gradient combinedGradient;

    float[] randomNums;
    Color[] randomColors;
    Color[] parentAColors;
    Color[] parentBColors;

    bool hasStreakA;
    bool hasStreakB;

    float VariationTollerance = 0.10f;
    int itterationCounter;

    Color[] _mixedColorsDEBUG;
    Color[] _mixedValuesDEBUG;
    Color[] _mixedSatDEBUG;

    public void OnGeneratChildButtonClick()
    {
        switch (_chosenAlorythm)
        {
            case GeneticsAlgorythm.GRADIENT:
                GradientGenAlgo();
                break;

            case GeneticsAlgorythm.KIWI:
                KiwiGenAlgo();
                break;

            case GeneticsAlgorythm.DIRECT_PASSDOWN:
                break;

            default:
                break;
        }
    }

    public void Start()
    {
        randomNums = new float[7];
        randomColors = new Color[7];

        parentAColors = parentA.GetComponentInChildren<PonyColorManager>().allColors;
        parentBColors = parentB.GetComponentInChildren<PonyColorManager>().allColors;
    }

    #region Gradient Algorithm

    // Update is called once per frame
    public void GradientGenAlgo()
    {
        CreateParentalGradient(parentA.GetComponentInChildren<PonyColorManager>().GetCurrentColors(),
                               parentB.GetComponentInChildren<PonyColorManager>().GetCurrentColors());

        // for each child in scene:
        foreach (GameObject child in children) {
            
            randomNums = new float[7];

            // for each color in child (i = 3 and i = 4 are stripes)
            for (int i = 0; i < 7; i++)
            {                
                float randNum = 0;
                bool numFound = false;
                itterationCounter =0;

                // generating a Col-Index that is not already chosen
                while (numFound == false && itterationCounter < 20)
                {
                    itterationCounter++;
                    if (itterationCounter == 20)
                    {
                        Debug.Log("giving up on " + child.name); //prevent infinit loop
                        break;
                    }

                    randNum = UnityEngine.Random.Range(0.0f, 1.0f); // generate num
                    numFound = true;

                    foreach (float num in randomNums) // compare to other nums
                    {
                        if (num == 0) break;
                        if (randNum > num - VariationTollerance && randNum < num + VariationTollerance)
                        {
                            numFound = false;
                        }
                    }
                }
                randomNums[i] = randNum;
            }

            PonyColorsStruct newColors = new PonyColorsStruct(
                combinedGradient.Evaluate(randomNums[0]),
                Enumerable.Repeat(combinedGradient.Evaluate(randomNums[0]), 6).ToArray(),
                maximizeSaturation(combinedGradient.Evaluate(randomNums[2])),
                hasStreakA ? combinedGradient.Evaluate(randomNums[3]) : null,
                hasStreakB ? combinedGradient.Evaluate(randomNums[4]) : null
                );

            child.GetComponentInChildren<PonyColorManager>().SetNewColors(newColors);
        }
    }

    private Color maximizeSaturation(Color color)
    {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        return Color.HSVToRGB(h, s, 1);
    }

    private void CreateParentalGradient(PonyColorsStruct parentA, PonyColorsStruct parentB)
    {
        List<GradientColorKey> colorKeys = new List<GradientColorKey>();

        // Hair color at 0.20 and 0.80
        colorKeys.Add(new GradientColorKey(parentA.mainColorStripes[0], 0f));
        colorKeys.Add(new GradientColorKey(parentB.mainColorStripes[0], 1f));

        // Skin color at 0.40 and 0.60
        colorKeys.Add(new GradientColorKey(parentA.skinColor, 0.33f));
        colorKeys.Add(new GradientColorKey(parentB.skinColor, 0.66f));

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[0];

        combinedGradient.SetKeys(colorKeys.ToArray(), alphaKeys); 
    }

    private DrawColor UnityCol2DrawCol(Color color)
    {
        return DrawColor.FromArgb(
                   Mathf.RoundToInt(color.a * 255),  // Alpha
                   Mathf.RoundToInt(color.r * 255),  // Red
                   Mathf.RoundToInt(color.g * 255),  // Green
                   Mathf.RoundToInt(color.b * 255)   // Blue
               );
    }

    private Color DrawCol2UnityCol(DrawColor color)
    {
        return new Color(
        color.R / 255f,  // Red
        color.G / 255f,  // Green
        color.B / 255f,  // Blue
        color.A / 255f   // Alpha
        );
    }

    #endregion


    #region Kiwi Algorythm 
    // by Prestigious_Kiwi_303


    public void KiwiGenAlgo()
    {
        Color parentAEye = parentA.GetComponentInChildren<PonyColorManager>().GetCurrentColors().eyeColor;
        Color parentBEye = parentB.GetComponentInChildren<PonyColorManager>().GetCurrentColors().eyeColor;
        
        List<float> parentAHues = parentA.GetComponentInChildren<PonyColorManager>().GetCurrentColors().hues;
        List<float> parentBHues = parentB.GetComponentInChildren<PonyColorManager>().GetCurrentColors().hues;

        List<float> parentSaturations = parentA.GetComponentInChildren<PonyColorManager>().GetCurrentColors().saturations;
        parentSaturations.AddRange(parentB.GetComponentInChildren<PonyColorManager>().GetCurrentColors().saturations);

        List<float> parentValue = parentA.GetComponentInChildren<PonyColorManager>().GetCurrentColors().brightness;
        parentValue.AddRange(parentB.GetComponentInChildren<PonyColorManager>().GetCurrentColors().brightness);

        List<float> combinedHues = new List<float>();

        // create hue collection

        combinedHues.AddRange(parentAHues);
        combinedHues.AddRange(parentBHues);

        if (combinedHues.Count == 0) combinedHues.Add(0.0f);
        float[] combinedHuesArray = combinedHues.ToArray();

        _mixedColorsDEBUG = System.Array.ConvertAll(combinedHues.ToArray(), hue => Color.HSVToRGB(hue, 1f, 1f));
        _mixedValuesDEBUG = System.Array.ConvertAll(parentValue.ToArray(), val => Color.HSVToRGB(0f, 1f, val));
        _mixedSatDEBUG = System.Array.ConvertAll(parentSaturations.ToArray(), sat => Color.HSVToRGB(0f, sat, 1f));

        Color[] _mixedColValDEBUG = new Color[_mixedColorsDEBUG.Length];
        for (int i = 0; i < combinedHues.Count; i++)
        {
            _mixedColValDEBUG[i] = Color.HSVToRGB(combinedHues.ToArray()[i], parentSaturations.ToArray()[i], parentValue.ToArray()[i]);
        }

        // for each child in scene:
        foreach (GameObject child in children)
        {
            Color[] OutputColors = new Color[5];

            // for each color in child (i = 3 and i = 4 are stripes)

            List<int> hueIndexHistory = new List<int>();
            List<int> satValIndexHistory = new List<int>();
            int maxNumOfLoops = 50;
            for (int i = 0; i < 5; i++)
            {
                int randomNum = 0;
                int currentNumOfLoops = 0;

                do {
                    randomNum = UnityEngine.Random.Range(0, combinedHuesArray.Count());
                    currentNumOfLoops++;
                    if (currentNumOfLoops > maxNumOfLoops)
                    {
                        Debug.Log("doWhile broken HUE");
                        break;
                    }
                } while (hueIndexHistory.Contains(randomNum));
                hueIndexHistory.Add(randomNum);

                float h = combinedHuesArray[randomNum];
                float val = parentValue[randomNum];
                float sat = parentSaturations[randomNum];

                OutputColors[i] = Color.HSVToRGB(h, sat, val);
            }

            int randomNumEyeCol = UnityEngine.Random.Range(0, 2);

            PonyColorsStruct newColors = new PonyColorsStruct(
                OutputColors[0],
                Enumerable.Repeat(OutputColors[1], 6).ToArray(),
                //OutputColors[2],
                randomNumEyeCol == 0 ? parentAEye : parentBEye,
                OutputColors[3],
                null //OutputColors[4]
                );

            child.GetComponentInChildren<PonyColorManager>().SetNewColors(newColors);

        }
    }

    float MixHues(float hueA, float hueB, float mixAmount)
    {
        float delta = hueB - hueA;

        if (delta > 0.5f) hueB -= 1f; // Adjust for wraparound
        if (delta < -0.5f) hueB += 1f;

        float mixedHue = Mathf.Lerp(hueA, hueB, mixAmount) % 1f;
        if (mixedHue < 0) mixedHue += 1f; // Ensure positive hue

        return mixedHue;
    }

    #endregion


    #region Hair Stripe Manager

    enum EHairColorDistributionType
    {
        STRIPES,
        SPLITS,
        RANDOM
    }

    // up-to 6 colors and 3 streaks
    Color[] MainStripeListGenerator(Color[] colors, EHairColorDistributionType hairType, Color[] streaks)
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

        foreach (Color strek in streaks)
        {
            outputCol[UnityEngine.Random.Range(0, 6)] = strek;
        }

        return outputCol;
    }
    #endregion
}
