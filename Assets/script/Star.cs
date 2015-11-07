using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {
	LevelControl lvl;
	public int xCord = -1;
	public int yCord = -1;
	// Use this for initialization
	void Start () {
		lvl = Camera.main.GetComponent<LevelControl> ();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void update (int x, int y)
	{
		if (x == xCord && y == yCord)
			return;
			
		//Debug.Log (Vector3.Distance(Input.mousePosition, Camera.main.WorldToScreenPoint(this.transform.position)));
		if (Vector3.Distance(Input.mousePosition, Camera.main.WorldToScreenPoint(this.transform.position)) > 50)
		{
			xCord = x;
			yCord = y;
			this.transform.localPosition = lvl.game2Display(x, y) + new Vector3(0, 0.1f, 0);
		}
	}
	
	public void select (int x, int y)
	{
		//Debug.Log ("select");
		this.gameObject.SetActive(true);
		xCord = x;
		yCord = y;
		this.transform.localPosition = lvl.game2Display(x, y) + new Vector3(0, 0.1f, 0);
		//Debug.Log ("1:" + Input.mousePosition);
		//Debug.Log ("2:" + Camera.main.WorldToScreenPoint(this.transform.position));
	}
	
	public void deselect ()
	{
		//Debug.Log ("deselect");
		this.gameObject.SetActive(false);
		Camera.main.GetComponent<LevelControl> ().click(xCord, yCord);
		xCord = -1;
		yCord = -1;
	}
}
