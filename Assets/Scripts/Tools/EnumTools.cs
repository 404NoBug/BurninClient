using UnityEngine;
using System;
using Google.Protobuf.Reflection;

public class EnumTools : MonoBehaviour
{
    //根据数值获取枚举的Name
    public static string GetMessageIdEnumNameByKey(UInt64 key)
    {
        return Enum.GetName(typeof(MessageId.MessageId), key);
    }
}

public static class EnumExtension
{
    public static string ToDescription(this Enum val)
    {
        var field = val.GetType().GetField(val.ToString());
        var customAttribute = Attribute.GetCustomAttribute(field, typeof(OriginalNameAttribute));
        if (customAttribute == null) { return val.ToString(); }
        else { return ((OriginalNameAttribute)customAttribute).Name; }
    }
}
