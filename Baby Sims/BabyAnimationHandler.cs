using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KobGamesSDKSlim;

public class BabyAnimationHandler : MonoBehaviour
{

    public enum eSitType
    {
        Default,
        Happy
    }

    [Header("References")]
    public Animator BabyAnimator;


    public void OnEnable() {
        GameManager.OnLevelLoaded += OnLevelLoaded;
    }

    public void OnDisable() {
        GameManager.OnLevelLoaded -= OnLevelLoaded;
    }


    public void OnLevelLoaded() {
        BabyAnimator.SetTrigger("Reset");
        Reset();

    }

    public void Reset() {
        BabyAnimator.SetBool("Idle", false);
        BabyAnimator.SetBool("Walk", false);
        BabyAnimator.SetBool("Lie", false);
        BabyAnimator.SetBool("Sit", false);
        BabyAnimator.SetBool("Cry", false);

        BabyAnimator.SetInteger("SitID", 0);
        //BabyAnimator.ResetTrigger("SitTrigger");
    }

    public void Idle() {
        Reset();
        BabyAnimator.SetBool("Idle", true);
    }

    public void Crawl() {
        Reset();
        BabyAnimator.SetBool("Walk", true);
    }

    public void Sleep() {
        Reset();
        BabyAnimator.SetBool("Lie", true);
        BabyAnimator.SetTrigger("Sleep");
    }

    public void CrawlToSit(eSitType type) {
        Reset();
        BabyAnimator.SetBool("Sit", true);
        //BabyAnimator.SetTrigger("SitTrigger");
        BabyAnimator.SetInteger("SitID", (int)type);
    }

    public void SitToCrawl() {
        Reset();
        Crawl();
    }

    public void Cry() {
        Reset();
        BabyAnimator.SetBool("Cry", true);
    }
}
