using UnityEngine;
using System;
using KobGamesSDKSlim;

[Serializable]
public class Bed : MonoBehaviour
{

    public void OnEnable() {
        GameManager.OnLevelLoaded += OnLevelLoaded;
    }

    public void OnDisable() {
        GameManager.OnLevelLoaded -= OnLevelLoaded;
    }

    public bool IsOccupied;
    [Header("References")]
    public Material[] Materials;
    public Renderer Renderer;

    public Material CompleteMaterial;

    private Material[] m_Materials;

    public enum eBedType
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H
    }

    public eBedType Type;

    public void ChangeBedType(eBedType i_Type) {
        Type = i_Type;
        m_Materials = Renderer.materials;

        for(int i = 0; i< m_Materials.Length; i++){// (Material m in m_Materials) {
            m_Materials[i] = Materials[(int)Type]; 
        }
        Renderer.materials = m_Materials;
        //Renderer.material = Materials[(int)Type];
    }

    public void ChangeComplete() {
        IsOccupied = true;
        m_Materials = Renderer.materials;

        for (int i = 0; i < m_Materials.Length; i++) {// (Material m in m_Materials) {
            m_Materials[i] = CompleteMaterial;
        }
        Renderer.materials = m_Materials;
        //Renderer.material = Materials[(int)Type];
    }

    void OnLevelLoaded() {
        IsOccupied = false;
    }
}
