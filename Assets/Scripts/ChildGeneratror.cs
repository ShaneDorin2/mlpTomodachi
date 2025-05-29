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

public class ChildGeneratror : MonoBehaviour
{
    [SerializeField] Gradient combinedGradient; // for fun

    [SerializeField] GameObject parentA;
    [SerializeField] GameObject parentB;
    [SerializeField] GameObject[] children;

    float[] randomNums;
    Color[] randomColors;
    Color[] parentAColors;
    Color[] parentBColors;

    bool hasStreakA;
    bool hasStreakB;

    float VariationTollerance = 0.10f;
    int itterationCounter;

    //[SerializeField] Color A;
    //[SerializeField] Color B;
    //[SerializeField] Color Output30;
    //[SerializeField] Color Output50;
    //[SerializeField] Color Output60;

    Color[] _mixedColorsDEBUG;
    Color[] _mixedValuesDEBUG;
    Color[] _mixedSatDEBUG;
    [SerializeField] Color[] _mixedColValDEBUG;

    public void Start()
    {
        randomNums = new float[7];
        randomColors = new Color[7];

        parentAColors = parentA.GetComponentInChildren<PonyColorManager>().allColors;
        parentBColors = parentB.GetComponentInChildren<PonyColorManager>().allColors;
    }

    #region Gradient Algorithm

    //public void GenerateChildFAKE()
    //{
    //    float h, s, v;
    //    UnityEngine.Color.RGBToHSV(A, out h, out s, out v);
    //    float hueA = h;
    //    UnityEngine.Color.RGBToHSV(B, out h, out s, out v);
    //    float hueB = h;

    //    Output30 = Color.HSVToRGB(MixHues(hueA, hueB, 0.25f), 1, 1);
    //    Output50 = Color.HSVToRGB(MixHues(hueA, hueB, 0.5f), 1, 1);
    //    Output60 = Color.HSVToRGB(MixHues(hueA, hueB, 0.75f), 1, 1);
    //}

