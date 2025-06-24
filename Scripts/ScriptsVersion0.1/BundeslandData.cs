using System.Collections.Generic;
using UnityEngine;

public class BundeslandData
{
    public string Name;
    public Sprite Wappen;

    public BundeslandStats Startwerte;
    public BundeslandStats AktuelleWerte;

    public List<MassnahmeData> GeplanteMassnahmen;
    public List<MassnahmeData> UmgesetzteMassnahmen;

    public Dictionary<string, int> MassnahmenBestand = new Dictionary<string, int>();
    public ErgebnisWerte BerechneteNeueWerte = new ErgebnisWerte();
}

[System.Serializable]
public class BundeslandJSON
{
    public string Name;
    public string Wappen;
    public float CO2Ausstoﬂ;
    public float Akzeptanz;
    public float Investitionskosten;
    public float Digitalisierungsgrad;
    public float Energieverbrauch;
    public List<MassnahmenBestandJSON> Massnahmen;
}

[System.Serializable]
public class MassnahmenBestandJSON
{
    public string Name;
    public int Anzahl;
}

[System.Serializable]
public class BundeslandJSONList
{
    public List<BundeslandJSON> Bundeslaender;
}

public class ErgebnisWerte
{
    public float Gesamtkosten;
    public float CO2Aenderung;
    public float DigitalisierungPlus;
    public float AkzeptanzPlus;
}
