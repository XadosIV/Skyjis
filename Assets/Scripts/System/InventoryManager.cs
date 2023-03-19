using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Text itemLootName;
    public Image itemLootImage;

    public int mushroomCount {
        get => count[ItemType.Mushroom];
    }

    public int gemCount {
        get => count[ItemType.Gem];
    }

    public int plantCount {
        get => count[ItemType.Plant];
    }

    public int spellCount {
        get => count[ItemType.Spell];
    }

    private UserInterfaceManager ui;
    private DialogManager dm;
    private GameManagerScript gm;

    private Dictionary<ItemType, int> count;

    private Image blurImage;

    private bool active = false;

    private List<Item> inventory;

    void Awake()
    {
        inventory = new List<Item>();
        blurImage = GetComponent<Image>();
        blurImage.color = new Color(0, 0, 0, 0);
        ui = FindObjectOfType<UserInterfaceManager>();
        dm = FindObjectOfType<DialogManager>();
        gm = FindObjectOfType<GameManagerScript>();

        count = new Dictionary<ItemType, int>();
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType))) { //parcours une énumération
            count.Add(type, 0);
        }

        itemLootImage.color = new Color(1f, 1f, 1f, 0);
        itemLootName.color = new Color(1f, 1f, 1f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory")) {
            if (dm.isUsed) return;
            PlayerPowers powers = FindObjectOfType<PlayerPowers>();
            if (powers.IsBlockingControl() != 0) return;
            PlayerMovement player = FindObjectOfType<PlayerMovement>();

            if (!active) {
                if (player.IsInCinematic()) return;
                blurImage.color = new Color(0, 0, 0, 0.75f);
                ui.Hide();
                player.StartCinematic(true, false);
            } else {
                blurImage.color = new Color(0, 0, 0, 0);
                ui.Show();
                player.ExitCinematic();
            }
            active = !active;
        }
    }

    public void SetInventory(List<int> ids) {
        inventory.Clear();
        foreach (int id in ids) {
            Item item = gm.items.getItem(id);
            AddToList(item);
        }
    }

    public List<int> GetInventory() {
        List<int> l = new List<int>();
        foreach (Item item in inventory) {
            l.Add(item.id);
        }
        return l;
    }

    public void AddItem(Item item) {
        itemLootImage.sprite = item.sprite;
        itemLootName.text = item.name;
        gm.save.itemsCollected.Add(item.id);
        StartCoroutine(AddItemAnimation());
        AddToList(item);
        gm.SaveGame(false);
    }

    public bool HasItem(int id) {
        foreach (Item item in inventory) {
            if (item.id == id) return true;
        }
        return false;
    }

    private void AddToList(Item item) {
        count[item.type]++;
        inventory.Add(item);
    }

    public void RemoveByType(ItemType type, int number = 1) {
        if (number < count[type]) return;

        Item[] removing = new Item[number];
        int index = 0;
        foreach (Item item in inventory) {
            if (item.type == type) {
                removing[index] = item;
                index++;
                if (index == number) { // liste pleine
                    break;
                }
            }
        }

        foreach (Item item in removing) {
            RemoveToList(item);
        }

        gm.SaveGame(false);
    }

    private void RemoveToList(Item item) {
        count[item.type]--;
        inventory.Remove(item);
    }

    private IEnumerator AddItemAnimation() {
        float fade = 0f;
        Color c;
        while (fade < 1f) {
            c = new Color(1f, 1f, 1f, fade);
            itemLootName.color = c;
            itemLootImage.color = c;
            fade += 0.1f;
            yield return new WaitForSeconds(0.1f);

        }
        yield return new WaitForSeconds(1f);
        while (fade > 0f) {
            fade -= 0.1f;
            c = new Color(1f, 1f, 1f, fade);
            itemLootName.color = c;
            itemLootImage.color = c;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
