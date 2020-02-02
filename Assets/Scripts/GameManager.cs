using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public new static Camera camera;
    public static GameManager instance;
    public Transform partBoxSpawnPoints;
    public GameObject boxPrefab;
    public GameObject explosionPrefab;
    public ItemData[] items;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI destroyedItemsText;

    int numberOfBoxesSpawned;
    int score;
    int numberOfPartsDestroyed;
    int numberOfItemsDestroyed;
    int numberOfItemsRepaired;
    int numberOfItemsMissed;

    List<BoxLocation> spawnedBoxes;

    List<Item.ItemType> itemTypes;

    PlayerController[] players;

    public GameObject gameOverMenu;

    private void Awake()
    {
        camera = Camera.main;
        instance = this;
        spawnedBoxes = new List<BoxLocation>();
        itemTypes = new List<Item.ItemType>();
        foreach (Item.ItemType t in System.Enum.GetValues(typeof(Item.ItemType)))
            itemTypes.Add(t);

        players = FindObjectsOfType<PlayerController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BoxSpawner());
    }

    bool quit;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !quit)
        {
            quit = true;
            LoadMainMenu();
        }
    }

    void SpawnBox()
    {
        bool done = false;
        int location = 0;
        while (!done) // Gotta love infinite loops
        {
            done = true;
            location = Random.Range(0, partBoxSpawnPoints.childCount);
            foreach (BoxLocation bl in spawnedBoxes)
            {
                if (bl.location == partBoxSpawnPoints.GetChild(location))
                    done = false;
            }
        }
        Transform loc = partBoxSpawnPoints.GetChild(location);
        GameObject box = Instantiate(boxPrefab);
        box.transform.position = loc.position;
        spawnedBoxes.Add(new BoxLocation(box.transform, loc));
        AudioManager.instance.PickupSpawnSound();
    }

    public Item.ItemType GetRandomItemType()
    {
        return itemTypes[Random.Range(0, itemTypes.Count)];
    }

    public GameObject GetRandomItemPrefab()
    {
        return items[Random.Range(0, items.Length)].itemPrefab;
    }

    public GameObject GetPartPrefab(Item.ItemType type)
    {
        foreach (ItemData id in items)
        {
            if (id.type == type)
                return id.sparePartPrefab;
        }
        return null;
    }

    public Sprite GetPickupIcon(Item.ItemType type)
    {
        foreach (ItemData id in items)
        {
            if (id.type == type)
                return id.icon;
        }
        return null;
    }

    public void BoxPickedUp(Transform box)
    {
        for (int i = spawnedBoxes.Count - 1; i >= 0; i--)
        {
            if (spawnedBoxes[i].box == box)
                spawnedBoxes.RemoveAt(i);
        }
    }

    public void PartsDestroyed()
    {
        numberOfPartsDestroyed++;
        AudioManager.instance.BreakSound();
    }

    public void ItemRepaired()
    {
        score++;
        numberOfItemsRepaired++;
        scoreText.text = score.ToString();
        AudioManager.instance.RepairSound();
    }

    public void ItemDestroyed(Vector3 position)
    {
        numberOfItemsDestroyed++;
        destroyedItemsText.text = numberOfItemsDestroyed.ToString();
        Instantiate(explosionPrefab).transform.position = position + Vector3.up * 0.5f;
        AudioManager.instance.ExplosionSound();
        if (numberOfItemsDestroyed == 10)
        {
            Time.timeScale = 0;
            gameOverMenu.SetActive(true);
        }
    }

    IEnumerator BoxSpawner()
    {
        while (true)
        {
            if (spawnedBoxes.Count < 4)
            {
                SpawnBox();                
            }
            yield return new WaitForSeconds(4);
        }
    }

    struct BoxLocation
    {
        public Transform box;
        public Transform location;

        public BoxLocation(Transform b, Transform l)
        {
            box = b;
            location = l;
        }
    }

    [System.Serializable]
    public struct ItemData
    {
        public Item.ItemType type;
        public GameObject sparePartPrefab;
        public GameObject itemPrefab;
        public Sprite icon;
    }

    public void ReloadLevel()
    {
        Fader.instance.FadeOut();
        StartCoroutine(LoadLevel(1));
    }

    public void LoadMainMenu()
    {
        Fader.instance.FadeOut();
        StartCoroutine(LoadLevel(0));
    }

    IEnumerator LoadLevel(int level)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene(level);
    }
}
