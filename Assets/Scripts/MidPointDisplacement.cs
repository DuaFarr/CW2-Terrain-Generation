using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MidPointDisplacement
{
    private static int nom;
    private static int edgeLength;
    private static float spread;
    private static float spreadReductionRate;
    private static float[,] heightMap;



    public static float[,] CreatHeightMap(int n, float sprd, float sprdReductionRate, int seed) 
    {
        nom = n;
        spread = sprd;
        spreadReductionRate = sprdReductionRate;

        //Random.InitState(System.DateTime.Now.Millisecond);
        //Debug.Log(GetRandomValue());

        edgeLength = CalculateLength(n);
        heightMap = new float[edgeLength, edgeLength];

        HeightMapClear();
        RandomiseCorners();
        MidPointDisplacementCalculation();
        //NormaliseHeightMap();

        return heightMap;
    }

    public static int CalculateLength(int n) 
    {
        return (int)Mathf.Pow(2, n) + 1;
    }

    private static void HeightMapClear() 
    {
        for (int y = 0; y < edgeLength; y++) 
        {
            for (int x = 0; x < edgeLength; x++) 
            {
                heightMap[x, y] = 0.0f;
            }
        }
    }

    private static void NormaliseHeightMap() 
    {
        float min = float.MinValue;
        float max = float.MaxValue;

        for (int y = 0; y < edgeLength; y++) 
        {
            for (int x = 0; x < edgeLength; x++) 
            {
                float height = heightMap[x, y];

                if (height < min) 
                {
                    min = height;
                }
                else if (height > max) 
                {
                    max = height;
                }
            }
        }

        for (int y = 0; y < edgeLength; y++) 
        {
            for (int x = 0; x < edgeLength; x++) 
            {
                heightMap[x, y] = heightMap[x, y] / max;
                //heightMap[x, y] = Mathf.InverseLerp(min, max, heightMap[x, y]);
            }        
        }
    }

    private static void RandomiseCorners() 
    {
        float rval = Random.Range(-1.0f, 1.0f);
        heightMap[0, 0] = rval; //Random.Range(-1.0f, 1.0f); //GetRandomValue();
        //Debug.Log("Top Left: " + rval);

        rval = Random.Range(-1.0f, 1.0f);
        heightMap[0, edgeLength - 1] = rval; //Random.Range(-1.0f, 1.0f);
        //Debug.Log("Top Right: " + rval);

        rval = Random.Range(-1.0f, 1.0f);
        heightMap[edgeLength - 1, 0] = rval; //Random.Range(-1.0f, 1.0f);
        //Debug.Log("Bottom Left: " + rval);

        rval = Random.Range(-1.0f, 1.0f);
        heightMap[edgeLength - 1, edgeLength - 1] = rval; //Random.Range(-1.0f, 1.0f);
        //Debug.Log("Bottom Right: " + rval);
    }

    private static void MidPointDisplacementCalculation() 
    {
        int i = 0;

        while (i < nom)
        {
            int totalQuads = (int)Mathf.Pow(4, i);
            int quadsPerRow = (int)Mathf.Sqrt(totalQuads);
            int quadLength = (edgeLength - 1) / quadsPerRow;

            for (int y = 0; y < quadsPerRow; y++) 
            {
                for (int x = 0; x < quadsPerRow; x++) 
                {
                    CalculateMidpoints(quadLength*x, quadLength*(x+1), quadLength*y, quadLength*(y+1));
                }
            }

            spread *= spreadReductionRate;
            i++;
        }
    }

    private static void CalculateMidpoints(int x0, int x1, int y0, int y1) 
    {
        int midX = GetMidpoint(x0, x1);
        //Debug.Log("MidX: " + midX);
        int midY = GetMidpoint(y0, y1);
        //Debug.Log("MidY: " + midY);

        float bottom = heightMap[midX, y0] = AverageOfTwoNumbers(heightMap[x0, y0], heightMap[x1, y0]) + GetOffset();
        //Debug.Log("bottom: " + bottom);
        float top = heightMap[midX, y1] = AverageOfTwoNumbers(heightMap[x0, y1], heightMap[x1, y1]) + GetOffset();
        //Debug.Log("top: " + top);
        float left = heightMap[x0, midY] = AverageOfTwoNumbers(heightMap[x0, y0], heightMap[x0, y1]) + GetOffset();
        //Debug.Log("left: " + left);
        float right = heightMap[x1, midY] = AverageOfTwoNumbers(heightMap[x1, y0], heightMap[x1, y1]) + GetOffset();
        //Debug.Log("right: " + right);

        heightMap[midX, midY] = AverageOfFourNumbers(bottom, top, left, right) + GetOffset();
    }

    private static float AverageOfFourNumbers(float a, float b, float c, float d) 
    {
        return (a + b + c + d) / 2.0f;
    }

    private static float AverageOfTwoNumbers(float a, float b) 
    {
        return (a + b) / 2.0f;
    }

    private static int GetMidpoint(int a, int b) 
    {
        return a + ((b - a) / 2);
    }

    private static float GetOffset() 
    {
        return GetRandomValue() * spread;
    }

    private static float GetRandomValue() 
    {
        return Random.Range(-1.0f, 1.0f);
    }
}
