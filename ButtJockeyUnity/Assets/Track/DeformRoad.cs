/* Copyright (C) 2013-2014 William James Dyce - All Rights Reserved
 * You may not use, distribute or modify this code without express permission.
 */

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(RailSection))]
public class DeformRoad : MonoBehaviour 
{
	#region EXPOSED TO INSPECTOR 
	
	public Mesh originalMesh;
	
	[Range(1, 100)]
	public int meshColumns = 11;
	
	#endregion EXPOSED TO INSPECTOR 
	
	
	#region RAIL SECTION SHORTCUTS 
	
	private RailSection __rail;

	private RailSection rail
	{
		get
		{
			if(__rail == null) 
				__rail = GetComponent<RailSection>();
			return __rail;
		}
	}

	private Transform start { get {return rail.start;} }
	private Transform end { get {return  rail.end; } }
	private Vector3 up(float t) { return rail.getCurveUp(t); }
	private Vector3 right(float t) { return rail.getCurveRight(t); }
	private Vector3 forward(float t) { return rail.getCurveForward(t); }
	private float width(float t) { return rail.getCurveWidth(t); }
	private float depth(float t) { return rail.getCurveDepth(t); }
	
	#endregion RAIL SECTION SHORTCUTS 
	

	#region CHECK FOR MOVEMENT 
	
	private struct PreviousTransform
	{
		private Vector3 scale;
		private Vector3 position;
		private Quaternion rotation;
		
		public bool equals(Transform t)
		{
			return ((t.localScale == scale) 
				&& (t.position == position) 
				&& (t.rotation == rotation));
		}
		
		public void setTo(Transform t)
		{
			scale = t.localScale;
			position = t.position;
			rotation = t.rotation;
		}
	}
	private PreviousTransform previousStart, previousEnd;
	
	
	void Update()
	{
		// watch for changes
		if(!previousStart.equals(start) || !previousEnd.equals(end))
		{
			// notify
			onTransform();
			
			// reset memorised transform
			previousStart.setTo(start);
			previousEnd.setTo(end);
		}
	}
	
	#endregion CHECK FOR MOVEMENT 

	#region RIBS

	private struct Rib
	{
		public Vector3 left, right;
	}

	private Rib[] __ribs;

	private void __recalculateRibs()
	{
		__ribs = new Rib[__meshRows];
		for(int i = 0; i < __meshRows; i++)
		{
			float rowNormalised = i/((float)__meshRows - 1);
			Vector3 ribOffset = right (rowNormalised)*width(rowNormalised)*__meshWidth*0.5f;
			__ribs[i].left = __curve[i] - ribOffset;
			__ribs[i].right = __curve[i] + ribOffset;
		}
	}

	#endregion
	
	
	#region CACHE BEZIER CURVE 
	
	private Vector3[] __curve;
	private Vector3[] __curveTangents;

	private bool curveReady
	{
		get { return (enabled 
	              && (__curve.Length == __meshRows) 
								&& (__curveTangents.Length == __meshRows)
			          && (__ribs.Length == __meshRows)); }
	}
	
	private void __recalculateCurve()
	{
		__curve = new Vector3[__meshRows];
		__curveTangents = new Vector3[__meshRows];
		float t = 0.0f, step = (1.0f / (__curve.Length - 1));
		for(int i = 0; i < __meshRows; i++, t += step)
		{
			// recalculate the curve
			__curve[i] = __rail.getCurvePosition(t);
			
			// recalculate the tangents
			__curveTangents[i] = __rail.getCurveForward(t); 
		}
	}
	
