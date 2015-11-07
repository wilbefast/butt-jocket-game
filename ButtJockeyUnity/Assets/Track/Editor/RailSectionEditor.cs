using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RailSection))]
class RailSectionEditor : Editor 
{
	public override void OnInspectorGUI() 
	{
		base.OnInspectorGUI();
		
		if(GUILayout.Button("SelectPrevious"))
			UnityEditor.Selection.activeObject = 
				TrackManager.instance.previousSection((RailSection)target);
		
		if(GUILayout.Button("SelectNext"))
			UnityEditor.Selection.activeObject = 
				TrackManager.instance.nextSection((RailSection)target);
	}
}