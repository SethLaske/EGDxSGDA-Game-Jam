using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public int bearsChasing;
    [SerializeField] private AudioSource sneakMusic;
    [SerializeField] private AudioSource chaseMusic;
    private bool sneaking;
    // Start is called before the first frame update
    private void Start()
    {
        sneaking = true;
    }
    public void AddBear()
    {
        bearsChasing += 1;
        if (sneaking)
        {
            sneaking = false;
            sneakMusic.Stop();
            chaseMusic.Play();
        }
    }
    public void RemoveBear()
    {
        bearsChasing -= 1;
        if(bearsChasing == 0 && !sneaking)
        {
            sneaking = true;
            chaseMusic.Stop();
            sneakMusic.Play();
        }
    }
}
