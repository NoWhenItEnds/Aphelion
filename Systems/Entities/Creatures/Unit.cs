using Aphelion.Entities.Creatures.Components;
using Aphelion.Entities.Interfaces;
using Aphelion.Models;
using Godot;
using System;
using System.Collections.Generic;

namespace Aphelion.Entities.Creatures
{
	public partial class Unit : CharacterBody3D
	{
		[ExportGroup("Nodes")]
		[Export] private Node3D _headNode;

		[Export] private Camera3D _firstPersonCamera;

		[Export] private Area3D _interactionArea;

		[Export] public MovementComponent MovementComponent;


		[ExportGroup("Settings")]
		[Export] private Single _jumpVelocity = 6f;


		public Action<Node3D, InteractionItemData[]> InteractionEntityEntered;

		public Action<Node3D> InteractionEntityExited;


		private IInteractable? _currentInteractable = null;


		public override void _Ready()
		{
			_interactionArea.BodyEntered += OnInteractionBodyEntered;
			_interactionArea.BodyExited += OnInteractionBodyExited;
		}

		private void OnInteractionBodyEntered(Node3D body)
		{

			if (body is IInteractable interactable)
			{
				_currentInteractable = interactable;
				List<InteractionItemData> iteractionData = new List<InteractionItemData>();
				iteractionData.Add(new InteractionItemData("Examine", interactable.DescribeEntity));
				if (body is IUsable usable)
				{
					iteractionData.Add(new InteractionItemData("Use", usable.UseEntity));
				}

				InteractionEntityEntered?.Invoke(body, iteractionData.ToArray());
			}
		}


		private void OnInteractionBodyExited(Node3D body)
		{
			if (body is IInteractable interactable && interactable == _currentInteractable)
			{
				_currentInteractable = null;
				InteractionEntityExited?.Invoke(body);
			}
		}
		

		public override void _ExitTree()
		{
			_interactionArea.BodyEntered -= OnInteractionBodyEntered;
			_interactionArea.AreaExited -= OnInteractionBodyExited;
		}
	}
}
