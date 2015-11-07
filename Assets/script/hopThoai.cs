using UnityEngine;
using System.Collections;

public class hopThoai : MonoBehaviour {
	public GUIText textThoai;
	public GUITexture gangTay;
	string strSay= "";
	string strHienTai="";
	public GameObject mc;
	 string tag = "";
	// Use this for initialization
	void writeText()
	{
		if(strHienTai.Length<strSay.Length)
		{
			strHienTai = strHienTai + strSay.Substring(strHienTai.Length,1);
			textThoai.text  = strHienTai;
		}
		else
		{
			mc.GetComponent<Animation>().Stop();
			CancelInvoke("writeText");
		}
	}
	void OnMouseUp()
	{
		switch (tag)
		{
			case "ready":
				mc.GetComponent<Animation>().wrapMode = WrapMode.Once;
				mc.GetComponent<Animation>().CrossFade("bien");
				
				this.gameObject.SetActive(false);		
			break;
		}
		//Camera.main.GetComponent<GameManager>().StartLevel(Camera.main.GetComponent<GameManager>().currentLevel);

	}
	public void say(string str,string status)
	{
		tag = status;
		this.gameObject.SetActive(true);
		strSay  = str;
		strHienTai  = "";
		mc.GetComponent<Animation>().wrapMode = WrapMode.Loop;
		mc.GetComponent<Animation>().Play("nhepMoi");
		InvokeRepeating("writeText",0.0f,0.1f);
	}
	void Start () {
		textThoai.fontSize  = Screen.width/16;
		gangTay.transform.localScale  = new Vector3(0.3f* ((float)Screen.width / (float)Screen.height),0.3f,43);
		this.transform.localScale = new Vector3(0.5f* ((float)Screen.height / (float)Screen.width),0.25f,43);

		say("Hahahghjgkghjdghkjdhjfdkjdfgjkdfhkghkghkghkfghkfghkfgaha","ready");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
