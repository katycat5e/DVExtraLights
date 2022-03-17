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
        public static Settings Settings { get; private set; }

        public static bool Load( UnityModManager.ModEntry modEntry )
        {
            ModEntry = modEntry;
            Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            ModEntry.OnGUI = OnGUI;
            ModEntry.OnSaveGUI = OnSaveGUI;

            var harmony = new Harmony("cc.foxden.extra_lights");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            PlayerManager.PlayerChanged += UpdatePlayerGlow;
            ModEntry.OnUpdate = Update;

            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.Draw(modEntry);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }

        private const string GLOW_HOLDER_NAME = "PlayerGlowHolder";
        public static Light PlayerLight { get; private set; } = null;

        private static bool _playerLightOn;
        public static bool PlayerLightOn
        {
            get => _playerLightOn;
            set
            {
                _playerLightOn = value;
                UpdatePlayerGlowIntensity();
            }
        }

        public static void UpdatePlayerGlow()
        {
            Transform glowHolder = PlayerManager.PlayerCamera.transform.Find(GLOW_HOLDER_NAME);

            if (!glowHolder)
            {
                var newHolderObj = new GameObject(GLOW_HOLDER_NAME);
                glowHolder = newHolderObj.transform;
                glowHolder.parent = PlayerManager.PlayerCamera.transform;
                glowHolder.localPosition = Vector3.up * 0.3f;

                PlayerLight = newHolderObj.AddComponent<Light>();
                PlayerLight.type = LightType.Point;
                PlayerLight.renderMode = LightRenderMode.ForceVertex;
            }
            else
            {
                PlayerLight = glowHolder.GetComponent<Light>();
            }

            UpdatePlayerGlowIntensity();
        }

        private static void UpdatePlayerGlowIntensity()
        {
            PlayerLight.intensity = PlayerLightOn ? Settings.PlayerLightIntensity : 0;
        }

        private static void Update(UnityModManager.ModEntry modEntry, float deltaTime)
        {
            if (Settings.TogglePlayerLightKey.Up())
            {
                PlayerLightOn = !PlayerLightOn;
            }
        }
    }

    [HarmonyPatch(typeof(StationController), "Start")]
    static class StationController_Awake_Patch
    {
        private static readonly string[] lightTransforms =
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
                        newLight.renderMode = LightRenderMode.ForceVertex;
                    }
                }
            }
        }
    }
}