    // Update is called once per frame
    public void GenerateChildBIS()
    {
        CreateParentalGradient(parentA.GetComponentInChildren<PonyColorManager>().GetCurrentColors(),
                               parentB.GetComponentInChildren<PonyColorManager>().GetCurrentColors());

        // for each child in scene:
        foreach (GameObject child in children) {

            DeterminIfChildHasHairSteaks(parentA.GetComponentInChildren<PonyColorManager>().GetCurrentColors(),
                                         parentB.GetComponentInChildren<PonyColorManager>().GetCurrentColors());
            
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
                combinedGradient.Evaluate(randomNums[1]),
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

    private void DeterminIfChildHasHairSteaks(PonyColorsStruct ponyColorsStruct1, PonyColorsStruct ponyColorsStruct2)
    {
        bool oneA = ponyColorsStruct1.hairStripeAColor != null;
        bool oneB = ponyColorsStruct1.hairStripeBColor != null;
        bool twoA = ponyColorsStruct2.hairStripeAColor != null;
        bool twoB = ponyColorsStruct2.hairStripeBColor != null;

        // streak A
        if (oneA && twoA)
        {
            hasStreakA = true;
        }
        else if (oneA || twoA)
        {
            hasStreakA = UnityEngine.Random.Range(0, 2) == 0 ? true : false;
        }
        else { hasStreakA = false; }

        // streak B
        if (oneB && twoB)
        {
            hasStreakB = true;
        }
        else if (oneB || twoB)
        {
            hasStreakB = UnityEngine.Random.Range(0, 2) == 0 ? false : true;
        }
        else { hasStreakB = false; }
    }

    private void CreateParentalGradient(PonyColorsStruct parentA, PonyColorsStruct parentB)
    {
        List<GradientColorKey> colorKeys = new List<GradientColorKey>();

        // Eye colors at either end
        colorKeys.Add(new GradientColorKey(parentA.eyeColor, 0.0f));
        colorKeys.Add(new GradientColorKey(parentB.eyeColor, 1.0f));

        // Hair color at 0.20 and 0.80
        colorKeys.Add(new GradientColorKey(parentA.mainColor, 0.20f));
        colorKeys.Add(new GradientColorKey(parentB.mainColor, 0.80f));

        // Skin color at 0.40 and 0.60
        colorKeys.Add(new GradientColorKey(parentA.skinColor, 0.40f));
        colorKeys.Add(new GradientColorKey(parentB.skinColor, 0.60f));

        // Add stripe Colors
        if (parentA.hairStripeAColor != null)
            colorKeys.Add(new GradientColorKey((Color)parentA.hairStripeAColor, 0.06f));        
        if (parentA.hairStripeBColor != null)
            colorKeys.Add(new GradientColorKey((Color)parentA.hairStripeBColor, 0.13f));

        if (parentB.hairStripeAColor != null && colorKeys.Count() < 8)
            colorKeys.Add(new GradientColorKey((Color)parentB.hairStripeAColor, 0.94f));
        if (parentB.hairStripeBColor != null && colorKeys.Count() < 8)
            colorKeys.Add(new GradientColorKey((Color)parentB.hairStripeBColor, 0.87f));

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


    public void GenerateChild()
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

        //int indexKeeper = 0;
        //foreach (float aHue in parentAHues)
        //{
        //    foreach (float bHue in parentBHues)
        //    {
        //        for (int i = 0; i < 2; i++)
        //        {
        //            if (math.abs(aHue - bHue) > 0.25) { continue; }
        //            float mixAmount = indexKeeper % 2 == 0 ? 0.5f : 0.95f;
        //            combinedHues.Add(MixHues(aHue, bHue, mixAmount));
        //            //Debug.Log(indexKeeper);
        //            indexKeeper++;
        //        }
        //    }
        //}

        if (combinedHues.Count == 0) combinedHues.Add(0.0f);
        float[] combinedHuesArray = combinedHues.ToArray();

        _mixedColorsDEBUG = System.Array.ConvertAll(combinedHues.ToArray(), hue => Color.HSVToRGB(hue, 1f, 1f));
        _mixedValuesDEBUG = System.Array.ConvertAll(parentValue.ToArray(), val => Color.HSVToRGB(0f, 1f, val));
        _mixedSatDEBUG = System.Array.ConvertAll(parentSaturations.ToArray(), sat => Color.HSVToRGB(0f, sat, 1f));

        //_mixedSatValDEBUG = new Color[_mixedValuesDEBUG.Length];
        //for (int i = 0; i < parentSaturations.Count; i++)
        //{
        //    _mixedSatValDEBUG[i] = Color.HSVToRGB(0f, parentSaturations.ToArray()[i], parentValue.ToArray()[i]);
        //}

        _mixedColValDEBUG = new Color[_mixedColorsDEBUG.Length];
        for (int i = 0; i < combinedHues.Count; i++)
        {
            _mixedColValDEBUG[i] = Color.HSVToRGB(combinedHues.ToArray()[i], parentSaturations.ToArray()[i], parentValue.ToArray()[i]);
        }

        // for each child in scene:
        foreach (GameObject child in children)
        {

            DeterminIfChildHasHairSteaks(parentA.GetComponentInChildren<PonyColorManager>().GetCurrentColors(),
                                         parentB.GetComponentInChildren<PonyColorManager>().GetCurrentColors());

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


                //currentNumOfLoops = 0;
                //do {
                //    randomNum = UnityEngine.Random.Range(0, parentSaturations.Count);
                //    currentNumOfLoops++;
                //    if (currentNumOfLoops > maxNumOfLoops)
                //    {
                //        Debug.Log("doWhile broken SATVAL");
                //        break;
                //    }
                //} while (satValIndexHistory.Contains(randomNum));
                //satValIndexHistory.Add(randomNum);

                
                

                OutputColors[i] = Color.HSVToRGB(h, sat, val);
            }

            int randomNumEyeCol = UnityEngine.Random.Range(0, 2);

            PonyColorsStruct newColors = new PonyColorsStruct(
                OutputColors[0],
                OutputColors[1],
                //OutputColors[2],
                randomNumEyeCol == 0 ? parentAEye : parentBEye,
                OutputColors[3],
                null //OutputColors[4]
                //hasStreakA ? OutputColors[3] : null,
                //hasStreakB ? OutputColors[4] : null
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

        //float mixedHue = Mathf.Lerp(hueA, hueB, mixAmount);

        //return mixedHue;
    }

    #endregion
}
