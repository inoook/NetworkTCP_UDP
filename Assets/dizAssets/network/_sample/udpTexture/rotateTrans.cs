using UnityEngine;
using System.Collections;

public class rotateTrans : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public float speed = 2;
	// Update is called once per frame
	void Update () {
		this.transform.Rotate(new Vector3(0, Time.deltaTime* speed, 0));
	}
}
