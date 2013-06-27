using UnityEngine;
using System.Collections;

public class Storage : MonoBehaviour {
	
	public int basketID = -1;
	
	int bottomResourceAmount = 1;
	int midResourceAmount = 5;
	int topResourceAmount = 10;
	
	GameObject resourceLevel;
	
	public enum Resource
	{
		Rock,
		None = 0
	}
	public Resource storingResource;
	
	public int storedQuantity = 0;
	
	void Start()
	{
		resourceLevel = transform.FindChild("Level").gameObject;
		MoveLevel();
	}
	
	void MoveLevel()
	{
		if(storedQuantity >= topResourceAmount)
		{
			resourceLevel.transform.localPosition = new Vector3(0, 0, 0.3F);
			resourceLevel.transform.localScale = new Vector3(1.4F, 0.1F, 1.5F);
		}
		else if(storedQuantity >= midResourceAmount)
		{
			resourceLevel.transform.localPosition = new Vector3(0, 0F, 0.1F);
			resourceLevel.transform.localScale = new Vector3(1.0F, 0.1F, 1.4F);
		}
		else if(storedQuantity >= bottomResourceAmount)
		{
			resourceLevel.transform.localPosition = new Vector3(0, 0F, -0.28F);
			resourceLevel.transform.localScale = new Vector3(0.9F, 0.1F, 0.9F);
		}
		else
		{
			resourceLevel.transform.localPosition = new Vector3(0, 0, 0);
			resourceLevel.transform.localScale = new Vector3(0.01F, 0.01F, 0.01F);
		}
	}
	
	public bool AddResourceIntoPile(Resource r, int amount)
	{
		bool success = false;
		
		if(r == Resource.None)
		{
			storingResource = r;
			storedQuantity = amount;
			success = true;
		}
		
		else if(r == storingResource)
		{	
			storedQuantity += amount;
			success = true;
		}
		
		MoveLevel();
		
		return success;
	}
	
	public int TakeResourcesFromPile(int amount)
	{
		int success = 0;
		
		if(storedQuantity >= amount)
		{
			success = storedQuantity;
			storedQuantity -= amount;	
			
		}
		
		MoveLevel();
		
		return success;
	}
}
