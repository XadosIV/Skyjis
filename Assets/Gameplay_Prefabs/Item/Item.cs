using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType {
    Mushroom,
    Gem,
    Plant,
    Spell
}

public class Item
{
    public int id {
        get;
    }
    /*
    0-14 => mushrooms (heal boost)
    15-19 => gems (damage boost)
    20-49 => plants (mana boost)
    50-?? => spells => itemSpellID - 50 = spellGiven => Artifeu (Spell ID = 0) <=> ItemArtifeu (Item ID = 50)
    */
    public string name {
        get;
    }
    public string description {
        get;
    }
    public Sprite sprite {
        get;
    }

    public ItemType type {
        get;
    }


    public Item(int id, string name, string description, string imageName) {
        this.id = id;
        this.name = name;
        this.description = description;

        if (id < 15) {
            this.type = ItemType.Mushroom;
        }else if (id < 20) {
            this.type = ItemType.Gem;
        }else if (id < 50) {
            this.type = ItemType.Plant;
        } else {
            this.type = ItemType.Spell;
        }

        string[] imageNameAndIndex = imageName.Split("_");

        imageName = imageNameAndIndex[0];
        int index = int.Parse(imageNameAndIndex[1]);

        /*Sprite[] sprites = Resources.LoadAll<Sprite>("items/" + imageName);

        this.sprite = sprites[index];*/
    }
}
