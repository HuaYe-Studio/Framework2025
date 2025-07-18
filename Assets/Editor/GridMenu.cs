using UnityEditor;
using UnityEngine;

public class GridMenuCreator
{
    // 创建基础网格菜单项
    [MenuItem("GameObject/Create Grid/Plane Grid", priority = 30)]
    private static void CreatePlaneGrid()
    {
        // 使用内置方法创建平面
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Plane_Grid";

        // 设置位置到场景中心
        plane.transform.position = Vector3.zero;

        // 添加一个自定义的Grid组件（用于战旗游戏）
        plane.AddComponent<GridComponent>();

        // 注册撤销操作
        Undo.RegisterCreatedObjectUndo(plane, "Create Plane Grid");

        // 选中新创建的对象
        Selection.activeObject = plane;
    }
public class GridComponent : MonoBehaviour
{
    [Range(1, 10)]
    public int gridSize = 5;  // 网格尺寸

    [Tooltip("网格单元大小")]
    [Range(1f,10f)]
    public float cellSize = 1.0f;

    [Tooltip("是否显示网格线")]
    public bool showGrid = true;

    [Tooltip("网格线颜色")]
    public Color gridColor = Color.green;

    // 在场景视图中绘制网格
    private void OnDrawGizmos()
    {
        if (!showGrid) return;

        Gizmos.color = gridColor;
        Vector3 origin = transform.position;

        // 绘制网格线
        for (int i = 0; i <= gridSize; i++)
        {
            // 水平线
            Vector3 startH = origin + new Vector3(0, 0, i * cellSize);
            Vector3 endH = startH + new Vector3(gridSize * cellSize, 0, 0);
            Gizmos.DrawLine(startH, endH);

            // 垂直线
            Vector3 startV = origin + new Vector3(i * cellSize, 0, 0);
            Vector3 endV = startV + new Vector3(0, 0, gridSize * cellSize);
            Gizmos.DrawLine(startV, endV);
        }
    }
}[MenuItem("GameObject/Create Grid/Cube Grid", priority = 31)]
    private static void CreateCubeGrid()
    {
        CreateGridObject("Cube_Grid", PrimitiveType.Cube);
    }

    // 创建六边形网格（高级）
    [MenuItem("GameObject/Create Grid/Hexagonal Grid", priority = 40)]
    private static void CreateHexGrid()
    {
        GameObject hexGrid = new GameObject("Hex_Grid");
        
        // 添加网格组件
        var filter = hexGrid.AddComponent<MeshFilter>();
        var renderer = hexGrid.AddComponent<MeshRenderer>();
        
        // 生成六边形网格
        filter.mesh = GenerateHexMesh(5, 1.0f); // 5x5网格，半径1
        
        // 设置材质
        renderer.material = new Material(Shader.Find("Standard"));
        
        // 添加网格碰撞体
        hexGrid.AddComponent<MeshCollider>();
        
        Undo.RegisterCreatedObjectUndo(hexGrid, "Create Hex Grid");
        Selection.activeObject = hexGrid;
    }

    // 通用网格创建方法
    private static void CreateGridObject(string name, PrimitiveType type)
    {
        GameObject gridObj = GameObject.CreatePrimitive(type);
        gridObj.name = name;
        
        // 设置位置到场景中心
        gridObj.transform.position = Vector3.zero;
        
        // 添加网格组件参考
        gridObj.AddComponent<GridComponent>();
        
        Undo.RegisterCreatedObjectUndo(gridObj, $"Create {type} Grid");
        Selection.activeObject = gridObj;
    }

    // 六边形网格生成算法
    private static Mesh GenerateHexMesh(int size, float radius)
    {
        Mesh mesh = new Mesh();
        
        // 顶点和三角形生成逻辑
        // (实际实现会更复杂，这里为简化示例)
        Vector3[] vertices = new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(radius, 0, 0),
            new Vector3(radius/2, 0, radius * Mathf.Sqrt(3)/2)
        };
        
        int[] triangles = new int[] { 0, 1, 2 };
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }
}

// 可选：添加到网格对象的组件
public class GridComponent : MonoBehaviour
{
    [Range(1, 10)] public int gridSize = 5;
    public Color gridColor = Color.cyan;
    
    private void OnDrawGizmos()
    {
        // 在场景视图中绘制网格
        Gizmos.color = gridColor;
        for (int x = -gridSize; x <= gridSize; x++) {
            for (int z = -gridSize; z <= gridSize; z++) {
                Vector3 pos = new Vector3(x, 0, z);
                Gizmos.DrawWireCube(pos, Vector3.one * 0.95f);
            }
        }
    }
}
