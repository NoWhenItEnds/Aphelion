using Aphelion.Entities.Interfaces;
using Godot;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aphelion.Entities.Machines
{
	public partial class Telescope : RigidBody3D, IUsable, IInputController
	{
		/// <summary> A reference to the camera node the telescope uses. </summary>
		[ExportGroup("Nodes")]
		[Export] private Camera3D _camera;

		/// <summary> A reference to the base the telescope will yaw around. </summary>
		[Export] private Node3D _base;

		/// <summary> A reference to the scope the telescope will pitch. </summary>
		[Export] private Node3D _scope;
		

		[ExportGroup("Settings")]
		[Export, Range(0f, 50f)] private Single _minZoomAmount = 50f;

		[Export, Range(0f, 50f)] private Single _maxZoomAmount = 0f;



        public override void _PhysicsProcess(double delta)
		{
			//current -= 0.001f;
			SetZoomAmount(0.1f);
		}

		public void SetZoomAmount(Single amount)
		{
			Single zoomLevel = Mathf.Lerp(_maxZoomAmount, _minZoomAmount, amount);
			_camera.Fov = zoomLevel;
		}

		public void RotatePitch(Single relativeDegree)
		{
			Single newValue = Mathf.Clamp(_scope.RotationDegrees.X + relativeDegree, -80, 80);
			_scope.RotationDegrees = new Vector3(newValue, 0f, 0f);
		}


		public void RotateYaw(Single relativeDegree)
		{
			_base.RotateY(Mathf.DegToRad(relativeDegree));
		}


		/// <inheritdoc/>
		public void DescribeEntity()
		{
			GD.Print($"{Name} was examined.");
			RotatePitch(-10f);
		}


		/// <inheritdoc/>
		public void UseEntity()
		{
			GD.Print($"{Name} was used.");
			RotateYaw(10f);
		}
    }
}