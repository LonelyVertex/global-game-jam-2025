using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button playButton;

    private FadeController _fadeController;

    public void Start()
    {
        _fadeController = FindFirstObjectByType<FadeController>();
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(HandlePlay);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(HandlePlay);
    }

    private void HandlePlay()
    {
        StartCoroutine(PlayCoroutine());
    }

    private IEnumerator PlayCoroutine()
    {
        _fadeController.FadeOut();

        yield return new WaitForSeconds(_fadeController.duration);

        SceneManager.LoadScene(sceneBuildIndex: 1);
    }
}
