using UnityEngine;

public class FinishedGamePlayerUIController : MonoBehaviour
{
    [SerializeField] private GameObject wonGameUI;
    [SerializeField] private GameObject lostGameUI;

    public void EnableUI(bool isWinner)
    {
        Debug.Log($"Show winner UI: {isWinner}");
        lostGameUI.SetActive(!isWinner);
        wonGameUI.SetActive(isWinner);
    }
}
