using UnityEngine;
using System;

[Serializable]
public class DesiredObject : MonoBehaviour
{

    public enum eDesiredObjectType
    {
        Ball,
        Blocks,
        Bear,
        Truck,
        Train,
        //Milk,
        //Rattle,
        //Sample,     // Add objects here.
        Sleep // Always ensure Sleep is last in the enum.
    }

    public eDesiredObjectType Type;

    public void SetDesiredObjectType(eDesiredObjectType i_Type) {
        Type = i_Type;
    }
}
