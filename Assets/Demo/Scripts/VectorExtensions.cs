// /*
// Created by Darsan
// */

using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 WithX(this Vector3 vec, float x)
    {
        vec.x = x;
        return vec;
    }
    
    public static Vector3 WithY(this Vector3 vec, float y)
    {
        vec.y = y;
        return vec;
    }

    public static Vector3 WithZ(this Vector3 vec, float z)
    {
        vec.z = z;
        return vec;
    }

    public static Vector2 WithX(this Vector2 vec, float x)
    {
        vec.x = x;
        return vec;
    }

    public static Vector2 WithY(this Vector2 vec, float y)
    {
        vec.y = y;
        return vec;
    }


    public static Vector3 WithXY(this Vector3 vec, float x,float y)
    {
        vec.x = x;
        vec.y = y;
        return vec;
    }

    public static Vector3 WithXZ(this Vector3 vec, float x,float z)
    {
        vec.x = x;
        vec.z = z;
        return vec;
    }

    public static Vector3 WithYZ(this Vector3 vec, float y,float z)
    {
        vec.y = y;
        vec.z = z;
        return vec;
    }


    public static Vector2 ToXZ(this Vector3 vec)
    {
        return new Vector2(vec.x,vec.z);
    }

    // ReSharper disable once InconsistentNaming
    public static Vector3 ToXZtoXYZ(this Vector2 vec,float? y=null)
    {
        return new Vector3(vec.x, y ?? 0, vec.y);
    }
}