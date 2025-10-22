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
            { "Champibrun", "Un champignon commun dans la clairi�re d'Oakwood. Mix� avec un Champirouge et un Shroomax, ils forment une excellente soupe allongeant la dur�e vie.", "mushrooms_10"},
            { "Champirouge", "Un champignon commun dans la clairi�re d'Oakwood. Contrairement � son cousin, le Champibrun, celui-ci est tr�s toxique.", "mushrooms_12"},
            { "Shroomax", "Un champignon rare de la clairi�re d'Oakwood, il est bien appr�ci� par le plombier du coin.", "mushrooms_25"},
            //Ruvyn Shrooms
            { "Trotrolon", "Un champignon commun de la for�t de Ruvyn, sa tige est si longue et nutritive qu'elle permet de nourrir une famille pendant un mois.", "mushrooms_5" },
            { "Glantignon", "Un champignon commun de la for�t de Ruvyn, contrairement aux autres champignons, celui-ci pousse dans les arbres, comme les glands ! En plus, il a l'apparence d'un gland... Mais le manuel est tr�s clair, c'est un champignon.", "mushrooms_8" },
            { "Champyramide", "Un champignon rare de la for�t de Ruvyn, on ne sait pas d'o� vient sa forme pyramidale, certains pensent que les aliens les ont con�us.", "mushrooms_23" },
            //Cavernes Shrooms
            { "Lumos-Minichampos", "Un champignon commun des cavernes, la l�gende raconte qu'en les suivants, les voyageurs �gar�s retrouv�rent la sortie des cavernes en suivant leur faible lumi�re.", "mushrooms_18" },
            { "Lumos-Maxichampos", "Un champignon commun des cavernes, c'est un Lumos-Minichampos ayant �t� expos� � la lumi�re, il a pu alors grandir plus vite et absorber toute la lumi�re qu'il d�gage d�sormais.", "mushrooms_19" },
            { "Sunnytrope", "Un champignon rare des cavernes, comme les tournesols, il tourne sa t�te faire le peu de soleil qu'il trouve.", "mushrooms_22" },
            //Fungus Paradise Shrooms
            { "Courbus", "Un champignon commun du Paradis Fungus, son nom lui a �t� donn� en r�f�rence � sa forme courb�e.", "mushrooms_34" },
            { "Champido", "Un champignon commun du Paradis Fungus, vivant � c�t� des points d'eau, il en est imbib�, d'o� son nom et sa couleur bleu.", "mushrooms_33" },
            { "Magichampi", "Un champignon rare du Paradis Fungus, sa couleur rose provient de la grande quantit� de mana qu'il poss�de, il peut gu�rir toutes les maladies. Cependant, � usage unique et tr�s rare, cela n'a permis de sauver que le maitre des lieux � ce jour.", "mushrooms_35" },
            //Chronos Shrooms
            { "Pentakillus", "Un champignon commun du domaine de Chronos, chacune de ses 5 t�tes d�gagent un produit toxique diff�rent, lui permettant de tuer tout les types de pr�dateur qu'il rencontre.", "mushrooms_14" },
            { "Lungus", "Un champignon commun du domaine de Chronos, il a une couleur sinistre, il ne pourrait vous arriver que du mal � le go�ter sans pr�caution.", "mushrooms_28" },
            { "Mortigus", "Un champignon rare du domaine de Chronos, le champignon le plus toxique au monde, en l'ing�rant, son poison se m�lange au sang et fait souffrir sa victime plusieurs jours, de plus en plus fort, jusqu'� sa mort.", "mushrooms_27" },
        };

        for (int i = 0; i < itemsData.GetLength(0); i++) {
            itemList.Add(i, new Item(i, itemsData[i, 0], itemsData[i, 1], itemsData[i, 2]));
        }
    }

    public Item getItem(int id) {
        return itemList[id];
    }

}
