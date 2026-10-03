using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("--- TỐC ĐỘ DI CHUYỂN ---")]
    [Tooltip("Tốc độ dự phòng nếu không có GameManager")]
    [SerializeField] private float fallbackSpeed = 6f;

    [Header("--- TỌA ĐỘ TỰ HỦY ---")]
    [Tooltip("Khi bẫy trôi qua mép trái (X < -15) thì tự xóa khỏi bộ nhớ")]
    [SerializeField] private float destroyPosX = -15f;

    [Header("--- CĂN CHỈNH ĐỘ CAO Y ---")]
    [Tooltip("Điền số âm (ví dụ -0.5) để kéo bẫy thấp tụt xuống sát đất!")]
    [SerializeField] private float yOffset = 0f;

    private void Start()
    {
        
        if (yOffset != 0)
        {
            transform.position += new Vector3(0f, yOffset, 0f);
        }
    }
    private void Update()
    {
       
        float speed = fallbackSpeed;
        if (GameManager.Instance != null)
        {
          
            if (GameManager.Instance.currentState != GameState.Playing) return;

            speed = GameManager.Instance.currentSpeed;
        }

        transform.position += Vector3.left * speed * Time.deltaTime;

        
        if (transform.position.x < destroyPosX)
        {
            Destroy(gameObject);
        }
    }
}