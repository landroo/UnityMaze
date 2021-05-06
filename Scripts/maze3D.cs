using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class maze3D : MonoBehaviour
{
    private List<List<int>> testMaze = new List<List<int>>{
        new List<int>{-1, -1},
        new List<int>{-1, -1}
    };
    private List<int> startCell = new List<int> { 0, 0, 0};

    public int sizeX = 20; // X size
    public int sizeY = 20; // Y size
    public int offsetX = 0;  // x offset
    public int offsetY = 0;  // y offset
    public int width = 10;  // cell width
    public int height = 10; // cell height

    public Material wallMaterial;
    public Material floorMaterial;
    public Material pathMaterial;

    private List<List<int>> data = null;
    private List<List<int>> path = null;

    private OPMethod oP;

    // Start is called before the first frame update
    void Start()
    {
        oP = new OPMethod(sizeX, sizeY);
        this.data = oP.createMaze();
        this.path = oP.solveMaze(0, 0, sizeX - 1, sizeY - 1);

        drawMaze(false);
        drawFloor();

        Debug.Log("maze start (" + sizeX + ", " + sizeY + " path: " + path.Count);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        //drawMaze(true);
    }

    // draw maze
    private void drawMaze(bool gizmo)
    {

        Color color = Color.white;
        Gizmos.color = color;

        if (this.data != null)
        {
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    //Debug.Log("i: " + i + " j: " + j + " : " + data[i][j]);

                    float x = offsetX + width * i;
                    float y = offsetY + height * j;

                    // horisontal
                    if ((this.data[i][j] & 1) == 0)
                    {
                        Vector3 point1 = new Vector3(x, 0, y);
                        Vector3 point2 = new Vector3(offsetX + width * (i + 1), 0, offsetY + height * j);

                        if (gizmo) Gizmos.DrawLine(point1, point2);
                        else addWall(x, y, 0, " (" + i + ", " + j + ")");
                    }

                    // vertical
                    if ((this.data[i][j] & 2) == 0)
                    {
                        Vector3 point1 = new Vector3(x, 0, y);
                        Vector3 point2 = new Vector3(offsetX + width * i, 0, offsetY + height * (j + 1));

                        if (gizmo) Gizmos.DrawLine(point1, point2);
                        else addWall(x, y, 270f, " (" + i + ", " + j + ")");
                    }
                }
            }
        }

        if (gizmo)
        {
            Gizmos.DrawLine(new Vector3(offsetX, 0, offsetY + sizeY * height), new Vector3(offsetX + sizeX * width, 0, offsetY + sizeY * height));
            Gizmos.DrawLine(new Vector3(offsetX + sizeX * width, 0, offsetY), new Vector3(offsetX + sizeX * width, 0, offsetY + sizeY * height));
        }
        else
        {
            for (int i = 0; i < sizeX; i++)
            {
                float x = offsetX + width * i;
                addWall(x, offsetY + sizeY * height, 0, " (" + i + ", " + sizeY + ")");
            }
            for (int j = 0; j < sizeY; j++)
            {
                float y = offsetY + height * j;
                addWall(offsetX + sizeX * width, y, 270f, " (" + sizeX + ", " + j + ")");
            }
        }
    }

    // create wall shapes
    private void addWall(float x, float y, float ya = 0, string pos = "")
    {
        GameObject gameObject = new GameObject("Wall" + pos, typeof(MeshFilter), typeof(MeshRenderer));
        gameObject.transform.localScale = new Vector3(10, 10, 1);
        gameObject.transform.localPosition = new Vector3(x, 0, y);
        gameObject.transform.Rotate(0.0f, ya, 0.0f, Space.World);
        gameObject.GetComponent<MeshFilter>().mesh = getRect();
        gameObject.GetComponent<MeshRenderer>().material = wallMaterial;
        gameObject.transform.parent = GameObject.Find("Maze").transform;
    }

    // create rectangle mesh
    private Mesh getRect()
    {
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0, 1);
        vertices[1] = new Vector3(1, 1);
        vertices[2] = new Vector3(0, 0);
        vertices[3] = new Vector3(1, 0);

        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 1);
        uv[1] = new Vector2(1, 1);
        uv[2] = new Vector2(0, 0);
        uv[3] = new Vector2(1, 0);

        int[] triangles = new int[12];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 3;
        // back side
        triangles[6] = 2;
        triangles[7] = 1;
        triangles[8] = 0;
        triangles[9] = 3;
        triangles[10] = 1;
        triangles[11] = 2;

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        return mesh;
    }

    private void drawFloor()
    {
        if (this.data != null)
        {
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    float x = offsetX + width * i;
                    float y = offsetY + height * j;
                    bool path = false;

                    for (int k = 0; k < this.path.Count; k++)
                    {
                        if (i == this.path[k][0] && j == this.path[k][1])
                        {
                            path = true;
                            break;
                        }
                    }
                    addFloor(x, y, path, " (" + i + ", " + j + ")");
                }
            }
        }
    }

    private void addFloor(float x, float y, bool path = false, string pos = "")
    {
        GameObject gameObject = new GameObject("Floor" + pos, typeof(MeshFilter), typeof(MeshRenderer));
        gameObject.transform.localScale = new Vector3(10, 10, 1);
        gameObject.transform.localPosition = new Vector3(x, 0, y);
        gameObject.transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);
        gameObject.GetComponent<MeshFilter>().mesh = getRect();
        if (path)
            gameObject.GetComponent<MeshRenderer>().material = pathMaterial;
        else
            gameObject.GetComponent<MeshRenderer>().material = floorMaterial;
        gameObject.transform.parent = GameObject.Find("Maze").transform;
    }
}
