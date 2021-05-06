using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OPMethod 
{

	private int mSizeX;                 // maze width
	private int mSizeY;                 // maze height

	private List<int> mCellStatus;      // cell state <Integer>
	public List<List<int>> mCellData;	// cell value <ArrayList<Integer>>
	public List<int> mPath;             // solve path <int[]>

	private List<int> aActCell = null;

	public OPMethod(int iSizeX = 20, int iSizeY = 20, List<List<int>> cellData = null, List<int> startCell = null)
	{
		this.mSizeX = iSizeX;
		this.mSizeY = iSizeY;
		this.mCellStatus = new List<int>(mSizeX * mSizeY);
		this.mCellData = new List<List<int>>();
		this.mPath = new List<int>();

		if (cellData == null)
		{
			for (int i = 0; i < this.mSizeX; i++)
			{
				List<int> listLine = new List<int>();
				for (int j = 0; j < this.mSizeY; j++)
				{
					listLine.Add(0);
					this.mCellStatus.Add(-1);
				}
				mCellData.Add(listLine);
			}
			aActCell = new List<int>(3);
		}
		else
        {
			mCellData = cellData;
			aActCell = startCell;
        }
		//createMaze();
	}

	// create maze by optimal path method
	public List<List<int>> createMaze()
	{
		bool bEnd = false;
		int indexSrc;
		int indexDest;
		int tDir = 0;
		List<int> aNewCell = null;
		Stack<List<int>> aStack = new Stack<List<int>>();

		// first cell
		aActCell.Add(UnityEngine.Random.Range(0, this.mSizeX));
		aActCell.Add(UnityEngine.Random.Range(0, this.mSizeY));
		aActCell.Add(0);

		while (true)
		{
			if (aActCell[2] == 15)
			{
				while (aActCell[2] == 15)
				{
					if (aStack.Count == 0)
					{
						bEnd = true;
						break;
					}
					aActCell = aStack.Pop();
				}
				if (bEnd == true)
					break;
			}
			else
			{
				while (aActCell[2] != 15)
				{
					tDir = (int)Math.Pow(2, UnityEngine.Random.Range(0, 4));
					if ((aActCell[2] & tDir) == 0)
						break;
				}

				aActCell[2] = aActCell[2] | tDir;

				indexSrc = aActCell[0] + aActCell[1] * this.mSizeX;

				// left
				if (tDir == 1 && aActCell[0] > 0)
				{
					indexDest = aActCell[0] - 1 + aActCell[1] * mSizeX;
					var bs = baseCell(indexSrc);
					var bd = baseCell(indexDest);
					if (bs != bd)
					{
						mCellStatus[bd] = bs;
						mCellData[aActCell[0]][aActCell[1]] |= 2;

						aNewCell = copyCell(aActCell);
						aStack.Push(aNewCell);
						aActCell[0] -= 1;
						aActCell[2] = 0;
					}
				}

				// right
				if (tDir == 2 && aActCell[0] < this.mSizeX - 1)
				{
					indexDest = aActCell[0] + 1 + aActCell[1] * mSizeX;
					var bs = baseCell(indexSrc);
					var bd = baseCell(indexDest);
					if (bs != bd)
					{
						mCellStatus[bd] = bs;
						mCellData[aActCell[0] + 1][aActCell[1]] |= 2;


						aNewCell = copyCell(aActCell);
						aStack.Push(aNewCell);
						aActCell[0] += 1;
						aActCell[2] = 0;
					}
				}

				// up
				if (tDir == 4 && aActCell[1] > 0)
				{
					indexDest = aActCell[0] + (aActCell[1] - 1) * mSizeX;
					var bs = baseCell(indexSrc);
					var bd = baseCell(indexDest);
					if (bs != bd)
					{
						mCellStatus[bd] = bs;
						mCellData[aActCell[0]][aActCell[1]] |= 1;


						aNewCell = copyCell(aActCell);
						aStack.Push(aNewCell);
						aActCell[1] -= 1;
						aActCell[2] = 0;
					}
				}

				// down
				if (tDir == 8 && aActCell[1] < this.mSizeY - 1)
				{
					indexDest = aActCell[0] + (aActCell[1] + 1) * mSizeX;
					var bs = baseCell(indexSrc);
					var bd = baseCell(indexDest);
					if (bs != bd)
					{
						mCellStatus[bd] = bs;
						mCellData[aActCell[0]][aActCell[1] + 1] |= 1;


						aNewCell = copyCell(aActCell);
						aStack.Push(aNewCell);
						aActCell[1] += 1;
						aActCell[2] = 0;
					}
				}
			}
		}

		return this.mCellData;
	}

	// serach base cell
	private int baseCell(int pIndex)
	{
		// !!!
		if (this.mCellStatus.Count <= pIndex)
			return 0;

		int index = pIndex;
		while (this.mCellStatus[index] >= 0)
			index = this.mCellStatus[index];

		return index;
	}

	// copy cell content into a new 
	private List<int> copyCell(List<int> aOrig)
	{
		List<int> aNewCell = new List<int>(3);
		aNewCell.Add(aOrig[0]);
		aNewCell.Add(aOrig[1]);
		aNewCell.Add(aOrig[2]);
		return aNewCell;
	}

	public List<List<int>> solveMaze(int sx, int sy, int dx, int dy)
    {
		List<List<int>> mazePath = new List<List<int>>();

		var destOK = false;

		List<int> calcPos = new List<int>();
		List<int> cellPos = new List<int>();

		cellPos.Add(sx);
		cellPos.Add(sy);

		List<List<int>> state = new List<List<int>>();
		state.Add(cellPos);

		var step = 0;

		for (int i = 0; i < mSizeX; i++) {
			List<int> ar = new List<int>();
			mazePath.Add(ar);
			for (int j = 0; j < mSizeY; j++)
			{
				mazePath[i].Add(-1);
			}
		}

		mazePath[sx][sy] = step;

		while (destOK == false && state.Count > 0)
        {
			step = step + 1;
			var nextState = new List<List<int>>();

			for( int i = 0; i < state.Count; i++) {

				calcPos = state[i];

				// up
				if (calcPos[1] > 0 && (mazePath[calcPos[0]][calcPos[1] - 1] == -1 && (mCellData[calcPos[0]][calcPos[1]] & 1) != 0))
                {
					mazePath[calcPos[0]][calcPos[1] - 1] = step;
					var nextPos = new List<int>();
					nextPos.Add(calcPos[0]);
					nextPos.Add(calcPos[1] - 1);
					nextState.Add(nextPos);

					if (nextPos[0] == dx && nextPos[1] == dy)
						destOK = true;

				}

				// left
				if (calcPos[0] > 0 && mazePath[calcPos[0] - 1][calcPos[1]] == -1 && (mCellData[calcPos[0]][calcPos[1]] & 2) != 0)
                {
					mazePath[calcPos[0] - 1][calcPos[1]] = step;
					var nextPos = new List<int>();
					nextPos.Add(calcPos[0] - 1);
					nextPos.Add(calcPos[1]);
					nextState.Add(nextPos);

					if (nextPos[0] == dx && nextPos[1] == dy)
						destOK = true;

				}

				// down
				if (calcPos[1] < mSizeY - 1 && mazePath[calcPos[0]][calcPos[1] + 1] == -1 && (mCellData[calcPos[0]][calcPos[1] + 1] & 1) != 0)
                {
					mazePath[calcPos[0]][calcPos[1] + 1] = step;
					var nextPos = new List<int>();
					nextPos.Add(calcPos[0]);
					nextPos.Add(calcPos[1] + 1);
					nextState.Add(nextPos);

					if (nextPos[0] == dx && nextPos[1] == dy)
						destOK = true;

				}

				// rigth
				if (calcPos[0] < mSizeX - 1 && mazePath[calcPos[0] + 1][calcPos[1]] == -1 && (mCellData[calcPos[0] + 1][calcPos[1]] & 2) != 0)
                {
					mazePath[calcPos[0] + 1][calcPos[1]] = step;
					var nextPos = new List<int>();
					nextPos.Add(calcPos[0] + 1);
					nextPos.Add(calcPos[1]);
					nextState.Add(nextPos);

					if (nextPos[0] == dx && nextPos[1] == dy)
						destOK = true;

				}

			}

			state = nextState;
		}

		var tx = dx;
		var ty = dy;
		var ex = false;
		var path = new List<List<int>>();

		if (destOK != false) {

			mazePath[dx][dy] = step;

			var nextPos = new List<int>();
			nextPos.Add(tx);
			nextPos.Add(ty);
			path.Add(nextPos);

			while (tx != sx || ty != sy) {
				step = mazePath[tx][ty];
				ex = false;

				// up
				if (ty > 0 && ex == false && mazePath[tx][ty - 1] == step - 1 && (mCellData[tx][ty] & 1) != 0)
                {
					ty = ty - 1;
					ex = true;
					nextPos = new List<int>();
					nextPos.Add(tx);
					nextPos.Add(ty);
					path.Add(nextPos);
				}

				// left
				if (tx > 0 && ex == false && mazePath[tx - 1][ty] == step - 1 && (mCellData[tx][ty] & 2) != 0)
                {
					tx = tx - 1;
					ex = true;
					nextPos = new List<int>();
					nextPos.Add(tx);
					nextPos.Add(ty);
					path.Add(nextPos);
				}

				// down
				if (ty < mSizeY - 1 && ex == false && mazePath[tx][ty + 1] == step - 1 && (mCellData[tx][ty + 1] & 1) != 0)
                {
					ty = ty + 1;
					ex = true;
					nextPos = new List<int>();
					nextPos.Add(tx);
					nextPos.Add(ty);
					path.Add(nextPos);
				}

				// right
				if (tx < mSizeX - 1 && ex == false && mazePath[tx + 1][ty] == step - 1 && (mCellData[tx + 1][ty] & 2) != 0)
                {
					tx = tx + 1;
					ex = true;
					nextPos = new List<int>();
					nextPos.Add(tx);
					nextPos.Add(ty);
					path.Add(nextPos);
				}
			}
		}

		return path;
	}

}
