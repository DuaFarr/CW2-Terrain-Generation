using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangels;
    float[] fftHeightMap;
    float[,] heightMap;

    int edgesAlongX;
    int edgesAlongZ;

    int xSize = 9;
    int zSize = 9;

    [Header("Toggle For Changing Method")]
    [SerializeField] bool trueForFFTfalseForMPD = false;

    [Header("Midpoint Displacement Fields")]
    [SerializeField][Range(1, 7)] int exponent = 3;
    [SerializeField] float spread = 0.3f;
    [SerializeField] float spreadReductionFactor = 0.5f;
    [SerializeField] float mpdHeightFactor = 2.0f;

    [Header("FFT Fields")]
    [SerializeField] [Range(1, 50)] int powerOfTwo = 8;
    [SerializeField] float perlinFactor = 2.0f;
    [SerializeField] float fftHeightFactor = 5.0f;
    [SerializeField] int filterPow = 2;


    // Start is called before the first frame update
    void Start()
    {
        if (trueForFFTfalseForMPD)
        {
            int temp = powerOfTwo / 2;
            xSize = FastFourierTransform.CalculateLength(temp);
            zSize = FastFourierTransform.CalculateLength(powerOfTwo - temp);
        }
        else 
        {
            xSize = MidPointDisplacement.CalculateLength(exponent);
            zSize = xSize;
        }
        
        edgesAlongX = xSize + 1;
        edgesAlongZ = zSize + 1;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        if (trueForFFTfalseForMPD)
        {
            fftHeightMap = FastFourierTransform.GenerateFFTHeightMap(FastFourierTransform.CalculateLength(powerOfTwo), xSize, zSize, perlinFactor, filterPow);
        }
        else 
        {
            heightMap = MidPointDisplacement.CreatHeightMap(exponent, spread, spreadReductionFactor, 0);
            Debug.Log("HeightMap Length: " + heightMap.GetLength(0));
        }
        /*for (int y = 0; y < heightMap.GetLength(0); y++) 
        {
            for (int x = 0; x < heightMap.GetLength(0); x++) 
            {
                Debug.Log("HeightMap Value[" + x + ", " + y + "]: " + heightMap[x, y]);
            }
        }*/

        /*Debug.Log("FFtHeightMap Length: " + fftHeightMap.Length);
        for (int i = 0; i < fftHeightMap.Length; i++) 
        {
            Debug.Log("HeightMap[" + i + "]: " + fftHeightMap[i]);
        }*/


        CreateShape();
        UpdateMesh();
    }

    void CreateShape() 
    {
        vertices = new Vector3[edgesAlongX * edgesAlongZ];
        triangels = new int[edgesAlongX * edgesAlongZ * 6];
        int size = xSize * zSize;

        for (int i = 0,z = 0; z <= zSize; z++) 
        {
            for (int x = 0; x <= xSize; x++) 
            {
                float y = 0.0f;

                if (trueForFFTfalseForMPD)
                {
                    y = fftHeightMap[i % size] * fftHeightFactor;
                }
                else 
                {
                    y = heightMap[z % zSize, x % xSize] * mpdHeightFactor; 
                }
                
                //Debug.Log("Perlin: " + Mathf.PerlinNoise((float)x * Random.Range(0.3f, 0.9f), (float)y * Random.Range(0.3f, 0.9f)));
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        int vertice = 0;
        int triangle = 0;

        for (int z = 0; z < zSize; z++) 
        {
            for (int x = 0; x < xSize; x++) 
            {
                triangels[triangle + 0] = vertice + 0;
                triangels[triangle + 1] = vertice + edgesAlongX;
                triangels[triangle + 2] = vertice + 1;
                triangels[triangle + 3] = vertice + 1;
                triangels[triangle + 4] = vertice + edgesAlongX;
                triangels[triangle + 5] = vertice + edgesAlongX + 1;

                vertice++;
                triangle += 6;
            }

            vertice++;
        }
    }

    void UpdateMesh() 
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangels;

        mesh.RecalculateNormals();
    }
}
