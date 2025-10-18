using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class KiwiChGenAlgo : MonoBehaviour
{
    // by Prestigious_Kiwi_303
    public static void GenerateChild(GameObject parentA, GameObject parentB, GameObject child)
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

        Color[] OutputColors = new Color[5];

        // for each color in child (i = 3 and i = 4 are stripes)
        List<int> hueIndexHistory = new List<int>();
        List<int> satValIndexHistory = new List<int>();
        int maxNumOfLoops = 50;
        for (int i = 0; i < 5; i++)
        {
            int randomNum = 0;
            int currentNumOfLoops = 0;

            do
            {
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
            randomNumEyeCol == 0 ? parentAEye : parentBEye
            );

        child.GetComponentInChildren<PonyColorManager>().SetNewColors(newColors);

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
}
