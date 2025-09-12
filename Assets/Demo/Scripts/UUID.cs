// /*
// Created by Darsan
// */

using System;

public static class UUID
{

    private static long LastId
    {
        get => PrefManager.GetLong(nameof(LastId),-1) == -1 ? DateTime.Now.Ticks : PrefManager.GetLong(nameof(LastId), -1);
        set => PrefManager.SetLong(nameof(LastId),value);
    }

    public static string GetId()
    {
        var newId = LastId + 1;
        LastId = newId;
        return newId.ToString();
    }
}