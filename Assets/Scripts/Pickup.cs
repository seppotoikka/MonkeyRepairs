using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    Rigidbody rb;
    public Item.ItemType type;
    public GameObject breakEffect;
    public Transform leftHandIKTarget;
    public Transform rightHandIKTarget;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!rb.isKinematic)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Items") && other.GetComponent<Item>().Repair(type))
            {
                // Yay!
            }
            else
            {
                Instantiate(breakEffect).transform.position = transform.position;
                GameManager.instance.PartsDestroyed();
            }
            Destroy(gameObject);
        }        
    }
}
