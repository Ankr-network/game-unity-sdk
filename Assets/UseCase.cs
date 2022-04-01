using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
