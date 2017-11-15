using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionParser {

	public static Skill parse(string action,Character actor){

		Skill selected = null;

		switch(action){
		case Skill.Default:{
				selected = Skill.getDefaultSkill(actor.SkillList);
				break;
			}
		case Skill.LowDmg:{
				selected = Skill.getLowDamageSkill(actor.SkillList,actor.Mana);
				//Debug.Log(selected.Skillname);
				break;
			}
		case Skill.MedDmg:{
				selected = Skill.getMedDamageSkill(actor.SkillList,actor.Mana);
				break;
			}
		case Skill.HighDmg:{
				selected = Skill.getHighDamageSkill(actor.SkillList,actor.Mana);
				break;
			}
		case Skill.BuffAtk:{
				selected = Skill.getStatusSkill(Skill.BuffAtk,actor.SkillList,actor.Mana);
				break;
			}
		case Skill.BuffDef:{
				selected = Skill.getStatusSkill(Skill.BuffDef,actor.SkillList,actor.Mana);
				break;
			}
		case Skill.Blind:{
				selected = Skill.getStatusSkill(Skill.Blind,actor.SkillList,actor.Mana);
				break;
			}
		case Skill.Poison:{
				selected = Skill.getStatusSkill(Skill.Poison,actor.SkillList,actor.Mana);
				break;
			}
		case Skill.Clear:{
				selected = Skill.getClearSkill(actor.SkillList,actor.Mana);
				break;
			}
		case Skill.Heal:{
				selected = Skill.getHealSkill(actor.SkillList,actor.Mana);
				break;
			}
		default:{
				selected = Skill.getSkillByName(actor.SkillList,action,actor.Mana);
				break;
			}
		}

		return selected;
	}

}
