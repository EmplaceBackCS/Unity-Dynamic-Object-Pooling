using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmplaceBackCS
{
	public class TestManager : MonoBehaviour
	{
		/// <summary>
		/// This is just an example script on how to use the objectPoolingManager. You can use this and set it for bullets,
		/// AI, creatures, anything you want! This just shows you the bascics on how to use it for your game
		/// Since the ObjectPoolingManager accepts as many different game objects as you please!
		/// </summary>
		/// 
		//Ref to our pool manager
		private ObjectPoolingManager objectPoolManager;

		// Use this for initialization
		void Start()
		{
			objectPoolManager = ObjectPoolingManager.instance;
		}


		// Update is called once per frame
		void Update()
		{
			//Testing purposes, using our object pooling here...
			//Here you'll want to reuse your object, so you can put the name of the object you put in.
			//Such as Bullet, Ai, whatever you want! My example was just a ImATest/2/3
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				getObject1();
			}


			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				getObject2();
			}
		}

		//==== Get test object 1=====
		//Just will grab prefab 1 to test objectpooling
		void getObject1()
		{
			GameObject temp = objectPoolManager.getPooledObject("ImATest");
			//If our temp is null, well, well we aren't dynamic and there was non inactive!
			//And so we'll return so nothing happens
			if (temp == null) return;
			//We'll set it active to true here and then see the reused object!
			temp.transform.position = Vector3.zero;
			temp.SetActive(true);
		}

		//===== Get test object2 =====
		//Same as object 1. These can be literally anything though!
		void getObject2()
		{
			GameObject temp = objectPoolManager.getPooledObject("ImATest2");
			//If our temp is null, well, well we aren't dynamic and there was non inactive!
			//And so we'll return so nothing happens
			if (temp == null) return;
			//We'll set it active to true here and then see the reused object!
			temp.transform.position = Vector3.zero;
			temp.SetActive(true);
		}
	}
}
