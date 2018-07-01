using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDManager : MonoBehaviour
{
    public RectTransform healthBar;

    private PlayerStatus playerStatus;

    void Awake()
    {
        playerStatus = GameObject.FindGameObjectWithTag(Helpers.Tags.Player).GetComponent<PlayerStatus>();
    }

    void Update()
    {
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(playerStatus.Health / playerStatus.MaxHealth, 0, 1), 1, 1);
    }
}
