using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private float zeitSeitOeffnen = 0f;

    // Bundesland Panel
    public TMP_Text NameText;
    public Image WappenImage;
    public TMP_Text Co2Text;
    public TMP_Text EnergieverbrauchText;
    public TMP_Text InvestitionText;
    public TMP_Text AkzeptanzText;
    public TMP_Text DigitalisierungsgradText;

    // Baumenü
    public GameObject BauMenüPanel;
    public GameObject MaßnahmenPanel;
    public Button EnergieButton;
    public Button VerkehrButton;
    public Button IndustrieButton;
    public Button GebäudeButton;
    public Button LandwirtschaftButton;
    public Transform MassnahmenContent;
    public GameObject MassnahmenButtonPrefab;
    private string aktuellerSektor = "Energie";

    // Maßnahmen Panel
    public GameObject MassnahmeDetailPanel;
    public TMP_Text MassnahmeNameText;
    public TMP_Text BeschreibungText;
    public TMP_Text AktuellerBestandText;
    public TMP_Text Co2;
    public TMP_Text Kosten;
    public TMP_Text Digitalisierung;
    public TMP_Text Akzeptanz;
    public Image MassnahmeIcon;

    // Bauplan
    public GameObject bundeslandPrefab;
    public GameObject massnahmenPrefab;
    public Transform bauplanContent;
    private Dictionary<string, BauplanBundesland> bauplan = new Dictionary<string, BauplanBundesland>();
    private Dictionary<string, GameObject> bundeslandPanels = new Dictionary<string, GameObject>();

    //Ergebnisse
    public GameObject ErgebnisPanel;
    public Transform resultContent;
    public GameObject bundeslandRowPrefab;
    public Sprite gruenesPfeilIcon;
    public Sprite rotesPfeilIcon;
    public Sprite grauesPfeilIcon;

    void Awake()
    {
        Instance = this;
        BauMenüPanel.SetActive(false);
        ErgebnisPanel.SetActive(false);
    }

    void Start()
    {
        SetzeSektor("Energie");

        EnergieButton.onClick.AddListener(() => SetzeSektor("Energie"));
        VerkehrButton.onClick.AddListener(() => SetzeSektor("Verkehr"));
        IndustrieButton.onClick.AddListener(() => SetzeSektor("Industrie"));
        GebäudeButton.onClick.AddListener(() => SetzeSektor("Gebäude"));
        LandwirtschaftButton.onClick.AddListener(() => SetzeSektor("Landwirtschaft"));
    }

    void Update()
    {
        if (BauMenüPanel.activeSelf)
        {
            // Zähler hochsetzen solange das Panel offen ist
            zeitSeitOeffnen += Time.deltaTime;
        }

        if (GameManager.Instance.playerState == PlayerState.BundeslandSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Nur prüfen, wenn etwas Zeit vergangen ist nach dem Öffnen
                if (zeitSeitOeffnen > 0.15f)
                {
                    if (!IstMausÜberUI())
                    {
                        BauMenüPanel.SetActive(false);
                        GameManager.Instance.playerState = PlayerState.Default;
                    }
                }
            }
        }
    }

    bool IstMausÜberUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == BauMenüPanel || result.gameObject.transform.IsChildOf(BauMenüPanel.transform))
            {
                return true;
            }
        }
        return false;
    }


    public void ZeigeBundeslandInfo(string bundeslandName)
    {
        BundeslandData data = GameManager.Instance.AlleBundeslaender.Find(b => b.Name == bundeslandName);

        if (data != null)
        {
            NameText.text = data.Name;
            WappenImage.sprite = data.Wappen;
            Co2Text.text = $"{data.Startwerte.CO2Ausstoß} Mio t";
            EnergieverbrauchText.text = $"{data.Startwerte.Energieverbrauch} TWh";
            InvestitionText.text = $"{data.Startwerte.Investitionskosten} Mio €";
            AkzeptanzText.text = $"{data.Startwerte.Akzeptanz}%";
            DigitalisierungsgradText.text = $"{data.Startwerte.Digitalisierungsgrad}%";
        }
    }

    public void ÖffneBauMenü(string bundeslandName)
    {
        BauMenüPanel.SetActive(true);
        MaßnahmenPanel.SetActive(true);
        MassnahmeDetailPanel.SetActive(false);

        SetzeSektor("Energie");
        zeitSeitOeffnen = 0f;  // Zeitpuffer zurücksetzen beim Öffnen
        GameManager.Instance.playerState = PlayerState.BundeslandSelected;
    }

    public void SetzeSektor(string sektorName)
    {
        aktuellerSektor = sektorName;

        // Alle Buttonfarben zurücksetzen:
        SetzeAlleSektorButtonFarben();

        // Aktiven Button einfärben
        switch (sektorName)
        {
            case "Energie": EnergieButton.GetComponent<Image>().color = Color.yellow; break;
            case "Verkehr": VerkehrButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 1f); break;
            case "Industrie": IndustrieButton.GetComponent<Image>().color = Color.gray; break;
            case "Gebäude": GebäudeButton.GetComponent<Image>().color = Color.magenta; break;
            case "Landwirtschaft": LandwirtschaftButton.GetComponent<Image>().color = Color.green; break;
        }

        // Maßnahmen-Buttons neu laden:
        FülleMassnahmen();
    }

    private void SetzeAlleSektorButtonFarben()
    {
        Color standard = Color.white;
        EnergieButton.GetComponent<Image>().color = standard;
        VerkehrButton.GetComponent<Image>().color = standard;
        IndustrieButton.GetComponent<Image>().color = standard;
        GebäudeButton.GetComponent<Image>().color = standard;
        LandwirtschaftButton.GetComponent<Image>().color = standard;
    }

    private void FülleMassnahmen()
    {
        // Vorherige Buttons löschen
        foreach (Transform child in MassnahmenContent)
        {
            Destroy(child.gameObject);
        }

        // Aus GameManager die richtigen Maßnahmen holen:
        var sektor = GameManager.Instance.AlleSektoren.Find(s => s.Name == aktuellerSektor);
        if (sektor == null) return;

        foreach (var massnahme in sektor.Massnahmen)
        {
            GameObject neuerButton = Instantiate(MassnahmenButtonPrefab, MassnahmenContent);
            MassnahmenButton mb = neuerButton.GetComponent<MassnahmenButton>();
            mb.SetzeMassnahme(massnahme);
        }
    }

    public void ZeigeMassnahmeDetails(MassnahmeData massnahme)
    {
        GameManager.Instance.AktuelleMaßnahme = massnahme;
        MaßnahmenPanel.SetActive(false);
        MassnahmeDetailPanel.SetActive(true);

        MassnahmeNameText.text = massnahme.Name;
        BeschreibungText.text = massnahme.Beschreibung;
        MassnahmeIcon.sprite = Resources.Load<Sprite>(massnahme.IconPath);
        Co2.text = massnahme.CO2Ausstoß.ToString() + " g/kWh";
        Kosten.text = massnahme.Kosten.ToString() + " Mio €";
        Digitalisierung.text = DigitalisierungBewertung(massnahme);
        Akzeptanz.text = massnahme.Akzeptanz.ToString() + " %";

        var aktuellesBL = GameManager.Instance.AktuellesBundesland;
        if (aktuellesBL != null && aktuellesBL.MassnahmenBestand.ContainsKey(massnahme.Name))
        {
            AktuellerBestandText.text = aktuellesBL.MassnahmenBestand[massnahme.Name].ToString();
        }
        else
        {
            AktuellerBestandText.text = "0";
        }
    }

    public void CloseButton()
    {
        BauMenüPanel.SetActive(false);
        MaßnahmenPanel.SetActive(false);
        MassnahmeDetailPanel.SetActive(false);
    }

    public string DigitalisierungBewertung(MassnahmeData massnahme)
    {
        switch (massnahme.Digitalisierung)
        {
            case 5: return "Sehr hoch";
            case 4: return "Hoch";
            case 3: return "Mittel";
            case 2: return "Niedrig";
            case 1: return "Sehr niedrig";
            default: return "Keine Daten";
        }
    }

    public void ÄndereBauplan(string bundeslandName, string massnahmeName, int delta)
    {
        // Bundesland suchen oder neu anlegen
        if (!bauplan.ContainsKey(bundeslandName))
        {
            bauplan[bundeslandName] = new BauplanBundesland { BundeslandName = bundeslandName };
        }

        var bundeslandPlan = bauplan[bundeslandName];
        var eintrag = bundeslandPlan.Massnahmen.Find(m => m.MassnahmenName == massnahmeName);

        if (eintrag == null)
        {
            eintrag = new BauplanEintrag { MassnahmenName = massnahmeName, Amount = 0 };
            bundeslandPlan.Massnahmen.Add(eintrag);
        }

        eintrag.Amount += delta;

        if (eintrag.Amount == 0)
        {
            bundeslandPlan.Massnahmen.Remove(eintrag);
        }

        if (bundeslandPlan.Massnahmen.Count == 0)
        {
            bauplan.Remove(bundeslandName);
        }

        AktualisiereBauplanUI();
    }

    private void AktualisiereBauplanUI()
    {
        foreach (Transform child in bauplanContent)
            Destroy(child.gameObject);
        bundeslandPanels.Clear();

        var sortedBauplan = bauplan.Values.OrderBy(b => b.BundeslandName).Reverse().ToList();
        foreach (var bl in sortedBauplan)
        {
            int summe = 0;
            foreach (var e in bl.Massnahmen)
                summe += Mathf.Abs(e.Amount);

            // Wenn keine Bauvorhaben, Panel ggf. entfernen/deaktivieren
            if (summe == 0)
            {
                if (bundeslandPanels.TryGetValue(bl.BundeslandName, out GameObject vorhandenesPanel))
                {
                    vorhandenesPanel.SetActive(false);
                    bundeslandPanels.Remove(bl.BundeslandName);
                }
                continue;
            }

            GameObject blPanel;

            if (!bundeslandPanels.TryGetValue(bl.BundeslandName, out blPanel))
            {
                blPanel = Instantiate(bundeslandPrefab, bauplanContent);
                bundeslandPanels[bl.BundeslandName] = blPanel;

                // Direkt nach unten stapeln (unten == erstes Kind)
                blPanel.transform.SetAsFirstSibling();

                // Button-Listener hinzufügen
                Button blButton = blPanel.transform.Find("Button").GetComponent<Button>();
                Transform massnahmenContent = blPanel.transform.Find("MassnahmenContent");

                blButton.onClick.AddListener(() => {
                    massnahmenContent.gameObject.SetActive(!massnahmenContent.gameObject.activeSelf);
                });
            }

            blPanel.SetActive(true);


            // Panel-UI updaten (z.B. Name, Wappen, Anzahl)
            TMP_Text BLName = blPanel.transform.Find("Button/NameText").GetComponent<TMP_Text>();
            Image BLIcon = blPanel.transform.Find("Button/FlagImage").GetComponent<Image>();
            TMP_Text BLBauAnzahlGesamt = blPanel.transform.Find("Button/AnzahlText").GetComponent<TMP_Text>();

            BLName.text = bl.BundeslandName;

            var blData = GameManager.Instance.AlleBundeslaender.Find(b => b.Name == bl.BundeslandName);
            if (blData != null)
                BLIcon.sprite = blData.Wappen;

            BLBauAnzahlGesamt.text = summe.ToString();

            // Maßnahmen-Inhalte updaten (erst löschen, dann neu befüllen)
            Transform massnahmenParent = blPanel.transform.Find("MassnahmenContent");
            foreach (Transform child in massnahmenParent)
                Destroy(child.gameObject);

            foreach (var e in bl.Massnahmen)
            {
                GameObject mPanel = Instantiate(massnahmenPrefab, massnahmenParent);
                TMP_Text MName = mPanel.transform.Find("Name").GetComponent<TMP_Text>();
                TMP_Text MAmount = mPanel.transform.Find("Amount").GetComponent<TMP_Text>();
                Image MIcon = mPanel.transform.Find("Image").GetComponent<Image>();

                MName.text = e.MassnahmenName;
                MAmount.text = (e.Amount > 0 ? "+" : "") + e.Amount.ToString();

                MassnahmeData massnahmeData = FindeMassnahmeData(e.MassnahmenName);
                if (massnahmeData != null)
                    MIcon.sprite = Resources.Load<Sprite>(massnahmeData.IconPath);
            }
        }
    }

    private MassnahmeData FindeMassnahmeData(string name)
    {
        foreach (var sektor in GameManager.Instance.AlleSektoren)
        {
            foreach (var massnahme in sektor.Massnahmen)
            {
                if (massnahme.Name == name)
                {
                    return massnahme;
                }
            }
        }
        return null;
    }

    public void OnPlusClicked()
    {
        var aktuellesBL = GameManager.Instance.AktuellesBundesland;
        var aktuelleMassnahme = GameManager.Instance.AktuelleMaßnahme;
        if (aktuellesBL != null)
        {
            ÄndereBauplan(aktuellesBL.Name, GameManager.Instance.AktuelleMaßnahme.Name, +1);
            ÄndereMassnahmenAnzahl(aktuellesBL.Name, aktuelleMassnahme.Name, +1);
        }
    }

    public void OnMinusClicked()
    {
        var aktuellesBL = GameManager.Instance.AktuellesBundesland;
        var aktuelleMassnahme = GameManager.Instance.AktuelleMaßnahme;
        if (aktuellesBL != null)
        {
            ÄndereBauplan(aktuellesBL.Name, GameManager.Instance.AktuelleMaßnahme.Name, -1);
            ÄndereMassnahmenAnzahl(aktuellesBL.Name, aktuelleMassnahme.Name, -1);
        }
    }

    public void ÄndereMassnahmenAnzahl(string bundeslandName, string massnahmeName, int delta)
    {
        var bundesland = GameManager.Instance.AlleBundeslaender.Find(b => b.Name == bundeslandName);
        if (bundesland == null)
            return;

        if (!bundesland.MassnahmenBestand.ContainsKey(massnahmeName))
        {
            // Maßnahme ist noch nicht drin, nur bei +1 hinzufügen
            if (delta > 0)
                bundesland.MassnahmenBestand[massnahmeName] = 0;
            else
                return; // Minus macht hier nix
        }

        int aktuellerWert = bundesland.MassnahmenBestand[massnahmeName];
        int neuerWert = Mathf.Clamp(aktuellerWert + delta, 0, 20);

        // Nur aktualisieren, wenn sich Wert ändert
        if (neuerWert != aktuellerWert)
        {
            bundesland.MassnahmenBestand[massnahmeName] = neuerWert;
            Debug.Log($"[{bundeslandName}] Maßnahme {massnahmeName} Bestand aktualisiert: {neuerWert}");
            AktuellerBestandText.text = neuerWert.ToString();
        }
    }

    public void OpenResults()
    {
        ErgebnisPanel.SetActive( true );
        GameManager.Instance.BerechneUndZeigeErgebnisse();
    }

    public void BackButton()
    {
        MassnahmeDetailPanel.SetActive( false );
        MaßnahmenPanel.SetActive( true );
        GameManager.Instance.playerState = PlayerState.BundeslandSelected;
    }

    // Ergebnisse

    public void ZeigeErgebnisTabelle(List<BundeslandData> alleBundeslaender)
    {
        foreach (Transform child in resultContent)
        {
            Destroy(child.gameObject);
        }

        var sortiert = alleBundeslaender.OrderBy(b => b.Name).ToList();

        foreach (var bundesland in sortiert)
        {
            GameObject row = Instantiate(bundeslandRowPrefab, resultContent);

            row.transform.Find("Name").GetComponent<TMP_Text>().text = bundesland.Name;

            var alt = bundesland.AktuelleWerte;
            var neu = bundesland.BerechneteNeueWerte; 

            SetzeZelle(row, "Kosten", "ArrowKosten", alt.Investitionskosten, neu.Gesamtkosten, "Mio");

            SetzeZelle(row, "Co2", "ArrowCo2", alt.CO2Ausstoß, neu.CO2Aenderung, "Mio");

            SetzeZelle(row, "Digitalisierung", "ArrowDig", alt.Digitalisierungsgrad, neu.DigitalisierungPlus, "%");

            SetzeZelle(row, "Akzeptanz", "ArrowAkz", alt.Akzeptanz, neu.AkzeptanzPlus, "%");
        }
    }

    public void SetzeZelle(GameObject row, string textName, string pfeilName, float altWert, float neuWert, string einheit, float faktor = 1f)
    {
        if (neuWert > 999 || neuWert < -999)
        {
            neuWert = neuWert / 1000;
            einheit = "Mrd";
        }
        float altNorm = altWert / faktor;
        float neuNorm = neuWert / faktor;
        float differenz = neuNorm - altNorm;

        TMP_Text text = row.transform.Find(textName).GetComponent<TMP_Text>();
        text.text = $"{neuNorm:0.##} {einheit}";

        Image pfeil = row.transform.Find(pfeilName).GetComponent<Image>();
        float toleranz = 0.01f * Mathf.Abs(altNorm);

        if (differenz > toleranz)
            pfeil.sprite = gruenesPfeilIcon;
        else if (differenz < -toleranz)
            pfeil.sprite = rotesPfeilIcon;
        else
            pfeil.sprite = grauesPfeilIcon;
    }

}
