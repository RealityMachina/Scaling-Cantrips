using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using UnityEngine;

namespace ScalingCantrips
{
    [TypeId("a7f5a54170ec4a0aa88fa1638b16dc61")]
    public class AddCasterStatToDamage : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateDamage>, IRulebookHandler<RuleCalculateDamage>, ISubscriber, IInitiatorRulebookSubscriber
    {
        // Token: 0x0600BD9F RID: 48543 RVA: 0x002F5C48 File Offset: 0x002F3E48
        public void OnEventAboutToTrigger(RuleCalculateDamage evt)
        {
            MechanicsContext context = evt.Reason.Context;
            if ((context?.SourceAbility) == null)
            {
                return;
            }
            if ((context?.MaybeCaster) == null)
            {
                return;
            }
            if ( (!context.SourceAbility.IsSpell && this.SpellsOnly) || context.SourceAbility.Type == AbilityType.Physical)
            {
                return;
            }

            if(!context.SourceAbility.IsCantrip && CantripsOnly)
            {
                return;
            }

            if(statType == 0 || (int)statType > 6)
            {
                return;
            }
            ModifiableValueAttributeStat CasterStat = context.MaybeCaster.Stats.GetAttribute(statType);  //if this is null somehow something has gone very wrong

            if(CasterStat == null)
            {
                return;
            }
            foreach (BaseDamage baseDamage in evt.DamageBundle)
            {
                int bonus = CasterStat.Bonus;
                if (!baseDamage.Sneak)
                {
                    baseDamage.AddModifier(bonus, base.Fact);
                }
            }
        }

        // Token: 0x0600BDA0 RID: 48544 RVA: 0x00003AD6 File Offset: 0x00001CD6
        public void OnEventDidTrigger(RuleCalculateDamage evt)
        {
        }

        
        public StatType statType;

        public bool SpellsOnly = false;

        // Token: 0x04007BFF RID: 31743
        public bool UseContextBonus;

        public bool CantripsOnly = true;

        // Token: 0x04007C00 RID: 31744
        [ShowIf("UseContextBonus")]
        public ContextValue Value;
    }
}
