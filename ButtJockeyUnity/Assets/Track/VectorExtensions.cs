/* Copyright (C) 2013-2014 William James Dyce - All Rights Reserved
 * You may not use, distribute or modify this code without express permission.
 */

using UnityEngine;
using System.Collections;

public static class VectorExtensions 
{
	public static Vector3 left90(this Vector3 v)
	{
		// 90 degree rotation to the left on the plane horizontal
		return new Vector3(-v.z, v.y, v.x);
	}
	
	public static Vector3 right90(this Vector3 v)
	{
		// 90 degree rotation to the right on the plane horizontal
		return new Vector3(v.z, -v.y, -v.x);
	}
}
