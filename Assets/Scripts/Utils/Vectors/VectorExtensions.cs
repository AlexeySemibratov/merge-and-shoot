using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 WithX(this Vector3 vector, float xCoord)
    {
        return new Vector3(xCoord, vector.y, vector.z);
    }

    public static Vector3 DirectedTo(this Vector3 vector, Vector3 direction)
    {
        return direction - vector;
    }
}
