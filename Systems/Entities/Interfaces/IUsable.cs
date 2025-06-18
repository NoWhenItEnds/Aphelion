namespace Aphelion.Entities.Interfaces
{
	/// <summary> Gives an entity the ability to be 'used.' This functionality changes between entities, so specific functionality needs to be defined. </summary>
	public interface IUsable : IInteractable
	{
		/// <summary> Activates an entity; whatever that means. </summary>
		public void UseEntity();
	}
}

