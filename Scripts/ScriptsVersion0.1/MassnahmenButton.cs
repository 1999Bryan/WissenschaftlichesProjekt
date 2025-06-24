using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MassnahmenButton : MonoBehaviour
{
    public Image IconImage;

    private MassnahmeData gespeicherteMassnahme;

    public void SetzeMassnahme(MassnahmeData massnahme)
    {
        gespeicherteMassnahme = massnahme;
        IconImage.sprite = Resources.Load<Sprite>(massnahme.IconPath);
    }

    public void OnButtonClick()
    {
        Debug.Log("Ausgewählt: " + gespeicherteMassnahme.Name);
        UIManager.Instance.ZeigeMassnahmeDetails(gespeicherteMassnahme);
    }
}
