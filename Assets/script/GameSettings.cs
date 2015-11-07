using UnityEngine;
using System.Collections;

/*
//	general : 5 viên ban đầu, 3 viên sau mỗi nước, 4 viên nổ
//	CLASSIC MODE: xuất 5 màu, ko bomb
//	ENDLESS MODE: xuất 3 màu, có bomb, sau mỗi 10 lượt có khả năng bị tăng thêm 1 màu
//	STORY MODE: xuất 5 màu, có bomb, boss rầm rầm :v
//	CHALLENGE MODE: chưa nghiên cứu :v
*/

public enum GAMEMODE 
{
	CLASSIC = 1,
	ENDLESS = 2,
	STORY = 3,
	CHALLENGE = 4
}

public class GameSettings
{
	public static int size = 7;

	public static float scoreBase = 32;
	public static float scoreRateBall = 1.5f;
	
	public static int[] bonusLevels = {4, 5, 6, 7};
	public static int numBonusLevels = 4;

	public static int minimum2Score = 4; //Balance
	public static int initBallsEachRound = 4; //Balance
	public static int initBalls =8; //Balance
	public static int initColorNormal = 5; //Balance
	
	public static int initColorEndless = 3; //Balance
	public static int maxGenTurnEndless = 6; //Balance
	public static int maxExplodeTurnEndless = 3; //Balance
	public static float rateGenBarriersEndless = 0.2f;
	public static int numBarriersEndless = 1;
	
	/////////////////////////////////////////
	public static bool ableExplodingToGainPoint = true;
	public static GAMEMODE TESTMODE = GAMEMODE.CHALLENGE;
}