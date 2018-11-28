using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hammerplay.Utils.JsonAttribute;

public class WaveSystem : MonoBehaviour {

    [SerializeField]
    [Json("Waves")]
    private Wave waves;

	// Use this for initialization
	void Start () {

        Debug.Log(waves.interval);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

[System.Serializable]
public class Wave {

    public float interval;

    public EnemyType[] enemies;

    public enum EnemyType { Human, Bike, Car }
}
