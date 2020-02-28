using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class LevelPlatformInfo : MonoBehaviour
{
    public enum PlatformType
    {
        Broken, Bounce, Spring
    }

    public enum FeatureType
    {
        None, Moving, Fan
    }

    public TextMesh textMesh;
    public int number;
    public PlatformType type;
    public FeatureType feature;
    public float bouncePower;
    public bool isDestroyed = false;
    public RawImage bounceUI;

    // To move the individual platform back and forth
    private bool moving = false;
    Vector3 posA, posB;

    public void Bounced() {
        StopCoroutine("ShowBounceImage");
        StartCoroutine("ShowBounceImage");
    }

    IEnumerator ShowBounceImage() {
        Color c = bounceUI.color;
        c.a = 1;
        bounceUI.color = c;
        float time = 0;
        while(c.a > 0) {
            time += Time.deltaTime / 5;
            c.a -= time;
            bounceUI.color = c;
            yield return null;
        }

        yield return null;
    }

    private void Update() {
        if (isDestroyed) {
            if (transform.localScale.x > 0) {
                transform.localScale -= new Vector3(Time.deltaTime*2, Time.deltaTime*2, Time.deltaTime*2);

                if(transform.localScale.x < 0)
                    transform.localScale = Vector3.zero;
            }
                
        } else {
            transform.localScale = Vector3.one;

            switch (feature) {
                case FeatureType.Moving:
                    if (!moving) {
                        StartMove();
                    }
                    break;
                case FeatureType.Fan:
                    break;
            }
        }
        
    }

    void StartMove() {
        posA = transform.GetChild(1).position;
        posB = transform.GetChild(2).position;
        moving = true;

        StartCoroutine("MoveCycle");
    }

    private IEnumerator MoveCycle() {

        while (!isDestroyed) {
            yield return StartCoroutine("MoveToA");
            yield return StartCoroutine("MoveToB");
            yield return null;
        }

        yield return null;
    }

    private IEnumerator MoveToA() {
        while(transform.GetChild(0).position != posA) {
            transform.GetChild(0).position = Vector3.MoveTowards(transform.GetChild(0).position, posA, Time.deltaTime * Random.Range(.5f,1));
            yield return null;
        }

        yield return null;
    }

    private IEnumerator MoveToB() {
        while (transform.GetChild(0).position != posB) {
            transform.GetChild(0).position = Vector3.MoveTowards(transform.GetChild(0).position, posB, Time.deltaTime * Random.Range(.5f, 1));
            yield return null;
        }

        yield return null;
    }
}
