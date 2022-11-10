using System;
using UnityEngine;

namespace StormWarfare.Models
{
    public class Enums
    {
        public enum CardType
        {
            Unit = 0,
            Event = 1,
            Hero = 2,
            Weapon = 3,
            Commander = 4,
        }
        public enum Faction
        {
            US = 0,
            DE = 1,
        }

        public enum ClassType
        {
            Infantry = 0,
            Tank = 1,
            Artillery = 2,
            AirCraft = 3,
            Navy = 4,
            Support = 5,
        }

        public enum SubClass
        {
            LightInfantry = 0,
            HeavyInfantry = 1,
            Paratroopers = 2,
            SpecialOperations = 3,
            MechanizedInfantry = 4,

            LightTank = 10,
            MediumTank = 11,
            HeavyTank = 12,
            TankDestroyer = 13,

            AntiTankUnit = 20,
            AntiAircraft = 21,
            AntiPersonal = 22,
            HeavyArtillery = 23,

            LightFighter = 30,
            MediumFighter = 31,
            MediumBomber = 32,
            HeavyBomber = 33,
            DiveBomber = 34,
            StrategicBomber = 35,

            Submarine = 40,
            Destroyer = 41,
            AircraftCarrier = 42,
            BattleShip = 43,
            LightCrusier = 44,
            HeavyCrusier = 45,
            DestroyerCrusier = 46,
            LandingCraft = 47,

            MedicsInfantrySupport = 50,
            ArmourSupport = 51,
            ArtillerySupport = 52,
            AircraftSupport = 53,
            NavySupport = 54,
        }

        public enum Abilities
        {
            Ambush = 0,
            Storming = 1,
            Guardian = 2,
            DeathToll = 3,
            FinalStand = 4,
            SmokeGrenader = 5,
            Fortress = 6,
            Thunder = 7,
        }

        public enum SpecialEffectTypes
        {
            DealDamage = 0,
            GiveDefensePoint = 1,
            GiveAttackPoint = 2,
            GainCommandingPoint = 3,
            DrawCard = 4,
            Retreat = 5
        }

        public enum SpecialEffectTarget
        {
            Unit = 0,
            RandomUnit = 1,
            Board = 2,
            MyCommander = 3,
            OppCommander = 4,
        }

        public enum SpecialEffectCondition
        {
            HaveTypeOf = 0
        }

        public enum SpecialEffectCountFormula
        {
            Default = 0,
            NumberOfUnits = 1
        }

        [Flags]
        public enum CardState : byte
        {
            Neutral = 0,
            AttackBuffed = 1 << 0,
            HealthBuffed = 1 << 1,
            AttackNerfed = 1 << 2,
            HealthNerfed = 1 << 3
        }

        #region StaticFields
        public static readonly Color NeutralColor = new(185.0f / 255.0f, 185.0f / 255.0f, 113.0f / 255.0f, 1.0f);
        public static readonly Color NerfedColor = new(168.0f / 255.0f, 9.0f / 255.0f, 9.0f / 255.0f, 1.0f);
        public static readonly Color BuffedColor = new(103f / 255.0f, 168.0f / 255.0f, 17.0f / 255.0f, 1.0f);
        #endregion
    }
}