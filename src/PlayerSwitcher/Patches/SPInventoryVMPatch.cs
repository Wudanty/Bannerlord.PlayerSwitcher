﻿using HarmonyLib;
using HarmonyLib.BUTR.Extensions;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;

namespace PlayerSwitcher
{
    public class SPInventoryVMPatch
    {
        private delegate bool UpdateCurrentCharacterIfPossibleDelegate(SPInventoryVM instance, int characterIndex);
        private static readonly UpdateCurrentCharacterIfPossibleDelegate? UpdateCurrentCharacterIfPossible =
            AccessTools2.GetDelegate<UpdateCurrentCharacterIfPossibleDelegate>(typeof(SPInventoryVM), "UpdateCurrentCharacterIfPossible");

        public static bool Enable(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools2.Method(typeof(SPInventoryVM), "OnCharacterSelected"),
                prefix: new HarmonyMethod(typeof(SPInventoryVMPatch).GetMethod(nameof(OnCharacterSelectedPrefix))));

            return true;
        }

        private static bool OnCharacterSelectedPrefix(SPInventoryVM __instance, InventoryLogic? ____inventoryLogic, SelectorVM<SelectorItemVM> selector)
        {
            if (____inventoryLogic is null)
            {
                return false;
            }

            var selected = selector.SelectedItem?.StringItem ?? Hero.MainHero.Name.ToString();

            for (var i = 0; i < __instance.TroopRoster.Count; i++)
            {
                if (__instance.TroopRoster.GetCharacterAtIndex(i).Name.ToString() == selected)
                {
                    UpdateCurrentCharacterIfPossible?.Invoke(__instance, i);
                    return false;
                }
            }
            return false;
        }
    }
}