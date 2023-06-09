
using System;

namespace MirageSDK.UseCases.MirageNFT
{
	public class SerializableNftProperty : Attribute
	{
		public string PropertyName { get; }

		public SerializableNftProperty(string propertyName)
		{
			PropertyName = propertyName;
		}
	}
}