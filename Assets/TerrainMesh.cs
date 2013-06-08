using UnityEngine;
using System.Collections;

public class TerrainMesh : MonoBehaviour {	
	
	float size = 10;
	float rows = 10;
	float cols = 10;
	
	Vector3[] vertices;
	Vector2[] uv;
	int[] triangles;
	
	
    void Start()
	{
		/*
		
        Mesh mesh = new Mesh();
		mesh.name = "poop";
		
		vertices = new Vector3[(int)(rows * (cols+1))];
		uv = new Vector2[(int)(rows * (cols+1))];
		triangles = new int[(int)(rows * (cols+1) * 6)];
		
		
		
		mesh.vertices = new Vector3[] { new Vector3(-size, -size, 0.01f), new Vector3(size, -size, 0.01f), new Vector3(size, size, 0.01f), new Vector3(-size, size, 0.01f) };
		mesh.uv = new Vector2[] { new Vector2 (0, 0), new Vector2 (0, 1), new Vector2(1, 1), new Vector2 (1, 0)};		
		mesh.triangles = new int[] {0, 1, 2, 0, 2, 3};	
		
		
		int verts = 0;
		
		for(int i=0; i<rows; i++)
		{
			for(int j=0; j<cols; j++)
			{
				vertices[verts++] = new Vector3(i, 0, j);
			}
		}
		
		int uvs = 0;
		
		for(int i=0; i<rows; i++)
		{
			for(int j=0; j<cols; j++)
			{
				uv[uvs++] = new Vector2(i / (int)(rows * (cols+1)), j / (int)(rows * (cols+1)));
			}
		}
		
		int trigs = 0;
		
		for(int i=0; i<rows; i++)
		{
			for(int j=0; j<cols; j++)
			{				
				triangles[trigs++] = (int)((i * rows) + i);
				triangles[trigs++] = (int)((j * cols) + cols + i);
				triangles[trigs++] = (int)((j * cols) + cols + i + 1);
				
				triangles[trigs++] = (int)((j * rows) + i);
				triangles[trigs++] = (int)((j * cols) + cols + i + 1);
				triangles[trigs++] = (int)((j * rows) + i + 1);
				
				print(triangles[trigs-6] + ", " + triangles[trigs-5] + ", " + triangles[trigs-4]);
			}
		}
		
		
		uv[0] = new Vector2(0,1); //top-left
		uv[1] = new Vector2(1,1); //top-right
		uv[2] = new Vector2(0,0); //bottom-left
		uv[3] = new Vector2(1,0); //bottom-right
		
		
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		
		MeshFilter mf = GetComponent<MeshFilter>();
		mf.mesh = mesh;
		
		
		*/
    }
}
