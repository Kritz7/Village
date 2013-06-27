using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class World
{
	static float distanceFromOtherHouses = 11.5F;
	
	public static int currentHouses = 0;
	public static List<Vector3> housePositions = new List<Vector3>();
	
	public static List<Vector3> objectPositions = new List<Vector3>();
	public static List<string> objectNames = new List<string>();
	
	public static bool showPathfindingLines = false;
	public static bool showBadPathfindingLines = false;
	
	public static void StoreNewHouse(Vector3 location)
	{
		housePositions.Add(location);
		currentHouses++;
	}
	
	public static void StoreNewObject(Vector3 location, string name)
	{
		objectPositions.Add(location);
		objectNames.Add(name);
	}
	
	public static bool CanCreateHouseHere(Vector3 location)
	{
		bool r = true;
		
		RaycastHit hit;
		if(Physics.Raycast(location, -Vector3.up, out hit, 2F))
		{
			if(housePositions.Count > 0)
			{
				foreach(Vector3 pos in housePositions)
				{
					if((location - pos).magnitude < distanceFromOtherHouses)
						r = false;
				}
				
				foreach(Vector3 pos in objectPositions)
				{
					if((location - pos).magnitude < distanceFromOtherHouses)
						r = false;
				}
			}
			else r = true;
		}
		else
		{
			r = false;
		}
		
		return r;
	}
	
}
