using UnityEngine;
using System.Collections;

public class listSetting : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	public void menuIdle()
	{
		GetComponent<Animation>().Play("menuIdle");
		GetComponent<Animation>().wrapMode = WrapMode.Loop;

	}
	public void toChallenge()
	{
		GameSettings.TESTMODE = GAMEMODE.CHALLENGE;
		Application.LoadLevel("line");
	}
	// Update is called once per frame
	void Update () {
	
	}
}
