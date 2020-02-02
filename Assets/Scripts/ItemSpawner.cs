using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public float timeBetweenSpawns;
    public bool active;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while (true)
        {          
            if (active)
            {
                Instantiate(GameManager.instance.GetRandomItemPrefab()).transform.position = transform.position;
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
            else
            {
                yield return null;
            }
                
        }      
    }
}
