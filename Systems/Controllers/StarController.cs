using Aphelion.Entities.Celestial;
using Aphelion.Models;
using Aphelion.Models.Extensions;
using Godot;
using System;

namespace Aphelion.Controllers
{
	public partial class StarController : Node3D
	{
		[ExportGroup("Nodes")]
		[Export] private Node3D _starParentNode;

		[Export] private PackedScene _starPrefab;

		private StarData[] _stars;

		public override void _Ready()
		{
			_stars = StarDataExtensions.LoadFromFile();
			foreach (StarData star in _stars)
			{
				Star newNode = _starPrefab.InstantiateOrNull<Star>();
				_starParentNode.AddChild(newNode);
				newNode.Name = $"Star_{star.CatalogNumber}";
				newNode.Initialise(star);
			}
		}
	}
}

