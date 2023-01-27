using System;
using Newtonsoft.Json;

namespace AnkrSDK.MirageAPI.MirageID.Data.CreateUser
{
	[Serializable]
	public class CreateUserRequestDTO
	{
		[JsonProperty(PropertyName = "userName")]
		public string UserName;
		[JsonProperty(PropertyName = "email")]
		public string EMail;
		[JsonProperty(PropertyName = "firstName")]
		public string FirstName;
		[JsonProperty(PropertyName = "lastName")]
		public string LastName;
	}
}