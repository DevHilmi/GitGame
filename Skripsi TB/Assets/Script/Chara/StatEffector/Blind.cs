using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blind: StatusEffect {

	int missChance;

	public Blind(int duration,int missChance){
		this.effectName = Skill.Blind;
		this.verbalLog = "miss";
		this.duration = duration;
		this.accumulateEffect = false;
		this.missChance = missChance;
		base.effect_value = missChance;
	}

	public override int getEffectValue ()
	{
		return missChance;
	}

	public int MissChance {
		get {
			return this.missChance;
		}
		set {
			missChance = value;
		}
	}

	public override void OnStartEffect (Character _target, int startTurnEffect)
	{
		base.OnStartEffect (_target, startTurnEffect);
	}

	public override void OnNewTurn ()
	{
		base.OnNewTurn ();
	}

	public override void OnApplyEffect ()
	{
		effected.MissChance = missChance;
		base.OnApplyEffect();
	}

	public override void OnRemovedEffect ()
	{
		effected.MissChance = 0;
		base.OnRemovedEffect ();
	}
}
