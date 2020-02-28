using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KobGamesSDKSlim;

public class Baby : MonoBehaviour
{

    private void Start() {
        Behaviour = GetComponent<BabyBehaviour>();
    }
    public enum eClotheType
    {
        Diaper,
        Pajamas
    }

    public enum eHairType
    {
        A,
        B,
        C,
        D,
        Bald // Always leave for last
    }

    public enum eSkinType
    {
        A,
        B,
        C
    }
    public BabyBehaviour Behaviour;
    public Animator BabyAnimator;

    [Header("References")]
    public Renderer[] Hairs;
    public Renderer Eyes;
    public Renderer Face;
    //public Renderer[] Clothes;
    public Renderer Pajamas;
    public Renderer Diaper;
    public Renderer BabySkin;



    public eClotheType ClotheType;
    public eHairType HairType;
    public eSkinType SkinType;

    private Material[] m_PajamasMaterials;
    private int bedTypeIndex;
    private BabyVariablesEditor m_BabyVars { get { return GameConfig.Instance.Baby; } }
    private GamePlayVariablesEditor m_GamePlayVars { get { return GameConfig.Instance.GamePlay; } }

    public void SetUpBaby() {
        HairType = (Baby.eHairType)Random.Range(0, System.Enum.GetValues(typeof(Baby.eHairType)).Length);

        foreach (Renderer i_Hair in Hairs) {
            i_Hair.gameObject.SetActive(false);
        }

        int i_HairIndex = (int)HairType;

        switch (HairType) {

            case Baby.eHairType.A:
                Hairs[i_HairIndex].material = BabyHandler.Instance.HairAMaterials[Random.Range(0, BabyHandler.Instance.HairAMaterials.Length)];
                break;
            case Baby.eHairType.B:
                Hairs[i_HairIndex].material = BabyHandler.Instance.HairBMaterials[Random.Range(0, BabyHandler.Instance.HairBMaterials.Length)];
                break;
            case Baby.eHairType.C:
                Hairs[i_HairIndex].material = BabyHandler.Instance.HairCMaterials[Random.Range(0, BabyHandler.Instance.HairCMaterials.Length)];
                break;
            case Baby.eHairType.D:
                Hairs[i_HairIndex].material = BabyHandler.Instance.HairDMaterials[Random.Range(0, BabyHandler.Instance.HairDMaterials.Length)];
                break;
        }
        if (HairType != Baby.eHairType.Bald)
            Hairs[i_HairIndex].gameObject.SetActive(true);


        int i_Rand = Random.Range(0, 100);

        if (i_Rand < m_BabyVars.RandomClotheVariationValue) {
            ClotheType = Baby.eClotheType.Diaper;
        }
        else {
            ClotheType = Baby.eClotheType.Pajamas;
        }

        if (ClotheType == Baby.eClotheType.Diaper) {

            Pajamas.gameObject.SetActive(false);
            Diaper.gameObject.SetActive(true);
            BabySkin.gameObject.SetActive(true);
            Diaper.material = BabyHandler.Instance.DiaperMaterials[Random.Range(0, BabyHandler.Instance.DiaperMaterials.Length)];
        }

        if (ClotheType == Baby.eClotheType.Pajamas) {
            BabySkin.gameObject.SetActive(false);
            Diaper.gameObject.SetActive(false);
            Pajamas.gameObject.SetActive(true);
            bedTypeIndex = Random.Range(0, BabyHandler.Instance.pajamaMaterials.Length);
            m_PajamasMaterials = Pajamas.materials;
            m_PajamasMaterials[1] = BabyHandler.Instance.pajamaMaterials[bedTypeIndex];
            Pajamas.materials = m_PajamasMaterials;
        }
        SkinType = (eSkinType)Random.Range(0, BabyHandler.Instance.SkinMaterials.Length);
        BabySkin.material = BabyHandler.Instance.SkinMaterials[(int)SkinType];
        Face.material = BabyHandler.Instance.SkinMaterials[(int)SkinType];

        Eyes.material = BabyHandler.Instance.EyeMaterials[Random.Range(0, BabyHandler.Instance.EyeMaterials.Length)];

        Behaviour.SetDesireList();

        if (m_GamePlayVars.MatchBedPajamas) {
            Behaviour.SetDesiredBedType(bedTypeIndex);
        } else {
            Behaviour.SetDesiredBedType(0);
        }
    }
}
