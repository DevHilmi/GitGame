using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison: StatusEffect {

	int poisonDmgPercntge;
	Character damager;

	public Poison(int duration,int poisonPercent,Character damager){
		this.effectName = Skill.Poison;
		this.verbalLog = "poisoned";
		this.duration = duration;
		this.accumulateEffect = true;
		this.poisonDmgPercntge = poisonPercent;
		base.effect_value = poisonPercent;
		this.damager = damager;
	}

	public override int getEffectValue ()
	{
		return poisonDmgPercntge;
	}

	public int PoisonDmgPercntge {
		get {
			return this.poisonDmgPercntge;
		}
		set {
			poisonDmgPercntge = value;
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
		effected.OnPoisoned = true;
		effected.takeStatusDamage(Mathf.RoundToInt((float)(poisonDmgPercntge * effected.MaxHP)/100f),damager);
		//Debug.Log(effected.profession+effected.Controller.CharaTeam.team_id+" "+verbalLog+" "+Mathf.RoundToInt((poisonDmgPercntge * effected.MaxHP)/100));
		base.OnApplyEffect();
	}

	public override void OnRemovedEffect ()
	{
		effected.OnPoisoned = false;
		base.OnRemovedEffect ();
	}
}
