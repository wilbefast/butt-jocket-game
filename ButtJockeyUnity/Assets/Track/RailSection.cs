/* Copyright (C) 2013-2014 William James Dyce - All Rights Reserved
 * You may not use, distribute or modify this code without express permission.
 */

using UnityEngine;
using System.Collections;

public class RailSection : MonoBehaviour 
{
	#region EXPOSED TO INSPECTOR 
	
	public Transform start;
	public Transform end;
	
	#endregion EXPOSED TO INSPECTOR 
	
	#region BEZIER CURVE 

	public Vector3 getCurvePosition(float t)
	{
		return (Bezier.cubic(start, end, t));
	}
	
	public Vector3 getCurveForward(float t)
	{
		return (Bezier.cubicTangent(start, end, t));
	}
	
	public Vector3 getCurveRight(float t)
	{
		Quaternion tRotation = 
				Quaternion.Slerp(start.rotation, end.rotation, t);
		return (tRotation * Vector3.right);
	}
	
	public Vector3 getCurveUp(float t)
	{
		Quaternion tRotation = 
				Quaternion.Slerp(start.rotation, end.rotation, t);
		return (tRotation * Vector3.up);
	}
	
	public float getCurveWidth(float t)
	{
		return Mathf.Lerp (start.localScale.x, end.localScale.x, t);
	}
	
	public float getCurveDepth(float t)
	{
		return Mathf.Lerp (start.localScale.y, end.localScale.y, t);
	}
	
	public float getCurveLength()
	{
		float total = 0.0f;
		Vector3 A = start.position, B;
		for(float t = 0.0f; t < 1.0f; t += 0.01f)
		{
			B = Bezier.cubic(start, end, t);
			total += Vector3.Distance(A, B);
			A = B;
		}
		return total;

		// FIXME - use actual curve length, not bee-line
		//return (Vector3.Distance(start.localPosition, end.localPosition));
	}
	
	#endregion BEZIER CURVE 
}
