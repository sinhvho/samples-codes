using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KobGamesSDKSlim;

public class BabyHandler : Singleton<BabyHandler>
{
    public override void Awake() {
        base.Awake();
    }
    [Header("Clothes")]
    public Material[] pajamaMaterials;
    public Material[] DiaperMaterials;
    
    [Header("Hair")]
    public Material[] HairAMaterials;
    public Material[] HairBMaterials;
    public Material[] HairCMaterials;
    public Material[] HairDMaterials;
    

    [Header("Features")]
    public Material[] EyeMaterials;
    public Material[] SkinMaterials;

    private Animator BabyAnimator;
}
