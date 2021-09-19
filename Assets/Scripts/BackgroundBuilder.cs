using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundBuilder : MonoBehaviour
{
    public GameObject[] Layers;
    public int[] LayerHeights;
    public float HorizontalOffset,VerticalOffset;
    public int HorizontalDistance;
/*
    public void BuildBackGround()
    {
        Vector3 offset = new Vector3(-((HorizontalDistance/2)*HorizontalOffset),7.5f,10);
        GameObject newLayer;
        
        for(int x = HorizontalDistance; x > 0; x--)
        {
            
            int i = 0;
            for(int layers = 0; layers < LayerHeights[i]; layers++)
            {
                for(int y = 0; y < Layers.Length; y++)
                {
                    newLayer = Instantiate(Layers[i],offset,Quaternion.identity,gameObject.transform);
                    newLayer.SetActive(true);
                    offset.y += VerticalOffset;
                }
                i++;
            }
        }
    }*/

    public void BuildBackGround()
    {
        Vector3 offset = new Vector3(-((HorizontalDistance/2)*HorizontalOffset),2.3f,10);
        GameObject newLayer;
        for(int y = 0; y < Layers.Length; y++)
        {
            for(int extraRows = LayerHeights[y]; extraRows > 0; extraRows--)
            {
                for(int x = HorizontalDistance; x > 0; x--)
                {
                    offset.x += HorizontalOffset;
                    newLayer = Instantiate(Layers[y],offset,Quaternion.identity,gameObject.transform);
                    newLayer.SetActive(true);
                }
                
                offset.x = -((HorizontalDistance/2)*HorizontalOffset);
                offset.y += VerticalOffset;
            }
        }
    }
}   
