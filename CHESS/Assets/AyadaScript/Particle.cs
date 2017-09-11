using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class Particle : MonoBehaviour {
    [SerializeField]
    private GameObject particle;


	// Use this for initialization
	void Start () {
        particle.SetActive(false);
	}
    //移動できる位置を受け取りその位置でセットする
    public void SetPartucle()
    {
        particle.SetActive(true);
    }

    public void ResetPartucle()
    {
        particle.SetActive(false);
    }
}
