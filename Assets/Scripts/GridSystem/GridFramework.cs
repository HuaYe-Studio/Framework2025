using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GridFramework : MonoBehaviour
{
    [Header("网格基础设置")]
    public Vector2 cellSize = new Vector2(1f, 1f);
    public Vector2Int gridSize = new Vector2Int(10, 10);
    public Vector3 originPosition = Vector3.zero;
    public float gridHeight = 0.05f;

    [Header("网格线设置")]
    public Color gridLineColor = Color.gray;
    public float lineWidth = 0.05f;
    public bool showGridLines = true;

    [Header("寻路与可视化")]
    public Color pathColor = Color.green;
    public Color obstacleColor = Color.red;
    public Color startColor = Color.blue;
    public Color endColor = Color.magenta;
    public Vector2Int startCoord;
    public Vector2Int endCoord;
    public float gizmoDuration = 5f;

    // 核心数据（用struct值类型存储单元格，避免null）
    private GridCell[,] gridCells;
    private LineRenderer lineRenderer;
    private List<GridCell> lastPath;

    // 预分配寻路集合（复用，减少GC）
    private List<GridCell> openList = new List<GridCell>();
    private HashSet<GridCell> closedList = new HashSet<GridCell>();

    // 缓存参数
    private Vector2 lastCellSize;
    private Vector2Int lastGridSize;
    private Vector3 lastOrigin;
    private float lastHeight;

    #region 初始化与网格管理
    private void OnEnable()
    {
        InitializeGrid();
        InitializeLineRenderer();
        CacheParams();

        // 预分配集合容量
        openList.Capacity = gridSize.x * gridSize.y;
        closedList.Clear();
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            if (cellSize != lastCellSize || gridSize != lastGridSize ||
                originPosition != lastOrigin || gridHeight != lastHeight)
            {
                InitializeGrid();
                CacheParams();
                openList.Capacity = gridSize.x * gridSize.y; // 重新调整容量
            }
        }
    }

    private void CacheParams()
    {
        lastCellSize = cellSize;
        lastGridSize = gridSize;
        lastOrigin = originPosition;
        lastHeight = gridHeight;
    }

    public void InitializeGrid()
    {
        gridSize.x = Mathf.Max(1, gridSize.x);
        gridSize.y = Mathf.Max(1, gridSize.y);
        cellSize.x = Mathf.Max(0.1f, cellSize.x);
        cellSize.y = Mathf.Max(0.1f, cellSize.y);

        // 初始化单元格（struct值类型，无null）
        gridCells = new GridCell[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPos = GetWorldPosition(x, y);
                gridCells[x, y] = new GridCell(
                    x, y, worldPos, 
                    isValid: true, 
                    isWalkable: true, 
                    cost: 1
                );
            }
        }

        if (showGridLines) UpdateGridLines();
    }

    private void InitializeLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.widthMultiplier = lineWidth;
            lineRenderer.useWorldSpace = true;
        }
        lineRenderer.material.color = gridLineColor;
    }
    #endregion

    #region 网格线绘制
    private void UpdateGridLines()
    {
        if (!showGridLines || lineRenderer == null) return;

        List<Vector3> linePoints = new List<Vector3>();
        float maxX = originPosition.x + gridSize.x * cellSize.x;
        float maxZ = originPosition.z + gridSize.y * cellSize.y;
        float yPos = originPosition.y + gridHeight;

        for (int x = 0; x <= gridSize.x; x++)
        {
            float xPos = originPosition.x + x * cellSize.x;
            linePoints.Add(new Vector3(xPos, yPos, originPosition.z));
            linePoints.Add(new Vector3(xPos, yPos, maxZ));
        }

        for (int z = 0; z <= gridSize.y; z++)
        {
            float zPos = originPosition.z + z * cellSize.y;
            linePoints.Add(new Vector3(originPosition.x, yPos, zPos));
            linePoints.Add(new Vector3(maxX, yPos, zPos));
        }

        lineRenderer.positionCount = linePoints.Count;
        lineRenderer.SetPositions(linePoints.ToArray());
    }

    private void OnDrawGizmos()
    {
        if (!showGridLines || gridCells == null) return;

        Gizmos.color = gridLineColor;
        float maxX = originPosition.x + gridSize.x * cellSize.x;
        float maxZ = originPosition.z + gridSize.y * cellSize.y;
        float yPos = originPosition.y + gridHeight;

        for (int x = 0; x <= gridSize.x; x++)
        {
            float xPos = originPosition.x + x * cellSize.x;
            Gizmos.DrawLine(new Vector3(xPos, yPos, originPosition.z), new Vector3(xPos, yPos, maxZ));
        }

        for (int z = 0; z <= gridSize.y; z++)
        {
            float zPos = originPosition.z + z * cellSize.y;
            Gizmos.DrawLine(new Vector3(originPosition.x, yPos, zPos), new Vector3(maxX, yPos, zPos));
        }

        // 绘制障碍物（用isValid和isWalkable判断，避免null）
        foreach (var cell in gridCells)
        {
            if (cell.isValid && !cell.isWalkable)
            {
                Gizmos.color = obstacleColor;
                Vector3 center = cell.worldPosition + new Vector3(cellSize.x/2, 0, cellSize.y/2);
                Gizmos.DrawCube(center, new Vector3(cellSize.x*0.9f, 0.02f, cellSize.y*0.9f));
            }
        }

        DrawStartEndGizmos();

        if (lastPath != null)
        {
            Gizmos.color = pathColor;
            foreach (var cell in lastPath)
            {
                if (cell.isValid)
                {
                    Vector3 center = cell.worldPosition + new Vector3(cellSize.x/2, 0.03f, cellSize.y/2);
                    Gizmos.DrawCube(center, new Vector3(cellSize.x*0.8f, 0.04f, cellSize.y*0.8f));
                }
            }
        }
    }

    private void DrawStartEndGizmos()
    {
        // 直接获取单元格（通过数组索引，避免null）
        if (IsInBounds(startCoord.x, startCoord.y))
        {
            GridCell start = gridCells[startCoord.x, startCoord.y];
            Gizmos.color = startColor;
            Vector3 center = start.worldPosition + new Vector3(cellSize.x/2, 0.06f, cellSize.y/2);
            Gizmos.DrawCube(center, new Vector3(cellSize.x*0.8f, 0.05f, cellSize.y*0.8f));
        }

        if (IsInBounds(endCoord.x, endCoord.y))
        {
            GridCell end = gridCells[endCoord.x, endCoord.y];
            Gizmos.color = endColor;
            Vector3 center = end.worldPosition + new Vector3(cellSize.x/2, 0.06f, cellSize.y/2);
            Gizmos.DrawCube(center, new Vector3(cellSize.x*0.8f, 0.05f, cellSize.y*0.8f));
        }
    }
    #endregion

    #region 坐标转换与单元格操作
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(
            originPosition.x + x * cellSize.x,
            originPosition.y + gridHeight,
            originPosition.z + y * cellSize.y
        );
    }

    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - originPosition.x) / cellSize.x);
        int y = Mathf.FloorToInt((worldPos.z - originPosition.z) / cellSize.y);
        return new Vector2Int(
            Mathf.Clamp(x, 0, gridSize.x - 1),
            Mathf.Clamp(y, 0, gridSize.y - 1)
        );
    }

    // 用边界检查代替null返回
    public bool TryGetCell(int x, int y, out GridCell cell)
    {
        if (IsInBounds(x, y))
        {
            cell = gridCells[x, y];
            return true;
        }
        cell = default; // struct默认值（非null）
        return false;
    }

    // 边界检查（避免数组越界）
    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y;
    }
    #endregion

    #region A*寻路算法（优化null和GC）
    [ContextMenu("计算路径")]
    public List<GridCell> CalculatePath()
    {
        // 用TryGetCell避免null判断
        if (!TryGetCell(startCoord.x, startCoord.y, out GridCell start) || 
            !TryGetCell(endCoord.x, endCoord.y, out GridCell end) ||
            !start.isWalkable || !end.isWalkable)
        {
            Debug.LogWarning("起点或终点无效");
            lastPath = null;
            return null;
        }

        // 复用集合，清空而非new
        openList.Clear();
        closedList.Clear();
        openList.Add(start);

        lastPath = FindPath(start, end);
        return lastPath;
    }

    private List<GridCell> FindPath(GridCell start, GridCell end)
    {
        while (openList.Count > 0)
        {
            GridCell current = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < current.fCost || 
                    (openList[i].fCost == current.fCost && openList[i].hCost < current.hCost))
                {
                    current = openList[i];
                }
            }

            openList.Remove(current);
            closedList.Add(current);

            if (current.x == end.x && current.y == end.y) // 用坐标判断相等（struct无引用相等）
                return RetracePath(start, end);

            // 获取邻居（通过坐标计算，避免null）
            for (int xOff = -1; xOff <= 1; xOff++)
            {
                for (int yOff = -1; yOff <= 1; yOff++)
                {
                    if (xOff == 0 && yOff == 0) continue;

                    int neighborX = current.x + xOff;
                    int neighborY = current.y + yOff;
                    if (!TryGetCell(neighborX, neighborY, out GridCell neighbor))
                        continue;

                    if (!neighbor.isWalkable || closedList.Contains(neighbor))
                        continue;

                    int newGCost = current.gCost + GetDistance(current, neighbor) + neighbor.cost;
                    if (newGCost < neighbor.gCost || !openList.Contains(neighbor))
                    {
                        // struct是值类型，需重新赋值到数组
                        neighbor.gCost = newGCost;
                        neighbor.hCost = GetDistance(neighbor, end);
                        neighbor.parentX = current.x; // 用坐标存储父节点，避免引用
                        neighbor.parentY = current.y;
                        gridCells[neighbor.x, neighbor.y] = neighbor; // 更新数组中的值

                        if (!openList.Contains(neighbor))
                            openList.Add(neighbor);
                    }
                }
            }
        }

        Debug.LogWarning("找不到路径");
        return null;
    }

    private int GetDistance(GridCell a, GridCell b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return 10 * (dx + dy);
    }

    private List<GridCell> RetracePath(GridCell start, GridCell end)
    {
        List<GridCell> path = new List<GridCell>();
        GridCell current = end;

        while (!(current.x == start.x && current.y == start.y))
        {
            path.Add(current);
            // 通过坐标获取父节点（避免null）
            if (!TryGetCell(current.parentX, current.parentY, out GridCell parent))
                break; // 异常处理，防止死循环
            current = parent;
        }

        path.Reverse();
        return path;
    }
    #endregion

    #region 单元格数据结构（struct值类型）
    [System.Serializable]
    public struct GridCell // 用struct代替class，避免null
    {
        public int x;
        public int y;
        public Vector3 worldPosition;
        public bool isValid; // 标记是否有效，代替null判断
        public bool isWalkable;
        public int cost;

        // 用坐标存储父节点，避免引用类型的null问题
        public int parentX;
        public int parentY;
        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;

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
    #endregion

    private void OnValidate()
    {
        startCoord = ClampCoord(startCoord);
        endCoord = ClampCoord(endCoord);
        if (lineRenderer != null) lineRenderer.widthMultiplier = lineWidth;
    }

    private Vector2Int ClampCoord(Vector2Int coord)
    {
        return new Vector2Int(
            Mathf.Clamp(coord.x, 0, gridSize.x - 1),
            Mathf.Clamp(coord.y, 0, gridSize.y - 1)
        );
    }
}