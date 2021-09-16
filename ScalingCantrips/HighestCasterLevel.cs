using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Mechanics.Properties;
using UnityEngine;

namespace ScalingCantrips
{
    [TypeId("6d0160c8a948402a907e095ed1741702")]
    public class HighestCasterLevel : PropertyValueGetter
    {
    
        public override int GetBaseValue(UnitEntityData unit)
        {
            // this won't be perfect but it will at least be a fix
            // we will go through every non-mythic spellbook and just send back the highest out
            int HighestCasterLevel = 0;
            foreach (Spellbook spellbook in unit.Descriptor.Spellbooks)
            {
                        if (HighestCasterLevel < spellbook.CasterLevel)
                        {
                            HighestCasterLevel = spellbook.CasterLevel;
                        }
                    
                


            }

            return HighestCasterLevel;
        }
        
       
    }
}
