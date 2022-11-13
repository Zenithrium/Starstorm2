using R2API;
using RoR2;
using RoR2.Items;
using System;
using UnityEngine;

namespace Moonstorm.Starstorm2.Items
{
    //[DisabledContent]
    public sealed class CoffeeBag : ItemBase
    {
        public const string token = "SS2_ITEM_COFFEEBAG_DESC";
        public override ItemDef ItemDef { get; } = SS2Assets.LoadAsset<ItemDef>("CoffeeBag");

        [ConfigurableField(ConfigDesc = "Duration of the buff gained upon using an interactable.")]
        [TokenModifier(token, StatTypes.Default, 0)]
        [TokenModifier("SS2_ITEM_COFFEEBAG_PICKUP", StatTypes.Default, 0)]
        public static float interactBuff = 15;

        [ConfigurableField(ConfigDesc = "Attack speed bonus granted per stack at the start of the stage. (1 = 100%)")]
        [TokenModifier(token, StatTypes.Percentage, 1)]
        public static float atkSpeedBonus = .225f;

        [ConfigurableField(ConfigDesc = "Movement speed bonus granted per stack at the start of the stage. (1 = 100%)")]
        [TokenModifier(token, StatTypes.Percentage, 2)]
        public static float moveSpeedBonus = .21f;

        public sealed class Behavior : BaseItemBodyBehavior, IBodyStatArgModifier //IStatItemBehavior
        {

            [ItemDefAssociation]
            private static ItemDef GetItemDef() => SS2Content.Items.CoffeeBag;

            private void OnEnable()
            {
                On.RoR2.GlobalEventManager.OnInteractionBegin += CoffeeBagBuff;
                //On.RoR2.CharacterBody.OnBuffFinalStackLost += CheckForCoffeeBuff;
                //On.RoR2.EquipmentSlot.OnEquipmentExecuted += RefreshCoffeeBag;
            }

            private void OnDisable()
            {
                On.RoR2.GlobalEventManager.OnInteractionBegin -= CoffeeBagBuff;
                //On.RoR2.CharacterBody.OnBuffFinalStackLost -= CheckForCoffeeBuff;
                //On.RoR2.EquipmentSlot.OnEquipmentExecuted -= RefreshCoffeeBag;
            }

            private void CoffeeBagBuff(On.RoR2.GlobalEventManager.orig_OnInteractionBegin orig, GlobalEventManager self, Interactor interactor, IInteractable interactable, GameObject interactableObject)
            {
                orig(self, interactor, interactable, interactableObject);
                var cb = interactor.GetComponent<CharacterBody>();
                if (cb)
                {
                    if (cb.inventory)
                    {
                        if(SS2Util.CheckIsValidInteractable(interactable, interactableObject))
                        {
                            ApplyCoffeeBagBuff(cb);
                        }
                        //var procFilter = interactableObject.GetComponent<InteractionProcFilter>();
                        ////procFilter.shouldAllowOnInteractionBeginProc
                        //if (procFilter)
                        //{
                        //    if (cb.inventory.GetItemCount(GetItemDef()) > 0 && procFilter.shouldAllowOnInteractionBeginProc)
                        //    {
                        //        ApplyCoffeeBagBuff(cb);
                        //        
                        //    }
                        //}
                        //else
                        //{
                        //    ApplyCoffeeBagBuff(cb);
                        //}
                    }
                }
            }

            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                //int count = body.GetBuffCount(SS2Content.Buffs.BuffCoffeeBag);
                if (body.GetBuffCount(SS2Content.Buffs.BuffCoffeeBag) > 0)
                {
                    args.attackSpeedMultAdd += atkSpeedBonus * stack;
                    args.moveSpeedMultAdd += moveSpeedBonus * stack;

                }
            }

            private void ApplyCoffeeBagBuff(CharacterBody cb)
            {
                int buffCount = cb.GetBuffCount(SS2Content.Buffs.BuffCoffeeBag);
                if (buffCount > 0)
                {
                    for (int i = 0; i < buffCount; i++)
                    {
                        cb.RemoveOldestTimedBuff(SS2Content.Buffs.BuffCoffeeBag.buffIndex);
                    }
                }
                for (int i = 1; i <= interactBuff + buffCount; i++)
                {
                    cb.AddTimedBuffAuthority(SS2Content.Buffs.BuffCoffeeBag.buffIndex, i);
                }
            }

