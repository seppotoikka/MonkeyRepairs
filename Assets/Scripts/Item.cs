using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public enum ItemType { Microwave, Fan, Lawnmower, Gramophone, Washer }

    public ItemType type;
    public GameObject part;
    public GameObject particles;
    
    Rigidbody rb;
    readonly float moveSpeedPerSecond = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + Vector3.right * moveSpeedPerSecond * Time.fixedDeltaTime);
    }

    public bool Repair(Item.ItemType type)
    {
        if (part.activeSelf)
            return false;

        if (type == this.type)
        {
            part.SetActive(true);
            particles.SetActive(true);
            GameManager.instance.ItemRepaired();
            return true;
        }

        GameManager.instance.ItemDestroyed(transform.position);
        Destroy(gameObject, 0.2f);
        return false;
    }
}
