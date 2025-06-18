using System;
using Aphelion.Models;
using Aphelion.UI;
using Godot;

namespace Aphelion.Controllers
{
	public partial class UIController : Control
	{
		/// <summary> A reference to interaction menu node. </summary>
		[ExportGroup("Nodes")]
		[Export] private InteractionMenu2D _interactionMenu2D;

		public Boolean IsInteractionMenuOpen { get; private set; } = false;


		public void MoveInteractionMenuSelection(Int32 relativeAmount)
		{
			_interactionMenu2D.MoveSelection(relativeAmount);
		}


		public void AcceptInteractionMenuSelection()
		{
			_interactionMenu2D.ActivateSelectedAction();
		}


		public void ShowInteractionMenu(Node3D sourceNode, InteractionItemData[] interactionData)
		{
			_interactionMenu2D.Build(sourceNode, interactionData);
			IsInteractionMenuOpen = true;
		}

		public void HideInteractionMenu(Node3D sourceNode)
		{
			_interactionMenu2D.Clear();
			IsInteractionMenuOpen = false;
		}
    }
}
