using UnityEngine;
using System.Collections;

public class Speak : MonoBehaviour {
	
	public TextMesh textMesh;
	public MeshRenderer rendera;
	
	float textDefaultFadeoutDelay = 5.0F;
	
	float textCurrentFadeoutDelay = 0.0F;
	float textTimerStart = 0;
	
	Transform player;
	
	// Use this for initialization
	void Start ()
	{
		player = GameObject.Find("Player").transform;
		
		textMesh = GetComponent<TextMesh>();
		rendera = GetComponent<MeshRenderer>();
		
		textTimerStart = Time.time;
	}
	
	void Update()
	{
		if(rendera.enabled && Physics.Linecast(player.position, transform.position))
		{
			rendera.enabled = false;
		}
		else if(!rendera.enabled && !Physics.Linecast(player.position, transform.position))
		{
			rendera.enabled = true;	
		}
		
		if(!textMesh.text.Equals(""))
		{
			if(Time.time > textTimerStart + textCurrentFadeoutDelay)
			{
				textMesh.text = "";	
			}
		}
		
		transform.rotation = Camera.main.transform.rotation;
	}
	
	public void Say(string s, bool interrupt = false)
	{
		if(textMesh == null) textMesh = GetComponent<TextMesh>();
		if(textMesh.text.Equals("") || interrupt)
		{
			textMesh.text = s;	
			textTimerStart = Time.time;
			textCurrentFadeoutDelay = textDefaultFadeoutDelay;
		}
	}
	
	public void Say(string s, float t, bool interrupt = false)
	{
		if(t == -1) t = 99999999;
			
		if(textMesh == null) textMesh = GetComponent<TextMesh>();
		if(textMesh.text.Equals("") || interrupt)
		{
			textMesh.text = s;	
			textTimerStart = Time.time;
			textCurrentFadeoutDelay = t;
		}
	}
}
