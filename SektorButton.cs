using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SektorButton : MonoBehaviour
{
    public TMP_Text NameText;
    public Image IconImage;
    private string sektorName;

    public void SetzeSektor(string name, Sprite icon)
    {
        sektorName = name;
        NameText.text = name;
        IconImage.sprite = icon;
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => UIManager.Instance.SetzeSektor(sektorName));
    }
}