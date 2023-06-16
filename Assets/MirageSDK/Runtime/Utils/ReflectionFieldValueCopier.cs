using System.Linq;

namespace MirageSDK.Utils
{
	/// <summary>
	/// This static utility class is intended to copy a state of an instance of type
	/// TSource to an instance of type TTarget for all the public instance fields having the same name
	/// </summary>
	public static class ReflectionFieldValueCopier
	{
		public static TTarget CopyFieldValues<TSource, TTarget>(TSource source)
			where TSource : class
			where TTarget : class, new()
		{
			var target = new TTarget();
			var sourceType = source.GetType();
			var targetType = target.GetType();
			var sourceFields = sourceType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			var targetFields = targetType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

			foreach (var sourceField in sourceFields)
			{
				var targetField = targetFields.FirstOrDefault(f => f.Name == sourceField.Name);
				if (targetField != null)
				{
					targetField.SetValue(target, sourceField.GetValue(source));
				}
			}
			return target;
		}
	}
}