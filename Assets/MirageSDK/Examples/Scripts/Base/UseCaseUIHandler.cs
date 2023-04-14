using System.Collections.Generic;
using UnityEngine;

namespace MirageSDK.Base
{
	public class UseCaseUIHandler : MonoBehaviour, IUseCaseUIController
	{
		[SerializeField] private List<UseCaseUI> _useCaseUIs;

		private void Awake()
		{
			foreach (var useCaseUi in _useCaseUIs)
			{
				useCaseUi.Setup(this);
			}
		}

		public void SetUseCaseButtonsActive(bool isActive)
		{
			_useCaseUIs.ForEach(item => item.SelectButton.gameObject.SetActive(isActive));
		}
	}
}