using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maze3DBlock : MonoBehaviour
{

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
        oP = new OPMethod(sizeX / 2, sizeY / 2);
        this.data = oP.createMaze();
        this.path = oP.solveMaze(0, 0, sizeX / 2 - 1, sizeY / 2 - 1);

        drawWalls();
        drawFloor();

        Debug.Log("maze start (" + sizeX / 2 + ", " + sizeY / 2 + " path: " + path.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void addBlock(float x, float y, Material material, string info = "")
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = new Vector3(10, 10, 10);
        cube.transform.localPosition = new Vector3(x + 5f, 5f, y + 5f);
        cube.GetComponent<MeshRenderer>().material = material;
        cube.transform.parent = GameObject.Find("Maze").transform;
        cube.name = info;
    }

    private void drawWalls()
    {
        // walls
        for (int j = 0; j < sizeY; j += 2) {
            for (int i = 0; i < sizeX; i += 2) {
                addBlock(offsetX + i * width, offsetY + j * height, wallMaterial, "Corner (" + i + ", " + j + ")");
            }
        }

        for(int i = 0; i < sizeX / 2; i++) {
            for(int j = 0; j < sizeY / 2; j++) {
                // up wall
                if ((data[i][j] & 1) == 0)
                    addBlock(offsetX + width * i * 2 + width, offsetY + height * j * 2, wallMaterial, "Wall (" + i + ", " + j + ")");
                // left wall
                if ((data[i][j] & 2) == 0)
                    addBlock(offsetX + width * i * 2, offsetY + height * j * 2 + height, wallMaterial, "Wall (" + i + ", " + j + ")");
            }
        }

        // bottom
        for (int i = 0; i < sizeX + 1; i++)
            addBlock(offsetX + i * width, offsetY + sizeY * height, wallMaterial, "Bottom (" + i + ", " + sizeY + ")");

        // right
        for (int i = 0; i < sizeY; i++)
            addBlock(offsetX + sizeX * width, offsetY + i * height, wallMaterial, "Rigt (" + sizeX + ", " + i + ")");

    }

    private void drawFloor()
    {
        for (int j = 1; j < sizeY; j += 2)
        {
            for (int i = 1; i < sizeX; i += 2)
            {
                bool path = false;
                for (int k = 0; k < this.path.Count; k++)
                {
                    if ((i - 1) / 2 == this.path[k][0] && (j - 1) / 2 == this.path[k][1])
                    {
                        path = true;
                        break;
                    }
                }

                if (path)
                    addFloor(offsetX + i * width, offsetY + j * height, true, "Path (" + i + ", " + j + ")");
                else
                    addFloor(offsetX + i * width, offsetY + j * height, false, "Floor (" + i + ", " + j + ")");
            }
        }

        for (int i = 0; i < sizeX / 2; i++)
        {
            for (int j = 0; j < sizeY / 2; j++)
            {
                bool path = false;

                for (int k = 0; k < this.path.Count; k++)
                {
                    if (i == this.path[k][0] && j == this.path[k][1])
                    {
                        path = true;
                        break;
                    }
                }

                // up way
                if ((data[i][j] & 1) != 0)
                {
                    if(path) 
                        addFloor(offsetX + width * i * 2 + width, offsetY + height * j * 2, true, "Path (" + i + ", " + j + ")");
                    else
                        addFloor(offsetX + width * i * 2 + width, offsetY + height * j * 2, false, "Floor (" + i + ", " + j + ")");
                }
                // left way
                if ((data[i][j] & 2) != 0)
                {
                    if (path)
                        addFloor(offsetX + width * i * 2, offsetY + height * j * 2 + height, true, "Path (" + i + ", " + j + ")");
                    else
                        addFloor(offsetX + width * i * 2, offsetY + height * j * 2 + height, false, "Floor (" + i + ", " + j + ")");
                }
            }
        }

    }

    private void addFloor(float x, float y, bool path = false, string info = "")
    {
        GameObject gameObject = new GameObject(info, typeof(MeshFilter), typeof(MeshRenderer));
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
}
