using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBehavioural : MonoBehaviour{
    public TerrainGenerator terrainGen;

    public TextMeshProUGUI seedValue;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateNewTerrain(){
        terrainGen.GenerateNewTerrain();
    }

    public void SetSeed(float value){
        terrainGen.seed = (int)value;
        seedValue.text = value.ToString();
    }
}
