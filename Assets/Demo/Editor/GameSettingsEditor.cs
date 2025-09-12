using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameSettings))]
public class GameSettingsEditor : Editor
{
    private SerializedProperty _iosAppId;
    //    private SerializedProperty _iosLeadersboardSetting;
    //    private SerializedProperty _androidLeadersboardSetting;
    private SerializedProperty _inAppSetting;
    //    private SerializedProperty _notificationSetting;
//    private SerializedProperty _dailyRewardSetting;


    private SerializedProperty _adsSettings;
    private SerializedProperty _privatePolicySetting;

    private void OnEnable()
    {
        //        _iosLeadersboardSetting = serializedObject.FindProperty(GameSettings.IOS_LEADERSBOARD_SETTINGS);
        //        _androidLeadersboardSetting = serializedObject.FindProperty(GameSettings.ANDROID_LEADERSBOARD_SETTINGS);
        _iosAppId = serializedObject.FindProperty(GameSettings.IOS_APP_ID);
        _inAppSetting = serializedObject.FindProperty(GameSettings.IN_APP_SETTING);
        //        _notificationSetting = serializedObject.FindProperty(GameSettings.NOTIFICATION_SETTING);
//        _dailyRewardSetting = serializedObject.FindProperty(GameSettings.DAILY_REWARD_SETTING);
        _privatePolicySetting = serializedObject.FindProperty(GameSettings.PRIVATE_POLICY_SETTING);
        _adsSettings = serializedObject.FindProperty(GameSettings.ADS_SETTINGS);
    }

