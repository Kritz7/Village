using UnityEngine;
using System.Collections;

public class Creation : MonoBehaviour {
	
	int[] pathCost = new int[] { 1 };
	int[] houseCost = new int[] { 4 };
	
	Gathering playerGathering;
	
	TerrainData startTerrain;
	
	// Use this for initialization
	void Start () {
		playerGathering = GameObject.Find("Player").GetComponent<Gathering>();
		
		startTerrain = Terrain.activeTerrain.terrainData;
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			if(World.CanCreateHouseHere(transform.position))
			{
				if(playerGathering.rock >= houseCost[0])
				{
					GameObject newHouse = Instantiate(Resources.Load("House"), transform.position - (Vector3.up * 7), transform.rotation) as GameObject;
					Spawn h = newHouse.GetComponent<Spawn>();
					h.goalPosition = new Vector3(transform.position.x, 0 + newHouse.transform.localScale.y, transform.position.z);
					h.homeID = World.currentHouses+1;
					
					World.StoreNewHouse(transform.position);
					
					playerGathering.rock -= houseCost[0];
				}
			}
		}		
		
		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			if(playerGathering.rock >= pathCost[0])
			{
				GameObject newObj = Instantiate(Resources.Load("Stone"), transform.position - (Vector3.up * 1.25F), transform.rotation) as GameObject;
				Spawn o = newObj.GetComponent<Spawn>();
				o.goalPosition = new Vector3(transform.position.x, 0 + newObj.transform.localScale.y, transform.position.z);
				
				playerGathering.rock -= pathCost[0];
			}
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			/* stolen from http://answers.unity3d.com/questions/11093/modifying-terrain-height-under-a-gameobject-at-run.html */
			
			Terrain terr = Terrain.activeTerrain;
			int hmWidth = terr.terrainData.heightmapWidth;
			int hmHeight = terr.terrainData.heightmapHeight;
			
			int posXInTerrain; // position of the game object in terrain width (x axis)
			int posYInTerrain; // position of the game object in terrain height (z axis)
			 
			int size = 50; // the diameter of terrain portion that will raise under the game object
			float desiredHeight = 0.005f; // the height we want that portion of terrain to be
			
			Vector3 tempCoord = (transform.position - terr.gameObject.transform.position);
			Vector3 coord;
			coord.x = tempCoord.x / terr.terrainData.size.x;
			coord.y = tempCoord.y / terr.terrainData.size.y;
			coord.z = tempCoord.z / terr.terrainData.size.z;
			
			posXInTerrain = (int) (coord.x * hmWidth);
			posYInTerrain = (int) (coord.z * hmHeight);
			 
				// we set an offset so that all the raising terrain is under this game object
			int offset = size / 2;
			 
			// get the heights of the terrain under this game object
			float[,] heights = terr.terrainData.GetHeights(posXInTerrain-offset,posYInTerrain-offset,size,size);
			 
			// we set each sample of the terrain in the size to the desired height
			for (int i=0; i<size; i++)
			{
				for (int j=0; j<size; j++)
				{
					heights[i,j] = desiredHeight;
				}
			}
			// go raising the terrain slowly
			desiredHeight += Time.deltaTime;
			 
			// set the new height
			terr.terrainData.SetHeights(posXInTerrain-offset,posYInTerrain-offset,heights);
 
			
		}
	}
	
    void OnApplicationQuit()
	{		
		Terrain terr = Terrain.activeTerrain;
		int hmWidth = terr.terrainData.heightmapWidth;
		int hmHeight = terr.terrainData.heightmapHeight;
		
		int posXInTerrain; // position of the game object in terrain width (x axis)
		int posYInTerrain; // position of the game object in terrain height (z axis)
		 
		int size = 510; // the diameter of terrain portion that will raise under the game object
		float desiredHeight = 0.01f; // the height we want that portion of terrain to be
		
		/*
		Vector3 tempCoord = (transform.position - terr.gameObject.transform.position);
		Vector3 coord;
		coord.x = tempCoord.x / terr.terrainData.size.x;
		coord.y = tempCoord.y / terr.terrainData.size.y;
		coord.z = tempCoord.z / terr.terrainData.size.z;
		*/
		
		posXInTerrain = (int) (hmWidth/2);
		posYInTerrain = (int) (hmHeight/2);
		 
		// we set an offset so that all the raising terrain is under this game object
		int offset = size / 2;
		 
		// get the heights of the terrain under this game object
		float[,] heights = terr.terrainData.GetHeights(posXInTerrain-offset,posYInTerrain-offset,size,size);
		 
		// we set each sample of the terrain in the size to the desired height
		for (int i=0; i<size; i++)
		{
			for (int j=0; j<size; j++)
			{
				heights[i,j] = desiredHeight;
			}
		}
		 
		// go raising the terrain slowly
		desiredHeight += Time.deltaTime;
		 
		// set the new height
		terr.terrainData.SetHeights(posXInTerrain-offset,posYInTerrain-offset,heights);
    }
}
