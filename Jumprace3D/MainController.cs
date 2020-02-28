using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainController : MonoBehaviour
{

    private Rigidbody rb;
    public float power;
    public Animator anim;

    LineRenderer lr;

    public GameObject indicator;

    public float offsetDistance;

    public Transform startingPoint;

    public GameObject fireFx;

    bool bounce = false;

    // Start is called before the first frame update
    void Start()    
    {
        rb = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>(); 
    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit hit;
        // Raycast below the player to indicate where they will land
        if (Physics.Raycast(transform.position, -Vector3.up, out hit)) {
            offsetDistance = hit.distance;
            indicator.transform.position = hit.point + new Vector3(0,.1f,0);
        }

        // Line Renderer Positions
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, indicator.transform.position);
    }       

    private void ToggleFall() {
        anim.SetBool("Jump", false);
    }
    public void AddBounce(float p) {

        rb.AddForce(Vector3.up * p,ForceMode.VelocityChange);
        anim.SetBool("Jump", true);
        //rb.AddForce(Vector3.forward * 150);
        Invoke("ToggleFall", .1f);
    }

    int current = 30, previous = 30;

    private void FixedUpdate() {
        if (bounce) {
            AddBounce(power);
            bounce = false;
        }
        
    }

    private void OnCollisionEnter(Collision collision) {


        switch (collision.gameObject.tag) {

            case "Platform":
                LevelPlatformInfo platformInfo = collision.gameObject.transform.parent.parent.GetComponent<LevelPlatformInfo>();

                bounce = true;
                power = platformInfo.bouncePower;

                platformInfo.Bounced();
                //AddBounce(platformInfo.bouncePower);

                //Debug.Log(Vector3.Distance(transform.position, collision.gameObject.transform.position));
                float distance = Vector3.Distance(transform.position, collision.gameObject.transform.position);


                if (platformInfo.number < current) {
                    previous = current;
                    current = platformInfo.number;

                    UIManager.Instance.UpdateProgress(platformInfo.number);

                    if (previous - current > 5) {
                        UIManager.Instance.ShowLongJump();
                        fireFx.SetActive(false);
                    }
                    else if (distance > .5f && distance < .89f) {
                        UIManager.Instance.ShowPerfect();
                        fireFx.SetActive(true);
                    }
                    else if (distance > .89f && distance < 1.6f) {
                        UIManager.Instance.ShowGood();
                        fireFx.SetActive(false);
                    }
                }

                Leaderboard.Instance.UpdateLeaderboard(current);

                switch (platformInfo.type) {
                    case LevelPlatformInfo.PlatformType.Broken:
                        platformInfo.isDestroyed = true;
                        SoundManager.Instance.PlaySfx(SoundManager.Instance.brokenSfx);
                        break;
                    case LevelPlatformInfo.PlatformType.Bounce:
                        SoundManager.Instance.PlaySfx(SoundManager.Instance.bounceSfx);
                        break;
                }
                break;

            case "Spring":
                LevelPlatformInfo springInfo = collision.gameObject.transform.parent.GetComponent<LevelPlatformInfo>();
                springInfo.Bounced();
                //AddBounce(springInfo.bouncePower);
                bounce = true;
                power = springInfo.bouncePower;
                SoundManager.Instance.PlaySfx(SoundManager.Instance.springSfx);
                fireFx.SetActive(false);
                break;

            case "Finish":
                anim.SetBool("Land", true);
                UIManager.Instance.CompleteProgress();
                SoundManager.Instance.PlaySfx(SoundManager.Instance.celebrationSfx);
                SoundManager.Instance.StopBGM();
                UIManager.Instance.CallCompletedLevel();
                fireFx.SetActive(false);
                GameManager.Instance.gameIsComplete = true;
                break;

            case "Ground":
                Debug.Log("Game Over");
                anim.SetBool("Death", true);
                Invoke("Restart", 3f);
                fireFx.SetActive(false);
                break;
        }
    }

    void Restart() {
        SceneManager.LoadScene(0);
    }
}
