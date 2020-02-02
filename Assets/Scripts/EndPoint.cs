using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Item>().part.activeSelf)
            GameManager.instance.ItemDestroyed(other.transform.position);
        Destroy(other.gameObject);
    }
}
