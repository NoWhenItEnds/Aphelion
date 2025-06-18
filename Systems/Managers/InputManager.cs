using Aphelion.Controllers;
using Aphelion.Entities.Creatures;
using Aphelion.Entities.Interfaces;
using Godot;
using System;

namespace Aphelion.Managers
{
	public partial class InputManager : Node
	{
		/// <summary> A reference to the UI controller. </summary>
		[ExportGroup("Controllers")]
		[Export] private UIController _uiController;

		/// <summary> A reference to the player-controlled unit. </summary>
		[ExportGroup("Nodes")]
		[Export] private Unit _player;


		/// <summary> How sensitive the mouse is. </summary>
		// TODO - Should be loaded from a config file.
		[ExportGroup("Settings")]
		[Export] private Single _mouseSensitivity = 0.006f;


		private Boolean _isInMenu = false;

		private IInputController? _currentInputController = null;


		public override void _Ready()
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;

			//	Setup events.
			_player.InteractionEntityEntered += _uiController.ShowInteractionMenu;
			_player.InteractionEntityExited += _uiController.HideInteractionMenu;
		}


		/// <summary> Handles input not captured by the UI. </summary>
		public override void _UnhandledInput(InputEvent @event)
		{
			//	Handle inventory.
			if (@event.IsActionReleased("ui_inventory_open"))
			{
				_isInMenu = !_isInMenu;
				Input.MouseMode = _isInMenu ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
			}

			//	Handle looking around.
			if (!_isInMenu && @event is InputEventMouseMotion mouseMotion)
			{
				Vector2 relativeRotation = new Vector2(-mouseMotion.Relative.X * _mouseSensitivity, -mouseMotion.Relative.Y * _mouseSensitivity);
				_player.MovementComponent.RotateLook(relativeRotation);
			}

			//	Control interaction menu if it's open.
			if (_uiController.IsInteractionMenuOpen)
			{
				if (@event.IsActionPressed("ui_selection_backward"))
				{
					_uiController.MoveInteractionMenuSelection(-1);
				}
				else if (@event.IsActionPressed("ui_selection_forward"))
				{
					_uiController.MoveInteractionMenuSelection(1);
				}
				else if (@event.IsActionPressed("ui_selection_accept"))
				{
					_uiController.AcceptInteractionMenuSelection();
				}
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
		
		
		public override void _ExitTree()
		{
			_player.InteractionEntityEntered -= _uiController.ShowInteractionMenu;
			_player.InteractionEntityExited -= _uiController.HideInteractionMenu;
		}
	}
}
