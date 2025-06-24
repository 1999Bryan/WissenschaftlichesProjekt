using UnityEngine;
using UnityEngine.EventSystems;

public class BundeslandController : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;
    public string BundeslandName;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

    }

    void OnMouseEnter()
    {
        if (GameManager.Instance.playerState == PlayerState.Default)
        {
            sr.color = Color.yellow;
            UIManager.Instance.ZeigeBundeslandInfo(BundeslandName);
        }
    }

    public void OnMouseExit()
    {
        if (GameManager.Instance.playerState == PlayerState.Default)
        {
            sr.color = originalColor;
        }
    }

    void OnMouseDown()
    {
        if(GameManager.Instance.playerState == PlayerState.Default)
        {
            Debug.Log("Klick auf: " + gameObject.name);
            GameManager.Instance.playerState = PlayerState.BundeslandSelected;
            GameManager.Instance.AktuellesBundesland = GameManager.Instance.AlleBundeslaender.Find(b => b.Name == gameObject.name);
            UIManager.Instance.ÖffneBauMenü(BundeslandName);
        }
        else if (GameManager.Instance.playerState == PlayerState.BundeslandSelected || GameManager.Instance.playerState == PlayerState.MassnahmeSelected)
        {
           // UIManager.Instance.CloseAllMenu();
            GameManager.Instance.playerState = PlayerState.Default;
        }
    }
}
