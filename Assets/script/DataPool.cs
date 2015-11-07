using UnityEngine;
using System.Collections;

public class DataPool : MonoBehaviour {
	public enum AccessType {GetAnyNonActive, Get1AxisData, Get2AxisData, RollRollRoll};

	public GameObject[] samples;
	private int MAX_TYPE = 101;
	private int n;
	private int[] size;
	private string[] names;
	private GameObject[][] pool;
	private bool[][] avai;
	private AccessType[] accessType;
	private int[] location;
	private int dimension;

	public void init()
	{
		size = new int[MAX_TYPE];
		names = new string[MAX_TYPE];
		pool = new GameObject[MAX_TYPE][];
		avai = new bool[MAX_TYPE][];
		accessType = new AccessType[MAX_TYPE];
		location = new int[MAX_TYPE];
		n = 0;
		dimension = GameSettings.size;
	}
	
	/// <summary>
	/// Thêm 1 loại data 
	/// </summary>
	/// <param name="newName">New name.</param>
	/// <param name="newSize">New size.</param> Max size của data
	/// <param name="type">Type.</param> Enum để biết đang dùng loại pool nào
	///		GetAnyNonActive
	
	public void addDataType (string newName, int newSize, AccessType type)
	{
		bool check = false;
		int index = -1;
		for (int i = 0; i < samples.GetLength(0); i++)
			if (newName == samples[i].name)
			{
				check = true;		
				index = i;
				break;
			}
		if (!check)
		{
			Debug.Log ("prefab type \"" + newName + "\" couldn't be found");
			return;
		}
			
		names [n] = newName;
		size [n] = newSize;
		if (type == AccessType.GetAnyNonActive)
		{
			pool [n] = new GameObject[newSize];	
		}
		else if (type == AccessType.Get1AxisData)
		{
			pool [n] = new GameObject[dimension * dimension + 1];	
		}
		else if (type == AccessType.Get2AxisData)
		{
			pool [n] = new GameObject[dimension * dimension + 1];	
		}
		else if (type == AccessType.RollRollRoll)
		{
			pool [n] = new GameObject[newSize];
		}
		
		avai [n] = new bool[newSize];
		accessType[n] = type;
		location [n] = 0;
		for (int i = 0; i < newSize; i++)
		{
			avai [n][i] = true;
			pool[n][i] = (GameObject)Instantiate(samples[index], new Vector3(-100, -100, -100), samples[index].transform.rotation);
			pool[n][i].SetActive(false);
		}
		n++;
	}
	
	private int name2Index (string dataName)
	{
		for (int i = 0; i < n; i++)
			if (names[i] == dataName)
				return i;
		return -1;
	}
	/// <summary>
	/// Allocate a position on pool for a piece of data
	/// </summary>
	/// <returns>The data.</returns>
	/// <param name="dataName">Data name.</param>
	public Vector2 allocateData (string dataName)
	{
		int index = name2Index (dataName);
		if (index == -1)
			return new Vector2(-1, -1);
		for (int i = 0; i < size[index]; i++)
			if (avai[index][i]) 
			{
				avai[index][i] = false;
				return new Vector2(index, i);
			}
		return new Vector2(-1, -1);
	}
	
	//Get data for GetAnyNonActive
	public GameObject getData (Vector2 index)
	{
		return pool[(int)index.x][(int)index.y];
	}
	//	Get data for Get1AxisData
	public GameObject getData (string dataName, int x)
	{
		int index = name2Index (dataName);
		if (index == -1)
			return null;
		avai[index][x] = false;
		pool[index][x].SetActive(true);
		return pool[index][x];
	}
	
	//	Get data for Get2AxisData
	public GameObject getData (string dataName, int x, int y)
	{
		int index = name2Index (dataName);
		if (index == -1)
			return null;
		int pos = xyTox (x, y);
		avai[index][pos] = false;
		pool[index][pos].SetActive(true);
		return pool[index][pos];
	}
	
	public GameObject getData (string dataName)
	{
		int index = name2Index (dataName);
		if (index == -1)
			return null;
		if (location[index] == size[index])
			location[index] = 0;
		pool[index][location[index]].SetActive(true);			
		
		location[index]++;
		return pool[index][location[index] - 1];
	}
	
	//	Finish a round of get data
	public void finishContinuoulyGetData (string dataName)
	{
		int index = name2Index (dataName);
		if (index == -1)
			return;
		location[index] = 0;
	}

	public Vector2 releaseData (Vector2 index)
	{
		//pool[(int)index.x][(int)index.y].SetActive(false);
		avai[(int)index.x][(int)index.y] = true;
		return new Vector2(-1, -1);
	}
	
	public string getDataType (Vector2 index)
	{
		if (index.x == -1)
			return "";
		return names[(int)index.x];
	}
	
	private int xyTox (int x, int y)
	{
		return x * dimension + y;
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
}
