using UnityEngine;

namespace AnkrSDK.Base
{
	public abstract class UseCaseBodyUI : MonoBehaviour
	{
		public virtual void SetUseCaseBodyActive(bool active)
		{
			gameObject.SetActive(active);
		}
	}
}