    public override void OnInspectorGUI()
    {
        DrawAppId();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        DrawAdsSettings();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //        DrawLeadersBoard();
        //        EditorGUILayout.Space();
        //        EditorGUILayout.Space();
        DrawInApp();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //        DrawNotificationSetting();
        //        EditorGUILayout.Space();
        //        EditorGUILayout.Space();
//        DrawDailyReward();
//        EditorGUILayout.Space();

        DrawPrivatePolicy();
        EditorGUILayout.Space();
        DrawFixIfNeeded();


        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawFixIfNeeded()
    {
        if (MissingSymbols())
        {
            if (GUILayout.Button("Fix Missing Symbols"))
            {
                FixMissingSymbols();
            }
        }
    }

    private void FixMissingSymbols()
    {
        var adsSettings = _adsSettings.ToObjectValue<AdsSettings>();


        HandleScriptingSymbol(BuildTargetGroup.iOS, adsSettings.iosAdmobSetting.enable, "ADMOB");
        HandleScriptingSymbol(BuildTargetGroup.Android, adsSettings.androidAdmobSetting.enable, "ADMOB");



        //        var androidLeadersboardSetting = _androidLeadersboardSetting.ToObjectValue<LeadersboardSetting>();
        //        HandleScriptingSymbol(BuildTargetGroup.Android, androidLeadersboardSetting.enable, "GAME_SERVICE");
        //
        //
        //        var iosLeadersboardSetting = _iosLeadersboardSetting.ToObjectValue<LeadersboardSetting>();
        //        HandleScriptingSymbol(BuildTargetGroup.iOS, iosLeadersboardSetting.enable, "GAME_SERVICE");

        //        var notificationSetting = _notificationSetting.ToObjectValue<NotificationSetting>();

        //        HandleScriptingSymbol(BuildTargetGroup.iOS, notificationSetting.enable, "NOTIFICATION");
        //        HandleScriptingSymbol(BuildTargetGroup.Android, notificationSetting.enable, "NOTIFICATION");

        var inAppSetting = _inAppSetting.ToObjectValue<InAppSetting>();
        HandleScriptingSymbol(BuildTargetGroup.iOS, inAppSetting.enable, "IN_APP");
        HandleScriptingSymbol(BuildTargetGroup.Android, inAppSetting.enable, "IN_APP");


//        var dailyRewardSetting = _dailyRewardSetting.ToObjectValue<DailyRewardSetting>();
//        HandleScriptingSymbol(BuildTargetGroup.iOS, dailyRewardSetting.enable, "DAILY_REWARD");
//        HandleScriptingSymbol(BuildTargetGroup.Android, dailyRewardSetting.enable, "DAILY_REWARD");
    }

    private bool MissingSymbols()
    {
        var adsSettings = _adsSettings.ToObjectValue<AdsSettings>();
        if (adsSettings.iosAdmobSetting.enable && !HaveBuildSymbol(BuildTargetGroup.iOS, "ADMOB"))
        {
            return true;
        }

        if (adsSettings.androidAdmobSetting.enable && !HaveBuildSymbol(BuildTargetGroup.Android, "ADMOB"))
        {
            return true;
        }

        //        if (adsSettings.iosAdColonySettings.enable && !HaveBuildSymbol(BuildTargetGroup.iOS, "ADMOB"))
        //        {
        //            return true;
        //        }


        //        var notificationSetting = _notificationSetting.ToObjectValue<NotificationSetting>();

        //        if (notificationSetting.enable && (!HaveBuildSymbol(BuildTargetGroup.Android, "NOTIFICATION") ||
        //                                           !HaveBuildSymbol(BuildTargetGroup.iOS, "NOTIFICATION")))
        //        {
        //            return true;
        //        }

        //        var androidLeadersboardSetting = _androidLeadersboardSetting.ToObjectValue<LeadersboardSetting>();
        //        if (androidLeadersboardSetting.enable && !HaveBuildSymbol(BuildTargetGroup.Android, "GAME_SERVICE"))
        //            return true;
        //
        //        var iosLeadersboardSetting = _iosLeadersboardSetting.ToObjectValue<LeadersboardSetting>();
        //        if (iosLeadersboardSetting.enable && !HaveBuildSymbol(BuildTargetGroup.iOS, "GAME_SERVICE"))
        //            return true;

        var inAppSetting = _inAppSetting.ToObjectValue<InAppSetting>();
        if (inAppSetting.enable && (!HaveBuildSymbol(BuildTargetGroup.Android, "IN_APP") ||
                                    !HaveBuildSymbol(BuildTargetGroup.iOS, "IN_APP")))
            return true;

//        var dailyRewardSetting = _dailyRewardSetting.ToObjectValue<DailyRewardSetting>();
//        if (dailyRewardSetting.enable && (!HaveBuildSymbol(BuildTargetGroup.Android, "DAILY_REWARD") ||
//                                          !HaveBuildSymbol(BuildTargetGroup.iOS, "DAILY_REWARD")))
//        {
//            return true;
//        }

        return false;
    }

    private void DrawAdsSettings()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Ads Settings", EditorStyles.boldLabel);
        _adsSettings.isExpanded = EditorGUILayout.ToggleLeft("", _adsSettings.isExpanded);
        EditorGUILayout.EndHorizontal();
        if (_adsSettings.isExpanded)
        {
            EditorGUI.indentLevel++;

            //            EditorGUILayout.BeginVertical(GUI.skin.box);
            //            EditorGUILayout.LabelField("Other Settings", EditorStyles.boldLabel);
            _adsSettings.DrawChildrenDefault(nameof(AdsSettings.iosAdmobSetting)
                , nameof(AdsSettings.androidAdmobSetting)
            //                , nameof(AdsSettings.iosAdColonySettings)
            //                , nameof(AdsSettings.androidAdColonySettings)
            );
            //            EditorGUILayout.EndVertical();


            DrawAdmobSetting(_adsSettings.FindPropertyRelative(nameof(AdsSettings.iosAdmobSetting)),
                _adsSettings.FindPropertyRelative(nameof(AdsSettings.androidAdmobSetting)));
            //            DrawAdColonySetting(_adsSettings.FindPropertyRelative(nameof(AdsSettings.iosAdColonySettings)),
            //                _adsSettings.FindPropertyRelative(nameof(AdsSettings.androidAdColonySettings)));

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
    }

    //    private void DrawNotificationSetting()
    //    {
    ////        var enableProperty = _notificationSetting.FindPropertyRelative(nameof(NotificationSetting.enable));
    //
    //        EditorGUILayout.BeginVertical(GUI.skin.box);
    //        EditorGUILayout.BeginHorizontal();
    //        EditorGUILayout.LabelField("Notification Setting", EditorStyles.boldLabel);
    //        var enable = EditorGUILayout.ToggleLeft("", enableProperty.boolValue);
    //        EditorGUILayout.EndHorizontal();
    //        EditorGUI.indentLevel++;
    //
    //        if (enable != enableProperty.boolValue)
    //        {
    //            enableProperty.boolValue = enable;
    //            if (enableProperty.boolValue)
    //            {
    //                AddBuildSymbol(BuildTargetGroup.iOS, "NOTIFICATION");
    //                AddBuildSymbol(BuildTargetGroup.Android, "NOTIFICATION");
    //            }
    //            else
    //            {
    //                RemoveBuildSymbol(BuildTargetGroup.iOS, "NOTIFICATION");
    //                RemoveBuildSymbol(BuildTargetGroup.Android, "NOTIFICATION");
    //            }
    //        }
    //
    //        if (enableProperty.boolValue)
    //        {
    //            _notificationSetting.DrawChildrenDefault(nameof(NotificationSetting.enable));
    //        }
    //
    //        EditorGUI.indentLevel--;
    //        EditorGUILayout.EndVertical();
    //    }

//    private void DrawDailyReward()
//    {
//        EditorGUILayout.BeginVertical(GUI.skin.box);
//        var enableProperty = _dailyRewardSetting.FindPropertyRelative(nameof(DailyRewardSetting.enable));
//        EditorGUILayout.BeginHorizontal();
//        EditorGUILayout.LabelField("Daily Reward Setting", EditorStyles.boldLabel);
//        var enable = EditorGUILayout.ToggleLeft("", enableProperty.boolValue);
//        EditorGUILayout.EndHorizontal();
//        EditorGUI.indentLevel++;
//        if (enable != enableProperty.boolValue)
//        {
//            enableProperty.boolValue = enable;
//            if (enableProperty.boolValue)
//            {
//                AddBuildSymbol(BuildTargetGroup.iOS, "DAILY_REWARD");
//                AddBuildSymbol(BuildTargetGroup.Android, "DAILY_REWARD");
//            }
//            else
//            {
//                RemoveBuildSymbol(BuildTargetGroup.iOS, "DAILY_REWARD");
//                RemoveBuildSymbol(BuildTargetGroup.Android, "DAILY_REWARD");
//            }
//        }
//
//        if (enableProperty.boolValue)
//        {
//            _dailyRewardSetting.DrawChildrenDefault(nameof(DailyRewardSetting.enable));
//        }
//
//        EditorGUI.indentLevel--;
//        EditorGUILayout.EndVertical();
//    }

    private void DrawPrivatePolicy()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        var enableProperty = _privatePolicySetting.FindPropertyRelative(nameof(PrivatePolicySetting.enable));
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Private Policy", EditorStyles.boldLabel);
        enableProperty.boolValue = EditorGUILayout.ToggleLeft("", enableProperty.boolValue);
        EditorGUILayout.EndHorizontal();



        if (enableProperty.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_privatePolicySetting.FindPropertyRelative(nameof(PrivatePolicySetting.url)));
            EditorGUI.indentLevel--;
        }


