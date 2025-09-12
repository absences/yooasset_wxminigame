using System;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;

public struct Line
{
    private float _m;

    public float M
    {
        get => IsVertical?Mathf.Infinity : _m;
        set { _m = value; }
    }

    public float C { get; set; }

    public float GetY(float x) =>  M * x + C;
    public float GetX(float y) => IsVertical? XVertical : (Math.Abs(M) < float.Epsilon? C : (y - C) / M);

    public bool IsVertical { get; set; }
    public float XVertical { get; set; }


    public Line(float m,float c)
    {
        this.C = c;
        _m = m;
        IsVertical = false;
        XVertical = 0;
        
    }

    public Line(Vector2 relVec,Vector2? baseVec = null)
    {
        baseVec = baseVec ?? Vector2.zero;

        if (Mathf.Abs(relVec.x) < 0.00001)
        {
            _m = Mathf.Infinity;
            C = 0;
            IsVertical = true;
            XVertical = baseVec.Value.x;
            return;
        }

        _m = relVec.y / relVec.x;
        var realVec = relVec + baseVec.Value;
        C = realVec.y - _m * realVec.x;
        XVertical = 0;
        IsVertical = false;
    }

    public Line(bool vertical, float xVertical)
    {
        IsVertical = vertical;
        XVertical = xVertical;
        _m = Mathf.Infinity;
        C = 0;
    }

    public override string ToString()
    {
        return IsVertical ? $"X = {XVertical}" : "m:" + M.ToString(CultureInfo.InvariantCulture) + " c:" + C.ToString(CultureInfo.InvariantCulture);
    }

    public static Vector2 GetCrossPoint(Line firstLine, Line secondLine)
    {
        if (firstLine.IsVertical && secondLine.IsVertical|| Math.Abs(firstLine.M - secondLine.M) < float.Epsilon)
            throw new InvalidOperationException();

        if (firstLine.IsVertical || secondLine.IsVertical)
        {
            var verticalLine = firstLine.IsVertical ? firstLine : secondLine;
            var otherLine = firstLine.IsVertical ? secondLine : firstLine;
            return new Vector2(verticalLine.XVertical,otherLine.GetY(verticalLine.XVertical));
        }

        var x = (secondLine.C - firstLine.C) / (firstLine.M - secondLine.M);
        var y = firstLine.M * x + firstLine.C;

        return new Vector2(x,y);
    }

    public float GetDistanceToPoint(Vector2 point)
    {
        // ReSharper disable once PossibleInvalidOperationException
        var crossPoint = GetCrossPoint(GetPerpendicularLine(point),this);
        return (point - crossPoint).magnitude;
    }

    public Vector2 GetReflectPoint(Vector2 point)
    {
        var crossPoint = GetCrossPoint(GetPerpendicularLine(point), this);
        return (crossPoint-point)*2 + point;
    }

    public Vector2 GetNearestDistancePoint(Vector2 point)
    {
        var line = GetPerpendicularLine(point);
        return GetCrossPoint(line,this);
    }

    public Line GetPerpendicularLine(Vector2 point)
    {
        if(IsVertical)
        {
            return new Line(0,point.y);
        }

        if (Mathf.Abs(M) <= 0.00001)
        {
            return new Line(true,point.x);
        }


        var mNew = -1 / M;
        return new Line(mNew,point.y - mNew*point.x);
    }

    public Line GetRotateAroundLine(Vector2 point,float angle)
    {
        var nearestPoint = GetNearestDistancePoint(point);
        var distanceVec = nearestPoint - point;
        var newDisVec = Quaternion.AngleAxis(angle,Vector3.forward)*distanceVec;
        return new Line(newDisVec).GetPerpendicularLine(point + (Vector2) newDisVec);
    }


    public Line GetRelativeLine(Vector2 point)
    {
        if (IsVertical)
        {
            return new Line
            {
                IsVertical = true,
                XVertical = XVertical - point.x
            };
        }

        var newC = GetY(point.x) - point.y; 
        return new Line(M,newC);
    }

    public Vector2 NormalizedVec=> IsVertical? Vector2.up : new Vector2(1,M).normalized;
}