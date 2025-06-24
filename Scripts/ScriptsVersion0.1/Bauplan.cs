using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BauplanEintrag
{
    public string MassnahmenName; 
    public int Amount;         
}

[System.Serializable]
public class BauplanBundesland
{
    public string BundeslandName;
    public List<BauplanEintrag> Massnahmen = new List<BauplanEintrag>();
}
