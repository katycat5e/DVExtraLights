using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace DVExtraLights
{
    public static class ExtraLightsMain
    {
        public static UnityModManager.ModEntry ModEntry { get; private set; }

        public static bool Load( UnityModManager.ModEntry modEntry )
        {
            ModEntry = modEntry;

            var harmony = new Harmony("cc.foxden.extra_lights");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            PlayerManager.PlayerChanged += OnPlayerChanged;

            return true;
        }

        private static void OnPlayerChanged()
        {
            GameObject glowHolder = new GameObject("PlayerGlowHolder");
            glowHolder.transform.parent = PlayerManager.PlayerCamera.transform;
            glowHolder.transform.localPosition = Vector3.up * 0.3f;

            Light playerGlow = glowHolder.AddComponent<Light>();
            playerGlow.type = LightType.Point;
            playerGlow.intensity = 0.15f;
        }
    }

    [HarmonyPatch(typeof(StationController), "Start")]
    static class StationController_Awake_Patch
    {
        private static string[] lightTransforms =
        {
            "Office_01/Office_1_interior/CeilingLights",
            "Office_02/Office_2_interior/CeilingLights",
            "Office_03/Office_3_interior/CeilingLights",
            "Office_04/Office_4_interior/CeilingLights",
            "Office_05/Office_5_interior/CeilingLights",
            "Office_06/Office_6_interior/CeilingLights",
            "Office_07/Office_7_interior/CeilingLights",
            "MilitaryOffice/MilitaryOffice_interior/CeilingLights"
        };

        private static void Postfix( StationController __instance )
        {
            Transform office_anchor = __instance.transform.parent;
            ExtraLightsMain.ModEntry.Logger.Log("Creating office lights at " + office_anchor.name);

            foreach( string tname in lightTransforms )
            {
                if( office_anchor.Find(tname) is Transform lightParent )
                {
                    for( int i = 0; i < lightParent.childCount; i++ )
                    {
                        Transform childLight = lightParent.GetChild(i);
                        Light newLight = childLight.gameObject.AddComponent<Light>();
                        newLight.intensity = 0.5f;
                    }
                }
            }
        }
    }
}
