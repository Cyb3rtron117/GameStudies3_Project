using UnityEngine;
using UnityEngine.InputSystem;

public class CubeSpawner : MonoBehaviour
{
    public void SpawnInput(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            SpawnCube();
        }
    }
    public void SpawnCube()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        PoolManager.Instance.SpawnFromPool("cube", spawnPos, Quaternion.identity);
    }
}
