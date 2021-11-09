using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBehavioural : MonoBehaviour{
    public TerrainGenerator terrainGen;

    public TextMeshProUGUI seedValue, mountainousValue, smoothnessValue, steepnessValue, heightValue, caveSmoothness, terracesValue, scaleValue, fillnessValue, warpFrequencyValue, warpAmplitudeValue, depthFillnessValue, inbetweenLevelMergeValue, heightStartValue, heightEndValue, vegetationProbabilityValue;
    public Slider seedSlider, mountainousSlider, smoothnessSlider, steepnessSlider, heightSlider, caveSmoothnessSlider, terracesSlider, scaleSlider, fillnessSlider, warpFrequencySlider, warpAmplitudeSlider, depthFillnessSlider, inbetweenLevelMergeSlider, heightStartSlider, heightEndSlider, vegetationProbabilitySlider;

    public GameObject waterLevel, waterPostProcessing;
    
    // Start is called before the first frame update
    void Start(){
        seedSlider.value = terrainGen.seed;
        mountainousSlider.value = terrainGen.mountainous;
        smoothnessSlider.value = terrainGen.smoothness;
        steepnessSlider.value = terrainGen.steepness;
        heightSlider.value = terrainGen.height;
        
        caveSmoothnessSlider.value = terrainGen.cave_Smoothness;
        terracesSlider.value = terrainGen.terraces;
        scaleSlider.value = terrainGen.scale;
        fillnessSlider.value = terrainGen.fillness;
        
        warpFrequencySlider.value = terrainGen.warpFrequency;
        warpAmplitudeSlider.value = terrainGen.warpAmplitude;
        depthFillnessSlider.value = terrainGen.depthFillness;
        inbetweenLevelMergeSlider.value = terrainGen.inBetweenLevelMerge;
        
        heightStartSlider.value = terrainGen.warpAmplitude;
        heightEndSlider.value = terrainGen.depthFillness;
        vegetationProbabilitySlider.value = Remap(terrainGen.vegetationProbability, 0.0f, 0.01f, 0.0f, 1.0f);
        
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
        GenerateNewTerrain();
    }
    
    public void SetMountainous(float value){
        terrainGen.mountainous = (int)value;
        mountainousValue.text = value.ToString();
        GenerateNewTerrain();
    }
    
    public void SetSmoothness(float value){
        terrainGen.smoothness = (int)value;
        smoothnessValue.text = value.ToString();
        GenerateNewTerrain();
    }
    
    public void SetSteepness(float value){
        terrainGen.steepness = (int)value;
        steepnessValue.text = value.ToString();
        GenerateNewTerrain();
    }
    
    public void SetHeight(float value){
        terrainGen.height = (int)value;
        heightValue.text = value.ToString();
        GenerateNewTerrain();
    }

    public void SetCaveSmoothness(float value){
        terrainGen.cave_Smoothness = value;
        caveSmoothness.text = value.ToString("F1");
        GenerateNewTerrain();
    }
    
    public void SetTerraces(float value){
        terrainGen.terraces = value;
        terracesValue.text = value.ToString("F1");
        GenerateNewTerrain();
    }
    
    public void SetScale(float value){
        terrainGen.scale = value;
        scaleValue.text = value.ToString("F1");
        GenerateNewTerrain();
    }
    
    public void SetFillness(float value){
        terrainGen.fillness = value;
        fillnessValue.text = value.ToString("F1");
        GenerateNewTerrain();
    }
    
    public void SetWarpFrequency(float value){
        terrainGen.warpFrequency = value;
        warpFrequencyValue.text = value.ToString("F1");
        GenerateNewTerrain();
    }
    
    public void SetWarpAmplitude(float value){
        terrainGen.warpAmplitude = value;
        warpAmplitudeValue.text = value.ToString("F1");
        GenerateNewTerrain();
    }
    
    public void SetDepthFillness(float value){
        terrainGen.depthFillness = (int)value;
        depthFillnessValue.text = value.ToString("F1");
        GenerateNewTerrain();
    }
    
    public void SetInBetweenLevelMerge(float value){
        terrainGen.inBetweenLevelMerge = value;
        inbetweenLevelMergeValue.text = value.ToString("F1");
        GenerateNewTerrain();
    }
    
    public void SetHeightStartValue(float value){
        terrainGen.heightStart = value;
        heightStartValue.text = value.ToString("F1");
        GenerateNewTerrain();
    }
    
    public void SetHeightEndValue(float value){
        terrainGen.inBetweenLevelMerge = value;
        heightEndValue.text = value.ToString("F1");
        GenerateNewTerrain();
    }
    
    public void SetVegetationProbability(float value){
        terrainGen.vegetationProbability = Remap(value, 0.0f, 1.0f, 0.0f, 0.01f);;
        vegetationProbabilityValue.text = value.ToString("F3");
        GenerateNewTerrain();
    }
    
    public static float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public void ToggleGO(GameObject go){
        if (go.activeSelf){
            go.SetActive(false);
        }
        else{
            go.SetActive(true);
        }
    }
}
