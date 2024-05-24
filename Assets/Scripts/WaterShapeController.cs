using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class WaterShapeController : MonoBehaviour
{
    private int corsnersCount = 2;
    [SerializeField] private SpriteShapeController spriteShapeController;
    [SerializeField] private int WavesCount = 6;
    [SerializeField] private float stringStiffness = 0.1f;
    [SerializeField] private float dampening = 0.03f;
    [SerializeField] private float spread = 0.006f;
    [SerializeField] private List<WaterSpringMovement> springs = new();

    private void FixedUpdate()
    {
        foreach (WaterSpringMovement waterSpringComponent in springs)
        {
            waterSpringComponent.WaveSprintUpdate(stringStiffness, dampening);
        }
        UpdateSprings();
    }
    private void SetWaves()
    {
        Spline waterSpline = spriteShapeController.spline;
        int waterPointsCount = waterSpline.GetPointCount();
        for (int i = corsnersCount; i < waterPointsCount - corsnersCount; i++)
        {
            waterSpline.RemovePointAt(corsnersCount);
        }
        Vector3 waterTopLeftCorner = waterSpline.GetPosition(1);
        Vector3 waterTopRightCorner = waterSpline.GetPosition(2);
        float waterWidth = waterTopRightCorner.x - waterTopLeftCorner.x;
        float spacingPerWave = waterWidth / (WavesCount + 1);
    }
    private void UpdateSprings()
    {
        int count = springs.Count;
        float[] left_deltas = new float[count];
        float[] right_deltas = new float[count];
        for (int i = 0; i < count; i++)
        {
            if (i > 0)
            {
                left_deltas[i] = spread * (springs[i].height - springs[i - 1].height);
                springs[i - 1].velocity += left_deltas[i];
            }
            if (i < springs.Count - 1)
            {
                right_deltas[i] = spread * (springs[i].height - springs[i + 1].height);
                springs[i + 1].velocity += right_deltas[i];
            }
        }
    }
    private void Splash(int index, float speed)
    {
        if (index >= 0 && index < springs.Count)
        {
            springs[index].velocity += speed;
        }
    }
}
