using UnityEngine;
using System.Collections;

public class LevelTracking : MonoBehaviour {
	private int MAX_TYPE = 101;
	private int MAX_SHOW = 10;
	
	public float score;
	public int[] scoreType;
	public int ball;
	public int[] ballType;
	public int[] ballDestroyedType;
	public int[] ballGeneratedType;
	public int bonus;
	public int[] bonusType;
	public int[] bonusDestroyedType;
	public int[] bonusGeneratedType;
	public int boss;
	public int[] bossType;
	public int[] bossDestroyedType;
	public int[] bossGeneratedType;
	public int turn;
	
	public LevelTracking ()
	{
		initTracking();
	}
	
	public void initTracking ()
	{
		score = 0f;
		scoreType = new int[MAX_TYPE];
		ball = 0;
		ballType = new int[MAX_TYPE];
		ballGeneratedType = new int[MAX_TYPE];
		ballDestroyedType = new int[MAX_TYPE];
		bonus = 0;
		bonusType = new int[MAX_TYPE];
		bonusGeneratedType = new int[MAX_TYPE];
		bonusDestroyedType = new int[MAX_TYPE];
		boss = 0;
		bossType = new int[MAX_TYPE];
		bossGeneratedType = new int[MAX_TYPE];
		bossDestroyedType = new int[MAX_TYPE];
		turn = 0;
	}
	
	public void updateScore (float increment)
	{
		score += increment;
	}
	
	public void updateScoreType (int type)
	{
		scoreType[type]++;	
	}
	
	public void nextTurn ()
	{
		turn++;
	}
	
	public void addBall (int type)
	{
		ball++;
		ballType[type]++;
		ballGeneratedType[type]++;
	}
	
	public void desBall (int type)
	{
		ball--;
		ballType[type]--;
		ballDestroyedType[type]++;
	}
	
	public void addBonus (int type)
	{
		bonus++;
		bonusType[type]++;
		bonusGeneratedType[type]++;
	}
	
	public void desBonus (int type)
	{
		bonus--;
		bonusType[type]--;
		bonusDestroyedType[type]++;
	}
	
	public void addBoss (int type)
	{
		boss++;
		bossType[type]++;
		bossGeneratedType[type]++;
	}
	
	public void desBoss (int type)
	{
		boss--;
		bossType[type]--;
		bossDestroyedType[type]++;
	}
	
	public string report ()
	{ 
		string st = "Level Tracking Report \n";
		st = st + "TURN: " + turn + "\n";
		
		st = st + "SCORE: " + score + "\n";
		
		st = st + "  type[";
		for (int i = 0; i < MAX_SHOW; i++)
			st = st + scoreType[i] + ", ";
		st = st + "] \n";
		
		st = st + "BALL: \n";
		st = st + "  total: " + ball + "\n";
		
		st = st + "  type[";
		for (int i = 0; i < MAX_SHOW; i++)
			st = st + ballType[i] + ", ";
		st = st + "] \n";
		
		st = st + "  gen[";
		for (int i = 0; i < MAX_SHOW; i++)
			st = st + ballGeneratedType[i] + ", ";
		st = st + "] \n";
		
		st = st + "  des[";
		for (int i = 0; i < MAX_SHOW; i++)
			st = st + ballDestroyedType[i] + ", ";
		st = st + "] \n";
		
		st = st + "BONUS: \n";
		st = st + "  total: " + bonus + "\n";
		
		st = st + "  type[";
		for (int i = 0; i < MAX_SHOW; i++)
			st = st + bonusType[i] + ", ";
		st = st + "] \n";
		
		st = st + "  gen[";
		for (int i = 0; i < MAX_SHOW; i++)
			st = st + bonusGeneratedType[i] + ", ";
		st = st + "] \n";
		
		st = st + "  des[";
		for (int i = 0; i < MAX_SHOW; i++)
			st = st + bonusDestroyedType[i] + ", ";
		st = st + "] \n";
		
		st = st +"BOSS: \n";
		st = st + "  total: " + boss + "\n";
		
		st = st + "  type[";
		for (int i = 0; i < MAX_SHOW; i++)
			st = st + bossType[i] + ", ";
		st = st + "] \n";
		
		st = st + "  gen[";
		for (int i = 0; i < MAX_SHOW; i++)
			st = st + bossGeneratedType[i] + ", ";
		st = st + "] \n";
		
		st = st + "  des[";
		for (int i = 0; i < MAX_SHOW; i++)
			st = st + bossDestroyedType[i] + ", ";
		st = st + "] \n";
		return st;
	}
}
