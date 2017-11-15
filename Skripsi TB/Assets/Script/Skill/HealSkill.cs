using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSkill: Skill{

	float healPercentage;

	public HealSkill(int id,string skillname,int mana,float healPercentage,Character skillmaster){
		skillId = id;
		this.category = Skill.Heal;
		this.skillname = skillname;
		this.mana_needed = mana;
		this.healPercentage = healPercentage;
		this.skillMaster = skillmaster;
	}

	public float HealPercentage {
		get {
			return this.healPercentage;
		}
	}

	public override IEnumerator unleash (Character target, Team teamBelong)
	{
		//check wether team mana is sufficient for executed this skill. Check with possible min_damage constraint.
		if(skillMaster.Mana < mana_needed){
			//Mana is not sufficient -> set result of skill activation to fail then exits from function.
			skillActivationSuccess = false;
			yield return null;
		}
			
		target.Hp += Mathf.RoundToInt(healPercentage/100f * target.MaxHP);
		if(target.Hp > target.MaxHP)
		   target.Hp = target.MaxHP;

		if(GameManager.BattleManager().withVisual)
			target.Controller.updateHPBar();

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
