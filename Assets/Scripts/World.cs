using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class World
{
	static float distanceFromOtherHouses = 11.5F;
	
	public static int currentHouses = 0;
	public static List<Vector3> housePositions = new List<Vector3>();
	
	public static bool showPathfindingLines = false;
	public static bool showBadPathfindingLines = false;
	
	public static void StoreNewHouse(Vector3 location)
	{
		housePositions.Add(location);
		currentHouses++;
	}
	
	public static bool CanCreateHouseHere(Vector3 location)
	{
		bool r = true;
		
		RaycastHit hit;
		if(Physics.Raycast(location, -Vector3.up, out hit, 2F))
		{
			if(!hit.transform.gameObject.name.Contains("Stone"))
			{
				if(housePositions.Count > 0)
				{
					foreach(Vector3 pos in housePositions)
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
				Debug.Log("Cannot build on " + hit.transform.gameObject.name);
			}
		}
		else
		{
			r = false;
		}
		
		return r;
	}
	
}
