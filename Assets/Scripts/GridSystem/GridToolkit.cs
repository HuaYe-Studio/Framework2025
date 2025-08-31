using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 网格通用工具类，封装坐标转换、寻路算法等可复用功能
/// </summary>
public static class GridToolkit
{
    #region 坐标转换
    /// <summary>
    /// 网格坐标转世界坐标（返回单元格左下角位置）
    /// </summary>
    public static Vector3 GridToWorld(int gridX, int gridY, Vector2 cellSize, Vector3 origin, float gridHeight)
    {
        return new Vector3(
            origin.x + gridX * cellSize.x,
            origin.y + gridHeight,
            origin.z + gridY * cellSize.y
        );
    }

    /// <summary>
    /// 世界坐标转网格坐标（自动限制在网格范围内）
    /// </summary>
    public static Vector2Int WorldToGrid(Vector3 worldPos, Vector2 cellSize, Vector3 origin, Vector2Int gridSize)
    {
        int x = Mathf.FloorToInt((worldPos.x - origin.x) / cellSize.x);
        int y = Mathf.FloorToInt((worldPos.z - origin.z) / cellSize.y);
        return new Vector2Int(
            Mathf.Clamp(x, 0, gridSize.x - 1),
            Mathf.Clamp(y, 0, gridSize.y - 1)
        );
    }
    #endregion


    #region 边界检查
    /// <summary>
    /// 检查网格坐标是否在有效范围内
    /// </summary>
    public static bool IsInBounds(int x, int y, Vector2Int gridSize)
    {
        return x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y;
    }

    /// <summary>
    /// 限制网格坐标在有效范围内
    /// </summary>
    public static Vector2Int ClampCoord(Vector2Int coord, Vector2Int gridSize)
    {
        return new Vector2Int(
            Mathf.Clamp(coord.x, 0, gridSize.x - 1),
            Mathf.Clamp(coord.y, 0, gridSize.y - 1)
        );
    }
    #endregion


    #region A*寻路核心逻辑
    /// <summary>
    /// 计算A*路径（通用方法，需传入网格数据）
    /// </summary>
    public static List<GridCell> CalculateAStarPath(
        GridCell[,] gridCells,
        Vector2Int gridSize,
        Vector2Int startCoord,
        Vector2Int endCoord,
        List<GridCell> openList,
        HashSet<GridCell> closedList
    )
    {
        // 检查起点终点有效性
        if (!IsInBounds(startCoord.x, startCoord.y, gridSize) || !IsInBounds(endCoord.x, endCoord.y, gridSize))
        {
            Debug.LogWarning("起点/终点超出网格范围");
            return null;
        }

        GridCell start = gridCells[startCoord.x, startCoord.y];
        GridCell end = gridCells[endCoord.x, endCoord.y];
        if (!start.isWalkable || !end.isWalkable)
        {
            Debug.LogWarning("起点/终点不可通行");
            return null;
        }

        // 重置数据
        openList.Clear();
        closedList.Clear();
        ResetPathParams(gridCells, gridSize);
        openList.Add(start);

        // 寻路循环
        while (openList.Count > 0)
        {
            GridCell current = GetLowestFCostCell(openList);
            openList.Remove(current);
            closedList.Add(current);

            // 到达终点，回溯路径
            if (current.x == end.x && current.y == end.y)
            {
                return RetracePath(gridCells, start, end);
            }

            // 处理邻居
            ProcessNeighbors(current, gridCells, gridSize, end, openList, closedList);
        }

        Debug.LogWarning("未找到路径");
        return null;
    }

    /// <summary>
    /// 获取开放列表中FCost最低的单元格
    /// </summary>
    private static GridCell GetLowestFCostCell(List<GridCell> openList)
    {
        GridCell lowest = openList[0];
        for (int i = 1; i < openList.Count; i++)
        {
            if (openList[i].fCost < lowest.fCost || 
                (openList[i].fCost == lowest.fCost && openList[i].hCost < lowest.hCost))
            {
                lowest = openList[i];
            }
        }
        return lowest;
    }

    /// <summary>
    /// 处理当前单元格的所有邻居
    /// </summary>
    private static void ProcessNeighbors(
        GridCell current,
        GridCell[,] gridCells,
        Vector2Int gridSize,
        GridCell end,
        List<GridCell> openList,
        HashSet<GridCell> closedList
    )
    {
        // 遍历8个方向
        for (int xOff = -1; xOff <= 1; xOff++)
        {
            for (int yOff = -1; yOff <= 1; yOff++)
            {
                if (xOff == 0 && yOff == 0) continue; // 跳过自身

                int neighborX = current.x + xOff;
                int neighborY = current.y + yOff;
                if (!IsInBounds(neighborX, neighborY, gridSize)) continue;

                GridCell neighbor = gridCells[neighborX, neighborY];
                if (!neighbor.isWalkable || closedList.Contains(neighbor)) continue;

                // 计算新成本
                int newGCost = current.gCost + GetDistance(current, neighbor) + neighbor.cost;
                if (newGCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = GetDistance(neighbor, end);
                    neighbor.parentX = current.x;
                    neighbor.parentY = current.y;
                    gridCells[neighborX, neighborY] = neighbor; // 更新数组（struct值类型需显式赋值）

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 回溯路径（从终点到起点）
    /// </summary>
    private static List<GridCell> RetracePath(GridCell[,] gridCells, GridCell start, GridCell end)
    {
        List<GridCell> path = new List<GridCell>();
        GridCell current = end;

        while (!(current.x == start.x && current.y == start.y))
        {
            path.Add(current);
            // 检查父节点有效性
            if (!IsInBounds(current.parentX, current.parentY, new Vector2Int(gridCells.GetLength(0), gridCells.GetLength(1))))
            {
                Debug.LogWarning("路径回溯失败：父节点越界");
                return null;
            }
            current = gridCells[current.parentX, current.parentY];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }

    /// <summary>
    /// 重置所有单元格的寻路参数
    /// </summary>
    private static void ResetPathParams(GridCell[,] gridCells, Vector2Int gridSize)
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                GridCell cell = gridCells[x, y];
                cell.gCost = 0;
                cell.hCost = 0;
                cell.parentX = -1;
                cell.parentY = -1;
                gridCells[x, y] = cell;
            }
        }
    }

    /// <summary>
    /// 计算两个单元格的曼哈顿距离
    /// </summary>
    public static int GetDistance(GridCell a, GridCell b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return 10 * (dx + dy); // 基础成本系数
    }
    #endregion
}

/// <summary>
/// 网格单元格数据结构（值类型，避免null）
/// </summary>
[System.Serializable]
public struct GridCell
{
    public int x; // 网格X坐标
    public int y; // 网格Y坐标
    public Vector3 worldPosition; // 世界坐标（左下角）
    public bool isValid; // 是否有效
    public bool isWalkable; // 是否可通行
    public int cost; // 通行成本

    // 寻路参数
    public int parentX; // 父节点X坐标
    public int parentY; // 父节点Y坐标
    public int gCost; // 起点到当前节点成本
    public int hCost; // 当前节点到终点预估成本
    public int fCost => gCost + hCost; // 总成本

    public GridCell(int x, int y, Vector3 worldPos, bool isValid, bool isWalkable, int cost)
    {
        this.x = x;
        this.y = y;
        this.worldPosition = worldPos;
        this.isValid = isValid;
        this.isWalkable = isWalkable;
        this.cost = cost;
        this.parentX = -1;
        this.parentY = -1;
        this.gCost = 0;
        this.hCost = 0;
    }
}