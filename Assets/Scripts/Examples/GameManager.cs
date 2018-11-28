using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hammerplay.Utils.JsonAttribute;

public class GameManager : MonoBehaviour {

    [SerializeField]
    //[Json("WaveInterval")]
    private float waveInterval = 5;


	// Use this for initialization
	void Start () {
        //Debug.Log(JsonUtility.ToJson(designParamets));
        Debug.Log(waveInterval);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

