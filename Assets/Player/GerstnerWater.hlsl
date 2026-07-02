#ifndef GERSTNER_WATER_INCLUDED
#define GERSTNER_WATER_INCLUDED

void GerstnerWave_float(
    float3 WorldPos,
    float2 Direction,
    float Amplitude,
    float WaveLength,
    float Speed,
    float Time,
    out float Height)
{
    Direction = normalize(Direction);

    float k = 6.28318530718 / max(WaveLength, 0.001);
    float phase = k * dot(Direction, WorldPos.xz);
    Height = sin(phase + Time * Speed) * Amplitude;
}

#endif