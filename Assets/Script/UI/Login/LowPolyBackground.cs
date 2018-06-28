using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LowPolyBackground : MonoBehaviour
{
    private MeshFilter meshFilter = null;

    public int XCount = 8;      //mesh的x方向平行四边形的数量
    public int YCount = 5;      //mesh的y方向平行四边形的数量
    public float ZOffset = 20;  //mesh的z方向的范围: (-ZOffset, 0)
    public float Speed = 1.0f;  //更新速度

    private Vector3 size = Vector3.zero;    //mesh的大小，(1, 1, 0)，通过缩放铺满整个屏幕
    private Vector3 origin = Vector3.zero;  //mesh的左下角坐标，(-size.x / 2.0f, -size.y / 2.0f, 0)
    private float perX = 0.0f;              //mesh的x方向平行四边形的间隔，size.x / XCount
    private float perY = 0.0f;              //mesh的y方向平行四边形的间隔，size.y / YCount

    private Vector3[] m_vertices; //顶点坐标
    private Vector2[] m_uvs;      //uv坐标
    private Vector3[] m_normals;  //法向量
    private int[] m_triangles;    //面的顶点索引

    private Vector3[] originRandom;     //不重复的顶点，size: (XCount + 1) * (YCount + 1)
    private Vector3[] originVertices;   //重复的顶点，m_vertices的缓存，size: XCount * YCount * 3 * 2
    private List<float> randomWeight = new List<float>();
    private float timer = 0.0f;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = GenerateLowPoly();

        for (int i = 0; i < originVertices.Length; i++)
        {
            float random = Random.Range(0.1f, 2);
            float dir = 1;
            if (random < 1f)
            {
                dir *= -1;
            }
            randomWeight.Add(dir * random);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime * Speed;

        var indexVertices = new List<Vector3>();
        for (int i = 0; i <= YCount; i++)
        {
            for (int j = 0; j <= XCount; j++)
            {
                indexVertices.Add(PosNormal(j, i));
                if (i == YCount || j == XCount || i == 0 || j == 0)
                {
                    continue;
                }
                float offsetX = Mathf.Cos(timer) / 15;
                float offsetY = Mathf.Sin(timer) / 20;
                float offsetZ = Mathf.Sin(timer) * 10;
                Vector3 pos = new Vector3(offsetX, offsetY, offsetZ) * randomWeight[(XCount + 1) * i + j] + originRandom[(XCount + 1) * i + j];
                indexVertices[indexVertices.Count - 1] = pos;
            }
        }
        TransformLowpoly(indexVertices);
    }

    // Mesh生成
    public Mesh GenerateLowPoly()
    {
        size = new Vector3(1, 1, 0);
        origin = new Vector3(-size.x / 2.0f, -size.y / 2.0f, 0);
        perX = size.x / XCount;
        perY = size.y / YCount;

        // 1.顶点坐标及uv坐标生成：
        GenerateVerticesUV();

        // 2.面顶点的索引生成：
        GenerateTriangles();

        // 3.法线计算：
        GenerateNormals();

        // 4.mesh构造
        Mesh mesh = new Mesh
        {
            name = "LowPolyBackground",
            vertices = m_vertices,
            uv = m_uvs,
            triangles = m_triangles,
            normals = m_normals
        };

        return mesh;
    }

    private void GenerateVerticesUV()
    {
        // 不重复的顶点, 边界的点 z = 0, 内部的点 z 随机
        Vector3[] orderVertices = new Vector3[(XCount + 1) * (YCount + 1)];
        // i != 0 && i != YCount && j != 0 && j != XCount
        for (int i = 1; i < YCount; i++)
        {
            for (int j = 1; j < XCount; j++)
            {
                int vertIndex = j + (XCount + 1) * i;
                orderVertices[vertIndex] = PosRandom(j, i);

            }
        }
        // i = 0 || i = YCount
        for (int j = 0; j <= XCount; j++)
        {
            orderVertices[j] = PosNormal(j, 0);
            orderVertices[j + (XCount + 1) * YCount] = PosNormal(j, YCount);
        }
        // (j = 0 || j = XCount ) && (i != 0 && i != YCount)
        for (int i = 1; i < YCount; i++)
        {
            orderVertices[(XCount + 1) * i] = PosNormal(0, i);
            orderVertices[XCount + (XCount + 1) * i] = PosNormal(XCount, i);
        }


        // 顶点坐标及uv坐标生成:
        m_vertices = new Vector3[XCount * YCount * 3 * 2];
        m_uvs = new Vector2[XCount * YCount * 3 * 2];
        // 右下角三角形
        for (int i = 0; i < YCount; i++)
        {
            for (int j = 0; j < XCount; j++)
            {
                int index = j + XCount * i;
                int vertIndex = j + (XCount + 1) * i;

                m_vertices[index * 3] = orderVertices[vertIndex];
                m_vertices[index * 3 + 1] = orderVertices[vertIndex + 1];
                m_vertices[index * 3 + 2] = orderVertices[vertIndex + XCount + 2];

                m_uvs[index * 3] = new Vector2(j * perX, i * perY);
                m_uvs[index * 3 + 1] = new Vector2((j + 1) * perX, i * perY);
                m_uvs[index * 3 + 2] = new Vector2((j + 1) * perX, (i + 1) * perY);
            }
        }
        // 左下角三角形
        for (int i = 0; i < YCount; i++)
        {
            for (int j = 0; j < XCount; j++)
            {
                int index = j + XCount * i;
                int vertIndex = j + (XCount + 1) * i;

                m_vertices[m_vertices.Length / 2 + index * 3] = orderVertices[vertIndex];
                m_vertices[m_vertices.Length / 2 + index * 3 + 1] = orderVertices[vertIndex + XCount + 2];
                m_vertices[m_vertices.Length / 2 + index * 3 + 2] = orderVertices[vertIndex + XCount + 1];
                m_uvs[m_vertices.Length / 2 + index * 3] = new Vector2(j * perX, i * perY);
                m_uvs[m_vertices.Length / 2 + index * 3 + 1] = new Vector2((j + 1) * perX, (i + 1) * perY);
                m_uvs[m_vertices.Length / 2 + index * 3 + 2] = new Vector2((j) * perX, (i + 1) * perY);
            }
        }


        originRandom = orderVertices;
        originVertices = m_vertices;
    }

    private void GenerateTriangles()
    {
        m_triangles = new int[XCount * YCount * 6];
        for (int i = 0, count = 0, total = 0; i < m_triangles.Length / 2; count++)
        {
            if (((count + 1) % (XCount + 1)).Equals(0))
            {
                continue;
            }
            m_triangles[i] = total + 1;
            m_triangles[i + 1] = total;
            m_triangles[i + 2] = total + 2;
            i += 3;
            total += 3;
        }
        for (int i = m_triangles.Length / 2, count = 0, total = m_vertices.Length / 2; i < m_triangles.Length; count++)
        {
            if (((count + 1) % (XCount + 1)).Equals(0))
            {
                continue;
            }
            m_triangles[i] = total + 2;
            m_triangles[i + 1] = total + 1;
            m_triangles[i + 2] = total;
            i += 3;
            total += 3;
        }
    }

    private void GenerateNormals()
    {
        // 法线必须是手动计算，以保证绘制的三个点的法线一致才保证片段着色器作色一致，否则将会因为着色器自动生成渐变色导致色差
        m_normals = new Vector3[m_vertices.Length];
        for (int i = 0; i < m_normals.Length; i += 3)
        {
            // 这里我们不需要计算精确的点的法向量，只需要计算出面向量，及算是平均向量，与相邻的面不相同即可
            Vector3 v1 = m_vertices[i + 1] - m_vertices[i];
            Vector3 v2 = m_vertices[i + 2] - m_vertices[i];
            Vector3 argNormal = -Vector3.Cross(v1, v2).normalized;
            m_normals[i] = argNormal;
            m_normals[i + 1] = argNormal;
            m_normals[i + 2] = argNormal;
        }
    }

    // 计算点的坐标，下面两个函数生成的坐标只有z的不同
    private Vector3 PosNormal(int x, int y)
    {
        return transform.position + origin + new Vector3(x * perX, y * perY, 0);
    }
    private Vector3 PosRandom(int x, int y)
    {
        float offsetZ = Random.Range(-ZOffset, 0);
        return transform.position + origin + new Vector3(x * perX, y * perY, offsetZ);
    }

    // Mesh的变化
    public void TransformLowpoly(List<Vector3> indexVertices)
    {
        // 重新赋值顶点坐标
        for (int i = 0; i < YCount; i++)
        {
            for (int j = 0; j < XCount; j++)
            {
                int index = j + XCount * i;
                int vertIndex = j + (XCount + 1) * i;
                originVertices[index * 3] = indexVertices[vertIndex];
                originVertices[index * 3 + 1] = indexVertices[vertIndex + 1];
                originVertices[index * 3 + 2] = indexVertices[vertIndex + XCount + 2];
            }
        }
        for (int i = 0; i < YCount; i++)
        {
            for (int j = 0; j < XCount; j++)
            {
                int index = j + XCount * i;
                int vertIndex = j + (XCount + 1) * i;
                originVertices[m_vertices.Length / 2 + index * 3] = indexVertices[vertIndex];
                originVertices[m_vertices.Length / 2 + index * 3 + 1] = indexVertices[vertIndex + XCount + 2];
                originVertices[m_vertices.Length / 2 + index * 3 + 2] = indexVertices[vertIndex + XCount + 1];
            }
        }

        // 重新计算法向量
        m_normals = new Vector3[m_vertices.Length];
        for (int i = 0; i < m_normals.Length; i += 3)
        {
            // 这里我们不需要计算精确的点的法向量，只需要计算出面向量，及算是平均向量，与相邻的面不相同即可
            Vector3 v1 = originVertices[i + 1] - originVertices[i];
            Vector3 v2 = originVertices[i + 2] - originVertices[i];
            Vector3 argNormal = -Vector3.Cross(v1, v2).normalized;
            m_normals[i] = argNormal;
            m_normals[i + 1] = argNormal;
            m_normals[i + 2] = argNormal;
        }

        meshFilter.mesh.vertices = originVertices;
        meshFilter.mesh.uv = m_uvs;
        meshFilter.mesh.triangles = m_triangles;
        meshFilter.mesh.normals = m_normals;
    }
}
