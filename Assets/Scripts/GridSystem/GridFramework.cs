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

    // 内部数据
    private GridCell[,] gridCells;
    private LineRenderer lineRenderer;
    private List<GridCell> lastPath;

    // 寻路集合（复用，减少GC）
    private List<GridCell> openList = new List<GridCell>();
    private HashSet<GridCell> closedList = new HashSet<GridCell>();

    // 参数缓存（检测变化）
    private Vector2 lastCellSize;
    private Vector2Int lastGridSize;
    private Vector3 lastOrigin;
    private float lastHeight;


    #region 初始化
    private void OnEnable()
    {
        InitializeGrid();
        InitializeLineRenderer();
        CacheParams();
        openList.Capacity = gridSize.x * gridSize.y;
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            // 参数变化时重新初始化
            if (cellSize != lastCellSize || gridSize != lastGridSize ||
                originPosition != lastOrigin || gridHeight != lastHeight)
            {
                InitializeGrid();
                CacheParams();
                openList.Capacity = gridSize.x * gridSize.y;
            }
        }
        // 运行时按空格键寻路
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            CalculatePath();
        }
    }

    /// <summary>
    /// 初始化网格单元格
    /// </summary>
    public void InitializeGrid()
    {
        // 修正参数
        gridSize.x = Mathf.Max(1, gridSize.x);
        gridSize.y = Mathf.Max(1, gridSize.y);
        cellSize.x = Mathf.Max(0.1f, cellSize.x);
        cellSize.y = Mathf.Max(0.1f, cellSize.y);
        startCoord = GridToolkit.ClampCoord(startCoord, gridSize);
        endCoord = GridToolkit.ClampCoord(endCoord, gridSize);

        // 创建单元格数组
        gridCells = new GridCell[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPos = GridToolkit.GridToWorld(x, y, cellSize, originPosition, gridHeight);
                gridCells[x, y] = new GridCell(x, y, worldPos, true, true, 1);
            }
        }

        UpdateGridLines();
    }

    /// <summary>
    /// 初始化网格线渲染器
    /// </summary>
    private void InitializeLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.useWorldSpace = true;
        }
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.material.color = gridLineColor;
    }

    private void CacheParams()
    {
        lastCellSize = cellSize;
        lastGridSize = gridSize;
        lastOrigin = originPosition;
        lastHeight = gridHeight;
    }
    #endregion


    #region 网格线绘制
    private void UpdateGridLines()
    {
        if (!showGridLines || lineRenderer == null) return;

        List<Vector3> points = new List<Vector3>();
        float maxX = originPosition.x + gridSize.x * cellSize.x;
        float maxZ = originPosition.z + gridSize.y * cellSize.y;
        float yPos = originPosition.y + gridHeight;

        // 竖线
        for (int x = 0; x <= gridSize.x; x++)
        {
            float xPos = originPosition.x + x * cellSize.x;
            points.Add(new Vector3(xPos, yPos, originPosition.z));
            points.Add(new Vector3(xPos, yPos, maxZ));
        }

        // 横线
        for (int z = 0; z <= gridSize.y; z++)
        {
            float zPos = originPosition.z + z * cellSize.y;
            points.Add(new Vector3(originPosition.x, yPos, zPos));
            points.Add(new Vector3(maxX, yPos, zPos));
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
    #endregion


    #region Gizmos可视化
    private void OnDrawGizmos()
    {
        if (gridCells == null) return;

        // 绘制网格线（编辑模式辅助）
        if (showGridLines)
        {
            Gizmos.color = gridLineColor;
            float maxX = originPosition.x + gridSize.x * cellSize.x;
            float maxZ = originPosition.z + gridSize.y * cellSize.y;
            float yPos = originPosition.y + gridHeight;

            // 竖线
            for (int x = 0; x <= gridSize.x; x++)
            {
                float xPos = originPosition.x + x * cellSize.x;
                Gizmos.DrawLine(new Vector3(xPos, yPos, originPosition.z), new Vector3(xPos, yPos, maxZ));
            }

            // 横线
            for (int z = 0; z <= gridSize.y; z++)
            {
                float zPos = originPosition.z + z * cellSize.y;
                Gizmos.DrawLine(new Vector3(originPosition.x, yPos, zPos), new Vector3(maxX, yPos, zPos));
            }
        }

        // 绘制障碍物
        foreach (var cell in gridCells)
        {
            if (cell.isValid && !cell.isWalkable)
            {
                Gizmos.color = obstacleColor;
                Vector3 center = cell.worldPosition + new Vector3(cellSize.x/2, 0.02f, cellSize.y/2);
                Gizmos.DrawCube(center, new Vector3(cellSize.x*0.9f, 0.04f, cellSize.y*0.9f));
            }
        }

        // 绘制起点终点
        DrawStartEndGizmos();

        // 绘制路径
        if (lastPath != null)
        {
            Gizmos.color = pathColor;
            foreach (var cell in lastPath)
            {
                if (cell.isValid)
                {
                    Vector3 center = cell.worldPosition + new Vector3(cellSize.x/2, 0.03f, cellSize.y/2);
                    Gizmos.DrawCube(center, new Vector3(cellSize.x*0.8f, 0.06f, cellSize.y*0.8f));
                }
            }
        }
    }
    private void DrawStartEndGizmos ()
    {
// 绘制起点
        if (GridToolkit.IsInBounds (startCoord.x, startCoord.y, gridSize))
        {
            GridCell start = gridCells [startCoord.x, startCoord.y];
            Gizmos.color = startColor;
            Vector3 center = start.worldPosition + new Vector3 (cellSize.x/2, 0.06f, cellSize.y/2);
            Gizmos.DrawCube (center, new Vector3 (cellSize.x*0.8f, 0.08f, cellSize.y*0.8f));
        }
// 绘制终点
        if (GridToolkit.IsInBounds (endCoord.x, endCoord.y, gridSize))
        {
            GridCell end = gridCells [endCoord.x, endCoord.y];
            Gizmos.color = endColor;
            Vector3 center = end.worldPosition + new Vector3 (cellSize.x/2, 0.06f, cellSize.y/2);
            Gizmos.DrawCube (center, new Vector3 (cellSize.x*0.8f, 0.08f, cellSize.y*0.8f));
        }
    }
    #endregion
    #region 路径计算与单元格操作
    [ContextMenu ("计算路径")]
    public void CalculatePath ()
    {
        if (gridCells == null)
        {
            Debug.LogWarning ("网格未初始化");
            return;
        }
// 调用工具类的寻路方法
        lastPath = GridToolkit.CalculateAStarPath (
            gridCells,
            gridSize,
            startCoord,
            endCoord,
            openList,
            closedList
        );
    }
    /// <summary>
    /// 设置单元格是否可通行
    /// </summary>
    public void SetCellWalkable(int x, int y, bool walkable)
    {
        if (GridToolkit.IsInBounds(x, y, gridSize))
        {
            GridCell cell = gridCells[x, y];
            cell.isWalkable = walkable;
            gridCells[x, y] = cell;
        }
    }
    /// <summary>
    /// 获取单元格（通过世界坐标）
    /// </summary>
    public GridCell GetCellByWorldPos(Vector3 worldPos)
    {
        Vector2Int gridPos = GridToolkit.WorldToGrid(worldPos, cellSize, originPosition, gridSize);
        return gridCells[gridPos.x, gridPos.y];
    }
    #endregion
    #region 参数验证
    private void OnValidate ()
    {
// 限制坐标在网格范围内
        startCoord = GridToolkit.ClampCoord (startCoord, gridSize);
        endCoord = GridToolkit.ClampCoord (endCoord, gridSize);
// 更新线宽
        if (lineRenderer != null)
        {
            lineRenderer.widthMultiplier = lineWidth;
            lineRenderer.material.color = gridLineColor;
        }
    }
    #endregion
}
