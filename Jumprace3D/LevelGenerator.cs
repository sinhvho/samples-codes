using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public Transform levelCurve;
    public GameObject platformPrefab;
    public GameObject finishPrefab;

    public Material bounceMat, brokenMat;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        Generate();
    }

    LineRenderer lr;
    void Generate() {
        int size = levelCurve.childCount;
        UIManager.Instance.SetMaxPlatforms(size);

        for (int i=0; i< size-1; i++) {
            GameObject go = Instantiate(platformPrefab, levelCurve.GetChild(i).transform.position, Quaternion.identity);
            LevelPlatformInfo platformInfo = go.GetComponent<LevelPlatformInfo>();

            platformInfo.textMesh.text = (size-1 - i).ToString(); // Update the number
            platformInfo.number = (size-1 - i);

            // Determine the type by random
            int rand = Random.Range(0, 100);
            if (rand > 80) {
                platformInfo.type = LevelPlatformInfo.PlatformType.Broken;
                platformInfo.bouncePower = 8;
                platformInfo.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material = brokenMat;
            }
            else {
                platformInfo.type = LevelPlatformInfo.PlatformType.Bounce;
                platformInfo.bouncePower = 10;
            }

            if (i > 0) { // This ensures that the first iteration is ignored to avoid a out of bounds error.
                var lookPos = go.transform.position-levelCurve.GetChild(i - 1).transform.position; // Face the next platform tothe previous platform.
                var rotation = Quaternion.LookRotation(lookPos);
                rotation.x = go.transform.rotation.x;
                rotation.z = go.transform.rotation.z;
                go.transform.rotation = rotation;       
                
            }
            rand = Random.Range(0, 100);
            if (rand > 80) {
                platformInfo.feature = LevelPlatformInfo.FeatureType.Moving;
            }
            else {
                platformInfo.feature = LevelPlatformInfo.FeatureType.None;

            }
            go.transform.SetParent(transform);
        }


        GameObject finish = Instantiate(finishPrefab, levelCurve.GetChild(size - 1).transform.position, Quaternion.identity);
        finish.transform.SetParent(transform);
    }
}
