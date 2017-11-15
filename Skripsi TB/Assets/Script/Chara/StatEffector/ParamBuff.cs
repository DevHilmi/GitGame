using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamBuff : StatusEffect {

	public static int def_stack_bonus = 0;
	public static int atk_stack_bonus = 0;
	int percentageBonus;

	public ParamBuff(string buffname,string verbal,int duration,bool is_accumulate,int percentageBonus){
		this.effectName = buffname;
		this.verbalLog = verbal;
		this.duration = duration;
		this.accumulateEffect = is_accumulate;
		this.percentageBonus = percentageBonus;
		base.effect_value = percentageBonus;
	}

	public override int getEffectValue ()
	{
		return percentageBonus;
	}

	public int PercentageBonus {
		get {
			return this.percentageBonus;
		}
		set {
			percentageBonus = value;
		}
	}

	public override void OnStartEffect (Character _target, int startTurnEffect)
	{
		base.OnStartEffect (_target, startTurnEffect);
	}

	public override void OnNewTurn ()
	{
		//reset at new turn effect
		effected.Strength = effected.MaxStrength;
		effected.Def = effected.MaxDef;
		atk_stack_bonus = def_stack_bonus = 0;
	}

	public override void OnApplyEffect ()
	{
		switch(effectName){
		case Skill.BuffAtk:{
				//apply new
				atk_stack_bonus += (Mathf.RoundToInt((effected.MaxStrength * percentageBonus)/100));
				effected.Strength += atk_stack_bonus;
				effected.OnAtkBuff = true;
				break;
			}
		case Skill.BuffDef:{
				//apply new
				def_stack_bonus += percentageBonus;
				effected.Def += def_stack_bonus;
				effected.OnDefBuff = true;
				break;
			}
		}
		base.OnApplyEffect();
	}



	public override void OnRemovedEffect ()
	{
		OnNewTurn();
		switch(effectName){
		case Skill.BuffAtk:{
				effected.OnAtkBuff = false;
				break;
			}
		case Skill.BuffDef:{
				effected.OnDefBuff = false;
				break;
			}
		}
		base.OnRemovedEffect();
	}
}
