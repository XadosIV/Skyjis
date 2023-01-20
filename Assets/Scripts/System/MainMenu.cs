using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject gameManager;

    public int saveFileId;

    public void PlayGame() {
        EventSystem.current.enabled = false;

        GameObject gmObject = Instantiate(gameManager);
        gmObject.GetComponent<GameManagerScript>().SetSaveFileId(saveFileId, true);
        gameObject.SetActive(false);
    }
}
