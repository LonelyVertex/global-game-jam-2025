using UnityEngine;
using UnityEngine.UI;

public class GameSettingsController : MonoBehaviour
{
    [SerializeField] private Toggle aiBotsEnabledToggle;
    
    void Start()
    {
        aiBotsEnabledToggle.SetIsOnWithoutNotify(GetAIBotsEnabled());
        aiBotsEnabledToggle.onValueChanged.AddListener(OnAIBotsEnabledToggleChanged);
    }
    void OnDestroy()
    {
        aiBotsEnabledToggle.onValueChanged.RemoveListener(OnAIBotsEnabledToggleChanged);
    }

    private void OnAIBotsEnabledToggleChanged(bool enabled)
    {
        SetAIBotsEnabled(enabled);
    }

    public static void SetAIBotsEnabled(bool enabled)
    {
        PlayerPrefs.SetInt("aiBotsEnabled", enabled ? 1 : 0);
    }

    public static bool GetAIBotsEnabled()
    {
        return PlayerPrefs.GetInt("aiBotsEnabled") > 0;
    }

}
