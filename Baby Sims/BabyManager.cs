using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KobGamesSDKSlim;
using KobGamesSDKSlim.MenuManagerV1;
using Random = UnityEngine.Random;
using System;

public class BabyManager : Singleton<BabyManager>
{

    public int Progress;

    [ReadOnly]
    public Baby[] Babies;
    [ReadOnly]
    public Bed[] Beds;
    [ReadOnly]
    public DesiredObject[] DesiredObjects;

    public int DesireAmount = 0;

    public float CurrentLevel;
    public AnimationCurve LevelCurve;

    [ReadOnly]
    public List<Vector3> m_BabyPositions = new List<Vector3>();
    [ReadOnly]
    public List<Vector3> m_BedPositions = new List<Vector3>();

    private List<Vector3> m_DummyBabyPositions = new List<Vector3>();
    private List<Vector3> m_DummyBedPositions = new List<Vector3>();
    private Vector3 m_DummyPosition;

    private BabyVariablesEditor m_BabyVars { get { return GameConfig.Instance.Baby; } }

    [Button]
    public void SetReferences() {
        Babies = transform.GetComponentsInChildren<Baby>();
        Beds = transform.GetComponentsInChildren<Bed>();
        DesiredObjects = transform.GetComponentsInChildren<DesiredObject>();
        getAllObjectPositions();
    }


    #region Unity Initialization
    private void OnEnable() {
        GameManager.OnGameReset += OnReset;
        GameManager.OnLevelLoaded += OnLevelLoaded;
    }

    public override void OnDisable() {
        GameManager.OnGameReset -= OnReset;
        GameManager.OnLevelLoaded -= OnLevelLoaded;
    }
    #endregion

    public void OnReset()
    {
        Progress = 0;
        DesireAmount = 0;   
    }


    public void OnLevelLoaded() {
        m_DummyBabyPositions.Clear();
        m_DummyBedPositions.Clear();

        m_DummyBabyPositions = new List<Vector3>(m_BabyPositions);
        m_DummyBedPositions = new List<Vector3>(m_BedPositions);

        setupBaby();
        generateBedTypes();

    }


    public void AddProgression() {
        Progress++;
        UIManager.Instance.UpdateProgress();
        CheckLevelFinish();
    }

    public void CheckLevelFinish() {

        if (Progress >= DesireAmount) {
            MenuManager.Instance.OpenMenuScreen(nameof(Screen_LevelCompleted));
            GameManager.Instance.LevelCompleted();
        }
    }
    private int m_AvailableBabies;
    private int m_MaxGoal;
    public void SetLevelParameters(int level) {
        switch (level) {
            case 0:
                m_AvailableBabies = 1;
                m_MaxGoal = 2;
                break;
            case 1:
                m_AvailableBabies = 2;
                m_MaxGoal = 4;
                break;
            case 2:
                m_AvailableBabies = 3;
                m_MaxGoal = 5;
                break;
            case 3:
                m_AvailableBabies = 4;
                m_MaxGoal = 8;
                break;
            case 4:
                m_AvailableBabies = 5;
                // Randomized Goals
                break;
        }
    }

    private void setupBaby() {
        int i_Count = 0;
        int i_Level = 0;

        if (StorageManager.Instance.CurrentLevel < 4) {
            SetLevelParameters(StorageManager.Instance.CurrentLevel);
        }
        else if (StorageManager.Instance.CurrentLevel > 4) {
            {
                i_Level = Random.Range(2, 5);
                SetLevelParameters(i_Level);
            }
        }
        else if (StorageManager.Instance.CurrentLevel == 4) {
            m_AvailableBabies = 5;
            i_Level = 4;
        }
    

        int i_Max = m_MaxGoal;

        foreach(Baby b in Babies) {
            if (i_Count < m_AvailableBabies) {

                if (i_Level <= 3) {
                    if (i_Max >= 2) {
                        b.Behaviour.SetDesireAmount(2 - 1);
                        i_Max -= 2;
                    }
                    else {
                        b.Behaviour.SetDesireAmount(i_Max - 1);
                        i_Max = 0;
                    }
                } else {
                    b.Behaviour.SetDesireAmount(Random.Range(0, m_BabyVars.MaxDesireListSize + 1));
                }

                b.SetUpBaby();
                // Change transform here
                m_DummyPosition = getRandomPositionFromStorage("Baby");

                // Keep the same y value because we don't want to change object's elevation
                b.transform.position = new Vector3(m_DummyPosition.x, b.transform.position.y, m_DummyPosition.z);

                b.gameObject.SetActive(true);
                i_Count++;
            }
            else {
                b.gameObject.SetActive(false);
            }
        }
    }
    private void generateBedTypes() {
        for (int i=0; i<Beds.Length; i++) {
            if (i < m_AvailableBabies) {
                Beds[i].ChangeBedType(Babies[i].Behaviour.DesiredBedType);
                m_DummyPosition = getRandomPositionFromStorage("Bed");
                Beds[i].transform.position = new Vector3(m_DummyPosition.x, Beds[i].transform.position.y, m_DummyPosition.z);
                Beds[i].gameObject.SetActive(true);
            } else {
                Beds[i].gameObject.SetActive(false);
            }
        }
    }

    private void getAllObjectPositions() {
        // When utilizing this, only change the Y

        m_BabyPositions.Clear();
        m_BedPositions.Clear();

        foreach (Baby baby in Babies) {
            m_BabyPositions.Add(baby.transform.position);
        }

        foreach (Bed bed in Beds) {
            m_BedPositions.Add(bed.transform.position);
        }
    }

    private Vector3 getRandomPositionFromStorage(string type) {
        int i_Rand = 0;
        switch (type) {
            case "Baby":
                i_Rand = Random.Range(0, m_DummyBabyPositions.Count - 1);
                m_DummyPosition = m_DummyBabyPositions[i_Rand];
                m_DummyBabyPositions.RemoveAt(i_Rand);
                break;
            case "Bed":
                i_Rand = Random.Range(0, m_DummyBedPositions.Count - 1);
                m_DummyPosition = m_DummyBedPositions[i_Rand];
                m_DummyBedPositions.RemoveAt(i_Rand);
                break;
        }
        return m_DummyPosition;
    }
}
