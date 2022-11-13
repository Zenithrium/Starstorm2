using Moonstorm.Components;
using R2API;
using RoR2;
using System;

namespace Moonstorm.Starstorm2.Buffs
{
    public sealed class CoffeeBagConsumed : BuffBase
    {
        public override BuffDef BuffDef { get; } = SS2Assets.LoadAsset<BuffDef>("BuffCoffeeBagConsumed");
    }
}
