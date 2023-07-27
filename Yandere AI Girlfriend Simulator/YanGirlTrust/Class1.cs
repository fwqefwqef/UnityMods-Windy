using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;


namespace YanGirlTrust
{
    [BepInPlugin(GUID, "YanGirl Patch", version)]
    [BepInProcess("Yandere AI Girlfriend Simulator.exe")]
    public class RareSkillsPlugin : BaseUnityPlugin
    {
        public const string GUID = "org.windy.yangirl";
        public const string version = "1.0.0";
        private static readonly Harmony harmony = new Harmony(GUID);

        void Awake()
        {
            harmony.PatchAll();
        }
        void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchSelf();
        }

        // Refreshes after use
        [HarmonyPatch(typeof(GameManager), "Awake")]

        class SkillUsePatch
        {
            [HarmonyPostfix]
            static void Postfix(GameManager __instance)
            {
                AccessTools.FieldRef<GameManager, float> trustLevel = AccessTools.FieldRefAccess<float>(typeof(GameManager), "trustLevel");
                trustLevel(__instance) = 9999f;

                AccessTools.FieldRef<GameManager, string> trustLevelIndicator = AccessTools.FieldRefAccess<string>(typeof(GameManager), "trustLevelIndicator");
                trustLevelIndicator(__instance) = "Fully Trust";
                Debug.Log("trust max");

                MethodInfo methodInfo = typeof(GameManager).GetMethod("OpenExitDoor", BindingFlags.NonPublic | BindingFlags.Instance);
                var parameters = new object[] {};
                methodInfo.Invoke(__instance, parameters);
                Debug.Log("Door opened");
                //BindingFlags.Static for static method
                //__result = (IntVec3)methodInfo.Invoke(null, parameters);
            }
        }

    }
}
