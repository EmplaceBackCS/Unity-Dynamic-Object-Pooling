using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmplaceBackCS
{
	/// <summary>
	/// Just a test script for ImATest/2/3 prefabs. This just moves the cube and scales up in size! and on disable, it'll
	/// Reset the timer so it will be alive again for 5 seconds
	/// </summary>
	public class IAmATestScript : MonoBehaviour
	{
		//A timer before we 'destroy' this game object
		float timer = 5.0f;
		float defaultTimer;
		float speed = 0.4f;
		Vector3 scaleSize = new Vector3(0.5f, 0.5f, 0.5f);
		
		void Start()
		{
			defaultTimer = timer;
		}
		
		private void Update()
		{
			//Just going to move this foward and incrase it's size
			moveAndScaleObject();
			//Once our timer reachs 0, we will disable this
			checkTimer();
		}

		//MoveAndScale this object
		void moveAndScaleObject()
		{
			this.transform.Translate(-Vector3.forward * Time.fixedDeltaTime * speed);
			this.transform.localScale += scaleSize * Time.fixedDeltaTime;

		}

		//Just to check when we should disable this
		void checkTimer()
		{
			if (timer <= 0.0f)
			{
				this.gameObject.SetActive(false);
			}
			else
			{
				timer -= Time.deltaTime;
			}
		}


		private void OnDisable()
		{
			this.transform.localScale = Vector3.one;
			timer = defaultTimer;
		}
	}
}
