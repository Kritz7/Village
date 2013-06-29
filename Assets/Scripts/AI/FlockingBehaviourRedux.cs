using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlockingBehaviourRedux : MonoBehaviour {
	
	// Game script references
	Transform player;
	GraphScript graph;
	Speak voice;
	
	// Villager ownership
	public int villagerID = -1;
	public int homeID = -1;
	public int basketID = -1;
	public List<int> ownedNodes = new List<int>();
	
	// villager's held resources
	public int carryingStone = 0;
	
	// Locomotion logic
	Vector3 target = Vector3.zero;
	public List<NavigationScript> pathToFollow = new List<NavigationScript>();
	public List<int> nodeHistory = new List<int>();
	int nodesToRemember = 3;
	
	void Say(params string[] s)
	{
		if(s.Length > 0)
		{
			float sayChanceInc = 1 / s.Length;
			float sayChance = sayChanceInc;
			
			float rand = Random.value;
			
			for(int i=0; i<s.Length; i++)
			{
				if(rand <= sayChance)
				{
					voice.Say(s[i]);	
					break;
				}
			}
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		player = GameObject.Find("Player").transform;
		graph = GameObject.Find("NavigationGraph").GetComponent<GraphScript>();
		voice = transform.FindChild("Speech").GetComponent<Speak>();
		
		transform.FindChild("Cube").renderer.material.mainTexture = Resources.Load("Villager-" + Random.Range(1,4)) as Texture;
		
		Say ("Exuberant!", "Alive again!", "Why?", "Hello, world!", "Statistical!", "Help", "*hick*", "Hats!", "I'm lost.");		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(pathToFollow.Count == 0)
		{
			CreatePath();	
		}
		else
		{
			transform.LookAt(transform.position);
		}
	}
	
	int FindClosestNode(Vector3 t, int s)
	{
		float dist = 9999; // infinity
		int nearestNode = 0;
		
		foreach(NavigationScript node in graph.nodes)
		{
			if((node.transform.position - t).magnitude < dist && node.index != s)
			{
				bool found = false;
				foreach(int i in nodeHistory)
				{
					if(node.index == i) found = true;	
				}
				
				// not in history and, not owned or villager owns or low random chance
				// won't go into owned houses unless I own it, unless I seldom randomly want to
				if(!found && ( node.ownedByVillager == -1 ||  (villagerID!=-1 && node.ownedByVillager == villagerID) || Random.Range(1,100)>90 ))
				{
					dist = (node.transform.position - t).magnitude;
					nearestNode = graph.nodes.IndexOf(node);
				}
				
				// Randomly reallows sheep to go to these locations.
				while(nodeHistory.Count>nodesToRemember)
				{
					int rand = Random.Range(0, nodesToRemember);
					nodeHistory.RemoveAt(rand);
				}
			}
		}
		
		return nearestNode;
	}
	
	int FindClosestVisibleNode(Transform t)
	{
		float dist = 9999; // infinity
		int nearestNode = 0;
		int layerMask = ~(1<<8);
		
		foreach(NavigationScript node in graph.nodes)
		{
			if(!Physics.Linecast(t.position - t.up, node.transform.position, layerMask))
			{
				if((node.transform.position - t.position).magnitude < dist && node.edges.Count > 0)
				{
					dist = (node.transform.position - t.position).magnitude;
					nearestNode = graph.nodes.IndexOf(node);
				}				
			}
		}
		return nearestNode;
	}
	
	void CreatePath()
	{		
		int startPath = FindClosestVisibleNode(transform);
		int endPath = FindClosestNode(target, startPath);	
		
		if(ownedNodes.Count > 0 && Random.Range(0,100)>30)
		{
			int chosenNode = Random.Range(0, ownedNodes.Count-1);
			
			if(!nodeHistory.Contains(chosenNode))
			{
				endPath = ownedNodes[chosenNode];
			}
		}
		
		if((transform.position - player.transform.position).magnitude<3F) print (startPath + " -> " + endPath);
		
		pathToFollow = graph.FindPath(startPath, endPath);

		nodeHistory.Add(endPath);
	}
}
