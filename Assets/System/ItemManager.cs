using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager
{
    private Dictionary<int, Item> itemList;


    public ItemManager() {
        itemList = new Dictionary<int, Item>();

        string[,] itemsData = {
            //OakWood Shrooms
            { "Champibrun", "Un champignon commun dans la clairière d'Oakwood. Mixé avec un Champirouge et un Shroomax, ils forment une excellente soupe allongeant la durée vie.", "mushrooms_10"},
            { "Champirouge", "Un champignon commun dans la clairière d'Oakwood. Contrairement à son cousin, le Champibrun, celui-ci est très toxique.", "mushrooms_12"},
            { "Shroomax", "Un champignon rare de la clairière d'Oakwood, il est bien apprécié par le plombier du coin.", "mushrooms_25"},
            //Ruvyn Shrooms
            { "Trotrolon", "Un champignon commun de la forêt de Ruvyn, sa tige est si longue et nutritive qu'elle permet de nourrir une famille pendant un mois.", "mushrooms_5" },
            { "Glantignon", "Un champignon commun de la forêt de Ruvyn, contrairement aux autres champignons, celui-ci pousse dans les arbres, comme les glands ! En plus, il a l'apparence d'un gland... Mais le manuel est très clair, c'est un champignon.", "mushrooms_8" },
            { "Champyramide", "Un champignon rare de la forêt de Ruvyn, on ne sait pas d'où vient sa forme pyramidale, certains pensent que les aliens les ont conçus.", "mushrooms_23" },
            //Cavernes Shrooms
            { "Lumos-Minichampos", "Un champignon commun des cavernes, la légende raconte qu'en les suivants, les voyageurs égarés retrouvèrent la sortie des cavernes en suivant leur faible lumière.", "mushrooms_18" },
            { "Lumos-Maxichampos", "Un champignon commun des cavernes, c'est un Lumos-Minichampos ayant été exposé à la lumière, il a pu alors grandir plus vite et absorber toute la lumière qu'il dégage désormais.", "mushrooms_19" },
            { "Sunnytrope", "Un champignon rare des cavernes, comme les tournesols, il tourne sa tête faire le peu de soleil qu'il trouve.", "mushrooms_22" },
            //Fungus Paradise Shrooms
            { "Courbus", "Un champignon commun du Paradis Fungus, son nom lui a été donné en référence à sa forme courbée.", "mushrooms_34" },
            { "Champido", "Un champignon commun du Paradis Fungus, vivant à côté des points d'eau, il en est imbibé, d'où son nom et sa couleur bleu.", "mushrooms_33" },
            { "Magichampi", "Un champignon rare du Paradis Fungus, sa couleur rose provient de la grande quantité de mana qu'il possède, il peut guérir toutes les maladies. Cependant, à usage unique et très rare, cela n'a permis de sauver que le maitre des lieux à ce jour.", "mushrooms_35" },
            //Chronos Shrooms
            { "Pentakillus", "Un champignon commun du domaine de Chronos, chacune de ses 5 têtes dégagent un produit toxique différent, lui permettant de tuer tout les types de prédateur qu'il rencontre.", "mushrooms_14" },
            { "Lungus", "Un champignon commun du domaine de Chronos, il a une couleur sinistre, il ne pourrait vous arriver que du mal à le goûter sans précaution.", "mushrooms_28" },
            { "Mortigus", "Un champignon rare du domaine de Chronos, le champignon le plus toxique au monde, en l'ingérant, son poison se mélange au sang et fait souffrir sa victime plusieurs jours, de plus en plus fort, jusqu'à sa mort.", "mushrooms_27" },
        };

        for (int i = 0; i < itemsData.GetLength(0); i++) {
            itemList.Add(i, new Item(i, itemsData[i, 0], itemsData[i, 1], itemsData[i, 2]));
        }
    }

    public Item getItem(int id) {
        return itemList[id];
    }

}
