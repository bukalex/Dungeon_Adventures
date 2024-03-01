using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data
{

    public class ModifierValues
    {
        private static PlayerData playerData;
        public Hashtable Value = new Hashtable()
        {
            {ModifierID.HealthRestore, playerData.health += (float)ModifierID.HealthRestore},
            {ModifierID.HealthRecovery, playerData.healthRestoreRate += (float)ModifierID.HealthRecovery},
            {ModifierID.HealthMax, playerData.maxHealth += (float)ModifierID.HealthMax},
            {ModifierID.ManaRestore, playerData.mana += (float)ModifierID.ManaRestore},
            {ModifierID.ManaRecovery, playerData.manaRestoreRate += (float)ModifierID.ManaRecovery},
            {ModifierID.ManaMax, playerData.maxMana += (float)ModifierID.ManaMax},
            {ModifierID.StaminaRestore, playerData.stamina += (float)ModifierID.StaminaRestore},
            {ModifierID.StaminaRecovery, playerData.staminaRestoreRate += (float)ModifierID.StaminaRecovery},
            {ModifierID.StaminaMax, playerData.maxStamina += (float)ModifierID.StaminaMax},
            {ModifierID.AttackIncrease, playerData.attack += (float)ModifierID.AttackIncrease},
            {ModifierID.AttackDecrease, playerData.attack -= (float)ModifierID.AttackDecrease},
            {ModifierID.AttackMax, playerData.attack += (float)ModifierID.AttackMax},
            {ModifierID.DefenseIncrease, playerData.defense += (float)ModifierID.DefenseIncrease},
            {ModifierID.DefenseDecrease, playerData.defense += (float)ModifierID.DefenseDecrease },
            {ModifierID.DefenseMax, playerData.defense += (float)ModifierID.DefenseMax},
            {ModifierID.SpeedIncrease, playerData.speed += (float)ModifierID.SpeedIncrease},
            {ModifierID.SpeedDecrease, playerData.speed -= (float)ModifierID.SpeedDecrease},
            {ModifierID.SpeedMax, playerData.speed += (float)ModifierID.SpeedMax},


        };
    }

}
