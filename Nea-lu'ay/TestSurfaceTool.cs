using Godot;
using System;

public partial class TestSurfaceTool : MeshInstance3D
{
	public override void _Ready()
	{
		
		Vector3[] vertices =
		[
			new Vector3(0, 2, 0),
			new Vector3(2, 0, 0),
			new Vector3(0, 0, 2),
		];
		
		// Initialize the ArrayMesh.
		var arrMesh = new ArrayMesh();
		Godot.Collections.Array arrays = [];
		arrays.Resize((int)Mesh.ArrayType.Max);
		arrays[(int)Mesh.ArrayType.Vertex] = vertices;
		
		// Create the Mesh.
		arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
		//var m = new MeshInstance3D();
		this.Mesh = arrMesh;
	}
	
	public override void _Process(double delta)
	{
		this.RotateY(0.5f);
	}
}
