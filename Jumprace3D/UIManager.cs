using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance { get { return instance; } set { instance = value; } }

    public AccuracyUI accuracyText;

    public Color perfectColor, goodColor, longJumpColor;
    // Start is called before the first frame update

    public Slider progressSlider;

    // Completion UI
    public RectTransform completionUI;
    public RectTransform tapRT;

    public RectTransform[] leaderboard;

    private void Awake() {
        if (instance == null)
            instance = this;
    }
  

    public void CallCompletedLevel() {
        StartCoroutine("ShowCompletion");
    }

    IEnumerator ShowCompletion() {
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine("ShowCompletionUI");
        yield return StartCoroutine("ShowUsers");
    }

    IEnumerator ShowCompletionUI() {
        float time = 0, delay = 1;
        while(time < delay) {
            time += Time.deltaTime * 3;
            completionUI.localScale = new Vector3(time, time, time);
            yield return null;
        }

        yield return null;

    }

    IEnumerator ShowUsers() {
        
        foreach(RectTransform rt in leaderboard) {
            float time = 0, delay = .3f;
            rt.gameObject.SetActive(true);
            while (time < delay) {
                time += Time.deltaTime;
                rt.localScale = Vector2.one - new Vector2(time, time);
                yield return null;
            }

        }
        tapRT.gameObject.SetActive(true);
        yield return null;
    }


    public void UpdateProgress(int val) {

        progressSlider.value = progressSlider.maxValue - val;

    }

    public void CompleteProgress() {
        progressSlider.value = progressSlider.maxValue;
    }

    public void SetMaxPlatforms(int val) {
        progressSlider.value = 0;
        progressSlider.maxValue = val;
    }

    public void ShowPerfect() {
        accuracyText.color = perfectColor;
        accuracyText.text = "Perfect!";
        accuracyText.ShowJumpStatus();

    }

    public void ShowGood() {
        accuracyText.text = "Good!";
        accuracyText.color = goodColor;
        accuracyText.ShowJumpStatus();
    }

    public void ShowLongJump() {
        accuracyText.text = "Long jump!";
        accuracyText.color = longJumpColor;
        accuracyText.ShowJumpStatus();
    }

}
