using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// automatically causes attached gameObject to self destruct after time runs out.
/// </summary>
public class SelfDestruct : MonoBehaviour {

	public float time;

	void Update () {
		time -= Time.deltaTime;
		if(time < 0)
			Destroy(this.gameObject);
	}
}
