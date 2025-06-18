using Aphelion.Models;
using Godot;
using System;
using System.Collections.Generic;

namespace Aphelion.UI
{
	public partial class InteractionMenu2D : Control
	{
		[ExportGroup("Nodes")]
		[Export] private VBoxContainer _labelContainer1;

		[Export] private VBoxContainer _labelContainer;


		private Node3D? _selectedObject = null;

		private List<InteractionItemData> _currentData = new List<InteractionItemData>();

		private Int32 _currentSelectionIndex = 0;


        public override void _Process(Double delta)
        {
			if (_selectedObject != null)
			{
                Vector2I windowSize = DisplayServer.WindowGetSize();
                Vector2 objectScreenPosition = GetViewport().GetCamera3D().UnprojectPosition(_selectedObject.GlobalPosition);
				Vector2 menuScreenPosition = new Vector2(
					Mathf.Clamp(objectScreenPosition.X + 128, 0, windowSize.X),
					Mathf.Clamp(objectScreenPosition.Y, 0, windowSize.Y));
				Position = menuScreenPosition;
			}
        }



		/// <summary> Builds the menu with new content. </summary>
		/// <param name="interactionData"> The content to add to the menu. </param>
		public void Build(Node3D selectedObject, InteractionItemData[] interactionData)
		{
			//	Ensure the memory is clear.
			Clear();

			_selectedObject = selectedObject;

			//	Generate each label.
			foreach (InteractionItemData data in interactionData)
			{
				RichTextLabel label = data.InitialiseLabel();
				_labelContainer.AddChild(label);
				_currentData.Add(data);
			}

			MoveSelection(0);   //	Force the selected to update.
		}


		/// <summary> Clear the ui element. </summary>
		public void Clear()
		{
			_selectedObject = null;
			Int32 length = _currentData.Count;
			for (Int32 i = 0; i < length; i++)
			{
				_currentData[i].Label.QueueFree();
			}
			_currentData.Clear();
			_currentSelectionIndex = 0;
		}


		/// <summary> Move the current selected by the given amount. </summary>
		/// <param name="amount"> The relative amount of index to move. </param>
		public void MoveSelection(Int32 amount)
		{
			//	Correctly set index.
			_currentSelectionIndex += amount;
			Int32 length = _currentData.Count;
			if (_currentSelectionIndex > length - 1)
			{
				_currentSelectionIndex = length - 1;
			}
			if (_currentSelectionIndex < 0)
			{
				_currentSelectionIndex = 0;
			}

			//	Colour the selected object.
			for (int i = 0; i < length; i++)
			{
				InteractionItemData data = _currentData[i];
				data.SetSelected(_currentSelectionIndex == i);
			}
		}

		public void ActivateSelectedAction()
		{
			_currentData[_currentSelectionIndex].Method();
		}
	}
}
