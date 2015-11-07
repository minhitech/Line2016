using UnityEngine;
using System.Collections;

public class LevelControl : MonoBehaviour {
	private LevelTracking levelTracking;
	private GAMEMODE gameMode;
	//	***************************************************************************************************
	//	Sửa chỗ này lại thành mỗi loại rồi test

	//	***************************************************************************************************
	
	//	all mode variables
	int numColor;
	int size;
	
	//	endless variables
	int curGenTurn = 0;
	int curExplodeTurn = 0;
	int endlessX = -1;
	int endlessY = -1;
	
	public GameObject textScore;
	public GameObject textBoss;
	public GameObject khoi;
	public GameObject clickStar;
	
	public DataPool pool;
	Vector2 nullVector2;
	int[,] map;
	Vector2[,] panelBlocks;
	Vector2[,] blocks;
	
	Vector2[,] preBalls;
	Vector3[] genList;
	int genNum;

	float botX = -2.769344f;
	float botY = 0.6379896f;
	float botZ = -2.769344f;
	float space = 0.919416f;

	int maxCount;
	int curCount;
	
	int oldSelectedX = -1;
	int oldSelectedY = -1;
	int newSelectedX;
	int newSelectedY;
	string selecting = "panelBlock";
	
	float[] scoreRateBall;
	float score;
	
	int bossMaxHealth;
	int bossCurHeatth;
	int bossX;
	int bossY;
	
	int[] queueX;
	int[] queueY;
	int[] trace;
	int[] moves;
	int[] moveMap;
	bool[,] avail;
	int steps;
	
	//	State of level
	public bool isMoving;
	public bool isGenerating;
	public bool isReporting;
	
	int[] d8x = {-1, -1, -1, 0, 0, 1, 1, 1};
	int[] d8y = {-1, 0, 1, -1, 1, -1, 0, 1};
	int[] d4x = {-1, 0, 1, 0};
	int[] d4y = {0, 1, 0, -1};
	
	// hiệu ứng âm thanh
	public AudioClip jumpEffect;
	public AudioClip runEffect;

	public AudioClip tiengNoEffect;

