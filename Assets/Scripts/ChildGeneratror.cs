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

    public void OnGeneratChildButtonClick()
    {
        switch (_chosenAlorythm)
        {
            case GeneticsAlgorythm.GRADIENT:
                foreach (GameObject child in children)
                {
                    combinedGradient = GradientChGenAlgo.GenerateChild(parentA, parentB, child);
                }
                break;

            case GeneticsAlgorythm.KIWI:
                foreach (GameObject child in children)
                {
                    KiwiChGenAlgo.GenerateChild(parentA, parentB, child);
                }
                break;

            case GeneticsAlgorythm.DIRECT_PASSDOWN:
                break;

            default:
                break;
        }
    }


    #region Hair Stripe Manager

    public enum EHairColorDistributionType
    {
        STRIPES,
        SPLITS,
        RANDOM
    }

    // up-to 6 colors and 3 streaks
    public static Color[] MainStripeListGenerator(Color[] colors, EHairColorDistributionType hairType, Color[] streaks = null)
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
    #endregion
}
