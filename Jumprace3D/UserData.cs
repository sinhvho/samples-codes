using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour
{
    public string name;
    public int score;

    public UserData(string n, int s) {
        name = n;
        score = s;
    }
}
