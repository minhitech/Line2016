using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	public float ACTIONS_LENGTH = 0.1f;
	
	int[] d4x = {-1, 0, 1, 0};
	int[] d4y = {0, 1, 0, -1};
	
	Vector3[] pos;
	int nStep;
	int curStep;
	
	Vector3 newPos;
	Vector3 translation;
	Vector3 rotation;
	LevelControl lvl;
	
	float time;
	bool isMoving;
	
	int x1, y1, x2, y2;
	bool isExploding;
	
	public int endlessFlag = 0;
	
	// Use this for initialization
	void Start () {
		lvl = Camera.main.GetComponent<LevelControl> ();
		pos = new Vector3[GameSettings.size * GameSettings.size + 10];
		isMoving = false;
	}
	
	public void select () {
		GetComponent<Animation>().wrapMode = WrapMode.Loop;
		GetComponent<Animation>().Play("reset");
		GetComponent<Animation>().Play("selected");
	}
	
	public void xuatHien()
	{
		endlessFlag = 0;
	
		GetComponent<Animation>().wrapMode = WrapMode.Once;
		GetComponent<Animation>().Play("reset");
		GetComponent<Animation>().Play("xuatHien");
	}
	
	public void xuatHien(int challengeFlag)
	{
		
		GetComponent<Animation>().wrapMode = WrapMode.Once;
		GetComponent<Animation>().Play("reset");
		GetComponent<Animation>().Play("xuatHien");

	}
	
	public void unSelect ()
	{	GetComponent<Animation>().wrapMode = WrapMode.Once;
		GetComponent<Animation>().Play("reset");
		GetComponent<Animation>().wrapMode = WrapMode.Loop;
		GetComponent<Animation>().CrossFade("idle1");
	}
	
	public void idle()
	{
		GetComponent<Animation>().wrapMode = WrapMode.Once;
		GetComponent<Animation>().Play("reset");
		GetComponent<Animation>().wrapMode = WrapMode.Loop;
		GetComponent<Animation>().CrossFade("idle1");
	}
	
	public void explode ()
	{
		if (endlessFlag == 1)
		{
			lvl.endlessReset();
		}
		lvl = Camera.main.GetComponent<LevelControl>();
		Camera.main.GetComponent<AudioSource>().PlayOneShot(lvl.tiengNoEffect);

		GameObject objTemp = lvl.pool.getData("noEffect");
		objTemp.transform.position  = this.transform.position + new Vector3(0,0.6f,-0.27216f);
		objTemp.GetComponent<ParticleSystem>().Play();
		GetComponent<Animation>().wrapMode = WrapMode.Once;
		GetComponent<Animation>().Play ("no");
	}

	public void toggleEndlessCheck ()
	{
		endlessFlag = 1;
		//	TO DO: làm xong GetComponent<Animation>() thì bỏ vào đây tạm thời cho nó quay ngược :v
		this.transform.rotation = Quaternion.Euler(270, 0, 0);
	}

	public void walk (int x1, int y1, int x2, int y2, int[] moveMap, int steps, bool isExploding)
	{
		this.x1 = x1;
		this.y1 = y1;
		this.x2 = x2;
		this.y2 = y2;
		this.nStep = steps;
		this.isExploding = isExploding;
		
		isMoving = true;
		int x = x1;
		int y = y1;
		for (int i = 0; i < nStep; i++)
		{
			pos[i] = lvl.game2Display(x, y);
			x += d4x[moveMap[nStep - i - 1]];
			y += d4y[moveMap[nStep - i - 1]];
		}
		pos[nStep] = lvl.game2Display(x, y);
	
		curStep = 0;
		nextStep();
	}

	private void nextStep ()
	{
		translation = (pos[curStep + 1] - pos[curStep]) / ACTIONS_LENGTH;
		int temp = 90;
		if(pos[curStep+1].z == pos[curStep].z)
		{
			if(pos[curStep+1].x<pos[curStep].x)
				temp = 270;
			else
				temp = 90;
		}else if (pos[curStep+1].x == pos[curStep].x)
		{
			if(pos[curStep+1].z<pos[curStep].z)
				temp = 180;
			else
				temp = 360;
		}
		transform.rotation = Quaternion.Euler(270,temp,0);
		time = 0f;
	} 

	// Update is called once per frame
	void Update () {
		if (isMoving)
		{
			if (Camera.main.GetComponent<LevelControl> ().isMoving)
			{
				time += Time.deltaTime;
				if (time < ACTIONS_LENGTH)
					this.transform.localPosition += translation * Time.deltaTime;
				else
				{
					curStep++;
					this.transform.localPosition = pos[curStep];
					nextStep ();
					if (curStep >= nStep)
					{
						isMoving = false;
						GameObject gameObjTemp = GameObject.Find("khoi");
						transform.rotation = Quaternion.Euler(270,180,0);
						gameObjTemp.GetComponent<ParticleSystem>().Stop();
						gameObjTemp.GetComponent<ParticleSystem>().Clear();
						lvl.finishMoving (x1, y1, x2, y2, isExploding);
						unSelect();
					}
				}
			}
		}
	}
	
	public void setActiveFalse()
	{
		if (endlessFlag == 1)
		{
			//	TO DO: có GetComponent<Animation>() cho cái special thì bỏ cái này đi
			this.transform.rotation = Quaternion.Euler(270, 180, 0);
			lvl.endlessReset();
			endlessFlag = 0;
		}
		//GetComponent<Animation>().wrapMode = WrapMode.Once;
		//GetComponent<Animation>().Play("reset");
		this.gameObject.SetActive(false);
	}
	
	public void jumpEffect()

	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(lvl.jumpEffect,0.3f);
	}
	public void runEffect()
		
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(lvl.runEffect);
	}

	public void tiengNoEffect()
		
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(lvl.tiengNoEffect);
	}
}