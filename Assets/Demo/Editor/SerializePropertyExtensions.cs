using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class SerializePropertyExtensions
{
    public static IEnumerable<T> ToValueEnumerable<T>(this SerializedProperty property)
    {
        if (!property.isArray)
        {
            yield break;
        }

        foreach (var val in property.ToValueEnumerable(typeof(T)))
        {
            yield return val == null ? default(T) : (T) val;
        }
    }

    public static IEnumerable<object> ToValueEnumerable(this SerializedProperty property, Type type)
    {
        if (!property.isArray)
        {
            yield break;
        }

//        Debug.Log(nameof(ToValueEnumerable) + type.Name +" "+property.arraySize);
        for (var i = 0; i < property.arraySize; i++)
        {
            var elementAtIndex = property.GetArrayElementAtIndex(i);
            yield return elementAtIndex.ToObjectValue(type);
        }
    }

    public static T ToObjectValue<T>(this SerializedProperty property)
    {
        var value = ToObjectValue(property, typeof(T));
        return value == null ? default(T) : (T) value;
    }

    public static object ToObjectValue(this SerializedProperty property, Type type)
    {
        if (property.isArray && property.propertyType != SerializedPropertyType.String)
            return null;

        if (property.propertyType.IsBuildInSerializableField())
        {
            return GetBuildInFieldValue(property);
        }

        var instance = Activator.CreateInstance(type);
        if (property.type != type.Name)
        {
            throw new InvalidOperationException($"Value MisMatched Property-{property.type} Type-{type.Name}");
        }

        foreach (var fieldInfo in type
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(info => info.IsPublic ||
                           info.CustomAttributes.Any(data => data.AttributeType == typeof(SerializeField))))
        {
            var fieldProperty = property.FindPropertyRelative(fieldInfo.Name);

            var value = fieldProperty.isArray && fieldProperty.propertyType != SerializedPropertyType.String
                ? fieldProperty.ToValueEnumerable(fieldInfo.FieldType.GetInterfaces()
                        .First(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        .GenericTypeArguments[0])
                    .ConvertToTypeEnumerable(fieldInfo.FieldType)
                : fieldProperty.hasChildren && fieldProperty.propertyType != SerializedPropertyType.String
                    ? fieldProperty.ToObjectValue(fieldInfo.FieldType)
                    : GetBuildInFieldValue(fieldProperty);

            fieldInfo.SetValue(instance, value);
        }

        return instance;
    }


    public static void SetObjectValue<T>(this SerializedProperty property, T value)
    {
        property.SetObjectValue(value,typeof(T));
    }

    public static void Foreach(this SerializedProperty property,Action<SerializedProperty> action)
    {
        if (!property.isArray)
        {
            throw new InvalidOperationException();
        }

        for (var i = 0; i < property.arraySize; i++)
        {
            action?.Invoke(property.GetArrayElementAtIndex(i));
        }
    }

    public static IEnumerable<SerializedProperty> ToEnumerable(this SerializedProperty property)
    {
        if (!property.isArray)
        {
            throw new InvalidOperationException();
        }

        for (var i = 0; i < property.arraySize; i++)
        {
            yield return property.GetArrayElementAtIndex(i);
        }
    }

    public static void SetObjectValue(this SerializedProperty property, object value,Type type)
    {
//        Debug.Log(type.Name);
        if (property.type.ToLower() != type.Name.ToLower())
            throw new ArgumentException($"Type Mismatched Property Type:{property.type} Value Type:{type.Name}");

        if (property.isArray && property.propertyType != SerializedPropertyType.String)
        {
//            property.SetObjectValueEnumerable(value as IEnumerable,type);
            return;
        }

        if (property.propertyType.IsBuildInSerializableField())
        {
            SetBuildInFieldValue(property, value);
            return;
        }

        foreach (var fieldInfo in type
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(info => info.IsPublic ||
                           info.CustomAttributes.Any(data => data.AttributeType == typeof(SerializeField))))
        {
            var fieldProperty = property.FindPropertyRelative(fieldInfo.Name);
            if (fieldProperty.isArray && fieldProperty.propertyType != SerializedPropertyType.String)
            {
                fieldProperty.SetObjectValueEnumerable(
                    fieldInfo.GetValue(value) as IEnumerable
                    ,fieldInfo.FieldType.GetInterfaces()
                    .First(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .GenericTypeArguments[0]);
            }
            else if (fieldProperty.hasChildren && fieldProperty.propertyType != SerializedPropertyType.String)
            {
                fieldProperty.SetObjectValue(fieldInfo.GetValue(value));
            }
            else
            {
                SetBuildInFieldValue(fieldProperty, fieldInfo.GetValue(value));
            }
        }
    }

    public static void SetObjectValueEnumerable<T>(this SerializedProperty property, IEnumerable<T> value)
    {
        property.SetObjectValueEnumerable(value,typeof(T));
    }

    public static void SetObjectValueEnumerable(this SerializedProperty property, IEnumerable value,Type elementType)
    {
        property.ClearArray();
        var i = 0;



        foreach (var v in value)
        {
            property.InsertArrayElementAtIndex(i);
            property.GetArrayElementAtIndex(i).SetObjectValue(v,elementType);
            i++;
        }
    }

    //    public static object ConvertToTypeList(this IEnumerable<object> list, Type type)
    //    {
    //        return list.Select(item => Convert.ChangeType(item, type)).ToList();
    //    }

    public static object ConvertToTypeEnumerable(this IEnumerable value, Type type)
    {
        var list = (IList) Activator.CreateInstance(type);
        foreach (var item in value)
        {
            list.Add(item);
        }

        return list;
    }

    private static object GetBuildInFieldValue(SerializedProperty property)
    {
        if (property.propertyType == SerializedPropertyType.Generic)
        {
            Debug.LogError("Invalid Property Field:" + property.propertyType);
            return null;
        }

        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                return property.intValue;
            case SerializedPropertyType.Boolean:
                return property.boolValue;
            case SerializedPropertyType.Float:
                return property.floatValue;
            case SerializedPropertyType.String:
                return property.stringValue;
            case SerializedPropertyType.Color:
                return property.colorValue;
            case SerializedPropertyType.ObjectReference:
                return property.objectReferenceValue;
            case SerializedPropertyType.LayerMask:
                return property.intValue;
            case SerializedPropertyType.Enum:
                return property.enumValueIndex;
            case SerializedPropertyType.Vector2:
                return property.vector2Value;
            case SerializedPropertyType.Vector3:
                return property.vector3Value;
            case SerializedPropertyType.Vector4:
                return property.vector4Value;
            case SerializedPropertyType.Rect:
                return property.rectValue;
            case SerializedPropertyType.ArraySize:
                return property.arraySize;
            case SerializedPropertyType.Character:
                return property.stringValue;
            case SerializedPropertyType.AnimationCurve:
                return property.animationCurveValue;
            case SerializedPropertyType.Bounds:
                return property.boundsValue;
//            case SerializedPropertyType.Gradient:
//                return property.;
            case SerializedPropertyType.Quaternion:
                return property.quaternionValue;
            case SerializedPropertyType.ExposedReference:
                return property.exposedReferenceValue;

            case SerializedPropertyType.FixedBufferSize:
                return property.fixedBufferSize;
            case SerializedPropertyType.Vector2Int:
                return property.vector2IntValue;
            case SerializedPropertyType.Vector3Int:
                return property.vector3IntValue;
            case SerializedPropertyType.RectInt:
                return property.rectIntValue;
            case SerializedPropertyType.BoundsInt:
                return property.boundsIntValue;
            default:
                Debug.LogError("Invalid Property Field:" + property.propertyType);
                return null;
        }
    }

    private static void SetBuildInFieldValue(SerializedProperty property, object value)
    {
        if (property.hasChildren && property.propertyType != SerializedPropertyType.String)
        {
            Debug.LogError("Invalid Property Field:" + property.propertyType);
        }

        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                property.intValue = (int) value;
                break;
            case SerializedPropertyType.Boolean:
                property.boolValue = (bool) value;
                break;
            case SerializedPropertyType.Float:
                property.floatValue = (float) value;
                break;
            case SerializedPropertyType.String:
                property.stringValue = (string) value;
                break;
            case SerializedPropertyType.Color:
                property.colorValue = (Color) value;
                break;
            case SerializedPropertyType.ObjectReference:
                property.objectReferenceValue = value as UnityEngine.Object;
                break;
            case SerializedPropertyType.LayerMask:
                property.intValue = (int) value;
                break;
            case SerializedPropertyType.Enum:
                property.enumValueIndex = (int) value;
                break;
            case SerializedPropertyType.Vector2:
                property.vector2Value = (Vector2) value;
                break;
            case SerializedPropertyType.Vector3:
                property.vector3Value = (Vector3) value;
                break;
            case SerializedPropertyType.Vector4:
                property.vector4Value = (Vector4) value;
                break;
            case SerializedPropertyType.Rect:
                property.rectValue = (Rect) value;
                break;
            case SerializedPropertyType.ArraySize:
                property.arraySize = (int) value;
                break;
            case SerializedPropertyType.Character:
                property.stringValue = (string) value;
                break;
            case SerializedPropertyType.AnimationCurve:
                property.animationCurveValue = (AnimationCurve) value;
                break;
            case SerializedPropertyType.Bounds:
                property.boundsValue = (Bounds) value;
                break;
//            case SerializedPropertyType.Gradient:
//                return property.;
            case SerializedPropertyType.Quaternion:
                property.quaternionValue = (Quaternion) value;
                break;
            case SerializedPropertyType.ExposedReference:
                property.exposedReferenceValue = value as UnityEngine.Object;
                break;
//            case SerializedPropertyType.FixedBufferSize:
//                property.fixedBufferSize = value;
            case SerializedPropertyType.Vector2Int:
                property.vector2IntValue = (Vector2Int) value;
                break;
            case SerializedPropertyType.Vector3Int:
                property.vector3IntValue = (Vector3Int) value;
                break;
            case SerializedPropertyType.RectInt:
                property.rectIntValue = (RectInt) value;
                break;
            case SerializedPropertyType.BoundsInt:
                property.boundsIntValue = (BoundsInt) value;
                break;

            default:
                Debug.LogError("Invalid Property Field:" + property.propertyType);
                break;
        }
    }

    public static bool IsBuildInSerializableField(this SerializedPropertyType type)
    {
        return type != SerializedPropertyType.ArraySize && type != SerializedPropertyType.Generic;
    }
}