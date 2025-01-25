using TMPro;
using UnityEngine;

public class PlayerGameplayStatUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;

    public void SetScoreValue(int value)
    {
        valueText.text = value.ToString();
    }
    
}