            //bool checkIsValidInteractable(IInteractable interactable, GameObject interactableObject)
            //{
            //    var procFilter = interactableObject.GetComponent<InteractionProcFilter>();
            //    MonoBehaviour interactableAsMonobehavior = (MonoBehaviour)interactable;
            //    if ((bool)procFilter)
            //    {
            //        return procFilter.shouldAllowOnInteractionBeginProc;
            //    }
            //    if ((bool)interactableAsMonobehavior.GetComponent<GenericPickupController>())
            //    {
            //        return false;
            //    }
            //    if ((bool)interactableAsMonobehavior.GetComponent<VehicleSeat>())
            //    {
            //        return false;
            //    }
            //    if ((bool)interactableAsMonobehavior.GetComponent<NetworkUIPromptController>())
            //    {
            //        return false;
            //    }
            //    return true;
            //}
            //
            //int count = 0;
            //bool expired = false;
            //
            //private void Awake()
            //{
            //    base.Awake();
            //
            //    float stacks; // = stageTimer / 2f;
            //    stacks = stageTimer;
            //    int buffCount = body.GetBuffCount(SS2Content.Buffs.BuffCoffeeBag);
            //    stacks += buffCount;
            //    if (buffCount > 0)
            //    {
            //        for (int i = 0; i < buffCount; i++)
            //        {
            //            body.RemoveOldestTimedBuff(SS2Content.Buffs.BuffCoffeeBag.buffIndex);
            //        }
            //    }
            //    if (body.isPlayerControlled)
            //    {
            //        for (int i = 0; i < stacks; i++)
            //        {
            //            body.AddTimedBuffAuthority(SS2Content.Buffs.BuffCoffeeBag.buffIndex, i);
            //        }
            //    }
            //    else
            //    {
            //        body.AddTimedBuffAuthority(SS2Content.Buffs.BuffCoffeeBag.buffIndex, stacks);
            //    }
            //    //count = 1;
            //}
            //
            //public void OnEnable()
            //{
            //    On.RoR2.CharacterBody.OnInventoryChanged += CoffeeBagInvChanged;
            //    On.RoR2.CharacterBody.OnBuffFinalStackLost += CheckForCoffeeBuff;
            //    On.RoR2.EquipmentSlot.OnEquipmentExecuted += RefreshCoffeeBag;
            //    //On.RoR2.CharacterBody.RemoveBuff_BuffDef += CheckForCoffeeConsumed;
            //}
            //
            //private void RefreshCoffeeBag(On.RoR2.EquipmentSlot.orig_OnEquipmentExecuted orig, EquipmentSlot self)
            //{
            //    orig(self);
            //    if (self.characterBody.inventory)
            //    {
            //        if (self.characterBody.inventory.GetItemCount(SS2Content.Items.CoffeeBag) > 0 && self.equipmentIndex == RoR2Content.Equipment.Cleanse.equipmentIndex)
            //        {
            //            int consCount = body.GetBuffCount(SS2Content.Buffs.BuffCoffeeBagConsumed);
            //            if (consCount > 0)
            //            {
            //                int cycles = (int)stageTimer / 2;
            //                for (int i = 0; i < cycles; i++)
            //                {
            //                    body.AddTimedBuffAuthority(SS2Content.Buffs.BuffCoffeeBag.buffIndex, i);
            //                }
            //                for (int i = 0; i < consCount; i++)
            //                {
            //                    body.RemoveBuff(SS2Content.Buffs.BuffCoffeeBagConsumed);
            //                }
            //                for (int i = 0; i < 5; i++)
            //                {
            //                    body.AddBuff(SS2Content.Buffs.BuffCoffeeBagConsumed);
            //                }
            //                //body.RemoveBuff(SS2Content.Buffs.BuffCoffeeBagConsumed);
            //            }
            //            //int cycles = (int)stageTimer / 2;
            //            //for (int i = 0; i < cycles; i++)
            //            //{
            //            //    body.AddTimedBuffAuthority(SS2Content.Buffs.BuffCoffeeBag.buffIndex, i);
            //            //}
            //            //body.RemoveBuff(SS2Content.Buffs.BuffCoffeeBagConsumed);
            //            //if (body.GetBuffCount(SS2Content.Buffs.BuffCoffeeBagConsumed) > 0)
            //            //{
            //            //    body.RemoveBuff(SS2Content.Buffs.BuffCoffeeBagConsumed);
            //            //}
            //        }
            //    }
            //}

