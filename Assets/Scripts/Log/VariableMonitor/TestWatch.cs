using UnityEngine;

public class TestWatch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [Watchable("TestFloat")]
    private float testFloat;
    

    
    [Watchable("TestFloat")]
    private static Vector2 testVector2;
    
    [Watchable]
    private int testInt;
    
    

    void Start()
    {
        WatchManager.Instance.Register(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            testFloat += Time.deltaTime;
            testVector2 += Vector2.up * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            testFloat -= Time.deltaTime;
            testVector2 += Vector2.down * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            testVector2 += Vector2.left * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            testVector2 += Vector2.right * Time.deltaTime;
        }
        
        
    }
}
