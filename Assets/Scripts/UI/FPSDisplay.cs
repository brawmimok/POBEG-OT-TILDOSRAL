using UnityEngine;
using TMPro;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text display_Text;
    [SerializeField] private float updateInterval = 0.5f;
    private float smoothedDeltaTime = 0.0f;

    private void Start()
    {
        smoothedDeltaTime = Time.unscaledDeltaTime;
        StartCoroutine(UpdateFPSDisplay());
    }

    private void Update()
    {
        smoothedDeltaTime += (Time.unscaledDeltaTime - smoothedDeltaTime) * 0.1f;
    }

    private IEnumerator UpdateFPSDisplay()
    {
        while (true)
        {
            // Вычисляем FPS на основе накопленного сглаженного времени
            float fps = 1.0f / smoothedDeltaTime;

            // Обновляем «тяжелый» UI только раз в полсекунды
            display_Text.text = $"FPS: {Mathf.Ceil(fps)}";

            yield return new WaitForSecondsRealtime(updateInterval);
        }
    }
}