using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SceneChanger : MonoBehaviour {

    private void Start() {
        FadeToScreen();
    }
    public RawImage rawImage;

	public void PrepareLoad(int value) {
        FadeToBlack();
        switch (value) {
            case 1:
                Invoke("LoadDrawScene",1);
                break;
            default:
                Invoke("LoadMainMenu", 1);
                break;
        }
        
    }

    private void LoadMainMenu() {
        SceneManager.LoadScene(0);
    }
    private void LoadDrawScene() {
        SceneManager.LoadScene(1);
    }

    // Fading / Loading screen to avoid abrupt scene transitions
    public void FadeToBlack() {
        rawImage.DOFade(1, 1);
    }

    public void FadeToScreen() {
        rawImage.DOFade(0, 1);
        
    }
}
