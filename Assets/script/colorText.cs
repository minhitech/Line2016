using UnityEngine;
using System.Collections;

public class colorText : MonoBehaviour {
	public Texture2D[] vangXanhStyle = new Texture2D[10];

	public Texture2D[] style;

	public GameObject chuCaiMau;

	int kichThuoc;
	public int size ;
	public float khoangCach;
	GameObject[] moji;
	string textHienTai="@@@@@@@@@$%$%@@@@@@@@@@@@@";
	// Use this for initialization
	void Start () {
		kichThuoc =size *size;
		style = vangXanhStyle;
		moji  = new GameObject[kichThuoc];
		textHienTai = "@@@@@@@@@@@@@@%$%#$%@@@@@@@@";
		for(int i = 0; i<kichThuoc;i++)
		{
			moji[i] = (GameObject)Instantiate(chuCaiMau, new Vector3(-100, -100, -100), this.transform.rotation);
			moji[i].transform.parent = this.transform;
			moji[i].transform.localPosition = new Vector3(khoangCach*i,0,0);
			moji[i].GetComponent<Animation>().wrapMode = WrapMode.Once;
			moji[i].gameObject.SetActive(false);
			
		}
		//animation.wrapMode = WrapMode.Once;
		setText ("00");
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
			float start = 0-(khoangCach/2)*(len-1);
			textHienTai = "";
			for(int i=0;i<kichThuoc;i++)
			{
				if(i<len)
				{
					if(!moji[i].activeSelf)
					{
						moji[i].SetActive(true);
					}
					moji[i].transform.localPosition = new Vector3(start + khoangCach*i,0,0);
					
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
				moji[i].GetComponent<Animation>().Play("xuatHien1");
			}
		}
		textHienTai  = strText;
		//animation.Play("anDiem");
	}
	// Update is called once per frame
	void Update () {
		
	}
}