	//
	public GameObject danhGiaBox;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	/// <summary>
	/// DEBUG PURPOSE, IF RETURN != 0 THEN THERE IS A FUCKING ERROR
	/// </summary>
	private void runTest()
	{
		int count = 0;
		for (int i = 0; i < size; i++)
			for (int j = 0; j < size; j++)
				if (map[i, j] != 0)
					count++;
					
		for (int i = 0; i < size; i++)
			for (int j = 0; j < size; j++)			
			{
				if ((map[i, j] == 0) && blocks[i, j] != nullVector2)
				{
					Debug.Log ("Test: " + -1 + ", curCount: " + curCount + ", realCount: " + count);
					return;
				}
					
				if ((map[i, j] != 0) && blocks[i, j] == nullVector2)	
				{
					Debug.Log ("Test: " + -2 + ", curCount: " + curCount + ", realCount: " + count);
					return;
				}
			}
		Debug.Log ("Test: " + 0 + ", curCount: " + curCount + ", realCount: " + count + "    " + Random.Range (1, 1000));
	}
	/// <summary>
	/// LOOK ABOVE
	/// </summary>

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//	Từ tọa độ game ra tọa độ màn hình
	//	Chỉnh mấy cái panel linh tinh ở đây :d
	public Vector3 game2Display(int x, int y)
	{
		return new Vector3 (botX + x * space, botY, botZ + y * space);
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// Use this for initialization
	void Start () {	
		size = GameSettings.size;
		panelBlocks = new Vector2[size, size];
		blocks = new Vector2[size, size];
		map = new int[size, size];
		queueX = new int[size * size + 1];
		queueY = new int[size * size + 1];
		trace = new int[size * size + 1];
		moves = new int[size * size + 1];
		moveMap = new int[size * size + 1];
		avail = new bool[size, size];
		scoreRateBall = new float[size * size + 1];
		preBalls = new Vector2[size, size];
		genList = new Vector3[size * size + 1];
		maxCount = size * size;		
		pool = Camera.main.GetComponent<DataPool>();
		nullVector2 = new Vector2(-1, -1);
		levelTracking = new LevelTracking();
		
		initPreBalls();
		initScoreRate();
		initPool();
		initPanelBoard ();
		
		startLevel(GameSettings.TESTMODE);
	}

	public void resetLevel()
	{
		for (int i = 0; i < size; i++)
			for (int j = 0; j < size; j++)
			{
				if (map[i, j] != 0)
				{
					pool.getData(blocks[i, j]).GetComponent<Block>().gameObject.SetActive(false);
					blocks[i, j] = pool.releaseData(blocks[i, j]);
					map[i, j] = 0;
				}
				if (preBalls[i, j] != nullVector2)
					destroyPreBall(i, j);
			}
		isMoving = false;
		isReporting = true;
		selecting = "panelBlock";
		oldSelectedX = -1;
		oldSelectedY = -1;
		
		levelTracking.initTracking ();
	}
	
	public void startLevel(GAMEMODE gameMode)
	{
		resetLevel ();
		this.gameMode = gameMode;
		
		score = 0;
		updateScore(0);
		curCount = 0;
		
		if (gameMode == GAMEMODE.CLASSIC)
		{
			numColor = GameSettings.initColorNormal;
			textBoss.SetActive(false);
			genPreBalls (GameSettings.initBalls, 0);
			genBalls (GameSettings.initBallsEachRound);
		}
		else if (gameMode == GAMEMODE.STORY)
		{	
			numColor = GameSettings.initColorNormal;
			genBoss(3, 3, 1, 10);
			genBarrier (3);
			damageBoss(0);
			genPreBalls (GameSettings.initBalls, 0);
			genBalls (GameSettings.initBallsEachRound);
		}
		else if (gameMode == GAMEMODE.ENDLESS)
		{
			numColor = GameSettings.initColorEndless;
			genBonus (2, 2, 10);
			genBonus (5, 5, 10);
			genBonus (2, 5, 10);
			genBonus (5, 2, 10);
			genPreBalls (GameSettings.initBalls, 0);
			genBalls (GameSettings.initBallsEachRound);
			curGenTurn = 0;
			curExplodeTurn = 0;
			endlessX = -1;
			endlessY = -1;
			updateEndlessNumColor();
		} 
		else if (gameMode == GAMEMODE.CHALLENGE)
		{
			numColor = GameSettings.initColorNormal;
			textBoss.SetActive(false);
			genPreBalls (GameSettings.initBalls, 0);
			genBalls (GameSettings.initBallsEachRound);
			//genBarrier (3);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//	TO DO: final thì bỏ cái này
		if (Input.GetKeyDown(KeyCode.R))
		{
			Debug.Log ("reseting");
			startLevel(gameMode);
			return;
		}
		
		if (curCount == maxCount || curCount == 0)
		{
			startLevel(gameMode);
		}
		else if (!isMoving && isGenerating)
		{
			if (gameMode == GAMEMODE.ENDLESS)
			{
				if (endlessX + endlessY != -2)
				{
					curExplodeTurn++;
				}
				else
				{ 
					curGenTurn++;
				}
				if (curGenTurn >= GameSettings.maxGenTurnEndless)
					genEndlessCheckPoint();
			
				if (curExplodeTurn >= GameSettings.maxExplodeTurnEndless)
				{
					//	TO DO: co animation no thi cho 1 vao
					numColor++;
					destroyBlock(endlessX, endlessY, 0, false);
				}
				
				if (Random.value <= GameSettings.rateGenBarriersEndless)
				{
					genBarrier (GameSettings.numBarriersEndless);
				}
			}
				
			genBalls (GameSettings.initBallsEachRound);
		}
		
		if (isReporting)
		{
			Debug.Log (levelTracking.report());
			isReporting = false;
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void initPool()
	{
		pool.init ();
		pool.addDataType("ball", size * size, DataPool.AccessType.GetAnyNonActive);
		pool.addDataType("panelBlock", size * size, DataPool.AccessType.GetAnyNonActive);
		pool.addDataType("bonus", 20, DataPool.AccessType.GetAnyNonActive);
		pool.addDataType("boss1", 1, DataPool.AccessType.GetAnyNonActive);
		pool.addDataType("barrier", 15, DataPool.AccessType.GetAnyNonActive);
		pool.addDataType("preBall", 10, DataPool.AccessType.GetAnyNonActive);
		pool.addDataType("noEffect", size * size, DataPool.AccessType.RollRollRoll);
		pool.addDataType("kiemBac", 4,DataPool.AccessType.RollRollRoll );
		pool.addDataType("kiemVang",4,DataPool.AccessType.RollRollRoll);
		pool.addDataType("kiemNiji",4,DataPool.AccessType.RollRollRoll);
		
	
	}

	//	Tính hệ số điểm
	public void initScoreRate()
	{
		scoreRateBall[GameSettings.minimum2Score] = 1;
		for (int i = GameSettings.minimum2Score + 1; i <= size * size; i++)
			scoreRateBall[i] = scoreRateBall[i - 1] * GameSettings.scoreRateBall;
	}

	//	Vẽ bàn cờ ban đầu
	void initPanelBoard()
	{
		for (int i = 0; i < size; i++) 
			for (int j = 0; j < size; j++) 
		{
			blocks[i, j] = nullVector2;
			panelBlocks[i, j] = pool.allocateData("panelBlock");
			pool.getData(panelBlocks[i, j]).transform.localPosition = game2Display(i, j);
			pool.getData(panelBlocks[i, j]).gameObject.SetActive(true);
			pool.getData(panelBlocks[i, j]).GetComponent<PanelBlock>().xCord = i;
			pool.getData(panelBlocks[i, j]).GetComponent<PanelBlock>().yCord = j;
		}
	}

	void initPreBalls()
	{
		for (int i = 0; i < size; i++)
			for (int j = 0; j < size; j++)
				preBalls[i, j] = nullVector2;
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	void drawBlock(string type, int x, int y, int color, int flag, bool tracking)
	{
		if (map[x, y] == 0)
		{

			blocks[x, y] = pool.allocateData(type);
			pool.getData(blocks[x, y]).gameObject.SetActive(true);
			curCount++;
			map[x, y] = color;
			if (type == "ball")
			{
				pool.getData(blocks[x, y]).GetComponent<Renderer>().material.mainTexture = Camera.main.GetComponent<GameManager>().textureBall[color];
				if (tracking)
					levelTracking.addBall(color);
			}
			else if (type == "bonus")
			{
			    pool.getData(blocks[x, y]).GetComponent<Renderer>().material.mainTexture = Camera.main.GetComponent<GameManager>().textureBonus[color];
				//Debug.Log("COLOR  "+ color);
				switch(color)
				{
				case 1:
					pool.getData(blocks[x, y]).transform.Find("Ps1").GetComponent<ParticleSystem>().Stop();
					pool.getData(blocks[x, y]).transform.Find("Ps2").GetComponent<ParticleSystem>().Stop();
					break;
				case 2:

					pool.getData(blocks[x, y]).transform.Find("Ps1").GetComponent<ParticleSystem>().startColor = new Color(1,1,1);
					pool.getData(blocks[x, y]).transform.Find("Ps1").GetComponent<ParticleSystem>().Play();
					pool.getData(blocks[x, y]).transform.Find("Ps2").GetComponent<ParticleSystem>().Stop();
					break;
				case 3:

					pool.getData(blocks[x, y]).transform.Find("Ps1").GetComponent<ParticleSystem>().startColor = new Color(1,248f/255f,0);
					pool.getData(blocks[x, y]).transform.Find("Ps1").GetComponent<ParticleSystem>().Play();
					pool.getData(blocks[x, y]).transform.Find("Ps2").GetComponent<ParticleSystem>().Stop();
					break;
				case 4:
					pool.getData(blocks[x, y]).transform.Find("Ps1").GetComponent<ParticleSystem>().Stop();
					pool.getData(blocks[x, y]).transform.Find("Ps2").GetComponent<ParticleSystem>().Play();

					break;
				}
				if (tracking)
					levelTracking.addBonus(color);
			}
			else if (type == "boss1")
			{
				if (tracking)
					levelTracking.addBoss(1);
			}
			    
			pool.getData(blocks[x, y]).transform.localPosition = game2Display(x, y);
		
			
			if (flag == 1)
				pool.getData(blocks[x, y]).GetComponent<Block>().xuatHien();
			//pool.getData(blocks[x, y]).GetComponent<Block>().idle();
			//pool.getData(blocks[x, y]).animation.Play("xuatHien");
		}
	}

	void destroyBlock (int x, int y, int flag, bool tracking)// flag la 0 thi xoa binh thuong neu la 1 thi xoa co animation
	{
    	if (map[x, y] != 0)
    	{
      		curCount--;  
      		if (tracking)
      		{
				if (pool.getDataType(blocks[x, y]) == "ball")
				{
					levelTracking.desBall(map[x, y]);
				}
				else if (pool.getDataType(blocks[x, y]) == "bonus")
				{
					levelTracking.desBonus(map[x, y]);
				}
				else if (pool.getDataType(blocks[x, y]).IndexOf("boss") != -1)
				{
					levelTracking.desBoss(map[x, y]);
				}
			}
			map[x, y] = 0;
			if (flag == 1)
				pool.getData(blocks[x, y]).GetComponent<Block>().explode();
			else
			{
				// TO DO: bao gio co animation thi sua thanh SetActive(false);
				pool.getData(blocks[x, y]).GetComponent<Block>().setActiveFalse();
			}
			blocks[x, y] = pool.releaseData(blocks[x, y]);
		}
	}
	
	void drawPreBall (int x, int y, int color, int displayFlag)
	{
		if (displayFlag == 0)
			return;
		preBalls[x, y] = pool.allocateData("preBall");
		//Debug.Log ("x:" + x + ", y:" + y + ", status:" + preBalls[x, y]);
		pool.getData(preBalls[x, y]).transform.localPosition = game2Display(x, y) + new Vector3(0, 0.1f, 0);
		pool.getData(preBalls[x, y]).gameObject.SetActive(true);
		pool.getData(preBalls[x, y]).GetComponent<Renderer>().material.mainTexture = Camera.main.GetComponent<GameManager>().texturePreBall[color];
	}
	
	void destroyPreBall (int x, int y)
	{
		pool.getData(preBalls[x, y]).gameObject.SetActive(false);
		preBalls[x, y] = pool.releaseData(preBalls[x, y]);
	}

	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	Vector3 getFreeMapSpot ()
	{
		if (curCount >= size * size)
			return (new Vector3(-1, -1, -1));
			
		int x, y;
		x = Random.Range(0, size);
		y = Random.Range(0, size);
		while (map[x, y] != 0)
		{
			x = Random.Range(0, size);
			y = Random.Range(0, size);
		}
		return new Vector3(x, y, Random.Range(1, numColor + 1));
	}
	
	Vector3 getFreePreSpot ()
	{
		if (curCount >= size * size)
			return (new Vector3(-1, -1, -1));
		
		int x, y;
		x = Random.Range(0, size);
		y = Random.Range(0, size);
		while (map[x, y] != 0 || preBalls[x, y] != nullVector2)
		{
			x = Random.Range(0, size);
			y = Random.Range(0, size);
		}
		return new Vector3(x, y, Random.Range(1, numColor + 1));
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//	Sinh trước 1 số cái :v
	void genPreBalls (int count, int oldGenNum)
	{
		int gen = Mathf.Min(count, maxCount - curCount - oldGenNum);		
		for (int i = 0; i < gen; i++) 
		{
			genList[oldGenNum + i] = getFreePreSpot();
			drawPreBall((int)genList[oldGenNum + i].x, (int)genList[oldGenNum + i].y, (int)genList[oldGenNum + i].z, 1);
		}
		genNum = oldGenNum + gen;
	}

	void checkPreGenBalls ()
	{
		int oldGenNum = 0;
		for (int i = 0; i < genNum; i++)
		{
			if (map[(int)genList[i].x, (int)genList[i].y] != 0)
			{
				destroyPreBall((int)genList[i].x, (int)genList[i].y);
			}
			else
			{
				genList[oldGenNum] = genList[i];
				oldGenNum++;
			}
		}
		
		genPreBalls(genNum - oldGenNum, oldGenNum);
	}

	//	Thêm những cái đã tạo từ preGenBalls đồng thời preGen thêm count cái nữa
	void genBalls (int count)
	{
		checkPreGenBalls();
		
		for (int i = 0; i < genNum; i++)
		{	
			drawBlock("ball", (int)genList[i].x, (int)genList[i].y, (int)genList[i].z, 1, true);
			destroyPreBall((int)genList[i].x, (int)genList[i].y);
		}
			
		for (int i = 0; i < genNum; i++)
			checkScore ((int)genList[i].x, (int)genList[i].y);
		genPreBalls(count, 0);
		isGenerating = false;
		//runTest ();
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//	Thêm mấy quả bonus vào board :v
	void genBonus (int x, int y, int point)
	{
		levelTracking.updateScoreType(point);
		if (gameMode == GAMEMODE.CLASSIC)
			return;
		if(point == 4 && gameMode != GAMEMODE.STORY)
			return;
		for (int i = GameSettings.numBonusLevels - 1; i >= 0; i--)
			if (point >= GameSettings.bonusLevels[i])
			{
				drawBlock ("bonus", x, y, i + 1, 1, true);
				checkScore(x, y);
				break;
			}
	}
	
	void genBoss (int x, int y, int type, int health)
	{
		drawBlock ("boss" + type, x, y, 1, 1, true);
		bossX = x;
		bossY = y;
		bossMaxHealth = health;
		bossCurHeatth = bossMaxHealth;
	}
	
	void genEndlessCheckPoint ()
	{
		Vector2 v;
		if (genNum > 0)
		{
			genNum--;
			v = genList[genNum];
			destroyPreBall((int)genList[genNum].x, (int)genList[genNum].y);
		}
		else 
		{
			v = getFreeMapSpot();
		}
		
		int x = (int)v.x;
		int y = (int)v.y;
		Debug.Log ("Special: x:" + x + ", y:" + y);
		drawBlock ("ball", x, y, Random.Range(1, numColor + 1), 1, true);
		//genBoss (x, y, "boss1", 1);
		
		// TO DO: Bao giờ làm xong nhấp nháy thì sửa chỗ này trong Block.cs
		pool.getData (blocks[x, y]).GetComponent<Block>().toggleEndlessCheck();
		
		endlessX = x;
		endlessY = y;
		curExplodeTurn = 0;
		curGenTurn = 0;
	}
	
	void genBarrier (int x, int y)
	{
		drawBlock ("barrier", x, y, 1, 0, true);
	}
	
	void genBarrier (int count)
	{
		Vector2 v;
		int gen = Mathf.Min(count, maxCount - curCount);
		for (int i = 0; i < gen; i++) 
		{
			v = getFreeMapSpot();
			drawBlock("barrier", (int)v.x, (int)v.y, 1, 0, true);
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//	Select 1 em - Đơn giản là 1 cái state machine
	public void click (int x, int y)
	{
		if (isMoving)
			return;
		
		string type;
		if (map[x, y] == 0)
			type = "panelBlock";
		else
			type = pool.getDataType (blocks[x, y]);	
			
		oldSelectedX = newSelectedX;
		oldSelectedY = newSelectedY;
		
		if (selecting == "panelBlock")
		{
			if (type == "panelBlock")
			{	
			}
			else if (type == "ball")
			{
				selecting = "ball";
				newSelectedX = x;
				newSelectedY = y;
				pool.getData(blocks[x, y]).GetComponent<Block>().select();
			}
			else if (type == "bonus")
			{
				selecting = "bonus";
				newSelectedX = x;
				newSelectedY = y;
				pool.getData(blocks[x, y]).GetComponent<Block>().select();
			}
		}
		else if (selecting == "ball")
		{
			if (type == "panelBlock")
			{
				if (able2Move(oldSelectedX, oldSelectedY, x, y, false))
				{
					selecting = "panelBlock";
					newSelectedX = -1;
					newSelectedY = -1;
					move(oldSelectedX, oldSelectedY, x, y, false);
				}
			}
			else if (type == "ball")
			{
				if (oldSelectedX != x || oldSelectedY != y)
				{
					selecting = "ball";
					newSelectedX = x;
					newSelectedY = y;
					pool.getData(blocks[oldSelectedX, oldSelectedY]).GetComponent<Block>().unSelect();
					pool.getData(blocks[newSelectedX, newSelectedY]).GetComponent<Block>().select();
				}
				else
				{
					selecting = "panelBlock";
					newSelectedX = -1;
					newSelectedY = -1;
					pool.getData(blocks[oldSelectedX, oldSelectedY]).GetComponent<Block>().unSelect();
				}
			}
			else if (type == "bonus")
			{
				selecting = "bonus";
				newSelectedX = x;
				newSelectedY = y;
				pool.getData(blocks[oldSelectedX, oldSelectedY]).GetComponent<Block>().unSelect();
				pool.getData(blocks[newSelectedX, newSelectedY]).GetComponent<Block>().select();
			}
		}
		else if (selecting == "bonus")
		{
			if (type == "panelBlock")
			{
				if (able2Move(oldSelectedX, oldSelectedY, x, y, false))
				{
					selecting = "panelBlock";
					newSelectedX = -1;
					newSelectedY = -1;
					move(oldSelectedX, oldSelectedY, x, y, false);
				}
			}
			else if (type == "ball")
			{
				if (able2Move(oldSelectedX, oldSelectedY, x, y, true))
				{
					selecting = "panelBlock";
					newSelectedX = -1;
					newSelectedY = -1;
					move(oldSelectedX, oldSelectedY, x, y, true);
				}
			}
			else if (type == "bonus")
			{
				if (oldSelectedX != x || oldSelectedY != y)
				{
					if (able2Move(oldSelectedX, oldSelectedY, x, y, true))
					{
						selecting = "panelBlock";
						newSelectedX = -1;
						newSelectedY = -1;
						move(oldSelectedX, oldSelectedY, x, y, true);
					}
				}
				else
				{
					selecting = "panelBlock";
					newSelectedX = -1;
					newSelectedY = -1;
					pool.getData(blocks[oldSelectedX, oldSelectedY]).GetComponent<Block>().unSelect();
				}
			}
			else if (type.Contains("boss"))
			{
				if (able2Move(oldSelectedX, oldSelectedY, x, y, true))
				{
					selecting = "panelBlock";
					newSelectedX = -1;
					newSelectedY = -1;
					move(oldSelectedX, oldSelectedY, x, y, true);
				}
			}
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//	Di chuyển 1 con cờ
	public void move(int x1, int y1, int x2, int y2, bool isExploding)
	{
		isMoving = true;
		pool.getData(blocks[x1, y1]).GetComponent<Animation>().CrossFade("run");
		khoi.transform.parent = pool.getData(blocks[x1, y1]).transform;
		khoi.transform.localPosition = new Vector3(0,1.266867f,0.6575381f);
		khoi.GetComponent<ParticleSystem>().Play();
		pool.getData(blocks[x1, y1]).GetComponent<Block>().walk(x1, y1, x2, y2, moveMap, steps, isExploding);
	}

	//	Check tọa độ nằm trong ô bàn cờ
	public bool OK(int x, int y)
	{
		return (0 <= x) && (x < size) && (0 <= y) && (y < size);
	}

	public bool sameTypeBlock (int x1, int y1, int x2, int y2)
	{
		if (pool.getDataType(blocks[x1, y1]) == "")
			return false;
		if (pool.getDataType(blocks[x2, y2]) == "")
			return false;
			
		return (map[x1, y1] == map[x2, y2]) && (pool.getDataType(blocks[x1, y1]) == pool.getDataType(blocks[x2, y2]));
	}

	//	Check điều kiện ghi điểm
	public void checkScore(int xx, int yy)
	{
		if (map[xx, yy] == 0)
			return;
		int count;
		int x, y;
		int[] sx = new int[4];
		int[] sy = new int[4];
		int[] sc = new int[4];
		int[] si = new int[4];
		int ss = 0;
		int total = 0;
		
		for (int i = 0; i < 4; i++) 
		{
			x = xx; 
			y = yy;
			count = 1;
			while (OK (x + d8x[i], y + d8y[i]) && sameTypeBlock(x, y, x + d8x[i], y + d8y[i]))
			{
				count++;
				x += d8x[i];
				y += d8y[i];
			}

			x = xx;
			y = yy;
			while (OK (x - d8x[i], y - d8y[i]) && sameTypeBlock(x, y, x - d8x[i], y - d8y[i]))
			{
				count++;
				x -= d8x[i];
				y -= d8y[i];
			}

			if (count >= GameSettings.minimum2Score)
			{
				total += count;
				sx[ss] = x;
				sy[ss] = y;
				sc[ss] = count;
				si[ss] = i;
				ss++;
				isGenerating = false;
			}
		}
		if (total < GameSettings.minimum2Score)
			return;
		danhGiaBox.GetComponent<danhGia>().hienDanhGia(total);
		if (pool.getDataType(blocks[xx, yy]) == "ball")
		{
			updateScore (scoreRateBall[total] * GameSettings.scoreBase);
		}
		else if (pool.getDataType(blocks[xx, yy]) == "bonus")
			//	TO DO : Tìm công thức tính điểm cho bonus, tạm thời để như ball bình thường
			updateScore (scoreRateBall[total] * GameSettings.scoreBase);
			
		for (int i = 0; i < ss; i++)
			scoreBlock(sx[i], sy[i], sc[i], si[i]);	
		genBonus(xx, yy, total);	
		/*if(total > 3)
		{
			Camera.main.audio.PlayOneShot(ghiDiemEffect[(total - 4) % 4]);

		}*/
		                                              
		if (curCount == 0)	
			isGenerating = true;
	}

	//	Ghi điểm
	void scoreBlock (int x, int y, int count, int type)
	{
		
		for (int i = 0; i < count; i++) 
		{
			destroyBlock(x, y, 1, true);
			x += d8x[type];
			y += d8y[type];
		}
	}

	//	BFS xem từ (x1, y1) có đi tới (x2, y2) được không
	int top = 0; 
	int bot = 0;

	bool able2Move (int x1, int y1, int x2, int y2, bool isExploding)
	{
		for (int i = 0; i < size; i++)
			for (int j = 0; j < size; j++) 
				if (map[i, j] == 0)
					avail [i, j] = true;
				else
					avail [i, j] = false;
		int x, y;
		top = 0; 
		bot = 0;

		queueX [top] = x1;
		queueY [top] = y1;
		top++;
		while (bot < top)
		{
			x = queueX[bot];
			y = queueY[bot];

			if ((x == x2) && (y == y2))
			{
				traceSteps(bot);
				return true;
			}

			for (int i = 0; i < 4; i++)
				if (OK(x + d4x[i], y + d4y[i]))
					if (avail[x + d4x[i], y + d4y[i]] || ((x + d4x[i] == x2) && (y + d4y[i] == y2) && isExploding))
					{	
						queueX[top] = x + d4x[i];
						queueY[top] = y + d4y[i];
						trace[top] = bot;
						moves[top] = i;
						avail[x + d4x[i], y + d4y[i]] = false;
						top++;
					}
			bot++;
		}
		return false;
	}

	void traceSteps (int pos)
	{
		steps = 0;
		while (pos > 0)
		{
			moveMap[steps] = moves[pos];
			pos = trace[pos];
			steps++;
		}
	}
	
	public void finishMoving(int x1, int y1, int x2, int y2, bool isExploding)
	{
		string type = pool.getDataType(blocks[x1, y1]);
		levelTracking.nextTurn();
		isReporting = true;
		if (!isExploding)
		{
			drawBlock(type, x2, y2, map[x1, y1], 0, false);
			destroyBlock(x1, y1, 0, false);
			pool.getData(blocks[x2, y2]).GetComponent<Block>().idle ();
			isMoving = false;
			isGenerating = true;
			if (type == "ball" || type == "bonus")
				checkScore(x2, y2);
		}
		else
		{
			int radius = map[x1, y1];
			destroyBlock(x1, y1, 0, true);
			int count = explode(x2, y2, radius);
			if (GameSettings.ableExplodingToGainPoint)
				updateScore (scoreRateBall[count + GameSettings.minimum2Score - 1] * GameSettings.scoreBase);
			isMoving = false;
		}
	}
	
	//	Nổ 1 quả tại x y với bán kính radius
	public int explode (int x, int y, int radius)
	{
		int res = 0;
		GameObject objTemp; 
		switch(radius)
		{
		case 2:
			objTemp = pool.getData("kiemBac");
			objTemp.transform.position = game2Display(x,y);
			objTemp.GetComponent<Animation>().Play("bomBac");
			break;
		case 3:
			objTemp = pool.getData("kiemVang");
			objTemp.transform.position = game2Display(x,y);
			objTemp.GetComponent<Animation>().Play("bomVang");
			break;
		case 4:
			objTemp = pool.getData("kiemNiji");
			objTemp.transform.position = game2Display(x,y);
			objTemp.GetComponent<Animation>().Play("bomNiji");
			break;
		}
	
		for (int i = -radius; i <= radius; i++)
			for (int j = -radius; j <= radius; j++)
				if (OK(x + i, y + j) && (Mathf.Abs(i) + Mathf.Abs(j) < radius))
				{
					
					if (map[x + i, y + j] != 0)
					{
						if (pool.getDataType(blocks[x + i, y + j]) == "ball")
						{
							destroyBlock(x + i, y + j, 1, true);
							res++;
						}
						else if (pool.getDataType(blocks[x + i, y + j]) == "bonus")
						{
							int r = map[x + i, y + j];
							//	TO DO: bao giờ làm xong animation nổ thì sửa lại 1
							destroyBlock(x + i, y + j, 0, true);
							res = res + explode (x + i, y + j, r);
						}	
						else if (pool.getDataType(blocks[x + i, y + j]) == "boss1")
						{
							pool.getData( blocks[x + i, y + j]).GetComponent<Animation>().Play("trungBom");
							damageBoss(radius);
							res++;
						}
					}
					else
					{
						objTemp = pool.getData("noEffect");
						objTemp.transform.position  = game2Display(x + i, y + j) + new Vector3(0,0.6f,-0.27216f);
						objTemp.GetComponent<ParticleSystem>().Play();
					//pool.getData("noEffect", x + i, y + j).transform.localPosition = 
					//	pool.getData("noEffect", x + i, y + j).GetComponent<ParticleSystem>().Play();	
					}
					
			}
		return res;				
	}
	
	public void updateScore (float increment)
	{ 
		if (increment == 0)
			return;
		levelTracking.updateScore(increment);
		score += increment;
		textScore.GetComponent<colorText>().setText(((int)score).ToString());
	}
	
	public void damageBoss(int damage)
	{
		bossCurHeatth -= damage;
		if (bossCurHeatth <= 0)
		{
			//	TO DO: cóó nhiều boss thì bỏ cái này
			if (gameMode == GAMEMODE.STORY)
			{
				startLevel(gameMode);
				return;
			}
			bossCurHeatth = 0;
			//	TO DO: co animation thi doi 0 thanh 1
			destroyBlock(bossX, bossY, 0, true);
		}
		if (gameMode != GAMEMODE.ENDLESS)
			textBoss.GetComponent<TextMesh>().text = "Boss health: " + ((int)bossCurHeatth).ToString();
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void endlessReset ()
	{
		updateEndlessNumColor();
		curExplodeTurn = 0;
		curGenTurn = 0;
		endlessX = -1;
		endlessY = -1;
	}
	
	public void updateEndlessNumColor ()
	{
		textBoss.GetComponent<TextMesh>().text = "Colors: " + ((int)numColor).ToString();
	}
	
}
