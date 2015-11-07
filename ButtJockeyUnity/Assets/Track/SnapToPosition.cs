/* Copyright (C) 2013-2014 William James Dyce - All Rights Reserved
 * You may not use, distribute or modify this code without express permission.
 */

using UnityEngine;
using System.Collections;

public class SnapToPosition : MonoBehaviour 
{
	public Transform followed;
	
	public void Update()
	{
		if(followed == null)
			return;
		
		transform.localPosition = followed.localPosition;
	}
}