	void OnDrawGizmosSelected()
	{
		if(!curveReady)
			return;
		for(int i = 0; i < __curve.Length; i++)
		{
			// draw the curve
			if(i < __curve.Length - 1)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawLine(__curve[i], __curve[i + 1]); 
			}
			
			// draw the tangents
			Gizmos.color = Color.white;
			Vector3 tangent = __curveTangents[i].normalized;
			Gizmos.DrawLine(__curve[i] - tangent, __curve[i] + tangent);
			
			// draw the ribs
			Gizmos.color = Color.green;
			Gizmos.DrawLine(__ribs[i].left, __ribs[i].right);
		}
	}
	
	#endregion CACHE BEZIER CURVE  
	
	
	#region CALLBACKS 
	
    private Vector3[] __originalVertices;
    private Mesh __mesh;
	
    void Start()
    {
			reset ();

			if(Application.isPlaying)
			{
				// reset material 
				GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, __rail.getCurveLength()/20);
			}
    }
	
	void onTransform()
	{
		// recalculate number of rows
		__meshRows = (__originalVertices.Length / meshColumns);
		
		// orient start towards end
		start.transform.LookAt(end.transform, start.up);
		
		// recalculate curve based on orientation
		__recalculateCurve();

		// recalculate the scaffold for mesh deformation
		__recalculateRibs();
		
		// modify the mesh based on the curve
		if(curveReady)
        	__displaceMesh();
  }
	
	#endregion CALLBACKS 
	
	#region MESH DISPLACEMENT 
	
	private float __meshWidth = 0.0f, __meshLength = 0.0f;
	private int __meshRows = 5;
	private float __distanceStartToEnd = 0.0f;
	
    private void __displaceMesh()
    {
			// recalculate the total length and width of the original mesh
			__meshWidth = Mathf.Abs(__originalVertices[0].x - __originalVertices[meshColumns - 1].x)*0.5f;
			__meshLength = Mathf.Abs(__originalVertices[0].z);
			
			// recalculate the length of this road segment
			__distanceStartToEnd = Vector3.Distance(start.localPosition, end.localPosition);
			
			// snap beginning of track to start node
			// ie. the centre of the track between the start and the end
			float distanceStartToMiddle = __distanceStartToEnd * 0.5f;
			transform.position = start.position + start.forward*distanceStartToMiddle;
			
			// scale the mesh so it reaches the end
			float scale = (distanceStartToMiddle / __meshLength);
			transform.localScale = new Vector3(1, 1, scale);
			
			// since start looks towards end, have the mesh look in the direction start is looking
			transform.localRotation = start.localRotation;
			
			// build a new set of vertices
	    Vector3[] vertices = __mesh.vertices;
	    for (int i = 0, row = 0, col = 0; i < vertices.Length; i++, col++)
			{
				// keep track of rows and columns
				if(col >= meshColumns)
				{
					col = 0;
					row++;
				}
				
				// deform each vertice
	      	vertices[i] = __displaceVertex(__originalVertices[i], row, col);
			}
			
			// overwrite previous vertices
	    __mesh.vertices = vertices;
			
			// reoptimise
	    __mesh.RecalculateNormals();
	    __mesh.RecalculateBounds();
			
			// destroy and rebuild mesh-collider
			if(GetComponent<MeshCollider>() != null)
				GameObject.DestroyImmediate(GetComponent<MeshCollider>());
			gameObject.AddComponent<MeshCollider>().sharedMesh = __mesh;
    }
	
  private Vector3 __displaceVertex(Vector3 vertex, int row, int col)
  {
		// defined for each row
		Rib rib = __ribs[row];
		float rowNormalised = row/((float)__meshRows - 1);
		Quaternion rowRotation = Quaternion.Slerp(start.rotation, end.rotation, rowNormalised);
		float rowWidth = Mathf.Lerp(start.localScale.x, end.localScale.x, rowNormalised);
		float rowDepth = Mathf.Lerp(start.localScale.y, end.localScale.y, rowNormalised)*rowWidth;
		// defined specifically for this row/collumn
		float colNormalised = (col/((float)meshColumns - 1));
		Vector3 position = Vector3.Lerp(rib.left, rib.right, colNormalised);
		
		// curved bowl shape
		Vector3 bowlOffset = 
			(rowRotation*Vector3.down)*Mathf.Sin(Mathf.PI*col/(float)meshColumns)*rowDepth;
		position += bowlOffset;
		
		// mesh is position in a global space and moved to where it will be used
		return transform.InverseTransformPoint(position);
	}
	
	#endregion MESH DISPLACEMENT 

	#region CUSTOM EDITOR 
	
	public void reset()
	{
		// create new mesh
        __mesh = Instantiate(originalMesh) as Mesh;
        GetComponent<MeshFilter>().sharedMesh = __mesh;
		
		// save old mesh
        originalMesh.Optimize();
        __originalVertices = __mesh.vertices;
		
		// spoof an initial transformation to reset deformation
		onTransform();
	}
	
	public void split()
	{
		// create new point in middle
		Vector3 between = (__rail.start.position + __rail.end.position)*0.5f;
		GameObject split = new GameObject(
			__rail.start.name + __rail.end.name, typeof(DrawGizmoSphere));
		split.transform.position = between;
		split.transform.parent = __rail.end.parent;
		
		// remember the next section, as this information will be lost during surgery
		RailSection previousNext = TrackManager.instance.nextSection(__rail);

		// end this section at the new -middle- control point
		__rail.end = split.transform;
		
		// recalculate this section's deformation
		reset();
	
		// create a new section
		GameObject secondHalf = (GameObject)Instantiate(gameObject);
		secondHalf.name = gameObject.name + previousNext.name;
		secondHalf.transform.parent = transform.parent;
		
		// new section goes from middle control point to original end
		RailSection secondHalfRail = secondHalf.GetComponent<RailSection>();
		secondHalfRail.start = __rail.end;
		secondHalfRail.end = previousNext.start;
		
		// drop in all the required values
		DeformRoad secondHalfDeform = secondHalf.GetComponent<DeformRoad>();
		secondHalfDeform.originalMesh = originalMesh;
		secondHalfDeform.meshColumns = meshColumns;
		
		#if UNITY_EDITOR
			// set the editor selection
			UnityEditor.Selection.activeObject = split;
		#endif
	}
	
	public void collapse()
	{
		// move start to middle
		Vector3 between = (__rail.start.position + __rail.end.position)*0.5f;
		__rail.start.position = between;
		
		// cache previous and next before surgery
		RailSection previous = TrackManager.instance.nextSection(__rail);
		RailSection next = TrackManager.instance.nextSection(__rail);
		
		// delete end control point
		next.start = __rail.start;
		DestroyImmediate(__rail.end.gameObject);
		
		// force reset of adjascent deformations
		previous.gameObject.GetComponent<DeformRoad>().reset();
		next.gameObject.GetComponent<DeformRoad>().reset();
		
		#if UNITY_EDITOR
			// set the editor selection
			UnityEditor.Selection.activeObject = __rail.start.gameObject;
		#endif
		
		// delete this
		DestroyImmediate(gameObject);
	}
	
	#endregion CUSTOM EDITOR 
}
