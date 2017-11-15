using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect{

	protected int max_duration,duration; //at this game, effeect duration counted by turn number
	public bool isActive;//indicates if this effect is active at target
	protected Character effected;//the character that has this effect
	public string effectName;//name of effect
	public string verbalLog;//verb it's effect uses in battle log
	public bool accumulateEffect;//effect is accumulated if there is more than one this type of effect at effected's statusEffectList 
	protected int effect_value;

	public StatusEffect(){
		
	}

	public virtual int getEffectValue(){
		return effect_value;
	}

	public static StatusEffect createStatusEffect(string _effectname,int duration,int value,Character damager){
		StatusEffect factoryResult = null; 

		switch(_effectname){
		case Skill.BuffAtk:{
				factoryResult = new ParamBuff(_effectname,"give ATK buff",duration,true,value) as StatusEffect;
				break;
			}
		case Skill.BuffDef:{
				factoryResult = new ParamBuff(_effectname,"give DEF buff",duration,true,value) as StatusEffect;
				break;
			}
		case Skill.Blind:{
				factoryResult = new Blind(duration,value) as StatusEffect;
				break;
			}
		case Skill.Poison:{
				factoryResult = new Poison(duration,value,damager) as StatusEffect;
				break;
			}
		}
		return factoryResult;
	}

	public virtual void OnStartEffect(Character _target,int startTurnEffect){
		effected = _target;
		max_duration = startTurnEffect + duration;
		isActive = true;
	}

	public virtual void OnApplyEffect(){
		//first reset all modifier
		//modify modifier
		if (GameManager.BattleManager ().withVisual && !effected.isSimulationChara)
			effected.Controller.SEffectIndicator.updateStatusEffectIndicator (effected.StatusEffectList);
	}

	public virtual void OnRemovedEffect(){
		isActive = false;
		effected.StatusEffectList.Remove(this);
		if(GameManager.BattleManager().withVisual && !effected.isSimulationChara)
			effected.Controller.SEffectIndicator.updateStatusEffectIndicator(effected.StatusEffectList);
	}

	public virtual void OnNewTurn(){
		
	}

	public int Max_duration {
		get {
			return this.max_duration;
		}
		set {
			max_duration = value;
		}
	}

	public int Duration {
		get {
			return this.duration;
		}
		set {
			duration = value;
		}
	}

	public bool IsActive {
		get {
			return this.isActive;
		}
		set {
			isActive = value;
		}
	}

	public Character Effected {
		get {
			return this.effected;
		}
		set {
			effected = value;
		}
	}

	public void checkStatusEffect(int currentGameTurn){//call each change turn event
		//Battle battle = GameManager.BattleManager();

		if(currentGameTurn > max_duration){
			//Debug.Log("Remove effect");
			OnRemovedEffect();
		}
		else{
			OnApplyEffect();
		}
	}
}
