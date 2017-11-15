using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character {

	public const string Healer = "HEALER";
	public const string Wizard = "WIZARD";
	public const string Warrior = "WARRIOR";

	public bool isSimulationChara;
	public Character simulChara;
	public string profession;
	private CharaController controller;
	protected int hp,mana,maxHP,maxMana,def,maxDef,strength,maxStrength,missChance;

	//buff and status effect param
	public bool OnDefBuff,OnAtkBuff,OnPoisoned,OnBlinded;
	protected List<StatusEffect> statusEffect = new List<StatusEffect>();

	protected List<Skill> skillList = new List<Skill>();

	public Character(){}

	public CharaController Controller {
		get {
			
			return this.controller;
		}
		set {
			controller = value;
		}
	}

	public void resetCharaStat(){
		this.hp = maxHP;
		this.def = maxDef;
		this.mana = maxMana;
		this.missChance = 0;
		this.strength = maxStrength;
		statusEffect.Clear();
	}

	public int Strength {
		get {
			return this.strength;
		}
		set {
			strength = value;
		}
	}

	public int Hp {
		get {
			return this.hp;
		}
		set {
			hp = value;
		}
	}

	public int MaxHP {
		get {
			return this.maxHP;
		}
		set {
			maxHP = value;
		}
	}

	public int MaxDef {
		get {
			return this.maxDef;
		}
		set {
			maxDef = value;
		}
	}

	public int MaxStrength {
		get {
			return this.maxStrength;
		}
		set {
			maxStrength = value;
		}
	}
		
	public int Def {
		get {
			return this.def;
		}
		set {
			def = value;
		}
	}

	public int Mana {
		get {
			return this.mana;
		}
		set {
			mana = value;
		}
	}

	public int MaxMana {
		get {
			return this.maxMana;
		}
		set {
			maxMana = value;
		}
	}

	public int MissChance {
		get {
			return this.missChance;
		}
		set {
			missChance = value;
		}
	}




	public List<Skill> SkillList {
		get {
			return this.skillList;
		}
		set {
			skillList = value;
		}
	}

	public List<StatusEffect> StatusEffectList {
		get {
			return this.statusEffect;
		}
		set {
			statusEffect = value;
		}
	}

	public void registerNewStatusEffect(StatusEffect effect){
		statusEffect.Add(effect);
	}

	public void removeStatusEffect(){
		for(int i=0; i<statusEffect.Count; i++){
			if (statusEffect [i] != null) {
				statusEffect [i].OnRemovedEffect ();
			}
		}
		statusEffect.Clear ();
	}

	public void takeStatusDamage(int damage,Character damager){
		if(this.hp <= 0)
			return;

		this.hp -= damage;
		if(isSimulationChara)//this chara is simulation chara
		{
			if(hp <= 0)
				hp = 0;
			return;
		}
		
		if(GameManager.BattleManager().withVisual)
			controller.updateHPBar();

		//calculate dmg received and dealed then store it to team status
		controller.CharaTeam.Last_damage_received = damage;
		controller.CharaTeam.Total_damage_received += damage; 
		damager.controller.CharaTeam.Last_damage_dealt = damage;
		damager.controller.CharaTeam.Total_damage_dealt += damage;

		if(hp <= 0){
			hp = 0;
			controller.onCharaDeath();
		}
	}

	public void createSimulationSelf(CharaController controller,string profession){
		Character character = null;
		switch (profession) {
		case Character.Healer:
			{
				character = new Healer ();
				break;
			}
		case Character.Warrior:
			{
				character = new Warrior ();
				break;
			}
		case Character.Wizard:
			{
				character = new Wizard ();
				break;
			}
		}
		character.isSimulationChara = true;
		character.initializeSkills ();
		character.Controller = controller;
		simulChara = character;
	}

	public void synchronizeSimulationChara(){
		simulChara.hp = this.hp;
		simulChara.mana = this.mana;
		simulChara.def = this.def;
		simulChara.strength = this.strength;
		simulChara.missChance = this.missChance;

		//synchronize status effect here
		foreach(StatusEffect effect in statusEffect){
			StatusEffect cloneEffect = StatusEffect.createStatusEffect(effect.effectName,effect.Duration,effect.getEffectValue(),null);
			simulChara.registerNewStatusEffect(cloneEffect);
			cloneEffect.Effected = simulChara;
			cloneEffect.isActive = true;
			cloneEffect.Max_duration = effect.Max_duration;
		}
	}

	public void takeDamage(int damage,Character damager){

		if(this.hp<=0)
			return;
		
		int dmg_resist = Mathf.RoundToInt(def/100) * damage;
		damage -= dmg_resist;
		this.hp -= damage;

		if(isSimulationChara)//this chara is simulation chara
		{
			if(hp <= 0)
				hp = 0;
			return;
		}

		if(damage <= 0 && GameManager.BattleManager().withVisual)
			GameManager.UiManager.updateActionInfo("Attack Missed");
		
		if(GameManager.BattleManager().withVisual)
			controller.updateHPBar();

		//calculate dmg received and dealed then store it to team status
		controller.CharaTeam.Last_damage_received = damage;
		controller.CharaTeam.Total_damage_received += damage; 
		damager.controller.CharaTeam.Last_damage_dealt = damage;
		damager.controller.CharaTeam.Total_damage_dealt += damage;

		if(hp <= 0){
			hp = 0;
			//Debug.Log(profession+controller.CharaTeam.team_id+" is dead by"+damager.profession+damager.Controller.CharaTeam.team_id+damager.controller.charaID);
			controller.onCharaDeath();
		}
	}

	public abstract void initializeSkills();

	public bool hasDebuff(string effectType){
		bool status = false;
		foreach(StatusEffect effect in statusEffect){
			if(effect != null && effect.effectName == effectType && (effect.Max_duration-GameManager.BattleManager().turn_number) > 1){
				status = true;
				break;
			}
		}
		return status;
	}
}