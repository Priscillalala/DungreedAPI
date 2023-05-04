using System;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DungreedAPI
{
    public static class Effects
    {
        internal static StringBuilder stringBuilder => _stringBuilder ??= new StringBuilder();
        internal static StringBuilder _stringBuilder;

        /// <returns>A formatted string.</returns>
        internal static string Effect(string name, object value)
        {
            string result = stringBuilder.Append(name).Append('/').Append(value).ToString();
            stringBuilder.Clear();
            return result;
        }
        /// <inheritdoc cref="Effect(string, object)"/>
        internal static string Effect(string name, params object[] values)
        {
            stringBuilder.Append(name);
            foreach (object value in values)
            {
                stringBuilder.Append('/').Append(value);
            }
            string result = stringBuilder.ToString();
            stringBuilder.Clear();
            return result;
        }
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Generate a random effect.
        /// </summary>
        public static string RANDOM_EFFECT() => "RANDOM_EFFECT";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Generate a random Daisy Ring tier effect.
        /// </summary>
        public static string RANDOM_EFFECT_DAISY() => "RANDOM_EFFECT_DAISY";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Generate a random Gerbera Ring tier effect.
        /// </summary>
        public static string RANDOM_EFFECT_GERBERA() => "RANDOM_EFFECT_GERBERA";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase damage dealt.
        /// </summary>
        public static string POWER(int value) => Effect("POWER", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase damage dealt by weapons.
        /// </summary>
        public static string POWER_WEAPONSWING(int value) => Effect("POWER_WEAPONSWING", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Gain additional flat damage applied after enemy defenses.
        /// </summary>
        public static string TRUE_DAMAGE(int value) => Effect("TRUE_DAMAGE", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Modify miniumum and maximum weapon damage.
        /// </summary>
        public static string DAMAGE(float minValue, float maxValue) => Effect("DAMAGE", minValue, maxValue);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Modify miniumum weapon damage.
        /// </summary>
        public static string DAMAGE_MIN(float value) => Effect("DAMAGE_MIN", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Modify maximum weapon damage.
        /// </summary>
        public static string DAMAGE_MAX(float value) => Effect("DAMAGE_MAX", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Reduce all damage recieved.
        /// </summary>
        public static string DEFENSE(float value) => Effect("DEFENSE", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Reduce all damage recieved by a flat value.
        /// </summary>
        public static string TOUGHNESS(int value) => Effect("TOUGHNESS", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Reduce damage recieved from melee attacks.
        /// </summary>
        public static string MELEE_RESIST(float value) => Effect("MELEE_RESIST", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Reduce damage recieved from bullet attacks.
        /// </summary>
        public static string BULLET_RESIST(float value) => Effect("BULLET_RESIST", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase health by a flat value.
        /// </summary>
        public static string HP(int value) => Effect("HP", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase health by a percentage.
        /// </summary>
        public static string HP_PERCENT(float value) => Effect("HP_PERCENT", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Gain bonus health which regenerates between rooms.
        /// </summary>
        public static string PROTECTIONSHIELD(int value) => Effect("PROTECTIONSHIELD", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Add a chance to evade damage.
        /// </summary>
        public static string EVASION(float value) => Effect("EVASION", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Add a chance to block damage.
        /// </summary>
        public static string BLOCK(float value) => Effect("BLOCK", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Add a chance to deal bonus critical damage.
        /// </summary>
        public static string CRITICAL(float value) => Effect("CRITICAL", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase critical damage.
        /// </summary>
        public static string CRITICAL_DAMAGE_RATE(float value) => Effect("CRITICAL_DAMAGE_RATE", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase health regenerated every second.
        /// </summary>
        public static string REGENERATION_HP(int value) => Effect("REGENERATION_HP", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase health regenerated every second while outside of battle.
        /// </summary>
        public static string REGENERATION_HP_NOBATTLE(int value) => Effect("REGENERATION_HP_NOBATTLE", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase gold attraction range.
        /// </summary>
        public static string ATTRACT_GOLD(float value) => Effect("ATTRACT_GOLD", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase the amount of gold dropped by a percentage.
        /// </summary>
        public static string GOLD_DROP(int value) => Effect("GOLD_DROP", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase damage per 2000 gold held by a percentage.
        /// </summary>
        public static string GOLD_DAMAGE(int value) => Effect("GOLD_DAMAGE", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase attack damage by a percentage.
        /// </summary>
        public static string FINALATK_PERCENT(int value) => Effect("FINALATK_PERCENT", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase defense by a percentage.
        /// </summary>
        public static string FINALDEF_PERCENT(int value) => Effect("FINALDEF_PERCENT", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase movement speed.
        /// </summary>
        public static string MOVE_SPEED(float value) => Effect("MOVE_SPEED", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase attack speed.
        /// </summary>
        public static string ATTACK_SPEED(float value) => Effect("ATTACK_SPEED", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase reload speed.
        /// </summary>
        public static string RELOAD_SPEED(float value) => Effect("RELOAD_SPEED", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Gain access to an advanced reload.
        /// </summary>
        public static string RAPID_RELOAD() => "RAPID_RELOAD";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Restore ammo on dash.
        /// </summary>
        public static string ADD_SHOT_ON_DASH(int value) => Effect("ADD_SHOT_ON_DASH", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Display a positive string.
        /// </summary>
        public static string STRING(string value) => Effect("STRING", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Display a string.
        /// </summary>
        public static string STRING(string value, StatusModule.EffectType effectType) => Effect("STRING", value, (int)effectType);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase maximum dash charges.
        /// </summary>
        public static string DASH(int value) => Effect("DASH", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Restores dash charges on kill.
        /// </summary>
        public static string ADD_DASH_ON_KILL(int value) => Effect("ADD_DASH_ON_KILL", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase dash recharge speed by a percentage.
        /// </summary>
        public static string DASH_RESTORE_SPEED(int value) => Effect("DASH_RESTORE_SPEED", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase dash attack damage by a percentage.
        /// </summary>
        public static string DASH_ATTACK_DAMAGE(int value) => Effect("DASH_ATTACK_DAMAGE", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Gain invincibility while dashing.
        /// </summary>
        public static string INVINCIBLE_ON_DASH() => "INVINCIBLE_ON_DASH";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase dash distance.
        /// </summary>
        public static string DASH_DISTANCE(float value) => Effect("DASH_DISTANCE", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Convert dashes to tackles.
        /// </summary>
        public static string DASH_TACKLE() => "DASH_TACKLE";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Inflict the burn status on hit.
        /// </summary>
        public static string BURN() => "BURN";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Inflict the poison status on hit.
        /// </summary>
        public static string POISON() => "POISON";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Inflict the stun status on hit.
        /// </summary>
        public static string STUN() => "STUN";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Inflict the shock status on hit.
        /// </summary>
        public static string SHOCK() => "SHOCK";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Inflict the chill status on hit.
        /// </summary>
        public static string CHILL() => "CHILL";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase the damage dealt by the burn status.
        /// </summary>
        public static string BURN_ADDITIONAL_DAMAGE(int value) => Effect("BURN_ADDITIONAL_DAMAGE", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase the amount of times burn status can be simultaneously applied to victims.
        /// </summary>
        public static string BURN_OVERLAP(int value) => Effect("BURN_OVERLAP", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Reduce the time between burn status damage ticks by a percentage.
        /// </summary>
        public static string BURN_SPEED(int value) => Effect("BURN_SPEED", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase the duration of the burn status by a percentage.
        /// </summary>
        public static string BURN_DURATION(int value) => Effect("BURN_DURATION", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase the damage dealt by the poison status.
        /// </summary>
        public static string POISON_ADDITIONAL_DAMAGE(int value) => Effect("POISON_ADDITIONAL_DAMAGE", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase the amount of times poison status can be simultaneously applied to victims.
        /// </summary>
        public static string POISON_OVERLAP(int value) => Effect("POISON_OVERLAP", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Reduce the time between poison status damage ticks by a percentage.
        /// </summary>
        public static string POISON_SPEED(int value) => Effect("POISON_SPEED", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase the duration of the burn status by a percentage.
        /// </summary>
        public static string POISON_DURATION(int value) => Effect("POISON_DURATION", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase the speed reduction of the chill status by a percentage.
        /// </summary>
        public static string CHILL_DECREASE_SPEED(int value) => Effect("CHILL_DECREASE_SPEED", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase the duration of the chill status by a percentage.
        /// </summary>
        public static string CHILL_DURATION(int value) => Effect("CHILL_DURATION", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase the duration of the shock status by a percentage.
        /// </summary>
        public static string SHOCK_DURATION(int value) => Effect("SHOCK_DURATION", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase the duration of the stun status by a percentage.
        /// </summary>
        public static string STUN_DURATION(int value) => Effect("STUN_DURATION", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Gain immunity to the burn status.
        /// </summary>
        public static string IMMUNE_BURN() => "IMMUNE_BURN";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Gain immunity to the poison status.
        /// </summary>
        public static string IMMUNE_POISON() => "IMMUNE_POISON";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Gain immunity to the stun status.
        /// </summary>
        public static string IMMUNE_STUN() => "IMMUNE_STUN";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Gain immunity to the shock status.
        /// </summary>
        public static string IMMUNE_SHOCK() => "IMMUNE_SHOCK";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Gain immunity to the chill status.
        /// </summary>
        public static string IMMUNE_CHILL() => "IMMUNE_CHILL";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase jump height.
        /// </summary>
        public static string JUMP_POWER(float value) => Effect("JUMP_POWER", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Reduce the spread of bullet weapons by a percentage.
        /// </summary>
        public static string PRECISION(int value) => Effect("PRECISION", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase damage dealt to <see cref="Creature.CreatureType.NORMAL"/> enemies by a percentage.
        /// </summary>
        public static string NORMAL_ENEMY_BONUS(int value) => Effect("NORMAL_ENEMY_BONUS", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase damage dealt to <see cref="Creature.CreatureType.BOSS"/> enemies by a percentage.
        /// </summary>
        public static string BOSS_ENEMY_BONUS(int value) => Effect("BOSS_ENEMY_BONUS", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Gain explosive bullets on a main-hand or off-hand weapon.
        /// </summary>
        public static string EXPLOSIVE_BULLET(bool offhand = false) => offhand ? Effect("EXPLOSIVE_BULLET", 10, "OFFHAND") : Effect("EXPLOSIVE_BULLET", 10);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Allow a main-hand or off-hand weapon to continously fire when input is held.
        /// </summary>
        public static string AUTOMATIC_FIRE(bool offhand = false) => offhand ? Effect("AUTOMATIC_FIRE", "OFFHAND") : "AUTOMATIC_FIRE";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Restore ammo on a main-hand or off-hand weapon when recieving damage.
        /// </summary>
        public static string PAIN_DETECTION(int value, bool offhand = false) => offhand ? Effect("PAIN_DETECTION", value, "OFFHAND") : Effect("PAIN_DETECTION", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase reload speed of a main-hand weapon by 50% when the clip is empty.
        /// </summary>
        public static string LAST_RELOAD() => "LAST_RELOAD";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase maximum ammo.
        /// </summary>
        public static string ADDITIONAL_SHOTS(int value) => Effect("ADDITIONAL_SHOTS", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase the damage of a main-hand weapon by a percentage for the first bullet in the clip.
        /// </summary>
        public static string FIRST_POWER_BULLET(int value) => Effect("FIRST_POWER_BULLET", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase the damage of a main-hand weapon by a percentage for the last bullet in the clip.
        /// </summary>
        public static string LAST_POWER_BULLET(int value) => Effect("LAST_POWER_BULLET", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase the percentage of enemy defense ignored.
        /// </summary>
        public static string IGNORE_DEFENSE(int value) => Effect("IGNORE_DEFENSE", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase the amount of bullets fired per shot of a main-hand weapon.
        /// </summary>
        public static string SPREAD_SHOT(int value) => Effect("SPREAD_SHOT", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase melee range by a percentage.
        /// </summary>
        public static string FULL_SWING(int value) => Effect("FULL_SWING", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Prevent an item from being un-equipped.
        /// </summary>
        public static string CANT_UNEQUIP() => "CANT_UNEQUIP";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Gain infinite ammo.
        /// </summary>
        public static string INFINITY_SHOT() => "INFINITY_SHOT";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Reduce the charge speed of #Charged weapons by a percentage.
        /// </summary>
        public static string FAST_CHARGE(int value) => Effect("FAST_CHARGE", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Reduce the cooldowns of skills by a percentage.
        /// </summary>
        public static string FAST_SKILL_COOLDOWN(int value) => Effect("FAST_SKILL_COOLDOWN", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase damage by a percentage of the victim's health when the victim has more than 50% health.
        /// </summary>
        public static string GIANT_SLAYER(float value) => Effect("GIANT_SLAYER", value);
    }
}
