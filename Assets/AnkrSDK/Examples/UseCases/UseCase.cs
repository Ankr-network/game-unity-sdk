using UnityEngine;

namespace AnkrSDK.UseCases
{
	public abstract class UseCase : MonoBehaviour
	{
		public virtual void ActivateUseCase()
		{
			gameObject.SetActive(true);
		}

		public void DeActivateUseCase()
		{
			gameObject.SetActive(false);
		}
	}
}
