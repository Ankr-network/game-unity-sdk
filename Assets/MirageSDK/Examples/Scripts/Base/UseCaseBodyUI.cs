using UnityEngine;

namespace MirageSDK.Base
{
	public abstract class UseCaseBodyUI : MonoBehaviour
	{
		public virtual void SetUseCaseBodyActive(bool isActive)
		{
			gameObject.SetActive(isActive);
		}
	}
}
