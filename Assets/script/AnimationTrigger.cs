using UnityEngine;
using System.Collections;

public class AnimationTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!this.GetComponent<ParticleSystem>().isPlaying)
			this.gameObject.SetActive(false);
	}
}
