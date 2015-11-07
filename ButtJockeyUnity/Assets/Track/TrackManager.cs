/* Copyright (C) 2013-2014 William James Dyce - All Rights Reserved
 * You may not use, distribute or modify this code without express permission.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackManager : MonoBehaviour 
{
	#region SINGLETON 
	
	private static TrackManager __instance = null;
	public static TrackManager instance
	{
		get 
		{
			if(__instance == null)
				if((__instance = (TrackManager)FindObjectOfType(typeof(TrackManager))) == null)
					Debug.LogError("Could not find TrackManager script on any object in this scene");	
			return __instance;
		}
	}
	
	public void OnDestroy()
	{
		if(__instance == this)
			__instance = null;
	}
	

	#endregion SINGLETON 
	
	#region SECTIONS
	
	#region first, last and distance 
	
	private RailSection __firstSection;
	public RailSection firstSection
	{
		get
		{
			if(__firstSection == null)
			{
				RailSection first = GetComponentInChildren<RailSection>();	
				RailSection previous, current = first;
				do
				{
					previous = current;
					current = previousSection(current);
				}
				while(current != null && current != first);
				__firstSection = previous;
			}
			return __firstSection;
		}
	}

	private RailSection __lastSection;
	public RailSection lastSection
	{
		get
		{
			if(__lastSection == null)
			{
				RailSection first = firstSection;
				RailSection previous, current = first;
				do
				{
					previous = current;
					current = nextSection(current);
				}
				while(current != null && current != first);
				__lastSection = previous;
			}
			return __lastSection;
		}

	}
	
	public int numberOfSections
	{
		get { return GetComponentsInChildren<RailSection>().Length; }
	}
	
	#endregion first, last and distance 
	
	#region iteration 
	
	public RailSection nextSection(RailSection currentSection)
	{
		foreach(var road in GetComponentsInChildren<RailSection>())
			if(road.start == currentSection.end)
				return road;
		//Debug.LogError ("'TrackManager::nextSection' failed to find section after " + currentSection);
		return null;
	}
	
	public RailSection previousSection(RailSection currentSection)
	{
		foreach(var road in GetComponentsInChildren<RailSection>())
			if(road.end == currentSection.start)
				return road;
		//Debug.LogError ("'TrackManager::previousSection' failed to find section before " + currentSection);
		return null;
	}
	
	public IEnumerable<RailSection> iterateForwards(RailSection start = null)
	{
		RailSection currentSection = (start ?? firstSection);
		while(currentSection != null)
		{
			yield return currentSection;
			currentSection = nextSection(currentSection);
		}
		yield break;
	}
	
	public IEnumerable<RailSection> iterateBackwards(RailSection start = null)
	{
		RailSection currentSection = (start ?? lastSection);
		while(currentSection != null)
		{
			yield return currentSection;
			currentSection = previousSection(currentSection);
		}
		yield break;
	}
	
	#endregion iteration 
	
	#region from control point 
	
	public RailSection sectionStartingWith(Transform start)
	{
		foreach(var road in GetComponentsInChildren<RailSection>())
			if(road.start == start)
				return road;
		
		Debug.LogError("'TrackManager::sectionStartingWith' failed to find section starting with "
			+ start);
		return null;
	}
	
	public RailSection sectionEndingWith(Transform end)
	{
		foreach(var road in GetComponentsInChildren<RailSection>())
			if(road.end == end)
				return road;
		
		Debug.LogError("'TrackManager::sectionStartingWith' failed to find section ending with "
			+ end);
		return null;
	}
	
	#endregion from control point 
	
	#endregion SECTIONS 
	
	#region ANCHOR 
	
	public struct RailAnchor
	{
		public RailSection section;
		public Vector3 position;
		public float sectionOffset;
		public float globalOffset;
		public float distance;
		public float xOffset;
		public float yOffset;
		
		public Vector3 up
		{
			get
			{
				if(section == null)
					return Vector3.up;
				else
					return section.getCurveUp(sectionOffset);
			}
		}
		public Vector3 right
		{
			get
			{
				if(section == null)
					return Vector3.right;
				else
					return section.getCurveRight(sectionOffset);
			}
		}
		public Vector3 forward
		{
			get
			{
				if(section == null)
					return Vector3.forward;
				else
					return section.getCurveForward(sectionOffset);
			}
		}

		public float width
		{
			get
			{
				if(section == null)
					return 0.0f;
				else
					return section.getCurveWidth(sectionOffset);
			}
		}

		public float depth
		{
			get
			{
				if(section == null)
					return 0.0f;
				else
					return section.getCurveDepth(sectionOffset);
			}
		}
	}
	
	#region anchor from offset 
	
	public RailAnchor getAnchor(float targetTotalOffset, float precision = 10.0f)
	{
		RailAnchor anchor = new RailAnchor();
		return getAnchor(targetTotalOffset, ref anchor, precision);
	}
	
	public RailAnchor getAnchor(float targetGlobalOffset, ref RailAnchor anchor, float precision = 10.0f)
	{
		float step = 1.0f/Mathf.Max(1.0f, precision);
		
		// special case for end
		if (targetGlobalOffset >= 1.0f - step)
		{
			anchor.section = TrackManager.instance.lastSection;
			anchor.globalOffset = anchor.sectionOffset = 1.0f;
			anchor.xOffset = anchor.yOffset = anchor.distance = 0.0f;
			anchor.position = anchor.section.end.position;
			return anchor;
		}
		
		// approximate anchor based on the offset
		float globalStep = step / numberOfSections;
		float globalOffset = 0.0f;
		foreach(var section in iterateForwards())
		{
			for(float sectionOffset = 0.0f; sectionOffset < 1.0f; 
				sectionOffset += step, globalOffset += globalStep)
			{
				if(globalOffset >= targetGlobalOffset)
				{
					Vector3 anchorPosition = section.getCurvePosition(sectionOffset);
					
					anchor.section = section;
					anchor.sectionOffset = sectionOffset;
					anchor.globalOffset = globalOffset;
					anchor.position = anchorPosition;
					anchor.distance = anchor.xOffset = anchor.yOffset = 0.0f;
					
					return anchor;
				}
			}
		}
		
		// failure :'(
		Debug.LogError("'TrackManager::getAnchor' Could not find an anchor for targetGlobalOffset = " 
			+ targetGlobalOffset + " best was " + globalOffset);
		return anchor;
	}

	#endregion anchor from offset 
	
	#region anchor from position 
	
	public RailAnchor getAnchor(Vector3 position, float precision = 10.0f)
	{
		RailAnchor anchor = new RailAnchor();
		return getAnchor(position, ref anchor, precision);
	}
	
	public RailAnchor getAnchor(Vector3 position, ref RailAnchor anchor, float precision = 10.0f)
	{
		float step = 1.0f/Mathf.Max(1.0f, precision);
		
		float bestAnchorDistance = Mathf.Infinity;
		
		float globalOffset = 0.0f;
		
		foreach(var section in iterateForwards())
		{
			for(float sectionOffset = 0.0f; sectionOffset < 1.0f; 
				sectionOffset += step, globalOffset += step)
			{
				Vector3 anchorPosition = section.getCurvePosition(sectionOffset);
				float anchorWidth = section.getCurveWidth(sectionOffset);
				float anchorDistance = Vector3.Distance(position, anchorPosition)
					/ (anchorWidth*anchorWidth);
				
				// we will always enter here at least once
				// assuming there is at least one RailSection
				if(anchorDistance < bestAnchorDistance)
				{
					bestAnchorDistance = anchor.distance = anchorDistance;

					anchor.section = section;
					anchor.sectionOffset = sectionOffset;
					anchor.globalOffset = globalOffset / numberOfSections;
					anchor.position = anchorPosition;
				}
			}
		}

		// calculate offsets
		Vector3 offset = (position - anchor.position);
		// calculate horizontal offset
		Vector3 left = -anchor.right;
		anchor.xOffset = Vector3.Dot(offset, left)/anchor.width;
		// calculate vertical offset
		Vector3 down = -anchor.up;
		anchor.yOffset = Vector3.Dot(offset, down)/anchor.depth;

		// return the anchor
		return anchor;
	}
	
	#endregion anchor from position 
	
	#endregion ANCHOR 
}
