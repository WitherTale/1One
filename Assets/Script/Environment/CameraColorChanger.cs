using UnityEngine;

public class CameraColorChanger : MonoBehaviour
{
    [Header("--- THAM CHIẾU ---")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TilemapManager tilemapManager;

    [Header("--- BỘ MÀU NỀN THEO MAP (0 -> MAX) ---")]
    [Tooltip("Cài đặt màu nền cho từng Map (0: Overworld, 1: Nether, 2: Sculk, 3: End...)")]
    [SerializeField] private Color[] mapColors;

    [Header("--- CẤU HÌNH ---")]
    [Tooltip("Tốc độ chuyển màu mượt mà giữa các Biome")]
    [SerializeField] private float colorTransitionSpeed = 2f;

    private int currentMapIndex = -1;
    private Color targetColor;

    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        
        if (mapColors != null && mapColors.Length > 0)
        {
            mainCamera.backgroundColor = mapColors[0];
            targetColor = mapColors[0];
        }
    }

    private void Update()
    {
        if (tilemapManager == null || mainCamera == null || mapColors == null || mapColors.Length == 0)
            return;

        int mapIndex = tilemapManager.CurrentMapIndex;

      
        if (mapIndex != currentMapIndex)
        {
            currentMapIndex = Mathf.Clamp(mapIndex, 0, mapColors.Length - 1);
            targetColor = mapColors[currentMapIndex];
        }

        mainCamera.backgroundColor = Color.Lerp(mainCamera.backgroundColor, targetColor, Time.deltaTime * colorTransitionSpeed);
    }
}