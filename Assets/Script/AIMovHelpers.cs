using UnityEngine;

internal static class AIMovHelpers
{
    public static Vector3 GetDestinationPoint(Vector3 center, float range)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;



        return new Vector3(randomPoint.x, 0, randomPoint.z);
    }
}