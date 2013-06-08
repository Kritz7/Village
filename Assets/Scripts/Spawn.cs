using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {
	
	public bool inPosition = false;
	public Vector3 goalPosition;
	public float goalYModifier = 0;
	Vector3 goalModifier;
	
	GraphScript gs;
	
	public enum Structure
	{
		House,
		Path,
		Resource
	}
	public Structure structureType;
	
	public int homeID = -1;
	
	// Use this for initialization
	void Start () {
		
		if(name.Contains("House")) structureType = Structure.House;
		if(name.Contains("Stone")) structureType = Structure.Path;
		
		gs = GameObject.Find("NavigationGraph").GetComponent<GraphScript>();
		goalModifier = new Vector3(0, goalYModifier, 0);
		
		if(collider) collider.enabled = false;
		if(name.Contains("House"))
		{
			foreach(Transform t in transform)
			{
				if(t.collider) t.collider.isTrigger = true;
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!inPosition)
		{
			transform.position = Vector3.Lerp(transform.position, goalPosition + goalModifier, 0.5F * Time.deltaTime);
			
			if((transform.position - (goalPosition + goalModifier)).magnitude < 0.5F)
			{
				if(collider) collider.enabled = true;
				if(name.Contains("House"))
				{
					foreach(Transform t in transform)
					{
						if(t.collider) t.collider.isTrigger = false;
					}
				}
				inPosition = true;
				DetectNewNodes();
			}
		}
	}
	
	void DetectNewNodes()
	{		
		List<Transform> nodes = new List<Transform>();
	//	List<Transform> nodesToDestroy = new List<Transform>();
//		List<NavigationScript> graphNodesToKeep = new List<NavigationScript>();
		GameObject nav = GameObject.Find("NavigationGraph");
		
		
		foreach(Transform t in transform)
		{			
			
			if(t.name.Contains("Node"))
			{
				/* slow removal code
				bool removeThis = false;
				
				foreach(NavigationScript worldNode in gs.nodes)
				{
					if((worldNode.transform.position - t.transform.position).magnitude < 1F)
					{
						if(worldNode.insideBuilding)
						{
							removeThis = true;	
						}
						else if(t.GetComponent<NavigationScript>().insideBuilding)
						{
							nodesToDestroy.Add(worldNode.transform);	
						}
					}
					
					if((transform.position - worldNode.transform.position).magnitude < 10F && name.Contains("House")) nodesToDestroy.Add(worldNode.transform);
				}
				
				if(!removeThis) nodes.Add(t);
				else nodesToDestroy.Add(t);
				*/
				nodes.Add(t);
			}
		}	
					/*
		
		foreach(NavigationScript graphnodes in gs.nodes)
		{
			bool destroy = false;
			
			foreach(Transform ntd in nodesToDestroy)
			{
				if(graphnodes == ntd.GetComponent<NavigationScript>()) destroy = true;
			}
			
			if(!destroy) graphNodesToKeep.Add(graphnodes);
		}
		
		
		gs.nodes = graphNodesToKeep;		
		nodesToDestroy.ForEach(child => Destroy(child.gameObject));
		*/
		
		foreach(Transform n in nodes)
		{			
			n.parent = nav.transform;
			n.GetComponent<NavigationScript>().parentHomeID = homeID;
			gs.AddNode(n.gameObject);
		}
		
		gs.RecalculatePaths();
	}
}
