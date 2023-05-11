
namespace MirageSDK.UseCases.MirageNFT
{
	public class MirageNftExample : MirageNftBase
	{
		[SerializableNftProperty("hp")]
		public int Hp { get; private set; }
		
		[SerializableNftProperty("ability1")]
		public string Ability1 { get; private set; }
		
		[SerializableNftProperty("ability2")]
		public string Ability2 { get; private set; }
	}
}