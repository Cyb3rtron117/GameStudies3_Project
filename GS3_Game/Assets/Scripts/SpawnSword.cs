using UnityEngine;

public class SpawnSword : MonoBehaviour
{
    public PickupItem[] swords;
    public bool spawn = false;
    public GameObject emptyPrefab;
    //if press button

    private void Update()
    {
        if(spawn)
        {
            spawn = false;
            GameObject temp = Instantiate(emptyPrefab, this.transform);
            int num = Random.Range(0, swords.Length);
            temp.GetComponent<MeshFilter>().mesh = swords[num].mesh;
            temp.GetComponent<MeshRenderer>().material = swords[num].material;

            Debug.Log($"Spawned the {swords[num].name}");
        }
    }
}
