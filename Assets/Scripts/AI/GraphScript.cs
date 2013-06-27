using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphScript : MonoBehaviour {

	public List<NavigationScript> nodes = new List<NavigationScript>();
	
	bool updateEdges = false;
	int edgeUpdateProgress = -1;
	
	// Use this for initialization
	void Start () {
		int i = 0;
		foreach(Transform n in transform)
		{
			NavigationScript navnode = n.GetComponent<NavigationScript>();
			nodes.Add(navnode);
			navnode.index = i;
			i++;
		}
		foreach(NavigationScript n in nodes)
		{
			n.CalcEdges();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(updateEdges)
		{
			if(edgeUpdateProgress < nodes.Count - 1)
			{
				nodes[edgeUpdateProgress].GetComponent<NavigationScript>().CalcEdges();
				edgeUpdateProgress++;
			}
			else
			{
				updateEdges = false;
				edgeUpdateProgress = -1;
			}				
		}
	}
	
	public void AddNode(GameObject n)
	{
		NavigationScript navnode = n.GetComponent<NavigationScript>();
		nodes.Add(navnode);
		navnode.index = nodes.Count-1;
	}
	
	public void RecalculatePaths()
	{
		updateEdges = true;
		edgeUpdateProgress = 0;
	}
	
	public List<int> FindPath(int start, int goal)
	{
		int[] fromNode = new int[nodes.Count]; // stores previous node
		int[] status = new int[nodes.Count];
		List<int> open = new List<int>();
		
		float[] g = new float[nodes.Count];
		float[] h = new float[nodes.Count];
		float[] f = new float[nodes.Count];
		
		for(int i=0; i<nodes.Count; i++)
		{
			status[i] = 0; // not open nor closed
			fromNode[i] = -1; // flag for unknown
			
			g[i] = -1; // infinity
			f[i] = -1; // infinity
			h[i] = Hueristic(i,goal); // space magic
		}
		
		// init open list
		open.Add(start);
		status[start] = 1;
		fromNode[start] = -1;
		g[start] = 0;
		f[start] = h[start];
		
		while(open.Count > 0)
		{
			// find nearest in open list
			float openIndex = 0;
			float minVal = f[open[0]];
			
			for(int i=1; i<open.Count; i++)
			{
				if(f[open[i]]<minVal)
				{
					minVal = f[open[i]];
					openIndex = i;
				}
			}
			
			float x = open[(int)openIndex];
			
			if(x==goal)
			{
				return ConstructedPath(fromNode,goal);	
			}
			
			// add to closed, remove from open
			status[(int)x] = 2;
			
			open.RemoveAt((int)openIndex);
			
			foreach(int y in nodes[(int)x].edges)
			{
				if(status[y]!=2) // not closed
				{
					// normally use edge weight here
					// but edge weight is same as the heuristic
					
					int newEst = (int)g[(int)x]+(int)Hueristic((int)x,y);
					
					if(status[y]!=1)
					{
						status[y] =1;
						open.Add(y);
						fromNode[(int)y] = (int)x;
						g[y] = newEst;
						f[y] = g[y] + h[y];
					}
					else if(newEst<g[y])
					{
						fromNode[(int)y] = (int)x;
						g[y] = newEst;
						f[y] = g[y] + h[y];
					}
				}
			}
		}
		
		return new List<int>();
	}
	
	float Hueristic(int a, int b)
	{
		return (nodes[a].transform.position - nodes[b].transform.position).magnitude;	
	}
	
	List<int> ConstructedPath(int[] f, int g)
	{
		List<int> path = new List<int>();
		int p = g;
		
		while(p!=-1)
		{
			path.Add(p);
			p = f[p];
		}
		
		path.Reverse();
		
		return path;
	}
}
