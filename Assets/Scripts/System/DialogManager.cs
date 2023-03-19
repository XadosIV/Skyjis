using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    private Image image;
    private PlayerMovement player;
    private GameManagerScript gm;
    private InventoryManager im;

    private readonly string[] keywords = {"TAG", "ID", "EQ", "GOTO", "INCR", "REMOVE"};
    /*
    => CH = Total champignon / PT = Total plante / GM = Total Gemme / HT = Health Bonus / MN = Mana Bonus / DGT = Dégats Bonus

    TAG:[idTag]:[newValue] <=> Modifie un tag
    ID:[id] <=> Etiquette d'un dialogue
    EQ:[HT/MN/DGT]:[VAL]:[ID] => Goto ID si HT/MN/DGT = VAL
    GOTO:[id] <=> Renvoie à un id.
    INCR:[HEALTH/MANA/DGT] <=> Donne une amélioration de vie/mana/dégats
    REMOVE:[CH/PT/GM]:[NB] <=> Retire de l'inventaire [NB] champis/plantes/gems
    */

    public float textSpeed;

    private List<string> dialog;

    private Dictionary<int, int> identifiers;

    private int index;

    public bool isUsed = false;

    void Start()
    {
        gm = FindObjectOfType<GameManagerScript>();
        im = FindObjectOfType<InventoryManager>();
        image = GetComponent<Image>();
        Show(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("MeleeHit") && image.enabled) {
            if (textComponent.text == dialog[index]) {
                NextLine();
            }
            else {
                StopAllCoroutines();
                textComponent.text = dialog[index];
            }
        }
    }
    void Show(bool showing) {
        image.enabled = showing;
        textComponent.enabled = showing;
        isUsed = showing;
    }

    private void LoadIdentifiers(string[] lines) {
        dialog = new List<string>();
        identifiers = new Dictionary<int, int>();
        int linecount = 0;
        foreach (string line in lines) {
            if (line.StartsWith("ID:")) {
                int id = int.Parse(line.Split(":")[1]);
                identifiers.Add(id, linecount+1);
            }
            dialog.Add(line);
            linecount++;
        }
    }

    private void ExecuteKeyword(string line) {
        string[] args = line.Split(":");
        switch (args[0]) {
            case "TAG": // TAG:[idTag]:[newValue] <=> Modifie un tag
                int i = int.Parse(args[1]);
                int v = int.Parse(args[2]);
                gm.save.flags[i] = v;
                break;
            case "ID":
                index = dialog.Count;
                break;
            case "EQ": // EQ:[CH/PT/GMHT/MN/DGT]:[VAL]:[ID] => Goto ID si HT/MN/DGT = VAL
                int compare = -1;
                switch (args[1]) {
                    case "CH":
                        compare = im.mushroomCount;
                        break;
                    case "PT":
                        compare = im.plantCount;
                        break;
                    case "GM":
                        compare = im.gemCount;
                        break;
                    case "HT":
                        compare = gm.save.nbUpgradeHealth;
                        break;
                    case "MN":
                        compare = gm.save.nbUpgradeMana;
                        break;
                    case "DGT":
                        compare = gm.save.nbUpgradeDamage;
                        break;
                    default:
                        print("Unknown parameter");
                        break;
                }
                int value = int.Parse(args[2]);
                int targetIndex = int.Parse(args[3]);
                if (compare == value) {
                    GotoLine(targetIndex);
                }
                break;
            case "GOTO": // GOTO:[id] <=> Renvoie à un id.
                int id = int.Parse(args[1]);
                GotoLine(id);
                break;
            case "INCR": // INCR:[HT/MN/DGT] <=> Donne une amélioration de vie/mana/dégats
                if (args[1] == "HT") {
                    gm.AddMaxHealth();
                }else if (args[1] == "MN") {
                    gm.AddMaxMana();
                }else if (args[1] == "DGT") {
                    gm.AddDamage();
                } else {
                    print("Unknown parameter");
                }
                break;
            case "REMOVE": // REMOVE:[CH/PT/GM]:[NB] <=> Retire de l'inventaire [NB] champis/plantes/gems
                int number = int.Parse(args[2]);
                if (args[1] == "CH") {
                    im.RemoveByType(ItemType.Mushroom, number);
                }
                else if (args[1] == "PT") {
                    im.RemoveByType(ItemType.Plant, number);
                }
                else if (args[1] == "GM") {
                    im.RemoveByType(ItemType.Gem, number);
                }
                else {
                    print("Unknown parameter");
                }
                break;
            default:
                print("Unknown Keyword.");
                break;
        }
    }

    private void GotoLine(int identifier) {
        index = identifiers[identifier];
        index--;
    }

    public void StartDialogue(string[] lines, int? readFlag) {
        player = FindObjectOfType<PlayerMovement>();
        player.StartCinematic(true);

        LoadIdentifiers(lines); //set dialog également
        
        if (readFlag == null) {
            index = 0;
        } else {
            index = identifiers[gm.save.flags[(int)readFlag]];
        }
        
        Show(true);

        index--; // est incrémenté au début de NextLine donc on le décrémente pour arriver au début.
        NextLine();
    }

    IEnumerator TypeLine() {
        textComponent.text = string.Empty;
        foreach (char c in dialog[index]) {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private bool DetectKeyword(string line) {
        string keyword = line.Split(":")[0];
        foreach (string kw in keywords) {
            if (kw == keyword) {
                return true;
            }
        }
        return false;
    }

    void NextLine() {
        if (index < dialog.Count -1) {
            index++;
            if (DetectKeyword(dialog[index])) {
                ExecuteKeyword(dialog[index]);
                NextLine();
            } else {
                StartCoroutine(TypeLine());
            }
        } else {
            player.ExitCinematic();
            Show(false);
        }
    }
}
