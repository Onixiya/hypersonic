using MelonLoader;
using Harmony;
using NKHook6.Api;
using NKHook6;
using Assets.Scripts.Unity.UI_New.InGame;
using NKHook6.Api.Events;
using Assets.Scripts.Unity.UI_New.Popups;
using Assets.Scripts.Unity.Bridge;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Simulation.Objects;
using TMPro;
using System;

namespace hypersonic
{
    public class Main : MelonMod
    {
        static float rate = 1;
        static float timer = 0;
        static System.Random random = new System.Random();
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            EventRegistry.instance.listen(typeof(Main));
            Logger.Log("Hypersonic loaded");
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            bool inAGame = InGame.instance != null && InGame.instance.bridge != null;
            if (inAGame)
            {
                timer += UnityEngine.Time.deltaTime;
                if (rate != 1 && timer > 1)
                {
                    foreach (TowerToSimulation towerToSimulation in InGame.instance.bridge.GetAllTowers())
                    {
                        StartOfRoundRateBuffModel rateBuffSORModel = new StartOfRoundRateBuffModel("69", 1 / rate, 30);
                        BehaviorMutator rateBuffModel = new StartOfRoundRateBuffModel.RateMutator(rateBuffSORModel);
                        towerToSimulation.tower.AddMutator(rateBuffModel, 600, true, true, false, true, false, false);
                    }
                    timer = 0;
                }
            }
        }
        static Il2CppSystem.Action<float> rateDel = (Il2CppSystem.Action<float>)delegate (float s)
        {
            rate = s;
        };
        [EventAttribute("KeyPressEvent")]
        public static void onEvent(KeyEvent e)
        {
            string key = e.key + "";
            if (key == "F9")
            {
                Il2CppSystem.Action<string> mod = (Il2CppSystem.Action<string>)delegate (string s)
                {
                    rate = float.Parse(s);
                };
                string ratestring = Convert.ToString(rate);
                PopupScreen.instance.ShowSetNamePopup("rate", "multiply fire rate by", mod, "999");
                PopupScreen.instance.GetFirstActivePopup().GetComponentInChildren<TMP_InputField>().characterValidation = TMP_InputField.CharacterValidation.None;
                Logger.Log("Fire rate is now set to:", ratestring);
            }
        }
        [EventAttribute("StartMatchEvent")]
        public static void onEvent(NKHook6.Api.Events._InGame.InGameEvents.StartMatchEvent e)
        {
            rate = 100;
            string ratestring = Convert.ToString(rate);
            Logger.Log("Fire rate is now set to:", ratestring);
        }
    }
}