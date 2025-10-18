using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

using Color = UnityEngine.Color;
using DrawColor = System.Drawing.Color;

public class GradientChGenAlgo : MonoBehaviour
{
    public static Gradient GenerateChild(GameObject parentA, GameObject parentB, GameObject child)
    {
        float variationTollerance = 0.10f;
        int maxPermittedLoops = 20;

        PonyColorsStruct ParentAColStruct = parentA.GetComponentInChildren<PonyColorManager>().GetCurrentColors();
        PonyColorsStruct ParentBColStruct = parentB.GetComponentInChildren<PonyColorManager>().GetCurrentColors();

        Gradient combinedGradient = CreateParentalGradient(ParentAColStruct, ParentBColStruct);

        float[] randomNums = new float[7];
        int itterationCounter;

        // for each color in child (i = 3 and i = 4 are stripes)
        for (int i = 0; i <= 6; i++)
        {
            float randNum = 0;
            bool numFound = false;
            itterationCounter = 0;

            // generating a Col-Index that is not already chosen
            while (numFound == false && itterationCounter < maxPermittedLoops)
            {
                randNum = UnityEngine.Random.Range(0.0f, 1.0f); // generate num
                numFound = true;

                foreach (float num in randomNums) // compare to other nums
                {
                    if (num == 0) break;
                    if (randNum > num - variationTollerance && randNum < num + variationTollerance)
                    {
                        numFound = false;
                    }
                }

                itterationCounter++;
                if (itterationCounter == maxPermittedLoops) // finding a uneak color took too many tries
                {
                    Debug.Log("giving up on " + child.name); //prevent infinit loop
                    break;
                }
            }
            randomNums[i] = randNum;
        }

        // determin hair colors
        List<Color> allParentMainColor = new List<Color>();
        foreach (Color mainCol in ParentAColStruct.mainColorStripes)
        {
            if (allParentMainColor.Contains(mainCol)) continue;
            allParentMainColor.Add(mainCol);
        }
        foreach (Color mainCol in ParentBColStruct.mainColorStripes)
        {
            if (allParentMainColor.Contains(mainCol)) continue;
            allParentMainColor.Add(mainCol);
        }
        if (allParentMainColor.Count > 6)
        {
            allParentMainColor = allParentMainColor.Take(6).ToList();
        }
        float randomNum = UnityEngine.Random.Range(0.0f, 3.0f);
        ChildGeneratror.EHairColorDistributionType hairPatternType = randomNum < 1 ? 
            ChildGeneratror.EHairColorDistributionType.STRIPES : randomNum < 2 ? 
            ChildGeneratror.EHairColorDistributionType.SPLITS : 
            ChildGeneratror.EHairColorDistributionType.RANDOM;

        PonyColorsStruct newColors = new PonyColorsStruct(
            combinedGradient.Evaluate(randomNums[0]), // skin
            ChildGeneratror.MainStripeListGenerator(allParentMainColor.ToArray(), hairPatternType), // hair
            maximizeSaturation(combinedGradient.Evaluate(randomNums[2])) // eye
            );

        child.GetComponentInChildren<PonyColorManager>().SetNewColors(newColors);
        return combinedGradient;
    }

    private static Color maximizeSaturation(Color color)
    {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        return Color.HSVToRGB(h, s, 1);
    }

    private static Gradient CreateParentalGradient(PonyColorsStruct parentA, PonyColorsStruct parentB)
    {
        List<GradientColorKey> colorKeys = new List<GradientColorKey>();

        // Hair color at 0.20 and 0.80
        colorKeys.Add(new GradientColorKey(parentA.mainColorStripes[0], 0f));
        colorKeys.Add(new GradientColorKey(parentB.mainColorStripes[0], 1f));

        // Skin color at 0.40 and 0.60
        colorKeys.Add(new GradientColorKey(parentA.skinColor, 0.33f));
        colorKeys.Add(new GradientColorKey(parentB.skinColor, 0.66f));

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[0];

        Gradient output = new Gradient();
        output.SetKeys(colorKeys.ToArray(), alphaKeys);
        return output;
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
}
