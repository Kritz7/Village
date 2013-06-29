using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {
	
	public bool inPosition = false;
	public bool disableCollider = true;
	public Vector3 goalPosition = Vector3.zero;
	public float goalYModifier = 0;
	Vector3 goalModifier;
	
	public List<NavigationScript> createdNodes = new List<NavigationScript>();
	
	GraphScript gs;
	
	public enum Structure
	{
		House,
		Path,
		Resource
	}
	public Structure structureType;
	
	public int homeID = -1;
	public int basketID = -1;
	public int resourceID = -1;
	
	// Use this for initialization
	void Awake () {
		
		if(name.Contains("House")) structureType = Structure.House;
		if(name.Contains("Stone")) structureType = Structure.Path;
		
		gs = GameObject.Find("NavigationGraph").GetComponent<GraphScript>();
		goalModifier = new Vector3(0, goalYModifier, 0);
		
		if(collider && disableCollider) collider.enabled = false;
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
			transform.localPosition = Vector3.Lerp(transform.localPosition, goalPosition + goalModifier, 0.5F * Time.deltaTime);
			
			if((transform.localPosition - (goalPosition + goalModifier)).magnitude < 0.5F)
			{
				if(collider && disableCollider) collider.enabled = true;
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
	
	/// <summary>
	/// Detects new nodes.
	/// WARNING: 
	/// </summary>
	public void DetectNewNodes()
	{		
		List<Transform> nodes = new List<Transform>();
		int layerMask = ~((1<<9) | (1<<10));
		
		foreach(Transform t in transform)
		{			
			if(t.name.Contains("Node"))
			{
				createdNodes.Add(t.GetComponent<NavigationScript>());
				
				RaycastHit hit;
				if(!Physics.SphereCast(t.position, t.lossyScale.y, t.forward, out hit, 0.01F, layerMask))
				{
					nodes.Add(t);
				}
				else
				{
					// print ("Hit " + hit.transform.name + " " + t.localScale.y);	
				}
			}
		}	
		
		foreach(Transform n in nodes)
		{			
			n.parent = gs.transform;
			n.GetComponent<NavigationScript>().parentHomeID = homeID;
			n.GetComponent<NavigationScript>().parentBasketID = basketID;
			n.GetComponent<NavigationScript>().parentResourceID = resourceID;
			gs.AddNode(n.gameObject);
		}
		
		gs.RecalculatePaths();
	}
}
