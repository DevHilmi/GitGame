using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivineSkill : Skill,IStatusEffect {

	string effectType;
	int effectValue,duration;

	public DivineSkill(int id,string category,string effectType,string skillname,int min_damage,int max_damage,int ef_val,int duration,int mana,Character skillmaster){
		skillId = id;
		this.category = category;
		this.effectType = effectType;
		this.skillname = skillname;
		this.min_damage = min_damage;
		this.max_damage = max_damage;
		this.effectValue = ef_val;
		this.skillMaster = skillmaster;
		this.duration = duration;
		this.mana_needed = mana;
	}

	public string getEffectType ()
	{
		return effectType;
	}

	public void addStatusEffect (Character target,string effectType)
	{
		StatusEffect sEffect = null;
		sEffect = StatusEffect.createStatusEffect(effectType,this.duration,this.effectValue,skillMaster);
		target.registerNewStatusEffect(sEffect);
		sEffect.OnStartEffect(target,GameManager.BattleManager().turn_number);
		sEffect.OnApplyEffect();
	}

	public void addStatusEffect (Character target,string effectType,int effectStartTurn)
	{
		StatusEffect sEffect = null;
		sEffect = StatusEffect.createStatusEffect(effectType,this.duration,this.effectValue,skillMaster);
		target.registerNewStatusEffect(sEffect);
		sEffect.OnStartEffect(target,effectStartTurn);
		sEffect.OnApplyEffect();
	}

	public override IEnumerator unleash (Character target, Team teamBelong)
	{
		if(target.Controller.isDead){
			skillActivationSuccess = false;
			yield return null;
		}
		//check wether team mana is sufficient for executed this skill. Check with possible min_damage constraint.
		if(skillMaster.Mana < mana_needed){
			//Mana is not sufficient -> set result of skill activation to fail then exits from function.
			skillActivationSuccess = false;
			yield return null;
		}

		int missModifier = 1;
		if(skillMaster.MissChance > Random.Range(0,100)){
			missModifier = 0;
			GameManager.ActionLog.ActionMiss = true;
		}
	
		//Mana sufficient -> do procedure below
		//calculate damage based on min_damage and max_damage of this skill and put effect on target.
		damageDeal = Random.Range(min_damage,max_damage+1);
		int strdmg = Mathf.RoundToInt(0.1f * skillMaster.Strength);
		target.takeDamage((strdmg + damageDeal) * missModifier,skillMaster);

		//add status aliment here
		if(!skillMaster.isSimulationChara)
			addStatusEffect(target,effectType);

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
