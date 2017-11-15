using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearSkill : Skill {

	public ClearSkill(int id,string skillname,int mana,Character skillmaster){
		skillId = id;
		this.category = Skill.Clear;
		this.skillname = skillname;
		this.mana_needed = mana;
		this.skillMaster = skillmaster;
	}

	public override IEnumerator unleash (Character target, Team teamBelong)
	{
		//check wether team mana is sufficient for executed this skill. Check with possible min_damage constraint.
		if(skillMaster.Mana < mana_needed){
			//Mana is not sufficient -> set result of skill activation to fail then exits from function.
			skillActivationSuccess = false;
			yield return null;
		}

		//no status ailment at target
		if(target.StatusEffectList.Count <= 0){
			skillActivationSuccess = false;
			//Debug.Log("Target doesn't need clearance!");
			yield return null;
		}

		//clear all target's status ailment here: POISON,BLIND...not Buff
		for(int i=0; i<target.StatusEffectList.Count; i++){
			if(target.StatusEffectList[i] != null && !(target.StatusEffectList[i] is ParamBuff)){
				target.StatusEffectList[i].OnRemovedEffect();
			}
		}

		if(GameManager.BattleManager().withVisual)
			target.Controller.SEffectIndicator.updateStatusEffectIndicator(target.StatusEffectList);

		//request mana decreased to class Team
		skillMaster.Mana -= mana_needed;
		if(skillMaster.Mana <= 0)
			skillMaster.Mana = 0;
		//update mana bar
		base.updateMana();


		//set result of skill activation to success.
		skillActivationSuccess = true;
		yield return null;//exit from function
	}
}
