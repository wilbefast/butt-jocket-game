using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class WibbleMesh : MonoBehaviour 
{
	public Mesh originalMesh;
	
	[Range(1, 100)]
	public int meshColumns = 11;

	void Start () 
	{
		reset ();
	}

	public void reset()
	{
		// create new mesh
		__mesh = Instantiate(originalMesh) as Mesh;
		GetComponent<MeshFilter>().sharedMesh = __mesh;
		
		// save old mesh
		originalMesh.Optimize();
		__originalVertices = __mesh.vertices;
		
		// reset deformation
		__displaceMesh();
	}


	private Vector3[] __originalVertices;
	private Mesh __mesh;
	
	[Range(0f, 100f)]
	public float wibbliness = 10f;
	
	private void __displaceMesh()
	{
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
			
			// deform each vertex
			var v = __originalVertices[i];
			if(col < meshColumns/2 - 1 || col > meshColumns/2 + 1)
				vertices[i] = v + Vector3.up*Random.Range(-wibbliness, wibbliness)*Mathf.Abs(v.x);
			else
				vertices[i] = v;
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
}
