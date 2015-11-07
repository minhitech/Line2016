using UnityEngine;
using System.Collections;

public class danhGia : MonoBehaviour {
	public int danhGiaHienTai =0;
	public int huyBoDanhGia = 0; 

	public Texture2D[] textures;
	public GameObject text;

	public AudioClip[] ghiDiemEffect;
	// Use this for initialization
	void Start () {
	
	}
	public void hienDanhGia(int total)
	{
		if(total>3)
		{
			if(huyBoDanhGia==0)
			{
				danhGiaHienTai = total-4;
			}
			else{
				danhGiaHienTai++;

			}
			if(danhGiaHienTai>3)
			{
				danhGiaHienTai =3;
			}
			Camera.main.GetComponent<AudioSource>().PlayOneShot(ghiDiemEffect[danhGiaHienTai]);
			text.GetComponent<Renderer>().material.mainTexture = textures[danhGiaHienTai];
			GetComponent<Animation>().Play("xuatHien1");
			huyBoDanhGia =3;

		}
		else
		{
			huyBoDanhGia--;
			if(huyBoDanhGia<0)
			{
				huyBoDanhGia =0;
			}
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
