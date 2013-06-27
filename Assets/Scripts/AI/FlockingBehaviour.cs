using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlockingBehaviour : MonoBehaviour
{
	
	public int villagerID = -1;
	public int homeID = -1;
	public int basketID = -1;
	
	float maxSpeed = 2;
	float maxForce = 3;
	
	float range = 6;
//	float fleeRange = 6;
	
//	float fleeWeight = 20F;
//	float alignWeight = 1.0F;
	float cohesionWeight = 1.0F;
	float separationWeight = 2F;
	float seekWeight = 10F;
	
	float idleDelay = 0F;
	
	Vector3 steeringAccumulator = Vector3.zero;
	Vector3 steerForce = Vector3.zero;
	
	// public GameObject target;
	
	Vector3 target = Vector3.zero;
	Transform player;
	GraphScript graph;
	
	List<GameObject> theFlock = new List<GameObject>();
	List<GameObject> inRange;
	
	public List<int> pathToFollow = new List<int>();
	public List<int> nodeHistory = new List<int>();
	int nodesToRemember = 3;
	
	public List<int> ownedNodes = new List<int>();
	
	// Use this for initialization
	void Awake ()
	{		
		player = GameObject.Find("Player").transform;
		graph = GameObject.Find("NavigationGraph").GetComponent<GraphScript>();
		
		transform.FindChild("Cube").renderer.material.mainTexture = Resources.Load("Villager-" + Random.Range(1,4)) as Texture;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if(idleDelay == 0)
		{
			rigidbody.AddForce(steerForce);
		}
	}
	
	void Update()
	{			
		if(graph.nodes.Count - 2 > nodesToRemember)
		{
			nodesToRemember = graph.nodes.Count - 2;	
		}
		else if(graph.nodes.Count < nodesToRemember)
		{
			nodesToRemember = graph.nodes.Count;	
		}
		if(rigidbody.velocity.magnitude>0.01F)
		{
			Vector3 look = transform.position + rigidbody.velocity;
			look.y = transform.position.y;
			transform.LookAt(look, new Vector3(0,1,0));
		}
		if(transform.position.y < 0) transform.position = new Vector3(transform.position.x, transform.localScale.y * 1.1F, transform.position.z);
		
			
		if(rigidbody.velocity.magnitude > 1F) animation.Blend( "Walk" );
		else if(idleDelay>0) animation.Blend( "Idle" );
	}
	
	public void StartFlocking(System.Collections.Generic.List<GameObject> flock)
	{
		theFlock = flock;
		StartCoroutine("FlockUpdate");
	}
	
	IEnumerator FlockUpdate()
	{
		while(true)
		{
			if(idleDelay == 0)
			{
				inRange = new List<GameObject>();
				
				foreach(GameObject boid in theFlock)
				{
					Vector3 separation = transform.position - boid.transform.position;
					FlockingBehaviour fs = boid.GetComponent<FlockingBehaviour>();
					
					if(separation.magnitude>0.01F && separation.magnitude<range)
					{
						inRange.Add(boid);	
					}				
					
					
					if(separation.magnitude<5F && Random.Range(0,100)>97 && fs.villagerID!=villagerID && fs.pathToFollow.Count > 3 && fs.pathToFollow != pathToFollow)
					{
						pathToFollow = fs.pathToFollow;
					}
				}
				
				steeringAccumulator = Vector3.zero;
	
				if(inRange.Count > 0)
				{
					steeringAccumulator += cohesionWeight * CohesionSteering();
					steeringAccumulator += separationWeight * SeparationSteering();
				}
				
				// if the world has nodes placed in it
				if(graph.nodes.Count>0)
				{
					if(pathToFollow.Count<=0) SetTarget();
					if(inRange.Count==0) steeringAccumulator += seekWeight * SeekNodeSteering();
					else steeringAccumulator += (seekWeight/2) * SeekNodeSteering();
				}
				
				steeringAccumulator.y = 0;			
				
				if(steeringAccumulator.magnitude>maxForce)
				{
					steerForce = steeringAccumulator.normalized * maxForce;	
				}
				else
				{
					steerForce = steeringAccumulator;	
				}
			}
			else
			{
				if(steerForce.magnitude>0) steerForce = Vector3.zero;
				if(rigidbody.velocity.magnitude>0) rigidbody.velocity *= 0.1f;
				
				idleDelay -= Time.deltaTime * 20;
				if(idleDelay < 0) idleDelay = 0;
			}
				
			float waitTime = Random.Range(0.1F, 0.4F);
			yield return new WaitForSeconds(waitTime);
		}
	}
	
	
	
	Vector3 CohesionSteering()
	{
		Vector3 desiredVel = Vector3.zero;
		
		Vector3 avgFlockPos = Vector3.zero;
		
		foreach(GameObject boid in inRange)
		{
			avgFlockPos += boid.transform.position;
		}
		
		desiredVel = (avgFlockPos - transform.position).normalized * maxSpeed;
		
		return desiredVel;
	}
	
	Vector3 SeparationSteering()
	{
		Vector3 desiredVel = Vector3.zero;
		
		Vector3 avgFlockPos = Vector3.zero;
		
		foreach(GameObject boid in inRange)
		{
			avgFlockPos += boid.transform.position;
		}
		
		desiredVel = (avgFlockPos - transform.position).normalized / (avgFlockPos - transform.position).magnitude;
		
		return desiredVel;
	}
	
	Vector3 AlignSteering()
	{
		Vector3 desiredVel = Vector3.zero;
		Vector3 avgVel = Vector3.zero;
		
		foreach(GameObject boid in inRange)
		{
			avgVel += boid.rigidbody.velocity;
		}
		
		desiredVel = (avgVel - rigidbody.velocity).normalized * maxForce;
		
		return desiredVel;
	}
	
	Vector3 SeekSteering()
	{
		Vector3 desiredVel = ((target - transform.position).normalized
							* maxSpeed) - rigidbody.velocity;	
		
		return desiredVel;
	}
	
	Vector3 FleeTarget()
	{
		Vector3 desiredVel = (player.transform.position - transform.position).normalized
							* maxSpeed;
		
		return -desiredVel;
	}
	
	void SetTarget()
	{
		target = transform.position + steeringAccumulator.normalized * 10;
		
		target.y = 0;
		
		CreatePath();
	}
	
	void SetTargetFleeing()
	{
		target = transform.position + FleeTarget().normalized * 3;
		
		target.y = 0;
		
		CreatePath();
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
					nearestNode = node.index;
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
					nearestNode = node.index;
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
	
	Vector3 SeekNodeSteering()
	{
		Vector3 desiredVel = Vector3.zero;
		int layerMask = ~(1<<8);
		
		//  || FindClosestNode(target)!=pathToFollow[pathToFollow.Count-1]
		if(pathToFollow.Count == 0) 
		{
			CreatePath();
		}
		
		if(pathToFollow.Count > 0)
		{
			if(Physics.Linecast(transform.position, graph.nodes[pathToFollow[0]].transform.position, layerMask))
			{
				pathToFollow = graph.FindPath(Random.Range(0, graph.nodes.Count-1), pathToFollow[pathToFollow.Count-1]);
				//pathToFollow = graph.FindPath(FindClosestVisibleNode(transform), Random.Range(0, graph.nodes.Count-1));
			}	
		}
		
		if(pathToFollow.Count>0)
		{			
			desiredVel = ((graph.nodes[pathToFollow[0]].transform.position - transform.position).normalized
							* maxSpeed) - rigidbody.velocity;	
			
			if((transform.position - graph.nodes[pathToFollow[0]].transform.position).magnitude <= 2F)
			{
				
				// Villager finds unowned house
				if(graph.nodes[pathToFollow[0]].parentHomeID != -1 && homeID == -1 && graph.nodes[pathToFollow[0]].ownedByVillager == -1)
				{
					int villagerNewHomeID = graph.nodes[pathToFollow[0]].parentHomeID;
					graph.nodes[pathToFollow[0]].VillagerNowOwns(this);
					
					foreach(NavigationScript ns in graph.nodes)
					{
						if(ns.parentHomeID == villagerNewHomeID)
						{
							ownedNodes.Add(ns.index);
							ns.VillagerNowOwns(this);
						}
					}
					
					GameObject hat;
					if(!transform.FindChild("Hat(Clone)"))
					{
						hat = Instantiate(Resources.Load("Hat"), transform.position + transform.up/2, transform.rotation) as GameObject;
						hat.transform.parent = this.transform;
					}
					else
					{
						hat = transform.FindChild("Hat(Clone)").gameObject;	
					}
					
					hat.renderer.material.color = new Color(Random.value, Random.value, Random.value, 1.0F);
				}
				
				// Villager finds unmanned basket
				if(graph.nodes[pathToFollow[0]].parentBasketID != -1 && basketID == -1 && graph.nodes[pathToFollow[0]].ownedByVillager == -1)
				{
					int villagerNewBasketID = graph.nodes[pathToFollow[0]].parentBasketID;
					graph.nodes[pathToFollow[0]].VillagerNowOwns(this);
					
					foreach(NavigationScript ns in graph.nodes)
					{
						if(ns.parentBasketID == villagerNewBasketID)
						{
							ownedNodes.Add(ns.index);
							ns.VillagerNowOwns(this);
						}
					}
					
					GameObject hat;
					if(!transform.FindChild("Hat(Clone)"))
					{
						hat = Instantiate(Resources.Load("Hat"), transform.position + transform.up/2, transform.rotation) as GameObject;
						hat.transform.parent = this.transform;
					}
					else
					{
						hat = transform.FindChild("Hat(Clone)").gameObject;	
					}
					
					hat.transform.FindChild("Cube").renderer.material.color = new Color(Random.value, Random.value, Random.value, 1.0F);
					
				}
				
				if(Random.Range(0,100) > 75) idleDelay = Random.Range(0, 5);
				pathToFollow.RemoveAt(0);
				 
			}
		}
		
		return desiredVel;
	}
}