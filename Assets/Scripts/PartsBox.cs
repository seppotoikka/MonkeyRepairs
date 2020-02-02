using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsBox : MonoBehaviour
{
    Animator animator;
    public Item.ItemType type;
    public Transform icon;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        icon.transform.position = transform.position + (GameManager.camera.transform.position - transform.position).normalized * 10 + Vector3.up * 2;
        icon.LookAt(GameManager.camera.transform);
        Vector3 rot = icon.rotation.eulerAngles;
        icon.rotation = Quaternion.Euler(rot.x, rot.y, 0);
        type = GameManager.instance.GetRandomItemType();
        icon.GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetPickupIcon(type);
    }

    public void Show()
    {
        animator.SetBool("Visible", true);
    }

    public void Hide()
    {
        animator.SetBool("Visible", false);
    }
}
