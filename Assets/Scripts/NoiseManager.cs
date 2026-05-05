using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Статический класс для управления шумом: хранит источники шума и уведомляет врагов.
/// </summary>
public static class NoiseManager
{
    private static List<NoiseSource> noiseSources = new List<NoiseSource>();

    public static void EmitNoise(Vector3 position, float intensity)
    {
        noiseSources.Add(new NoiseSource(position, intensity, Time.time));
        // Удаляем старые источники (живут 1 секунду)
        noiseSources.RemoveAll(s => Time.time - s.timestamp > 1f);
    }

    public static NoiseSource? GetClosestNoise(Vector3 listenerPos, float hearingRange)
    {
        NoiseSource? closest = null;
        float minDist = Mathf.Infinity;

        foreach (var source in noiseSources)
        {
            float dist = Vector3.Distance(listenerPos, source.position);
            if (dist <= hearingRange && dist < minDist)
            {
                minDist = dist;
                closest = source;
            }
        }
        return closest;
    }

    public struct NoiseSource
    {
        public Vector3 position;
        public float intensity;
        public float timestamp;

        public NoiseSource(Vector3 pos, float intensity, float time)
        {
            position = pos;
            this.intensity = intensity;
            timestamp = time;
        }
    }
}