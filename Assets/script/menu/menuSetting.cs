using UnityEngine;
using System.Collections;

public class menuSetting : MonoBehaviour {

	// Use this for initialization
	public GameObject menu;
	void Start () {
		InvokeRepeating("startMenu",1,1);
	}

	void startMenu()
	{
		menu.SetActive(true);
		menu.GetComponent<Animation>().Play("menuXuatHien");
		CancelInvoke("startMenu");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
