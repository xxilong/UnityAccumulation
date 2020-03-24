using System;
using UnityEngine;

[ExecuteInEditMode]
public class HeatMap : MonoBehaviour
{
    [SerializeField] private Camera rayCamera;
    [SerializeField]
    private Transform point;
    private Projector projector;
    public Gradient gradient;   
    [SerializeField]
    private Vector2[] vectors;
    private Texture2D texture;
    private int radius = 64;
    private const float limit = 1.74f;
    private const int size = 128;
    [SerializeField] private Vector2[] _pointers=new Vector2[3];


    public Vector2[] Pointers {
        set{_pointers = value;}
        get { return _pointers; }
        }

    Color[] colors;

    private void Start()
    {
        projector = GetComponent<Projector>();
        texture = new Texture2D(size, size);
        colors = new Color[vectors.Length];
        //CreatTexture(vectors);
        //projector.material.SetTexture("_ShadowTex", texture);
        GetVectors();
    }

    private void Update()
    {
        GetVectors();
        CreatTexture(vectors);
        projector.material.SetTexture("_ShadowTex", texture);
    }

    private void GetVectors()
    {
        //print("GetVectors");
        for (int i = 0; i < _pointers.Length; i++)
        {
            vectors[i]= CalculatePixelPosition(GetWorldPosition(_pointers[i]));
        }
    }

    Color temp;
    Vector2 vector;

    private void CreatTexture(Vector2[] points)
    { 
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                vector = new Vector2(x, y);
                temp = Color.clear;
                float dis = 0;
                for (int i = 0; i < points.Length; i++)
                {
                    dis += Mathf.Clamp01(1 - Vector2.Distance(vector, points[i]) / radius);

                    dis = Mathf.Clamp01(dis);
                }
                
                temp = CalcColor(dis);
                texture.SetPixel(x, y, temp);
            }
        }
        texture.Apply();
    }
   
    private Color CalcColor(float dis)
    {
        return gradient.Evaluate(dis);
    }
    Ray ray;
    RaycastHit hit;
    /// <summary>
    /// 获取对应的世界坐标
    /// </summary>
    /// <param name="screenPosion"></param>
    /// <returns></returns>
    private Vector2 GetWorldPosition(Vector2 screenPosion)
    {
        ray = rayCamera.ViewportPointToRay(screenPosion);

        if (Physics.Raycast(ray, out hit))
        {
            point.position = hit.point;
            return point.localPosition;
        }
        return Vector2.zero;
    }

    /// <summary>
    /// 计算像素坐标
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Vector2 CalculatePixelPosition(Vector2 position)
    {
        return position / limit * size;
    }
}