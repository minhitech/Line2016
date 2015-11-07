using UnityEngine;
using System.Collections;

public class itemMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	 	
	}
	void OnMouseUp()
	{
		if(name == "challengeMenu")
		{
			this.transform.parent.GetComponent<Animation>().wrapMode = WrapMode.Once;
			this.transform.parent.GetComponent<Animation>().Play("selectedChallenge");
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
