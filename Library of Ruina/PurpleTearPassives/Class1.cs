using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleTearPassives
{
    using HyperCard;
    using LOR_BattleUnit_UI;
    using LOR_DiceSystem;
    using MsLocal;
    using static FogEllipsoid;

    //public class PassiveAbility_RainlessWarzone_BurnCarverPlus : PassiveAbilityBase
    //{
    //    public override string debugDesc => "Max HP -7%.";

    //    public override int GetMaxHpBonus()
    //    {
    //        float hp1 = (float)owner.UnitData.unitData.MaxHp;
    //        return (int)(-0.07 * hp1);
    //    }
    //} 

    public class PassiveAbility_SlashingStance : PassiveAbilityBase
    {
        public override string debugDesc => "SlashingStance";
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail == BehaviourDetail.Slash)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 2,
                    dmgRate = 50
                });
            }
        }
    }

    public class PassiveAbility_PiercingStance : PassiveAbilityBase
    {
        public override string debugDesc => "PiercingStance";
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail == BehaviourDetail.Penetrate)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 2
                });
            }
        }

        public override int GetMultiplierOnGiveKeywordBufByCard(BattleUnitBuf cardBuf, BattleUnitModel target)
        {
            if (cardBuf.positiveType == BufPositiveType.Negative)
            {
                return 2;
            }
            return 1;
        }
    }

    public class PassiveAbility_BluntingStance : PassiveAbilityBase
    {
        public override string debugDesc => "BluntingStance";
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail == BehaviourDetail.Hit)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 2,
                    breakRate = 50
                });
            }
        }
    }

    public class PassiveAbility_GuardingStance : PassiveAbilityBase
    {
        public override string debugDesc => "GuardingStance";
        public override void OnWaveStart()
        {
            this.owner.bufListDetail.AddBuf(new StatusImmuneBuff());
        }

        // Token: 0x060027D3 RID: 10195 RVA: 0x0010851F File Offset: 0x0010671F
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (base.IsDefenseDice(behavior.Detail))
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 2
                });
            }
        }
    }

    public class PassiveAbility_SlashDicePower : PassiveAbilityBase
    {
        public override string debugDesc => "SlashDicePower";
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail == BehaviourDetail.Slash)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 2
                });
            }
        }
    }

    public class PassiveAbility_PierceDicePower : PassiveAbilityBase
    {
        public override string debugDesc => "PierceDicePower";
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail == BehaviourDetail.Penetrate)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 2
                });
            }
        }
    }

    public class PassiveAbility_BluntDicePower : PassiveAbilityBase
    {
        public override string debugDesc => "BluntDicePower";
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail == BehaviourDetail.Hit)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 2
                });
            }
        }
    }

    public class PassiveAbility_DefensiveDicePower : PassiveAbilityBase
    {
        public override string debugDesc => "DefensiveDicePower";
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (base.IsDefenseDice(behavior.Detail))
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 2
                });
            }
        }
    }

    public class PassiveAbility_SlashDamageUp : PassiveAbilityBase
    {
        public override string debugDesc => "SlashDamageUp";
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail == BehaviourDetail.Slash)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    dmgRate = 50
                });
            }
        }
    }
    public class PassiveAbility_MultiplyDebuff : PassiveAbilityBase
    {
        public override string debugDesc => "MultiplyDebuff";
        public override int GetMultiplierOnGiveKeywordBufByCard(BattleUnitBuf cardBuf, BattleUnitModel target)
        {
            if (cardBuf.positiveType == BufPositiveType.Negative)
            {
                return 2;
            }
            return 1;
        }
    }

    public class PassiveAbility_BluntStaggerDamageUp : PassiveAbilityBase
    {
        public override string debugDesc => "BluntStaggerDamageUp";
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail == BehaviourDetail.Hit)
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    breakRate = 50
                });
            }
        }
    }

    public class PassiveAbility_StatusImmunePassive : PassiveAbilityBase
    {
        public override string debugDesc => "StatusImmunePassive";
        public override void OnWaveStart()
        {
            this.owner.bufListDetail.AddBuf(new StatusImmuneBuff());
        }
    }

    public class StatusImmuneBuff : BattleUnitBuf 
    {
        public override bool IsImmune(BufPositiveType posType)
        {
            bool flag = this._owner.passiveDetail.HasPassive<PassiveAbility_GuardingStance>() || this._owner.passiveDetail.HasPassive<PassiveAbility_StatusImmunePassive>();
            return flag && (posType == BufPositiveType.Negative);
        }
    }

}
