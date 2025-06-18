using Aphelion.Entities.Creatures;
using Godot;
using System;

namespace Aphelion.Controllers
{
	public partial class PlayerController : Node
	{
		/// <summary> A reference to the player-controlled unit. </summary>
		[ExportGroup("Nodes")]
		[Export] private Unit _player;


		/// <summary> How sensitive the mouse is. </summary>
		// TODO - Should be loaded from a config file.
		[ExportGroup("Settings")]
		[Export] private Single _mouseSensitivity = 0.006f;


		/// <summary> The actual direction the character wants to move relative to itself. </summary>
		private Vector3 _actualDirection = Vector3.Zero;


		public override void _Ready()
		{
		}


        /// <summary> Handles input not captured by the UI. </summary>
        public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton)
			{
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
			else if (@event.IsActionPressed("ui_cancel"))
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}

			if (Input.MouseMode == Input.MouseModeEnum.Captured && @event is InputEventMouseMotion mouseMotion)
			{
				Vector2 relativeRotation = new Vector2(-mouseMotion.Relative.X * _mouseSensitivity, -mouseMotion.Relative.Y * _mouseSensitivity);
				_player.MovementComponent.RotateLook(relativeRotation);
			}
		}

		public override void _PhysicsProcess(Double delta)
		{
			if (Input.IsActionPressed("action_sprint"))
			{
				_player.MovementComponent.TrySetUnitState(Types.States.UnitState.SPRINTING);
			}
			else if (Input.IsActionPressed("action_crouch"))
			{
				_player.MovementComponent.TrySetUnitState(Types.States.UnitState.CROUCHING);
			}
			else
			{
				_player.MovementComponent.TrySetUnitState(Types.States.UnitState.WALKING);
			}

			Vector2 moveDirection = Input.GetVector("action_move_left", "action_move_right", "action_move_forward", "action_move_backward").Normalized();
			Single jumpDirection = Input.IsActionPressed("action_jump") ? 1f : 0f;
			Vector3 inputDirection = new Vector3(moveDirection.X, jumpDirection, moveDirection.Y);
			_player.MovementComponent.PhysicsMove(delta, inputDirection);
		}
	}
}