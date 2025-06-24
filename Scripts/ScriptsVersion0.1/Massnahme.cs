using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Sektor
{
    public string Name;
    public string Icon;  // Sprite-Pfad für den Button-Icon im Sektor
    public List<MassnahmeData> Massnahmen;
}

[System.Serializable]
public class Synergie
{
    public string Name;
}

[System.Serializable]
public class MassnahmeData
{
    public string Name;
    public string Sektor;  
    public string IconPath;
    public string Beschreibung;

    public float CO2Ausstoß;
    public float Kosten;
    public float Digitalisierung;
    public float Akzeptanz;
    public float Bauzeit;
    public float Betriebskosten;
    public float Versorgungssicherheit;
    public float Umweltbelastung;
    public float Subventionen;
    public float Resosourcenverbrauch;
    public float Rückbaukosten;
    public float Importabhängigkeit;
    public float Wartungsaufwand;

    public List<Synergie> Synergien;
}

[System.Serializable]
public class SektorListe
{
    public List<Sektor> Sektoren;
}
