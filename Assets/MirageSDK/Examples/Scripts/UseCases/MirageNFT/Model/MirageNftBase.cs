using System;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MirageSDK.UseCases.MirageNFT
{
	public class MirageNftBase
	{
		public string Id { get; private set; }
		public string Image { get; private set; }
		
		public void Init(MirageNftDto dto)
		{
			Id = dto.Id;
			Image = dto.Image;

			try
			{
				var propertiesToSet = GetType().GetProperties()
					.Where(f => f.GetCustomAttribute<SerializableNftProperty>() != null);

				foreach (var propertyInfo in propertiesToSet)
				{
					var propertyName = propertyInfo.GetCustomAttribute<SerializableNftProperty>().PropertyName;
					var propertyDto = dto.FindPropertyByName(propertyName);
					if (propertyDto != null)
					{
						object typedValue = Convert.ChangeType(propertyDto.Value, propertyInfo.PropertyType);
						propertyInfo.SetValue(this, typedValue);
					}
				}
			}
			catch (InvalidCastException e)
			{
				Debug.LogError("InvalidCastException during NFT attributes deserialization: " + e.Message);
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public override string ToString()
		{
			var propertiesToSet = GetType().GetProperties();

			var sb = new StringBuilder();
			foreach (var propertyInfo in propertiesToSet)
			{
				sb.Append($"{propertyInfo.Name}={propertyInfo.GetValue(this)} ");
			}

			return sb.ToString().TrimEnd();
		}
	}
}