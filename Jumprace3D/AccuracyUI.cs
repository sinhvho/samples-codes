using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AccuracyUI : MonoBehaviour
{

    RectTransform rt;
    TextMeshProUGUI textMeshPro;
    public string text;
    public Color color;

    public void ShowJumpStatus() {
        if(rt == null) {
            rt = GetComponent<RectTransform>();
        }
        if(textMeshPro == null) {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        textMeshPro.color = color;
        textMeshPro.text = text;

        StopCoroutine("UIPopOut");
        StartCoroutine("UIPopOut");
    }

    private IEnumerator UIPopOut() {
        float time = 0, threshold = 1;

        while (time < threshold) {
            time += Time.deltaTime * 2f;
            rt.localScale = new Vector3(time, time, time);

            yield return null;
        }
        rt.localScale = new Vector3(1, 1, 1); 
        yield return new WaitForSeconds(.7f);

        while (time > 0) {
            time -= Time.deltaTime * 2;
            rt.localScale = new Vector3(time, time, time);

            yield return null;
        }

        if(time <= 0)
            rt.localScale = new Vector3(0, 0, 0);

        this.enabled = false;

        yield return null;
    }
}
