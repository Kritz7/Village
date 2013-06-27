using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gathering : MonoBehaviour {
	
	public int maxCarryLimit = 10;
	
	public int rock = 0;
	
	bool isPlayer = false;
	
	// Use this for initialization
	void Start () {
		if(name.Contains("Player")) isPlayer = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(isPlayer)
		{
			if(Input.GetKeyDown(KeyCode.Mouse0))
			{
				int layerMask = ~(1<<10);
				
				RaycastHit hit;
				if(Physics.SphereCast(Camera.main.transform.position, 0.5F, Camera.main.transform.forward, out hit, 7F, layerMask))
				{
					if(hit.transform.name.Equals("Rock(Clone)"))
					{
						if(CurrentInventoryAmount() < maxCarryLimit)
						{
							rock++;
							Destroy(hit.transform.gameObject);
						}
					}
					
					if(hit.transform.name.Equals("RockResource"))
					{
						if(hit.transform.gameObject.GetComponent<Generator>().transform.childCount < hit.transform.gameObject.GetComponent<Generator>().maxConcurrentResources)
						{
							hit.transform.gameObject.GetComponent<Generator>().SpawnResource(transform.position, 
								-Random.Range((transform.position - hit.transform.position).magnitude/6F, (transform.position - hit.transform.position).magnitude/4F), 
								Random.Range(110, 250));
						}
					}
					
					if(hit.transform.name.Contains("basket"))
					{
						GameObject basket = hit.transform.gameObject;
						Storage basketStorage = basket.GetComponent<Storage>();
						
						if(basketStorage.storedQuantity > 0)
						{
								rock += basketStorage.TakeResourcesFromPile(basketStorage.storedQuantity);
						}
						else
						{
							basketStorage.AddResourceIntoPile(Storage.Resource.Rock, rock);
							rock = 0;
						}
					}
				}
			}
		}
	}
	
	int CurrentInventoryAmount()
	{
		return rock;	
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(20, 10, 100, 100), "Stone: " + rock);	
	}
}
