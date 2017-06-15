using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public struct ObjectInfo
{
	public int poolSizeMin;
	public GameObject prefab;
	public ArrayList objectArray;
}

public class ObjectsPoolManager : MonoBehaviour
{
	private Dictionary<string, ObjectInfo> gameElementPools = new Dictionary<string, ObjectInfo>();

	public static ObjectsPoolManager Instance { get { return _instance; } }
	private static ObjectsPoolManager _instance;

	void Awake()
	{
		_instance = this;
	}

	public GameObject GetObjectOfPrefabType(string prefabName, Transform parent, Vector3 localPosition)
	{
		ObjectInfo pool;

		try
		{   
			Debug.Log("GilLog - ObjectsPoolManager::GetObjectOfPrefabType - prefabName " + prefabName + " parent " + parent + " localPosition " + localPosition + " ");
			pool = gameElementPools[prefabName];
			Debug.Log("GilLog - ObjectsPoolManager::GetObjectOfPrefabType - prefabName " + prefabName + " parent " + parent + " localPosition " + localPosition + "  - pool " + pool + " ");
		}
		catch (KeyNotFoundException)
		{
			//Debug.LogError("GameElementsPoolsManager => key \"" + prefabName + "\" not found.");
			////Debug.LogException(e);
			return null;
		}

		GameObject go = FindGameObjectInPool(pool, parent, localPosition);

		return go;
	}

	public GameObject GetObjectOfPrefabType(GameObject prefab, Transform parent, Vector3 localPosition)
	{
		ObjectInfo pool;

		try
		{
			pool = gameElementPools[prefab.name];
		}
		catch (KeyNotFoundException)
		{
			//Debug.LogError("GameElementsPoolsManager => key \"" + prefab.name + "\" not found.");
			//Debug.LogException(e);
			return null;
		}

		GameObject go = FindGameObjectInPool(pool, parent, localPosition);

		return go;
	}

	public void CreatePool(string prefabName, int poolSizeMin)
	{
		GameObject prefab = Resources.Load<GameObject>(prefabName);

		if (prefab == null)
		{
			Debug.LogError("Unable to load prefab at path " + prefabName);
			return;
		}

		CreatePool(prefabName, prefab, poolSizeMin);
	}

	public void CreatePool(string prefabName, GameObject prefab, int poolSizeMin)
	{
		if (!gameElementPools.ContainsKey(prefabName))
		{
			ObjectInfo pool = new ObjectInfo();
			pool.prefab = prefab;
			pool.poolSizeMin = poolSizeMin;
			pool.objectArray = new ArrayList();
			gameElementPools.Add(prefabName, pool);
		}

		for (int i = 0; i < poolSizeMin; i++)
		{
			GameObject go = GameObject.Instantiate(prefab) as GameObject;
			go.transform.parent = transform;

			PoolObject(go, prefabName, true);
		}
	}

	public void PoolObject(GameObject go, string prefabName, bool disable = true)
	{
		ObjectInfo pool;

		try
		{
			pool = gameElementPools[prefabName];
		}
		catch (KeyNotFoundException)
		{
			//if (!LevelEditorGlobals.IsInEditor)
			Debug.LogError("GameElementsPoolsManager => key \"" + prefabName + "\" not found.");
			return;
		}

		PoolObject(pool.objectArray, go, disable);
	}

	private void PoolObject(ArrayList pool, GameObject go, bool disable = true)
	{
		if (disable) {
			go.BroadcastMessage("OnPoolObject", SendMessageOptions.DontRequireReceiver);
			go.SetActive(false);
		}

		go.transform.parent = transform;

		pool.Add(go);
	}

	private void UnpoolObject(ArrayList pool, GameObject go, Transform parent, Vector3 localPosition)
	{
		pool.Remove(go);

		// go.transform.parent = parent;
		go.transform.localPosition = localPosition;

		go.SetActive(true);

		go.BroadcastMessage("OnUnpoolObject", SendMessageOptions.DontRequireReceiver);
	}

	private GameObject FindGameObjectInPool(ObjectInfo pool, Transform parent, Vector3 localPosition)
	{
		for (int i = 0; i < pool.objectArray.Count; i++)
		{
			GameObject tempGO = pool.objectArray[i] as GameObject;

			if (tempGO != null && !tempGO.activeInHierarchy)
			{
				UnpoolObject(pool.objectArray, tempGO, parent, localPosition);
				return tempGO;
			}
		}

		GameObject go = GameObject.Instantiate(pool.prefab) as GameObject;

		PoolObject(pool.objectArray, go, false);

		go.transform.parent = parent;
		go.transform.localPosition = localPosition;

		return go;
	}
}