        EditorGUILayout.EndVertical();
    }


    private void DrawInApp()
    {
        var enableProperty = _inAppSetting.FindPropertyRelative(nameof(InAppSetting.enable));

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("In App", EditorStyles.boldLabel);
        var enable = EditorGUILayout.ToggleLeft("", enableProperty.boolValue);
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel++;
        if (enable != enableProperty.boolValue)
        {
            enableProperty.boolValue = enable;
            if (enableProperty.boolValue)
            {
                AddBuildSymbol(BuildTargetGroup.iOS, "IN_APP");
                AddBuildSymbol(BuildTargetGroup.Android, "IN_APP");
            }
            else
            {
                RemoveBuildSymbol(BuildTargetGroup.iOS, "IN_APP");
                RemoveBuildSymbol(BuildTargetGroup.Android, "IN_APP");
            }
        }


        if (enableProperty.boolValue)
        {
            _inAppSetting.DrawChildrenDefault(nameof(InAppSetting.enable));
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    //    private void DrawLeadersBoard()
    //    {
    //        var iosEnableProperty = _iosLeadersboardSetting.FindPropertyRelative(nameof(LeadersboardSetting.enable));
    //        var androidEnableProperty =
    //            _androidLeadersboardSetting.FindPropertyRelative(nameof(LeadersboardSetting.enable));
    //
    //        EditorGUILayout.BeginVertical(GUI.skin.box);
    //        EditorGUILayout.BeginHorizontal();
    //        EditorGUILayout.LabelField("Leadersboard Setting", EditorStyles.boldLabel);
    //        var toggleValue =
    //            EditorGUILayout.ToggleLeft("", iosEnableProperty.boolValue || androidEnableProperty.boolValue);
    //
    //        if (toggleValue != (androidEnableProperty.boolValue || iosEnableProperty.boolValue))
    //        {
    //            androidEnableProperty.boolValue = toggleValue;
    //            iosEnableProperty.boolValue = toggleValue;
    //        }
    //
    //        EditorGUILayout.EndHorizontal();
    //        if (toggleValue)
    //        {
    //            EditorGUILayout.Space();
    //
    //            EditorGUILayout.BeginVertical(GUI.skin.box);
    //            EditorGUI.indentLevel++;
    //            var iosEnable = EditorGUILayout.Toggle("Ios", iosEnableProperty.boolValue);
    //
    //            if (iosEnable != iosEnableProperty.boolValue)
    //            {
    //                iosEnableProperty.boolValue = iosEnable;
    //
    //                if (iosEnable)
    //                    AddBuildSymbol(BuildTargetGroup.iOS, "GAME_SERVICE");
    //                else
    //                {
    //                    RemoveBuildSymbol(BuildTargetGroup.iOS, "GAME_SERVICE");
    //                }
    //
    //                //            HandleScriptingSymbol(BuildTargetGroup.iOS, iosEnable);
    //            }
    //
    //            if (iosEnableProperty.boolValue)
    //            {
    //                EditorGUI.indentLevel++;
    //                _iosLeadersboardSetting.DrawChildrenDefault(nameof(LeadersboardSetting.enable));
    //                EditorGUI.indentLevel--;
    //            }
    //
    //            EditorGUI.indentLevel--;
    //            EditorGUILayout.EndVertical();
    //            EditorGUILayout.BeginVertical(GUI.skin.box);
    //            EditorGUI.indentLevel++;
    //
    //            var androidEnable = EditorGUILayout.Toggle("Android", androidEnableProperty.boolValue);
    //
    //            if (androidEnable != androidEnableProperty.boolValue)
    //            {
    //                androidEnableProperty.boolValue = androidEnable;
    //
    //                if (androidEnable)
    //                    AddBuildSymbol(BuildTargetGroup.Android, "GAME_SERVICE");
    //                else
    //                {
    //                    RemoveBuildSymbol(BuildTargetGroup.Android, "GAME_SERVICE");
    //                }
    //
    //                //            HandleScriptingSymbol(BuildTargetGroup.iOS, iosEnable);
    //            }
    //
    //            if (androidEnableProperty.boolValue)
    //            {
    //                EditorGUI.indentLevel++;
    //                _androidLeadersboardSetting.DrawChildrenDefault(nameof(LeadersboardSetting.enable));
    //                EditorGUI.indentLevel--;
    //            }
    //
    //            EditorGUI.indentLevel--;
    //            EditorGUILayout.EndVertical();
    //        }
    //
    //        EditorGUILayout.EndVertical();
    //    }

    private void DrawAdmobSetting(SerializedProperty iosAdmobSetting, SerializedProperty androidAdmobSetting)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        iosAdmobSetting.isExpanded = EditorGUILayout.Foldout(iosAdmobSetting.isExpanded, "Admob Setting");

        if (iosAdmobSetting.isExpanded)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;
            var iosEnableProperty = iosAdmobSetting.FindPropertyRelative(nameof(AdmobSetting.enable));
            var iosEnable = EditorGUILayout.Toggle("Ios", iosEnableProperty.boolValue);

            if (iosEnable != iosEnableProperty.boolValue)
            {
                iosEnableProperty.boolValue = iosEnable;
                HandleScriptingSymbol(BuildTargetGroup.iOS, iosEnable, "ADMOB");
            }

            if (iosEnableProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                iosAdmobSetting.DrawChildrenDefault(nameof(AdmobSetting.enable));
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;
            var androidEnableProperty = androidAdmobSetting.FindPropertyRelative(nameof(AdmobSetting.enable));
            var androidEnable = EditorGUILayout.Toggle("Android", androidEnableProperty.boolValue);

            if (androidEnable != androidEnableProperty.boolValue)
            {
                androidEnableProperty.boolValue = androidEnable;
                HandleScriptingSymbol(BuildTargetGroup.Android, androidEnable, "ADMOB");
            }

            if (androidEnableProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                androidAdmobSetting.DrawChildrenDefault(nameof(AdmobSetting.enable));
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();
    }



    //    private void DrawAdColonySetting(SerializedProperty iosAdColonySetting, SerializedProperty androidAdColonySetting)
    //    {
    //        EditorGUILayout.BeginVertical(GUI.skin.box);
    //        iosAdColonySetting.isExpanded = EditorGUILayout.Foldout(iosAdColonySetting.isExpanded, "AdColony Setting");
    //        if (iosAdColonySetting.isExpanded)
    //        {
    //            EditorGUILayout.Space();
    //
    //            EditorGUILayout.BeginVertical(GUI.skin.box);
    //            EditorGUI.indentLevel++;
    //            var iosEnableProperty = iosAdColonySetting.FindPropertyRelative(nameof(AdColonySettings.enable));
    //
    //            var iosEnable = EditorGUILayout.Toggle("Ios", iosEnableProperty.boolValue);
    ////
    //            if (iosEnable != iosEnableProperty.boolValue)
    //            {
    //                iosEnableProperty.boolValue = iosEnable;
    //                HandleScriptingSymbol(BuildTargetGroup.iOS, iosEnable, "ADCLONY");
    //            }
    //
    ////
    //            if (iosEnableProperty.boolValue)
    //            {
    //                EditorGUI.indentLevel++;
    //                iosAdColonySetting.DrawChildrenDefault(nameof(AdColonySettings.enable));
    //
    ////            _iosAdColonySetting.NextVisible(false);
    //                EditorGUI.indentLevel--;
    //            }
    //
    //            EditorGUI.indentLevel--;
    //            EditorGUILayout.EndVertical();
    //            EditorGUILayout.BeginVertical(GUI.skin.box);
    //            EditorGUI.indentLevel++;
    //            var androidEnableProperty = androidAdColonySetting.FindPropertyRelative(nameof(AdColonySettings.enable));
    //            var androidEnable = EditorGUILayout.Toggle("Android", androidEnableProperty.boolValue);
    //
    //            if (androidEnable != androidEnableProperty.boolValue)
    //            {
    //                androidEnableProperty.boolValue = androidEnable;
    //                HandleScriptingSymbol(BuildTargetGroup.Android, androidEnable, "ADCLONY");
    //            }
    //
    //            if (androidEnableProperty.boolValue)
    //            {
    //                EditorGUI.indentLevel++;
    //                androidAdColonySetting.DrawChildrenDefault(nameof(AdColonySettings.enable));
    //                EditorGUI.indentLevel--;
    //            }
    //
    //            EditorGUI.indentLevel--;
    //            EditorGUILayout.EndVertical();
    //        }
    //
    //        EditorGUILayout.EndVertical();
    //    }


    private void DrawAppId()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("App Ids", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(_iosAppId);
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    private static bool HaveBuildSymbol(BuildTargetGroup group, string symbol)
    {
        var scriptingDefineSymbolsForGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        var strings = scriptingDefineSymbolsForGroup.Split(';').ToList();

        return strings.Contains(symbol);
    }

    private static void AddBuildSymbol(BuildTargetGroup group, string symbol)
    {
        if (HaveBuildSymbol(group, symbol))
            return;
        var scriptingDefineSymbolsForGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        var strings = scriptingDefineSymbolsForGroup.Split(';').ToList();
        strings.Add(symbol);
        var str = "";
        foreach (var s in strings)
        {
            str += s + ";";
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, str);
    }

    private static void RemoveBuildSymbol(BuildTargetGroup group, string symbol)
    {
        if (!HaveBuildSymbol(group, symbol))
            return;
        var scriptingDefineSymbolsForGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        var strings = scriptingDefineSymbolsForGroup.Split(';').ToList();
        strings.Remove(symbol);
        var str = "";
        foreach (var s in strings)
        {
            str += s + ";";
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, str);
    }

    private static void HandleScriptingSymbol(BuildTargetGroup buildTargetGroup, bool enable, string key)
    {
        var scriptingDefineSymbolsForGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        var strings = scriptingDefineSymbolsForGroup.Split(';').ToList();

        if (enable)
        {
            strings.Add(key);
        }
        else
        {
            strings.Remove(key);
        }


        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", strings.Distinct()));
    }
}


public static class EditorExtensions
{
    public static void DrawChildrenDefault(this SerializedProperty property,
        params string[] exceptChildren)
    {
        var exceptList = exceptChildren?.ToList() ?? new List<string>();

        property = property.Copy();

        var parentDepth = property.depth;
        if (property.NextVisible(true) && parentDepth < property.depth)
        {
            do
            {
                if (exceptList.Contains(property.name))
                    continue;
                EditorGUILayout.PropertyField(property, true);
            } while (property.NextVisible(false) && parentDepth < property.depth);
        }
    }
}