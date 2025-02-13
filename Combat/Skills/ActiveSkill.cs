﻿using System.Reflection;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using ConsoleGodmist.TextService;
using ConsoleGodmist.Utilities;

namespace ConsoleGodmist.Combat.Skills;

public class ActiveSkill
{
    public string Name => NameAliasHelper.GetName(Alias);
    public string Alias { get; set; }
    public int ResourceCost { get; set; }
    public double ActionCost { get; set; }
    public bool AlwaysHits { get; set; }
    public int Accuracy { get; set; }
    public int Hits { get; set; }
    public List<IActiveSkillEffect> Effects { get; set; }

    public ActiveSkill()
    {
        
    }

    public ActiveSkill(string alias, int resourceCost, double actionCost, bool alwaysHits, int accuracy,
        List<IActiveSkillEffect> effects, int hits = 1)
    {
        Alias = alias;
        ResourceCost = resourceCost;
        ActionCost = actionCost;
        AlwaysHits = alwaysHits;
        Accuracy = accuracy;
        Effects = effects;
        Hits = hits;
    }

    public void Use(BattleUser caster, BattleUser enemy)
    {
        var resourceCost = (int)UtilityMethods.CalculateModValue(ResourceCost, caster.User.PassiveEffects.GetModifiers("ResourceCost"));
        if ((!(caster.User.CurrentResource >= resourceCost) && 
            (caster.User.ResourceType != ResourceType.Fury || 
             !(Math.Abs(caster.User.MaximalResource - caster.User.CurrentResource) < 0.001)) && resourceCost > 0) || 
            caster.CurrentActionPoints < caster.MaxActionPoints.BaseValue * ActionCost) return; // Skill not used, too little Resource or ActionPoints, TODO: Split conditions, add info banner
        var toHit = CheckHit(caster.User, enemy.User);
        ActiveSkillTextService.DisplayUseSkillText(caster.User, enemy.User, this, AlwaysHits ? 1 : toHit.Item2);
        caster.User.UseResource(resourceCost);
        caster.UseActionPoints(caster.MaxActionPoints.BaseValue * ActionCost);
        foreach (var effect in Effects.Where(x => x.Target == SkillTarget.Self)) 
            effect.Execute(caster.User, enemy.User, Alias);
        if (Effects.All(x => x.Target != SkillTarget.Enemy)) return;
        {
            for (var i = 0; i < Hits; i++)
            {
                {
                    if (toHit.Item1 && !AlwaysHits)
                    {
                        ActiveSkillTextService.DisplayMissText(caster.User);
                        continue;
                    }
                    foreach (var effect in Effects.Where(x => x.Target == SkillTarget.Enemy))
                    {
                        effect.Execute(caster.User, enemy.User, Alias);
                        caster.User.PassiveEffects.HandleBattleEvent(new BattleEventData("OnHit", caster, enemy));
                    }
                }
            }
        }
    }

    private (bool, double) CheckHit(Character caster, Character target)
    {
        var accuracy = (caster.Accuracy + Accuracy) / 2;
        var hitChance = accuracy * accuracy / (accuracy + target.Dodge);
        hitChance = Math.Min(UtilityMethods.CalculateModValue(hitChance, 
            caster.PassiveEffects.GetModifiers("HitChanceMod")), 100);
        return (Random.Shared.NextDouble() * 100 < hitChance, hitChance / 100);
    }
}