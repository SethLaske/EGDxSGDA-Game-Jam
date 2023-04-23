using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTransparencyScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>.alphaHitTestMinimumThreshold = 0.5f;
    } 
}
