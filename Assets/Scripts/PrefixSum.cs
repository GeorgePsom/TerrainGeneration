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
        
        ComputeBuffer pSumBuffer = new ComputeBuffer(arraySize, sizeof(int));
        pSumBuffer.SetData(pSum);
        prefixSumCs.SetInt("arraySize", arraySize);
        
        prefixSumCs.SetBuffer(0, "pSum", pSumBuffer);
        prefixSumCs.Dispatch(0, 1, 1, 1);
        
        pSumBuffer.GetData(pSum);
        
        for (int i = 0; i < arraySize; i++){
            Debug.Log(i + " - " + pSum[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
