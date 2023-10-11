using UnityEngine;

public static class Noise
{
    public static float RemapValue(float value, float initialMin, float initialMax, float outputMin, float outputMax)
    {
        return outputMin + (value - initialMax) * (outputMax - outputMin) / (initialMax - initialMin);
    }

    public static float RemapValueZeroToOne(float value, float outputMin, float outputMax)
    {
        return outputMin + (value - 1) * (outputMax - outputMin);
    }

    public static float RemapValueZeroToOneInt(float value, float outputMin, float outputMax)
    {
        return (int)RemapValueZeroToOne(value, outputMin, outputMax);
    }

    public static float Redistribution(float noise, NoiseSettings noiseSettings)
    {
        return Mathf.Pow(noise * noiseSettings.redistributionModifier, noiseSettings.exponent);
    }

    public static float OctavePerlin(float x, float z, NoiseSettings settings)
    {
        x *= settings.noiseZoom;
        z *= settings.noiseZoom;
        x += settings.noiseZoom;
        z += settings.noiseZoom;
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float amplitudeSum = 0; // Used for normalizing result to 0 - 1 range
        for (int i = 0; i < settings.octaves; i++)
        {
            total += Mathf.PerlinNoise((settings.offset.x + settings.WorldOffset.x + x) * frequency, (settings.offset.y + settings.WorldOffset.y + z) * frequency) * amplitude;

            amplitudeSum += amplitude;

            amplitude *= settings.persistance;
            frequency *= 2;
        }
        return total / amplitudeSum;
    }
}
