using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class WatchTransform : MonoBehaviour 
{
	public Transform watched;
	
	private Vector3 __previousPosition;
	private Quaternion __previousRotation;
	
	void Update () 
	{
		if(watched == null)
			return;
		
		// watch for changes
		if((watched.position != __previousPosition) 
		|| (watched.rotation != __previousRotation))
		{
			BroadcastMessage("OnTransform", watched, SendMessageOptions.DontRequireReceiver);
			__previousPosition = watched.position;
			__previousRotation = watched.rotation;
		}
	}
}
