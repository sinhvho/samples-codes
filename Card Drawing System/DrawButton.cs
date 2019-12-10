using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DrawButton : MonoBehaviour {

    bool swap;              // a Toggle to control the shaking of the drawing card icon
    public bool clicked;    // Toggled to true when clicked on the pack
    public GameObject buffParticle; // Emits this particle after finishing the animation sequence.
    [SerializeField] GameObject clickImage; // The image and text that indicates to click on the package to open.

    private void Start() {
        Rotate();
    }

    // Rotate and Shake the drawing card until player presses on it.
    void Rotate() {
        if (!clicked) {
            if (swap) {
                transform.DORotate(new Vector3(0, 0, -15), .35f, RotateMode.Fast);
                swap = false;
            }
            else {
                transform.DORotate(new Vector3(0, 0, 15), .35f, RotateMode.Fast);
                swap = true;
            }
            Invoke("Rotate", 0.35f);
        } else {
            //StartCoroutine("InitiateCardOpen");
        }
    }


    // When player clicks on card, start this sequence -- making it rotate faster and eventually burst into particles.
    public IEnumerator InitiateCardOpen() {
        clicked = true;
        clickImage.SetActive(false);
        float timer = 0;
        float delay = .15f;
        while (timer < delay) {
            if (swap) {
                transform.DORotate(new Vector3(0, 0, -15), .15f, RotateMode.Fast);
                swap = false;
            }
            else {
                transform.DORotate(new Vector3(0, 0, 15), .15f, RotateMode.Fast);
                swap = true;
            }
            timer += Time.deltaTime;
            yield return new WaitForSeconds(.1f);
        }
        GetComponent<Image>().enabled = false;

        buffParticle.gameObject.SetActive(true);

        Invoke("Disable", 2);
        yield return null;
    }


    // Disable the Drawing Image to avoid raycast issues.
    private void Disable() {
        gameObject.SetActive(false);
    }
}
