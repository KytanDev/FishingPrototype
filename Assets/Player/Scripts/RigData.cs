using UnityEngine;

public enum RigPartType
{
    Rod,
    Line,
    Float
}

public enum FloatWaterState
{
    Airborne,
    InWater
}

[System.Serializable]
public class RigPartData
{
    public RigPartType partType;
    public float massKg = 1f;
}

[System.Serializable]
public class PoleRigData
{
    [Header("Parts")]
    public RigPartData rod = new RigPartData { partType = RigPartType.Rod, massKg = 1f };
    public RigPartData line = new RigPartData { partType = RigPartType.Line, massKg = 0.05f };
    public RigPartData floatPart = new RigPartData { partType = RigPartType.Float, massKg = 0.02f };

    [Header("Line Settings")]
    public float lineLength = 4f;
    public float lineSlack = 0.2f;
    public float tensionStrength = 40f;
    public float damping = 4f;
    public float maxTensionForce = 10f;

    [Header("Float Buoyancy")]
    public float floatHeight = 0.4f;
    public float floatBuoyancyMultiplier = 1.4f;
    public float waterLinearDamping = 2.5f;
    public float airLinearDamping = 0.05f;
    public float waterAngularDamping = 2f;
    public float airAngularDamping = 0.05f;
    public float waterVerticalDamping = 0.15f;
}