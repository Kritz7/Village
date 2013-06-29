using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlockingBehaviour : MonoBehaviour
{
	
	public int villagerID = -1;
	public int homeID = -1;
	public int basketID = -1;
	
	float maxSpeed = 2;
	float maxForce = 1F;
	
	float range = 6;
//	float fleeRange = 6;
	
//	float fleeWeight = 20F;
//	float alignWeight = 1.0F;
	float cohesionWeight = 1.0F;
	float separationWeight = 2F;
	float seekWeight = 10F;
	
	float idleDelay = 0F;
	
	// villager resouces carrying
	public int carryingStone = 0;
	
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
	
	Speak voice;
	float talkThreashold = 0.98F; // The higher, the less likely they are to talk. 
	
	// Use this for initialization
	void Awake ()
	{		
		player = GameObject.Find("Player").transform;
		graph = GameObject.Find("NavigationGraph").GetComponent<GraphScript>();
		voice = transform.FindChild("Speech").GetComponent<Speak>();
		
		transform.FindChild("Cube").renderer.material.mainTexture = Resources.Load("Villager-" + Random.Range(1,4)) as Texture;
	
	
		float spawnSay = Random.value;
		
		if(spawnSay > 0.9) voice.Say("Exuberant!");
		else if(spawnSay > 0.8) voice.Say("Alive again!");
		else if(spawnSay > 0.7) voice.Say("Why?");
		else if(spawnSay > 0.6) voice.Say("Hello, world!");
		else if(spawnSay > 0.5) voice.Say("Statistical!");
		else if(spawnSay > 0.4) voice.Say("Help");
		else if(spawnSay > 0.3) voice.Say("*hick*");
		else if(spawnSay > 0.2) voice.Say("Hats!");
		else if(spawnSay > 0.1) voice.Say("I'm lost.");
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
			//Vector3 look = transform.position + rigidbody.velocity;
			Vector3 look;
			
			if(pathToFollow.Count>0) look = graph.nodes[pathToFollow[0]].transform.position;
			else look = transform.position + rigidbody.velocity;
			
			look.y = transform.position.y;
			transform.LookAt(look, new Vector3(0,1,0));
		}
		if(transform.position.y < 0) transform.position = new Vector3(transform.position.x, transform.localScale.y * 1.1F, transform.position.z);
		
		if(rigidbody.velocity.magnitude > 5F)
		{
			if(Random.value > talkThreashold)
			{
				float spawnSay = Random.value;
				if(spawnSay > 0.95F) voice.Say("WAAAAAHH", true);
				else if(spawnSay > 0.9F) voice.Say("OH NO", true);
				else if(spawnSay > 0.8F) voice.Say("TOO FAST", true);
				else if(spawnSay > 0.7F) voice.Say("AWAY!", true);
				else if(spawnSay > 0.5F) voice.Say("I MADE A MISTAKEEEEEE~", true);
				else if(spawnSay > 0.3F) voice.Say("WHOOOP", true);
				else if(spawnSay > 0.3F) voice.Say("AHH", true);
				else if(spawnSay > 0.2F) voice.Say("GOTTA GO FAST", true);
				else if(spawnSay > 0.1F) voice.Say("PATHFINDING?", true);
			}
			
			rigidbody.velocity *= 0.3F * Time.deltaTime;
		}
		
		if(rigidbody.velocity.magnitude > 1F) animation.Blend( "Walk" );
		else if(idleDelay>0)
		{
			animation.Blend( "Idle" );
			
			if(Random.value > talkThreashold)
			{
				float spawnSay = Random.value;
				if(spawnSay > 0.998F) voice.Say("Hello darkness my old friend");
				else if(spawnSay > 0.9F) voice.Say("I'm here.");
				else if(spawnSay > 0.8F) voice.Say("What's that?");
				else if(spawnSay > 0.7F && homeID != -1) voice.Say("Look at my hat! Everybody! Anybody?");
				else if(spawnSay > 0.7F && basketID == -1) voice.Say("No hats...");
				else if(spawnSay > 0.5F) voice.Say("Pentagons...");
				else if(spawnSay > 0.3F) voice.Say("*hick*");
				else if(spawnSay > 0.3F) voice.Say("Hrm.");
				else if(spawnSay > 0.2F) voice.Say("Maybe...");
				else if(spawnSay > 0.1F) voice.Say("It's not time.");
			}
		}
		
		if(pathToFollow.Count == 0) rigidbody.velocity = Vector3.zero;
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
				//	steeringAccumulator += cohesionWeight * CohesionSteering();
				//	steeringAccumulator += separationWeight * SeparationSteering();
				}
				
				// if the world has nodes placed in it\
				if(graph.nodes.Count>2)
				{
					if(pathToFollow.Count<=0) SetTarget();
					if(inRange.Count==0) steeringAccumulator += seekWeight * SeekNodeSteering();
					else steeringAccumulator += (seekWeight/2) * SeekNodeSteering();
				}
				
				/*
				int layermask = ~(1<<8);
				RaycastHit hit;
				if(Physics.SphereCast(transform.position, 1F, transform.forward, out hit, 2F, layermask))
				{
					print ("Hit " + hit.transform.name);
				}
				*/
				
				// steeringAccumulator.y = 0;			
				
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
		
//		pathToFollow = graph.FindPath(startPath, endPath);

		nodeHistory.Add(endPath);
	}
	
	Vector3 SeekNodeSteering()
	{
		Vector3 desiredVel = Vector3.zero;
		int layerMask = ~(1<<8);
		
		if(pathToFollow.Count == 0) 
		{
			CreatePath();
		}
		
		try
		{
//			List<int> checkPath = graph.FindPath(pathToFollow[0], pathToFollow[pathToFollow.Count-1]);
			if(!(pathToFollow.Count>0 && checkPath.Count > 0 && pathToFollow.Count == checkPath.Count
				&& pathToFollow[0] == checkPath[0] &&
				pathToFollow[pathToFollow.Count-1] == checkPath[checkPath.Count - 1]))
			{
				pathToFollow = checkPath;
			}
		}
		catch
		{
			if(Random.value > talkThreashold)
			{
				float spawnSay = Random.value;
				if(spawnSay > 0.8F) voice.Say("I'm confused!");
				else if(spawnSay > 0.6F) voice.Say("GAH!");
				else if(spawnSay > 0.4F) voice.Say("Not like this...");
				else if(spawnSay > 0.2F) voice.Say("Inconceivable!");
				else voice.Say("SO ANGRY");
			}
		}
		
		if(pathToFollow.Count > 0)
		{
			if(Physics.Linecast(transform.position + (transform.right * transform.localScale.y) - transform.up, graph.nodes[pathToFollow[0]].transform.position, layerMask) || 
				Physics.Linecast(transform.position - (transform.right * transform.localScale.y) - transform.up, graph.nodes[pathToFollow[0]].transform.position, layerMask))
			{
				int start = Random.Range(0, graph.nodes.Count-1);
				int end;
				
				end = FindClosestVisibleNode(transform);
					/*
					 * Random.Range(0, graph.nodes.Count-1);
				if(end == start && start < graph.nodes.Count-2) end = start+1;
				else if(end == start && start > 0) end = start-1;
				else
					print("fuck this shit");
					*/
				
//				pathToFollow = graph.FindPath(start, end);
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
					
					voice.Say("I AM HOME OWNER. HA!");
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
					
					voice.Say("This is mine.");
				}
				
				if(Random.Range(0,100) > 75) idleDelay = Random.Range(0, 5);
				pathToFollow.RemoveAt(0);
				 
			}
		}
		
		return desiredVel;
	}
}