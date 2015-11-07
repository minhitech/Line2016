using UnityEngine;
using System.Collections;

public class colorText : MonoBehaviour {
	public Texture2D[] vangXanhStyle = new Texture2D[10];

	public Texture2D[] style;

	public GameObject chuCaiMau;

	int kichThuoc;
	GameObject[] moji;
	string textHienTai="@@@@@@@@@@@@@@@@@@@@@@";
	// Use this for initialization
	void Start () {
		kichThuoc = GameSettings.size * GameSettings.size;
		style = vangXanhStyle;
		moji  = new GameObject[kichThuoc];
		textHienTai = "@@@@@@@@@@@@@@@@@@@@@@";
		for(int i = 0; i<kichThuoc;i++)
		{
			moji[i] = (GameObject)Instantiate(chuCaiMau, new Vector3(-100, -100, -100), this.transform.rotation);
			moji[i].transform.parent = this.transform;
			moji[i].transform.localPosition = new Vector3(0.5f*i,0,0);
			moji[i].GetComponent<Animation>().wrapMode = WrapMode.Once;
			moji[i].gameObject.SetActive(false);

		}
		GetComponent<Animation>().wrapMode = WrapMode.Once;
		setText("0");
	}
	public void setText(string strText)
	{
		string strTemp ="";
		string strTemp2 ="";
//		Debug.Log("AAAAAAAAAAAAAAAAAA    '" + strText+"'");
		strText = strText.Trim();
		if(strText.Length!=textHienTai.Length)
		{
			int len = strText.Length;
			float start = 0-0.3f*(len-1);
			textHienTai = "";
			for(int i=0;i<kichThuoc;i++)
			{
				if(i<len)
				{
					if(!moji[i].activeSelf)
					{
						moji[i].SetActive(true);
					}
					moji[i].transform.localPosition = new Vector3(start + 0.6f*i,0,0);
				
					textHienTai += "@";

				}
				else
				{
					if(moji[i].gameObject.activeSelf)
					{
						moji[i].SetActive(false);
					}
				}
			}
		}
		if(textHienTai=="")
			return;
		//Debug.Log("AAAAAAAAAAAAAAAAAA    '" + textHienTai+"'");
		for(int i=0;i<strText.Length;i++)
		{
			strTemp  = strText.Substring(i,1);
			//Debug.Log("AAAAAAAAAAAAAAAAAA    '" + textHienTai+"'");
			strTemp2  = textHienTai.Substring(i,1);
			
			if(strTemp!= strTemp2)
			{
				//Debug.Log(strTemp+"-"+strTemp2);
				moji[i].GetComponent<Renderer>().material.mainTexture = style[int.Parse(strTemp)];
				//moji[i].animation.Play("xuatHien1");
			}
		}
		textHienTai  = strText;
		GetComponent<Animation>().Play("anDiem");
	}
	// Update is called once per frame
	void Update () {
		
	}
}
