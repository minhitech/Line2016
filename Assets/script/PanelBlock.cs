using UnityEngine;
using System.Collections;

public class PanelBlock : MonoBehaviour {
	public int xCord;
	public int yCord;
	Star star;
	// Use this for initialization
	void Start () {
		star = Camera.main.GetComponent<LevelControl> ().clickStar.GetComponent<Star>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseUp () {
		star.deselect();
	}
	
	
	void OnMouseDown () {
		//Camera.main.GetComponent<LevelControl> ().click(xCord, yCord);
		star.select(xCord, yCord);
	}
	
	void OnMouseOver () {
		star.update(xCord, yCord);
	}
}
