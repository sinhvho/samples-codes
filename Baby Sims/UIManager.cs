using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KobGamesSDKSlim;

public class UIManager : Singleton<UIManager>
{
    // Start is called before the first frame update
    public Slider ProgressSlider;
    public Text ProgressText;

    public Text CurrentLevelText;
    public Text NextLevelText;
    public override void Awake() {
        base.Awake();

        
    }

    public void OnEnable() {
        GameManager.OnLevelLoaded += OnLevelLoaded;
        GameManager.OnLevelStarted += OnLevelStarted;
    }

    public void OnDisable() {
        GameManager.OnLevelLoaded -= OnLevelLoaded;
        GameManager.OnLevelStarted -= OnLevelStarted;
    }

    public void SetInitialProgress() {
        ProgressSlider.value = 0;
        ProgressSlider.minValue = 0;
        ProgressSlider.maxValue = BabyManager.Instance.DesireAmount;

        ProgressText.text = "0/" + BabyManager.Instance.DesireAmount;
    }

    public void SetLevel() {
        CurrentLevelText.text = (StorageManager.Instance.CurrentLevel + 1).ToString();
        NextLevelText.text = (StorageManager.Instance.CurrentLevel + 2).ToString();
    }
    public void UpdateProgress() {
        ProgressSlider.value++;
        ProgressText.text = ProgressSlider.value + "/"+ BabyManager.Instance.DesireAmount;
    }

    public void OnLevelLoaded() {
        SetLevel();
    }

    public void OnLevelStarted() {
        SetInitialProgress();
    }
}
