using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefixSum : MonoBehaviour{

    public ComputeShader prefixSumCs;
    public int arraySize;

    private int[] pSum;
    
    // Start is called before the first frame update
    void Start(){
        pSum = new int[arraySize];

        for (int i = 0; i < arraySize; i++){
            pSum[i] = i;
        }

        int iter = (int)System.Math.Ceiling(System.Math.Log(arraySize, 2))-1;
        ComputeBuffer pSumBuffer = new ComputeBuffer(arraySize, sizeof(int));
        ComputeBuffer initPSumBuffer = new ComputeBuffer(arraySize, sizeof(int));
        
        prefixSumCs.SetInt("arraySize", arraySize);
        prefixSumCs.SetBuffer(0, "initPSum", initPSumBuffer);
        initPSumBuffer.SetData(pSum);
        
        for (int i = 0; i < iter+1; i++){
            pSumBuffer.SetData(pSum);
            prefixSumCs.SetBuffer(0, "pSum", pSumBuffer);
            prefixSumCs.SetInt("iterNum", i);
            
            prefixSumCs.Dispatch(0, 2048/8, 1, 1);
            
            pSumBuffer.GetData(pSum);
            int x = 5;
        }

        /*for (int i = 0; i < arraySize; i++){
            Debug.Log(i + " - " + pSum[i]);
        }*/
        Debug.Log("Done");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
