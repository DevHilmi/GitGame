using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSkill : Skill,IStatusEffect {

	private int buffBonus;
	private int duration;

	public BuffSkill(int id,string category,string skillname,int mana,int buffBonus,Character skillmaster,int duration){
		skillId = id;
		this.category = category;
		this.skillname = skillname;
		this.mana_needed = mana;
		this.buffBonus = buffBonus;
		this.skillMaster = skillmaster;
		this.duration = duration;
	}

	public string getEffectType ()
	{
		return category;
	}

	public int BuffBonus {
		get {
			return this.buffBonus;
		}
		set {
			buffBonus = value;
		}
	}
		
	public void addStatusEffect (Character target,string effectType)
	{
		StatusEffect sEffect = null;
		sEffect = StatusEffect.createStatusEffect(effectType,this.duration,buffBonus,skillMaster);
		target.registerNewStatusEffect(sEffect);
		sEffect.OnStartEffect(target,GameManager.BattleManager().turn_number);
		sEffect.OnApplyEffect();
	}

	public void addStatusEffect (Character target,string effectType,int effectStartTurn)
	{
		StatusEffect sEffect = null;
		sEffect = StatusEffect.createStatusEffect(effectType,this.duration,buffBonus,skillMaster);
		target.registerNewStatusEffect(sEffect);
		sEffect.OnStartEffect(target,effectStartTurn);
		sEffect.OnApplyEffect();
	}

	public override IEnumerator unleash (Character target,Team teamBelong)
	{
		//check wether team mana is sufficient for executed this skill. Check with possible min_damage constraint.
		if(skillMaster.Mana < mana_needed){
			//Mana is not sufficient -> set result of skill activation to fail then exits from function.
			skillActivationSuccess = false;
			yield return null;
		}

		//add status effect
		if(!skillMaster.isSimulationChara)
			addStatusEffect(target,category);

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
