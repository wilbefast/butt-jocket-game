/* Copyright (C) 2013-2014 William James Dyce - All Rights Reserved
 * You may not use, distribute or modify this code without express permission.
 */

using UnityEngine;
using System.Collections;

public abstract class Bezier 
{
	#region QUADRATIC (3 points) 
	
	private static float __sqr(float k) { return k*k; }
	public static Vector3 quadratic(Vector3 A, Vector3 B, Vector3 C, float t)
	{
		return (__sqr(1-t)*A 
			+ 2*(1-t)*t*B 
			+ __sqr(t)*C);
	}
	
	#endregion QUADRATIC (3 points) 
	
	
	#region CUBIC (4 points)
		
	private static float __cub(float k) { return k*k*k; }
	public static Vector3 cubic(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
	{
		return (__cub(1 - t)*A 
			+ 3*__sqr(1 - t)*t*B 
			+ 3*(1 - t)*__sqr(t)*C 
			+ __cub(t)*D);
	}
	public static Vector3 cubic(Transform AB, Transform CD, float t)
	{
		return cubic (AB.position, 
						(AB.position + CD.position)*0.5f,
						CD.position - CD.forward*10, 
						CD.position, 
						t);
	}
	
	public static Vector3 cubicTangent(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
	{
		return (-3*__sqr(1 - t)*A
			+ (3*__sqr(1 - t) - 6*(1 - t)*t)*B
			+ (6*(1 - t)*t - 3*__sqr(t))*C
			+ 3*__sqr(t)*D);
	}
	public static Vector3 cubicTangent(Transform AB, Transform CD, float t)
	{
		return cubicTangent(AB.position, 
						(AB.position + CD.position)*0.5f,
						CD.position - CD.forward*10, 
						CD.position, 
						t);
	}
	

	
	#endregion CUBIC (4 points) 
}