            //private void CheckForCoffeeBuff(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
            //{
            //    if (buffDef == SS2Content.Buffs.BuffCoffeeBag)
            //    {
            //        body.AddBuff(SS2Content.Buffs.BuffCoffeeBagConsumed);
            //    }
            //}
            //
            //private void CoffeeBagInvChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
            //{
            //    orig(self);
            //    //SS2Log.Debug("item collected");
            //    //int amnt = self.inventory.GetItemCount();
            //    if (self.hasAuthority && self.inventory)
            //    {
            //        int amount = self.inventory.GetItemCount(SS2Content.Items.CoffeeBag.itemIndex);
            //        //SS2Log.Debug("amount:" + amount + " | " + count);
            //        if (amount > 0)
            //        {
            //            if (amount > count + 1)
            //            {
            //                int buffCount = self.GetBuffCount(SS2Content.Buffs.BuffCoffeeBag.buffIndex);
            //                int cycles = (int)stageTimer / 2;
            //                if (buffCount > 0)
            //                {
            //                    cycles += buffCount;
            //                    for (int i = 0; i < buffCount; i++)
            //                    {
            //                        self.RemoveOldestTimedBuff(SS2Content.Buffs.BuffCoffeeBag.buffIndex);
            //                    }
            //                }
            //                //for (float i = 1; i <= cycles; i++)
            //                //{
            //                //    self.AddTimedBuffAuthority(SS2Content.Buffs.BuffCoffeeBag.buffIndex, i);
            //                //}
            //                if (body.GetBuffCount(SS2Content.Buffs.BuffCoffeeBagConsumed) > 0)
            //                {
            //                    body.RemoveBuff(SS2Content.Buffs.BuffCoffeeBagConsumed);
            //                }
            //
            //                if (body.isPlayerControlled)
            //                {
            //                    for (int i = 0; i < cycles; i++)
            //                    {
            //                        body.AddTimedBuffAuthority(SS2Content.Buffs.BuffCoffeeBag.buffIndex, i);
            //                    }
            //                }
            //                else
            //                {
            //                    body.AddTimedBuffAuthority(SS2Content.Buffs.BuffCoffeeBag.buffIndex, cycles);
            //                }
            //                //count = amount;
            //            }
            //            count = amount;
            //        }
            //        //count = amount;
            //    }
            //}
            //

            //private void ApplyCoffeeBagBuff(CharacterBody cb, bool awake)
            //{
            //    float stacks = stageTimer; // = stageTimer / 2f;
            //    if (!awake)
            //    {
            //        stacks /= 2;
            //    }
            //    //stacks = stageTimer;
            //    int buffCount = cb.GetBuffCount(SS2Content.Buffs.BuffCoffeeBag);
            //    stacks += buffCount;
            //    if (buffCount > 0)
            //    {
            //        for (int i = 0; i < buffCount; i++)
            //        {
            //            cb.RemoveOldestTimedBuff(SS2Content.Buffs.BuffCoffeeBag.buffIndex);
            //        }
            //    }
            //    if (cb.isPlayerControlled)
            //    {
            //        for (int i = 0; i < stacks; i++)
            //        {
            //            cb.AddTimedBuffAuthority(SS2Content.Buffs.BuffCoffeeBag.buffIndex, i);
            //        }
            //    }
            //    else
            //    {
            //        cb.AddTimedBuffAuthority(SS2Content.Buffs.BuffCoffeeBag.buffIndex, stacks);
            //    }
            //}
            //
            //private void OnDestroy()
            //{
            //    if (body.HasBuff(SS2Content.Buffs.BuffCoffeeBag))
            //    {
            //        body.RemoveBuff(SS2Content.Buffs.BuffCoffeeBag);
            //    }
            //}
        }
    }
}
