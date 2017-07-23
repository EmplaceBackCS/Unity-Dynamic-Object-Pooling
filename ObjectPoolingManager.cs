using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmplaceBackCS
{
	public class ObjectPoolingManager : MonoBehaviour
	{
		//A small class to make this easier and more flexible for our pool to hold many game objects
		[System.Serializable]
		public class PooledObject
		{
			//Just going to hold a prefab and the size, so in the start method, we can loop through all of these and
			//Create a pool with the desired size
			public GameObject prefab;
			public ushort poolSize;
		};
		//======== Strings =======
		[Header("If you have only a resorce folder with no sub folders, don't touch this,", order = 0)]
		[Space(1, order = 1)]
		[Header("else rename it to what you named your prefab subfolder", order = 2)]
		[Space(5, order = 3)]
		[SerializeField]
		private string filePath = "EMPTY"; //To find our file path inside resoruces to get our prefabs
		//======== Bools =======
		[Header("This will make it so if we run out of objects, we'll create a new one and add it to our pool")]
		[SerializeField]
		private bool dynamic = true;
		//If you use the pool and don't have a prefab in the class PooledObject, this will search for the prefab
		//In the resoruce folder
		[SerializeField] private bool useResourceFolder = false; 
		//======== Object of our class above ======
		public PooledObject[] pooledObjects;
		//======== Lists ========
		private List<GameObject> listOfPooledObjects = new List<GameObject>();
		//====Ref of this object... =====
		public static ObjectPoolingManager instance;

		//Set up our singlton to this on our awake!
		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			//Fill our pool up!
			for (int i = 0; i < pooledObjects.Length; i++)
			{
				createPool(pooledObjects[i].prefab, pooledObjects[i].poolSize);
			}
		}

		//============ Create pool ==============
		//This should be called in a start method so that it can create any number of pools desired for any object
		public void createPool(GameObject prefab, int poolSize)
		{
			//Create a Game object here so we can use this as the parent for the pool we are creating
			//This way we can keep it organized and neat in the editor view!
			GameObject poolHolder = new GameObject(prefab.name + " Pool");
			//And we will just set the parent of this to the manager game object
			poolHolder.transform.parent = this.transform;
			//Loop through the pool size to instantiate the number of prefabs desired
			for (int i = 0; i < poolSize; i++)
			{
				GameObject newObject = Instantiate(prefab);
				//Rename it here, so that way we won't have (Clone) at the end of the prefab and it makes it easier
				//To search for
				newObject.name = prefab.name;
				//Set it active to false so we don't see it when we create it!
				newObject.SetActive(false);
				//Set the parent here once we create the game object
				newObject.transform.parent = poolHolder.transform;
				//Then add it to our list
				listOfPooledObjects.Add(newObject);
			}

			//Debug purposes...
			Debug.Log("Number of objects created for " + 
				poolHolder.transform.name + 
				" was: " +
				poolHolder.transform.childCount);
		}

		//============= Get pooled object ===============
		//This will return the game object to reuse, and if dynamic is checked
		//It will create a new game object if the pool has ran out!
		//This will also just create a pool if you still use my manager and have the array size of 0 for pooledObjects
		public GameObject getPooledObject(string prefabName)
		{
			//Loop through our list to find our game object requested....
			for (int i = 0; i < listOfPooledObjects.Count; i++)
			{
				//If we find our game object using the name and it's not active
				if (listOfPooledObjects[i].name == prefabName && !listOfPooledObjects[i].activeInHierarchy)
				{
					//We'll return it to reuse it!
					return listOfPooledObjects[i];
				}
			}

			//Else if we got here and we are dynamic, we'll create a new one!
			if (dynamic)
			{
				GameObject newObject = null;

				//Loop through all the array of pooledObjects to search for the prefab to incrase it's size
				for (int i = 0; i < pooledObjects.Length; i++)
				{
					if (pooledObjects[i].prefab.gameObject.name == prefabName)
					{
						newObject = Instantiate(pooledObjects[i].prefab);
						newObject.SetActive(false);
					}
				}

				if (useResourceFolder && newObject == null)
				{
					Debug.Log("Searching resoruce folder for item now instead since we were unable to find a pooledObject");
					if (filePath == "EMPTY")
					{
						//We'll search for the game object in our Resources folder(Make sure to have a folder
						//Named Resources! Or else you'll get an error if you try to 
						//Call something that doesn't exsit in the current scene!)
						newObject = (GameObject)Instantiate(Resources.Load(prefabName));
						newObject.SetActive(false);
					}
					else
					{
						newObject = (GameObject)Instantiate(Resources.Load(filePath + "/" + prefabName));
						newObject.SetActive(false);
					}

					if (newObject == null)
					{
						Debug.LogError("You gave an invalid objectName!");
						return null;
					}
				}
				//Just so if dynamic is checked, but we didn't find anything, we'll only do this if we found a game object
				//That way, this won't crash and instead just nothing will happen
				if (newObject != null)
				{
					//Set the name up here again so we don't get that extra "(Clone)" added to our object
					newObject.name = prefabName;

					//See if we can find a parent with that name already to orgainize it in the editor...
					GameObject tempParent = GameObject.Find(prefabName + " Pool");

					//And if it's not null, we'll set the parent to that!
					if (tempParent != null)
					{
						//To keep the editor clean
						newObject.transform.parent = tempParent.transform;

						//To see how many total we have after expanding...
						Debug.Log("Added another object to the pool " +
							tempParent.gameObject.name +
							". Total number of pooled objects for this is now: " +
							tempParent.transform.childCount);
					}
					else
					{
						Debug.Log("TempParent was null, so we are creating a new one for this new object!");
						//Else, we'll just make a new parent here
						tempParent = new GameObject(prefabName + " Pool");
						tempParent.transform.parent = this.transform;
						newObject.transform.parent = tempParent.transform;
					}

					//Then finally, we will add it to our list!
					listOfPooledObjects.Add(newObject);
					//And then return it so we can use it!
					return listOfPooledObjects[listOfPooledObjects.Count - 1];
				}
			}

			//If dynamic is false, then we'll we'll just return null and we'll be stuck waiting for one to not be active
			return null;
		}
	}
}
