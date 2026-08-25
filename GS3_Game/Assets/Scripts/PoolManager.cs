using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public int size;
        public GameObject prefab;
        public string tag;
    }
    
    public List<Pool> pools = new List<Pool>();

    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            Queue<GameObject> poolQueue = new Queue<GameObject>();
            for(int i = 0; i< pool.size; i++)
            {
                GameObject temp = Instantiate(pool.prefab);
                temp.SetActive(false);
                poolQueue.Enqueue(temp);
            }

            poolDictionary.Add(pool.tag, poolQueue);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rotation)
    {
        if(!poolDictionary.ContainsKey(tag))
        {
            Debug.Log("Tag does not exist!");
            return null;
        }

        GameObject toSpawn = poolDictionary[tag].Dequeue();

        toSpawn.transform.position = pos;
        toSpawn.transform.rotation = rotation;
        toSpawn.SetActive(true);

        poolDictionary[tag].Enqueue(toSpawn);

        return toSpawn;
    }
}
