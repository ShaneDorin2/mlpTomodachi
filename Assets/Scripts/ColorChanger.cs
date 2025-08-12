using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
//using Color = System.Drawing.Color; // to prevent conflic between UnityEngin.Color and System.Drawing.Color

public class ColorChanger : MonoBehaviour
{
    [SerializeField] SpriteRenderer HairLineart;
    [SerializeField] GameObject HairStripes;
    [SerializeField] SpriteRenderer EyeColor;
    [SerializeField] SpriteRenderer SkinLineart;
    [SerializeField] SpriteRenderer SkinBase;
    [SerializeField] SpriteRenderer BackLegsBase;

    void SetColor (SpriteRenderer ponyPart,  Color color)
    {
        ponyPart.color = color;
        //Debug.Log("setColorCalled");
    }

    public void SetHairStripesColor(Color[] color) 
    {
        SpriteRenderer[] HairStripesSR = HairStripes.GetComponentsInChildren<SpriteRenderer>();
        for (int i=0; i < HairStripesSR.Length; i++)
        {
            SetColor(HairStripesSR[i], color[i%6]);
        }
    }
    public void SetHairLineColor(Color color) { SetColor(HairLineart, color); }
    public void SetEyeColor(Color color) { SetColor(EyeColor, color); }
    public void SetSkinColor(Color color) { SetColor(SkinBase, color); }
    public void SetSkinLineColor(Color color) { SetColor(SkinLineart, color); }
    public void SetBackLegsColor(Color color) { SetColor(BackLegsBase, color); }
}
