using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Generator : MonoBehaviour {
	
	public enum Resource
	{
		Rock	
	}
	public Resource generator;
	
	public int maxConcurrentResources = 10;	
	public float resourceSpawnRadius;
	float minResourceSpawnRadius;
	
	// Use this for initialization
	void Start ()
	{
		resourceSpawnRadius = transform.localScale.y * 0.8F;
		minResourceSpawnRadius = transform.localScale.y * 0.5F;
		
		for(int i = 0; i<maxConcurrentResources; i++)
		{
			SpawnResource();	
		}
	}
	
	public void SpawnResource()
	{
		float angleInDegrees = Random.Range(0, 360);
		float spawnRadius = Random.Range(minResourceSpawnRadius, resourceSpawnRadius);
		
        float pointX = (spawnRadius * Mathf.Cos(angleInDegrees * Mathf.PI / 180.0f)) + transform.position.x;
        float pointZ = (spawnRadius * Mathf.Sin(angleInDegrees * Mathf.PI / 180.0f)) + transform.position.z;

        Vector3 spawnPoint = new Vector3(pointX, 0.0f, pointZ);
		
		GameObject newResource = Instantiate(Resources.Load(generator.ToString()), spawnPoint, transform.rotation) as GameObject;
		newResource.transform.parent = this.transform;
	}
	
	public void SpawnResource(Vector3 spawnPosition, float spawnRadius, float angleInDegrees)
	{
        float pointX = (spawnRadius * Mathf.Cos(angleInDegrees * Mathf.PI / 180.0f)) + spawnPosition.x;
        float pointZ = (spawnRadius * Mathf.Sin(angleInDegrees * Mathf.PI / 180.0f)) + spawnPosition.z;

        Vector3 spawnPoint = new Vector3(pointX, 0.0f, pointZ);
		
		GameObject newResource = Instantiate(Resources.Load(generator.ToString()), spawnPoint, transform.rotation) as GameObject;
		newResource.transform.parent = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
			
	}
}
