using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavigationScript : MonoBehaviour {

	public int index;
	public List<int> edges;
	float maxEdgeLength = 50F;
	
	public bool insideBuilding = false;
	public bool resourceNode = false;
	
	// IDs
	public int parentHomeID = -1;
	public int parentBasketID = -1;
	public int ownedByVillager = -1;
	public int parentResourceID = -1;
	
	GraphScript graph;
	
	// Use this for initialization
	void Start () {
		graph = GameObject.Find("NavigationGraph").GetComponent<GraphScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if(graph.nodes.IndexOf(this) != index)
		{
			index = graph.nodes.IndexOf(this);
		}
		
	}
	
	public void CalcEdges()
	{
		GraphScript graph = transform.parent.GetComponent<GraphScript>();
		edges = new List<int>();
		
		int layerMask = ~((1<<8) | (1<<9) | (1<<10));
		
		foreach(NavigationScript node in graph.nodes)
		{
			if(graph.nodes.IndexOf(node)!=graph.nodes.IndexOf(this))
			{
				float dist = (node.transform.position - transform.position).magnitude;

				if(dist<maxEdgeLength)
				{
					RaycastHit hit;
					if(!Physics.SphereCast(transform.position, 0.65F, (node.transform.position - transform.position).normalized, out hit, (node.transform.position - transform.position).magnitude, layerMask))
					{
						edges.Add(graph.nodes.IndexOf(node));	
					}
				}
			}
		}
	}
	
	public void VillagerNowOwns(FlockingBehaviourRedux villager)
	{
		villager.homeID = parentHomeID;
		ownedByVillager = villager.villagerID;
	}
}
