using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;

namespace DVExtraLights
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Player glow intensity", Type = DrawType.Field, Precision = 2, Min = 0)]
        public float PlayerLightIntensity = 0.15f;

        [Draw("Player glow toggle binding")]
        public KeyBinding TogglePlayerLightKey = new KeyBinding()
        {
            keyCode = KeyCode.Z
        };

        public void OnChange()
        {
            ExtraLightsMain.UpdatePlayerGlow();
        }

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
