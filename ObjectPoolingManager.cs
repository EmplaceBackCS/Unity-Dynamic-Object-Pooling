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
			private GameObject poolHolder; //So we can make a little home in the editor for our pooled objects. Makes it look clean!
			[HideInInspector] public GameObject prefab;
			private bool hasParent; //This is so we can create the parent only once and won't have to keep searching for the name
			public List<GameObject> pooledObjectList;

			//=============== Constructors ===================
			public PooledObject(GameObject _prefab, bool _hasParent = false)
			{
				prefab = _prefab;
				pooledObjectList = new List<GameObject>();
				hasParent = _hasParent;
				poolHolder = null;
			}

			//========== Fill List ==========
			//This will fill our list inside the class so it'll be easiesr to search for when we have many
			//Prefabs in this pool!
			public void fillList(ushort _poolSize)
			{
				//Create a parent for this list so it will be neat in the editor!
				if (!hasParent)
				{
					//If this object was just created, this should be false so we can create a parnet for it
					poolHolder = new GameObject(prefab.name + " Pool");
					poolHolder.transform.parent = instance.transform;
					//We'll set hasParent to true here so that we won't have many parents for this object
					hasParent = true;
				}
				
				for (int i = 0; i < _poolSize; i++)
				{
					//We'll loop through and create a new object as requested
					GameObject newObject = Instantiate(prefab);
					//Set it active to false so we don't see it when we create it!
					newObject.SetActive(false);
					//Rename it here, so that way we won't have (Clone) at the end of the prefab and it makes it easier
					//To search for
					newObject.name = prefab.name;
					//Set the parent here once we create the game object
					newObject.transform.parent = poolHolder.transform;
					//Then add it to our list!
					pooledObjectList.Add(newObject);
				}
			}
		};

		//mini class to hold prefabs as like a reference
		[System.Serializable]
		public class GameObjectPrefabs
		{
			public GameObject prefab;
			public ushort pooledAmount;
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
		//========= ushorts ===============
		private ushort currentIndex = 0;
		private ushort dynamicIncraseAmount = 1;
		private ushort resourceFolderObjectPoolAmount = 5;
		//======== Object of our class above ======
		public GameObjectPrefabs[] gameObjectPrefab; //This will be just a reference to the prefabs so we can create them
		//======== Lists ========
		[SerializeField] private List<PooledObject> pooledObjects = new List<PooledObject>();
		//====Ref of this object... =====
		public static ObjectPoolingManager instance;

		private void OnValidate()
		{
			if(pooledObjects.Count > 0)
			{
				Debug.LogError("Don't touch this! This is suppose to just be a visual aid to see how many objects you have pooled when dynamic is checked and such.");
				pooledObjects.Clear();
			}
		}

		//Set up our singlton to this on our awake!
		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			//Fill up our pool here
			for (int i = 0; i < gameObjectPrefab.Length; i++)
			{
				createPool(gameObjectPrefab[i].prefab, gameObjectPrefab[i].pooledAmount);
			}
		}

		//========== Create pool ================
		//This should be called in a start method so that it can create any number of pools desired for any object
		private void createPool(GameObject prefab, ushort poolSize)
		{
			pooledObjects.Add(new PooledObject(prefab));
			pooledObjects[currentIndex++].fillList(poolSize);
		}

		//============== get pooled object! ========
		//Will search like a rows/columns. This is because we can have many objects in our pool
		public GameObject getPooledObject(string prefabName)
		{
			//First we'll loop through every game object we put into the manager
			for (int y = 0; y < pooledObjects.Count; y++)
			{
				//To simply just search for the name of the prefab.
				if (pooledObjects[y].prefab.name == prefabName)
				{
					//Than once we get here and find the name, we'll search for an inactive one there!
					for(int x = 0; x < pooledObjects[y].pooledObjectList.Count; x++)
					{
						if (!pooledObjects[y].pooledObjectList[x].activeInHierarchy)
						{
							return pooledObjects[y].pooledObjectList[x];
						}
					}
				}
			}
			//If we are dynamic and want to incrase the size, we'll do so here
			if (dynamic)
			{
				//By looping through our pool to find the prefab we need
				for (int i = 0; i < pooledObjects.Count; i++)
				{
					if (pooledObjects[i].prefab.name == prefabName)
					{
						//Then we'll fill the list with another prefab here
						pooledObjects[i].fillList(dynamicIncraseAmount);
						//Then return the last element, since we just created it!
						return pooledObjects[i].pooledObjectList[pooledObjects[i].pooledObjectList.Count - 1];
					}
				}

				if (useResourceFolder)
				{
					GameObject newObject = null;

					Debug.Log("Searching resoruce folder for item now instead since we were unable to find a pooledObject of the name: " + prefabName);
					if (filePath == "EMPTY")
					{
						//We'll search for the game object in our Resources folder(Make sure to have a folder
						//Named Resources! Or else you'll get an error if you try to 
						//Call something that doesn't exsit in the current scene!)
						newObject = (GameObject)(Resources.Load(prefabName));
						//Set it active to false so we don't see it when we create it
						newObject.SetActive(false);
						//Don't forget to chang ethe name here, so "(Clone)" won't be added to the end of the name
						newObject.name = prefabName;
						createPool(newObject, resourceFolderObjectPoolAmount);
						//Then once we create our pool for this new object, we'll just return the last subscript of our list and the
						//First subscript of our pooled list since that is inactive
						return pooledObjects[pooledObjects.Count - 1].pooledObjectList[0];
					}
					else
					{
						//Grab our prefab from the file path given
						newObject = (GameObject)(Resources.Load(filePath + prefabName));
						//Set it active to false so we don't seee it
						newObject.SetActive(false);
						newObject.name = prefabName;
						createPool(newObject, resourceFolderObjectPoolAmount);
						//Then once we create our pool for this new object, we'll just return the last subscript of our list and the
						//First subscript of our pooled list since that is inactive
						return pooledObjects[pooledObjects.Count - 1].pooledObjectList[0];
					}

				}

				Debug.Log("We are dynamic, but we could not find the object asked for(AndUseResourceFolder was false), so we returned null!");
			}
			//If we aren't dynamic, we'll just return null so we will have to wait for one to reuse
			return null;
		}

	}
}
