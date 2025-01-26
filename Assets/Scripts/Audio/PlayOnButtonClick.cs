using UnityEngine;
using UnityEngine.UI;

public class PlayOnButtonClick : MonoBehaviour
{
    public Button button;
    public Toggle toggle;

    [Space] public AudioSource audioSource;

    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(HandleButtonOnClick);
        }

        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(HandleToggleValueChanged);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleButtonOnClick);
        }

        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(HandleToggleValueChanged);
        }
    }

    private void HandleButtonOnClick()
    {
        audioSource.Play();
    }

    private void HandleToggleValueChanged(bool isOn)
    {
        audioSource.Play();
    }
}
