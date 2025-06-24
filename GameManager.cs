using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Default,
    BundeslandSelected,
    MassnahmeSelected,
    ResultScreen
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerState playerState = PlayerState.Default;

    public List<BundeslandData> AlleBundeslaender = new List<BundeslandData>();
    public List<Sektor> AlleSektoren = new List<Sektor>();
    public BundeslandData AktuellesBundesland;
    public MassnahmeData AktuelleMaßnahme;


    public float Budget;
    public int JahreSimuliert;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            LadeMassnahmen();
            LadeBundeslaender();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LadeBundeslaender()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("Bundesländer");
        BundeslandJSONList import = JsonUtility.FromJson<BundeslandJSONList>(jsonText.text);

        foreach (var b in import.Bundeslaender)
        {
            BundeslandData neuesBL = new BundeslandData();
            neuesBL.Name = b.Name;
            neuesBL.Wappen = Resources.Load<Sprite>(b.Wappen);

            neuesBL.Startwerte = new BundeslandStats()
            {
                CO2Ausstoß = b.CO2Ausstoß,
                Akzeptanz = b.Akzeptanz,
                Investitionskosten = b.Investitionskosten,
                Digitalisierungsgrad = b.Digitalisierungsgrad,
                Energieverbrauch = b.Energieverbrauch
            };

            neuesBL.AktuelleWerte = neuesBL.Startwerte;
            neuesBL.MassnahmenBestand = new Dictionary<string, int>();

            foreach (var sektor in AlleSektoren)
            {
                foreach (var massnahme in sektor.Massnahmen)
                {
                    neuesBL.MassnahmenBestand[massnahme.Name] = 0;
                }
            }

            if (b.Massnahmen != null)
            {
                foreach (var bestand in b.Massnahmen)
                {
                    if (neuesBL.MassnahmenBestand.ContainsKey(bestand.Name))
                        neuesBL.MassnahmenBestand[bestand.Name] = bestand.Anzahl;
                    else
                        Debug.LogWarning($"Maßnahme {bestand.Name} nicht gefunden im Katalog.");
                }
            }

            AlleBundeslaender.Add(neuesBL);
        }

        Debug.Log("Alle Bundesländer geladen!");
    }

    void LadeMassnahmen()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("Maßnahmen");
        SektorListe import = JsonUtility.FromJson<SektorListe>(jsonText.text);

        AlleSektoren = import.Sektoren;

        Debug.Log("Alle Maßnahmen geladen!");
    }

    public void BerechneUndZeigeErgebnisse()
    {
        foreach (var bundesland in AlleBundeslaender)
        {
            float investitionskosten = 0f;
            float betriebskosten = 0f;
            float rückbaukosten = 0f;
            float co2Veränderung = 0f;
            float digitalisierungPlus = 0f;
            float akzeptanzPlus = 0f;

            foreach (var kvp in bundesland.MassnahmenBestand)
            {
                string massnahmenName = kvp.Key;
                int anzahl = kvp.Value;
                if (anzahl == 0) continue;

                MassnahmeData massnahme = FindeMassnahmeData(massnahmenName);
                if (massnahme == null) continue;

                if (anzahl > 0)
                {
                    // NEUBAU
                    investitionskosten += massnahme.Kosten * anzahl;
                    betriebskosten += massnahme.Betriebskosten * anzahl;
                    digitalisierungPlus += massnahme.Digitalisierung * anzahl;
                    akzeptanzPlus += massnahme.Akzeptanz * anzahl;
                    co2Veränderung -= massnahme.CO2Ausstoß * anzahl;
                }
                else if (anzahl < 0)
                {
                    // ABBAU (nur Rückbaukosten)
                    rückbaukosten += massnahme.Rückbaukosten * Mathf.Abs(anzahl);
                    co2Veränderung += massnahme.CO2Ausstoß * Mathf.Abs(anzahl);
                    // Optional auch Digitalisierung und Akzeptanz abziehen:
                    digitalisierungPlus -= massnahme.Digitalisierung * Mathf.Abs(anzahl);
                    akzeptanzPlus -= massnahme.Akzeptanz * Mathf.Abs(anzahl);
                }
            }

            float gesamtkosten = investitionskosten + betriebskosten + rückbaukosten;

            bundesland.BerechneteNeueWerte.Gesamtkosten = gesamtkosten;
            bundesland.BerechneteNeueWerte.CO2Aenderung = co2Veränderung;
            bundesland.BerechneteNeueWerte.DigitalisierungPlus = digitalisierungPlus;
            bundesland.BerechneteNeueWerte.AkzeptanzPlus = akzeptanzPlus;
            Debug.Log($"Ergebnis {bundesland.Name}:");
            Debug.Log($"Kosten: {gesamtkosten} €");
            Debug.Log($"CO2-Veränderung: {co2Veränderung} Tonnen");
            Debug.Log($"Digitalisierung: {digitalisierungPlus} Punkte");
            Debug.Log($"Akzeptanz: {akzeptanzPlus} %");
        }
        UIManager.Instance.ZeigeErgebnisTabelle(AlleBundeslaender);
        ZeigeSynergien(); 
        ZeigeLangzeitprojektion(); 
    }

    public void ZeigeSynergien()
    {

    }

    public void ZeigeLangzeitprojektion()
    {

    }

    private MassnahmeData FindeMassnahmeData(string name)
    {
        foreach (var sektor in AlleSektoren)
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


}
