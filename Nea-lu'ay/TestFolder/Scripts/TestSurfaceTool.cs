using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public partial class TestSurfaceTool : MeshInstance3D
{
	[Export]
	public Godot.Collections.Dictionary<Vector3, Vector3[]> _Graph = new Godot.Collections.Dictionary<Vector3, Vector3[]>();
	public Godot.Collections.Dictionary<Vector3, Vector3[]> Graph = new Godot.Collections.Dictionary<Vector3, Vector3[]>();
	
	public override void _Ready()
	{
		
		Vector3[] vertices =
		[
			new Vector3(-1, 0, 1),
			new Vector3(-1, 0, -1),
			new Vector3(0, -1, 0),
			new Vector3(0, 1, 0),
			new Vector3(1, 0, -1),
			new Vector3(1, 0, 1)
		];

		//Vector3[] temp = new Vector3[3];
		Godot.Collections.Array<Vector3[]> faces = new Godot.Collections.Array<Vector3[]>();
		
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3[] edges = [];
			for (int j = 0; j < vertices.Length; j++)
			{
				if(!_Graph.ContainsKey(vertices[i]))
				{
					GD.Print(vertices[i], ", ", edges);
					if ((Mathf.Sqrt(Mathf.Pow((vertices[i].X-vertices[j].X), 2)+Mathf.Pow((vertices[i].Z-vertices[j].Z), 2)) <= 2.0f))
					{
						edges.Append(vertices[j]);
						GD.Print(vertices[j]);
					}
				}
			}
			_Graph[vertices[i]] = edges;
		}
		
		// Initialize the ArrayMesh.
		var arrMesh = new ArrayMesh();
		Godot.Collections.Array arrays = [];
		arrays.Resize((int)Mesh.ArrayType.Max);
		arrays[(int)Mesh.ArrayType.Vertex] = vertices;
		
		// Create the Mesh.
		arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
		this.Mesh = arrMesh;

		//Other way of doing this
		Godot.Collections.Array surfaceArray = [];
		surfaceArray.Resize((int)Mesh.ArrayType.Max);

		List<Vector3> vertcies = [];
		List<Vector2> uvs = [];
		List<Vector3> normals = [];
		List<int> indices = [];
		
		//Mesh and Grpah gen
		Vector3[] bounds =
		{
			new Vector3(-10, 0, -10),
			new Vector3(-10, 0, 10),
			new Vector3(10, 0, -10),
			new Vector3(10, 0, 10)
		};
		
		Vector3[] tempGrid = [];
		
		//for this to work, 0 must be the smallest corner and 3 being the largest
		for(float i = bounds[0].X; i < bounds[3].X; i++)
		{
			for(float j = bounds[0].Z; j < bounds[3].Z; j++)
			{
				tempGrid.Append(new Vector3(i, 0, j));
			}
		}
		
		//creating graph
		for(int i = 0; i < tempGrid.Length; i++)
		{
			Vector3[] edges = [];
			for (int j = 0; j < tempGrid.Length; j++)
			{
				if(!Graph.ContainsKey(tempGrid[i]))
				{
					if ((Mathf.Sqrt(Mathf.Pow((tempGrid[i].X-tempGrid[j].X), 2)+Mathf.Pow((tempGrid[i].Z-tempGrid[j].Z), 2)) <= 2.0f))
					{
						edges.Append(tempGrid[j]);
					}
				}
			}
			Graph[tempGrid[i]] = edges;
		}
		
		//creating a mesh for surface tool
		for(int i = 0; i < tempGrid.Length; i++)
		{
			//get a corner and make a Vector3[] of 2 triangles
		}

		//generate triangle grid from square?

		surfaceArray[(int)Mesh.ArrayType.Vertex] = vertcies.ToArray();
		surfaceArray[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
		surfaceArray[(int)Mesh.ArrayType.Normal] = normals.ToArray();
		surfaceArray[(int)Mesh.ArrayType.Index] = indices.ToArray();

		var arrayMesh = Mesh as ArrayMesh;
		if (arrayMesh != null)
		{
			arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);
		}

	}
	
	public override void _PhysicsProcess(double delta)
	{
		//this.RotateY(Mathf.Pi/180f);
	}
}
