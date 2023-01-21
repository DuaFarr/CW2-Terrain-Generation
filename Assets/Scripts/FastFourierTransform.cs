using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public static class FastFourierTransform
{
    public static float[] GenerateFFTHeightMap(int arraySize, int rowNum, int colNum, float perlinFactor, int filterPow) 
    {
        Complex[] heightMapComplex = new Complex[arraySize];

        int row = 0;
        int col = 0;
        for (int i = 0; i < arraySize; i++) 
        {
            heightMapComplex[i] = Mathf.PerlinNoise(row * UnityEngine.Random.Range(0.3f, 0.9f), col * UnityEngine.Random.Range(0.3f, 0.9f)) * perlinFactor;

            row = i / rowNum;
            col = i % colNum;

            //Debug.Log("ComplexHeightmap: " + heightMapComplex[i]);
        }

        FFT(heightMapComplex);

        FrequencyFilter(heightMapComplex, filterPow);

        IFFT(heightMapComplex);

        float[] heightMap = new float[arraySize];
        for (int i = 0; i < arraySize; i++)
        {
            heightMap[i] = (float)heightMapComplex[i].Magnitude;
            //heightMap[i] = (float)heightMapComplex[i].Real;
            //Debug.Log("HeightMapComplex: " + heightMapComplex[i].Magnitude + "    heightMap: " + heightMap[i]);
        }

        return heightMap;
    }

    public static void FFT(Complex[] hm)
    {
        int bits = (int)Math.Log(hm.Length, 2);

        for (int i = 1; i < hm.Length / 2; i++)
        {
            int swapPos = ReverseBit(i, bits);
            var temp = hm[i];
            hm[i] = hm[swapPos];
            hm[swapPos] = temp;
        }

        for (int v = 2; v < hm.Length; v <<= 1) 
        {
            for (int i = 0; i < hm.Length; i += v) 
            {
                for (int j = 0; j < v / 2; j++) 
                {
                    int indexEven = i + j;
                    int indexOdd = i + j + (v / 2);
                    var even = hm[indexEven];
                    var odd = hm[indexOdd];

                    double term = -2 * Math.PI * j / (double)v;
                    Complex exp = new Complex(Math.Cos(term), Math.Sin(term)) * odd;

                    hm[indexEven] = even + exp;
                    hm[indexOdd] = even - exp;
                    //Debug.Log("hm[indexEven]: " + hm[indexEven] + "   hm[indexOdd]: " + hm[indexOdd]);
                }
            }
        }
    }

    public static void IFFT(Complex[] hm) 
    {
        for (int i = 0; i < hm.Length; i++) 
        {
            hm[i] = new Complex(hm[i].Real, -hm[i].Imaginary);
        }

        //Debug.Log("---------------------IFFT------------------");
        FFT(hm);

        for (int i = 0; i < hm.Length; i++) 
        {
            hm[i] = new Complex(real: hm[i].Real / hm.Length, imaginary: -hm[i].Imaginary / hm.Length);
        }
    }

    public static int ReverseBit(int val, int bits)
    {
        int reverseVal = val;
        int count = bits - 1;

        val >>= 1;

        while (val > 0)
        {
            reverseVal = (reverseVal << 1) | (val & 1);

            count--;
            val >>= 1;
        }

        return ((reverseVal << count) & ((1 << bits) - 1));
    }

    public static void FrequencyFilter(Complex[] hm, int frequencyPowfactor) 
    {
        for (int i = 0; i < hm.Length; i++) 
        {
            //hm[i] = 1f / hm[i];
            hm[i] = 1f / Math.Pow(hm[i].Magnitude, frequencyPowfactor);
        }
    }

    public static int CalculateLength(int n) 
    {
        return (int)Mathf.Pow(2, n);
    }
}
