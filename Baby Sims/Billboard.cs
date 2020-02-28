﻿using UnityEngine;

public class Billboard : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}