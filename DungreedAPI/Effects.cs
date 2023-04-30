﻿using System;
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
        /// Generate a random effect for each Daisy Ring.
        /// </summary>
        public static string RANDOM_EFFECT_DAISY() => "RANDOM_EFFECT_DAISY";
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Generate a random effect for each Gerbera Ring.
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
        /// Flat additional damage applied after defenses.
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
        /// Bonus health which regenerates after clearing a room.
        /// </summary>
        public static string PROTECTIONSHIELD(int value) => Effect("PROTECTIONSHIELD", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Chance to evade damage.
        /// </summary>
        public static string EVASION(float value) => Effect("EVASION", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Chance to block damage.
        /// </summary>
        public static string BLOCK(float value) => Effect("BLOCK", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Chance to deal bonus critical damage.
        /// </summary>
        public static string CRITICAL(float value) => Effect("CRITICAL", value);
        /// <inheritdoc cref="Effect(string, object)"/>
        /// <summary>
        /// Increase critical damage.
        /// </summary>
        public static string CRITICAL_DAMAGE_RATE(float value) => Effect("CRITICAL_DAMAGE_RATE", value);
        public static string REGENERATION_HP(int value) => Effect("REGENERATION_HP", value);
        public static string REGENERATION_HP_NOBATTLE(int value) => Effect("REGENERATION_HP_NOBATTLE", value);
        public static string ATTRACT_GOLD(float value) => Effect("ATTRACT_GOLD", value);
        public static string GOLD_DROP(int value) => Effect("GOLD_DROP", value);
        public static string GOLD_DAMAGE(int value) => Effect("GOLD_DAMAGE", value);
        public static string FINALATK_PERCENT(int value) => Effect("FINALATK_PERCENT", value);
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
        public static string RAPID_RELOAD() => "RAPID_RELOAD";
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
        public static string DASH(int value) => Effect("DASH", value);
        public static string ADD_DASH_ON_KILL(int value) => Effect("ADD_DASH_ON_KILL", value);
        public static string DASH_RESTORE_SPEED(int value) => Effect("DASH_RESTORE_SPEED", value);
        public static string DASH_ATTACK_DAMAGE(int value) => Effect("DASH_ATTACK_DAMAGE", value);
        public static string INVINCIBLE_ON_DASH() => "INVINCIBLE_ON_DASH";
        public static string DASH_DISTANCE(float value) => Effect("DASH_DISTANCE", value);
        public static string DASH_TACKLE() => "DASH_TACKLE";
        public static string BURN() => "BURN";
        public static string POISON() => "POISON";
        public static string STUN() => "STUN";
        public static string SHOCK() => "SHOCK";
        public static string CHILL() => "CHILL";
        public static string BURN_ADDITIONAL_DAMAGE(int value) => Effect("BURN_ADDITIONAL_DAMAGE", value);
        public static string BURN_OVERLAP(int value) => Effect("BURN_OVERLAP", value);
        public static string BURN_SPEED(int value) => Effect("BURN_SPEED", value);
        public static string BURN_DURATION(int value) => Effect("BURN_DURATION", value);
        public static string POISON_ADDITIONAL_DAMAGE(int value) => Effect("POISON_ADDITIONAL_DAMAGE", value);
        public static string POISON_OVERLAP(int value) => Effect("POISON_OVERLAP", value);
        public static string POISON_SPEED(int value) => Effect("POISON_SPEED", value);
        public static string POISON_DURATION(int value) => Effect("POISON_DURATION", value);
        public static string CHILL_DECREASE_SPEED(int value) => Effect("CHILL_DECREASE_SPEED", value);
        public static string CHILL_DURATION(int value) => Effect("CHILL_DURATION", value);
        public static string SHOCK_DURATION(int value) => Effect("SHOCK_DURATION", value);
        public static string STUN_DURATION(int value) => Effect("STUN_DURATION", value);
        public static string IMMUNE_BURN() => "IMMUNE_BURN";
        public static string IMMUNE_POISON() => "IMMUNE_POISON";
        public static string IMMUNE_STUN() => "IMMUNE_STUN";
        public static string IMMUNE_SHOCK() => "IMMUNE_SHOCK";
        public static string IMMUNE_CHILL() => "IMMUNE_CHILL";
        public static string JUMP_POWER(float value) => Effect("JUMP_POWER", value);
        public static string PRECISION(int value) => Effect("PRECISION", value);
        public static string NORMAL_ENEMY_BONUS(int value) => Effect("NORMAL_ENEMY_BONUS", value);
        public static string BOSS_ENEMY_BONUS(int value) => Effect("BOSS_ENEMY_BONUS", value);
        public static string EXPLOSIVE_BULLET(int value) => Effect("EXPLOSIVE_BULLET", value);
        public static string AUTOMATIC_FIRE() => "AUTOMATIC_FIRE";
        public static string PAIN_DETECTION(int value) => Effect("PAIN_DETECTION", value);
        public static string LAST_RELOAD() => "LAST_RELOAD";
        public static string ADDITIONAL_SHOTS(int value) => Effect("ADDITIONAL_SHOTS", value);
        public static string FIRST_POWER_BULLET(int value) => Effect("FIRST_POWER_BULLET", value);
        public static string LAST_POWER_BULLET(int value) => Effect("LAST_POWER_BULLET", value);
        public static string IGNORE_DEFENSE(int value) => Effect("IGNORE_DEFENSE", value);
        public static string SPREAD_SHOT(int value) => Effect("SPREAD_SHOT", value);
        public static string FULL_SWING(int value) => Effect("FULL_SWING", value);
        public static string CANT_UNEQUIP() => "CANT_UNEQUIP";
        public static string INFINITY_SHOT() => "INFINITY_SHOT";
        public static string FAST_CHARGE(int value) => Effect("FAST_CHARGE", value);
        public static string FAST_SKILL_COOLDOWN(int value) => Effect("FAST_SKILL_COOLDOWN", value);
        public static string GIANT_SLAYER(float value) => Effect("GIANT_SLAYER", value);
    }
}
