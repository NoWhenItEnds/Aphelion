using Aphelion.Models;
using Godot;
using System;

namespace Aphelion.Entities.Celestial
{
	/// <summary> A Godot-based star object. Represents star data in the game world. </summary>
	public partial class Star : Node3D
	{
		/// <summary> The 2D mesh used to render a star at a distance. </summary>
		[ExportGroup("Nodes")]
		[Export] private MeshInstance3D _mesh2D;

		/// <summary> The material to use when rendering a star at a distance. </summary>
		[Export] private ShaderMaterial _star2DMaterial;

		/// <summary> The real-world data used to build this star object. </summary>
		private StarData _data;


		/// <summary> Initialise a star object using the provided real-world data. </summary>
		/// <param name="data"></param>
		public void Initialise(StarData data)
		{
			_data = data;

			GlobalPosition = data.Position * 25000f;

			Random random = new Random();
			Generate2DMesh(random);
		}


		/// <summary> Uses star data to generate the object's 2D mesh. </summary>
		/// <param name="random"> A reference to the random used to generate fields. </param>
		private void Generate2DMesh(Random random)
		{
			ShaderMaterial material = (ShaderMaterial)_star2DMaterial.Duplicate();
			material.SetShaderParameter("colour", _data.Colour);
			Single thickness = Mathf.Lerp(5f, 15f, _data.Scale);
			material.SetShaderParameter("thickness", thickness);
			Single relativeSize = Mathf.Lerp(0.1f, 1f, _data.Scale);
			material.SetShaderParameter("relative_size", relativeSize);
			Single rotationSpeed = Mathf.Lerp(-3f, 3f, random.NextSingle());
			material.SetShaderParameter("rotation_speed", rotationSpeed);
			_mesh2D.SetSurfaceOverrideMaterial(0, material);
		}
	}
}