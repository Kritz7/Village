using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavigationScript : MonoBehaviour {

	public int index;
	public List<int> edges;
	float maxEdgeLength = 50F;
	
	public bool insideBuilding = false;
	public int parentHomeID = -1;
	public int ownedByVillager = -1;
	
//	List<Vector3> debugStart = new List<Vector3>();
//	List<Vector3> debugEnd = new List<Vector3>();
//	List<bool> debugHit = new List<bool>();
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if(World.showPathfindingLines)
		{
			foreach(Vector3 v in debugStart)
			{
				Color c;
				
				if(debugHit[debugStart.IndexOf(v)] == true) c = Color.green;
				else c = Color.red;
				
				if(c == Color.green || (c == Color.red && World.showBadPathfindingLines))
				{
					Debug.DrawLine(v, debugEnd[debugStart.IndexOf(v)], c);
					Debug.DrawRay(v + transform.up, (debugEnd[debugStart.IndexOf(v)] - v).normalized, Color.blue);
				}
			}
		}
		*/
	}
	
	public void CalcEdges()
	{
		GraphScript graph = transform.parent.GetComponent<GraphScript>();
		edges = new List<int>();
		
		int layerMask = ~((1<<8) | (1<<9) | (1<<10));
		
		foreach(NavigationScript node in graph.nodes)
		{
			if(node.index!=index)
			{
				float dist = (node.transform.position - transform.position).magnitude;
				
				/*
				if(dist<maxEdgeLength && !Physics.Linecast(transform.position, node.transform.position, layerMask))
				{
					edges.Add(node.index);	
				}
				*/
				if(dist<maxEdgeLength)
				{
					RaycastHit hit;
					if(!Physics.SphereCast(transform.position, 0.6F, (node.transform.position - transform.position).normalized, out hit, (node.transform.position - transform.position).magnitude, layerMask))
					{
						edges.Add(node.index);	
					/*	
						debugStart.Add(node.transform.position);
						debugEnd.Add(transform.position);
						debugHit.Add(true);
					}
					else
					{
						debugStart.Add(node.transform.position);
						debugEnd.Add(transform.position);
						debugHit.Add(false);
					*/
					}
				}
			}
		}
	}
	
	public void VillagerNowOwns(FlockingBehaviour villager)
	{
		villager.homeID = parentHomeID;
		ownedByVillager = villager.villagerID;
	}
}
