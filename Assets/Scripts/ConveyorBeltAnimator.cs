using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltAnimator : MonoBehaviour
{
    public Material beltMaterial;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BeltMover());
    }

    IEnumerator BeltMover()
    {
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            beltMaterial.SetTextureOffset("_BaseMap", new Vector2((timer / 1.95f) % 1, 0));
            yield return null;
        }
    }
}
