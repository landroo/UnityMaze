using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maze3DModule : MonoBehaviour
{

    private int[] module = {0, 0, 0, 0,
						    2, 2,
						    3, 3, 3, 3,
						    1, 1, 1, 1,
						    4};

    private int[] angles = {90, 0, 270, 180,
							0, 90,
							0, 90, 270, 180,
							0, 180, 90, 270,
							0};

    public int sizeX = 20; // X size
    public int sizeY = 20; // Y size
    public int offsetX = 0;  // x offset
    public int offsetY = 0;  // y offset
    public int width = 10;  // cell width
    public int height = 10; // cell height

	public GameObject End_Element;
	public GameObject I_Element;
	public GameObject U_Element;
	public GameObject T_Element;
	public GameObject X_Element;

	public GameObject End_Element_path;
	public GameObject I_Element_path;
	public GameObject U_Element_path;
	public GameObject T_Element_path;
	public GameObject X_Element_path;

	private List<List<int>> data = null;
    private List<List<int>> path = null;

    private OPMethod oP;

    // Start is called before the first frame update
    void Start()
    {
        oP = new OPMethod(sizeX, sizeY);
        this.data = oP.createMaze();
        this.path = oP.solveMaze(0, 0, sizeX - 1, sizeY - 1);

		drawModules();

		Debug.Log("maze start (" + sizeX + ", " + sizeY + " path: " + path.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void drawModules()
    {
		for (int i = 0; i < sizeX; i++) {
			for (int j = 0; j < sizeY; j++)
			{
				int type = getMazeType(i, j, sizeX, sizeY) - 1;
				var element = module[type];
				var angle = angles[type];
				var pnt = new Vector3(offsetX + width * i + width / 2, 0, offsetY + height * j + height / 2);

				bool path = false;
				for (int k = 0; k < this.path.Count; k++)
				{
					if (i == this.path[k][0] && j == this.path[k][1])
					{
						path = true;
						break;
					}
				}

				GameObject gameObject = null;
				switch (element)
                {
					case 0: // U
						if(path)
							gameObject = Instantiate(U_Element_path, pnt, Quaternion.identity);
						else
							gameObject = Instantiate(U_Element, pnt, Quaternion.identity);
						break;
					case 1: // End
						if (path)
							gameObject = Instantiate(End_Element_path, pnt, Quaternion.identity);
						else
							gameObject = Instantiate(End_Element, pnt, Quaternion.identity);
						break;
					case 2: // I
						if (path)
							gameObject = Instantiate(I_Element_path, pnt, Quaternion.identity);
						else
							gameObject = Instantiate(I_Element, pnt, Quaternion.identity);
						break;
					case 3: // T
						if (path)
							gameObject = Instantiate(T_Element_path, pnt, Quaternion.identity);
						else
							gameObject = Instantiate(T_Element, pnt, Quaternion.identity);
						break;
					case 4: // X
						if (path)
							gameObject = Instantiate(X_Element_path, pnt, Quaternion.identity);
						else
							gameObject = Instantiate(X_Element, pnt, Quaternion.identity);
						break;

				}

				if (gameObject != null)
				{
					gameObject.transform.localScale = new Vector3(10, 0.01f, 10);
					gameObject.transform.Rotate(0.0f, angle, 0.0f, Space.Self);
					gameObject.transform.parent = GameObject.Find("Maze").transform;
					gameObject.name = "type (" + i + ", " + j + " el: " + element + " type: " + type + " ang: " + angle + ")";
				}

			}
		}
	}

	private int getMazeType(int x, int y, int width, int height)
    {
        var left = false;
        var right = false;
        var up = false;
        var down = false;

        var type = 0;

        if ((data[x][y] & 2) == 0)
            left = true;

		if (x + 1 == width)
			right = true;
		else if((data[x + 1][y] & 2) == 0)
			right = true;

		if ((data[x][y] & 1) == 0)
			up = true;

		if (y + 1 == height)
			down = true;
		else if ((data[x][y + 1] & 1) == 0)
			down = true;

		// U from right to down way, left up wall
		if (left && !right && up && !down)
			type = 1;
		// U from left to down way, right, up wall
		if (!left && right && up && !down)
			type = 2;
		// U from up to left way, right up wall
		if (!left && right && !up && down)
			type = 3;
		// U from up to right way, left down wall
		if (left && !right && !up && down)
			type = 4;

		// I horizontal way, up, down wall
		if (!left && !right && up && down)
			type = 5;
		// I vertical way left, right wall
		if (left && right && !up && !down)
			type = 6;

		// T left, down and right way, up wall
		if (!left && !right && up && !down)
			type = 7;
		// T up, right and down way, left wall
		if (left && !right && !up && !down)
			type = 8;
		// T up, left and down way, right wall
		if (!left && right && !up && !down)
			type = 9;
		// T up, right and left way, down wall
		if (!left && !right && !up && down)
			type = 10;

		// End way from right
		if (!left && right && up && down)
			type = 11;
		// End way from left
		if (left && !right && up && down)
			type = 12;
		// End way from down
		if (left && right && up && !down)
			type = 13;
		// End way from up
		if (left && right && !up && down)
			type = 14;

		// X no wall
		if (!left && !right && !up && !down)
			type = 15;

		return type;

	}
}
