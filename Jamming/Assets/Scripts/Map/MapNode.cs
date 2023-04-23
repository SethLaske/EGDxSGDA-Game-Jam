using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    [SerializeField] public pointType type = pointType.cont;
    public enum pointType
    {
        cont,
        stop
    }
}
