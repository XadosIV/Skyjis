using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Warp : MonoBehaviour
{
    public string sceneName;
    public string spawnName;

    private FadeSystemScript fadeSystem;
    void Start() {
        fadeSystem = FindObjectOfType<FadeSystemScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            StartCoroutine(LoadScene());
        }
    }

    public IEnumerator LoadScene() {
        GameManagerScript gameManager = FindObjectOfType<GameManagerScript>();
        gameManager.spawnName = spawnName;
        fadeSystem.FadeIn();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }
}
