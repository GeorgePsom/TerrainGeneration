using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBehavioural : MonoBehaviour{
    public TerrainGenerator terrainGen;

    public TextMeshProUGUI seedValue, mountainousValue, smoothnessValue, steepnessValue, heightValue, caveSmoothness, terracesValue, scaleValue, fillnessValue, warpFrequencyValue, warpAmplitudeValue, depthFillnessValue, inbetweenLevelMergeValue;
    public Slider seedSlider, mountainousSlider, smoothnessSlider, steepnessSlider, heightSlider, caveSmoothnessSlider, terracesSlider, scaleSlider, fillnessSlider, warpFrequencySlider, warpAmplitudeSlider, depthFillnessSlider, inbetweenLevelMergeSlider;
    
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
    
    public void SetMountainous(float value){
        terrainGen.mountainous = (int)value;
        mountainousValue.text = value.ToString();
    }
    
    public void SetSmoothness(float value){
        terrainGen.smoothness = (int)value;
        smoothnessValue.text = value.ToString();
    }
    
    public void SetSteepness(float value){
        terrainGen.steepness = (int)value;
        steepnessValue.text = value.ToString();
    }
    
    public void SetHeight(float value){
        terrainGen.height = (int)value;
        heightValue.text = value.ToString();
    }

    public void SetCaveSmoothness(float value){
        terrainGen.cave_Smoothness = value;
        caveSmoothness.text = value.ToString("F1");
    }
    
    public void SetTerraces(float value){
        terrainGen.terraces = value;
        terracesValue.text = value.ToString("F1");
    }
    
    public void SetScale(float value){
        terrainGen.scale = value;
        scaleValue.text = value.ToString("F1");
    }
    
    public void SetFillness(float value){
        terrainGen.fillness = value;
        fillnessValue.text = value.ToString("F1");
    }
    
    public void SetWarpFrequency(float value){
        terrainGen.warpFrequency = value;
        warpFrequencyValue.text = value.ToString("F1");
    }
    
    public void SetWarpAmplitude(float value){
        terrainGen.warpAmplitude = value;
        warpAmplitudeValue.text = value.ToString("F1");
    }
    
    public void SetDepthFillness(float value){
        terrainGen.depthFillness = (int)value;
        depthFillnessValue.text = value.ToString("F1");
    }
    
    public void SetInBetweenLevelMerge(float value){
        terrainGen.inBetweenLevelMerge = value;
        inbetweenLevelMergeValue.text = value.ToString("F1");
    }
}
