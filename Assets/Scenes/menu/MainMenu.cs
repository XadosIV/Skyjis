using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject gameManager;
    private GameManager gm;
    public int saveFileId;

    public void PlayGame() {
        EventSystem.current.enabled = false;

        GameObject gmObject = Instantiate(gameManager);
        gm = gmObject.GetComponent<GameManager>();
        gmObject.GetComponent<GameManager>().SetSaveFileId(saveFileId, true);

        gameObject.SetActive(false);
    }
}
