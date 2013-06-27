using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavigationScript : MonoBehaviour {

	public int index;
	public List<int> edges;
	float maxEdgeLength = 50F;
	
	public bool insideBuilding = false;
	public int parentHomeID = -1;
	public int parentBasketID = -1;
	public int ownedByVillager = -1;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
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

				if(dist<maxEdgeLength)
				{
					RaycastHit hit;
					if(!Physics.SphereCast(transform.position, 0.6F, (node.transform.position - transform.position).normalized, out hit, (node.transform.position - transform.position).magnitude, layerMask))
					{
						edges.Add(node.index);	
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
