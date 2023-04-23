using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapNode : MonoBehaviour
{
    [SerializeField] public pointType type = pointType.cont;
    [SerializeField] public string scene;
    public enum pointType
    {
        cont,
        stop
    }

    public void Switch()
    {
        SceneManager.LoadScene(scene);
    }
}
