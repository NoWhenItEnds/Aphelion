using Godot;
using System;

namespace Aphelion.Models
{
	public record InteractionItemData
	{
		public String Text { get; init; }

		public Action Method { get; init; }

		public RichTextLabel Label { get; private set; }

		private const String SELECTED_FORMAT = "[color=#{2}]>[/color] [color=#{1}]{0}[/color]";

		private const String NONSELECTED_FORMAT = "  [color=#{1}]{0}[/color]";

		private Color CURSOR_COLOUR = new Color(1f, 0f, 0f);

		private Color TEXT_COLOUR = new Color(1f, 1f, 1f);


		public InteractionItemData(String text, Action method)
		{
			Text = text;
			Method = method;
		}

		public RichTextLabel InitialiseLabel()
		{
			Label = new RichTextLabel();
			Label.Text = String.Format(NONSELECTED_FORMAT, Text, TEXT_COLOUR.ToHtml());
			Label.FitContent = true;
			Label.BbcodeEnabled = true;
			return Label;
		}

		public void SetSelected(Boolean isSelected)
		{
			Label.Text = isSelected ? String.Format(SELECTED_FORMAT, Text, TEXT_COLOUR.ToHtml(), CURSOR_COLOUR.ToHtml()) : String.Format(NONSELECTED_FORMAT, Text, TEXT_COLOUR.ToHtml());
		}
	}
}
