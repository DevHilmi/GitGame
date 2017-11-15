using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Character {

	public Healer(){
		this.profession = Character.Healer;
		this.hp = 160;
		this.mana = 100;
		this.def = 7;
		this.maxMana = mana;
		this.maxHP = hp;
		this.strength = 20;
		maxStrength = strength;
		maxDef = def;


	}

	public override void initializeSkills ()
	{
		skillList.Add(new Skill(0, Skill.Default,"Attack",10,13,0,this));
		skillList.Add(new Skill(1, Skill.LowDmg,"Lightning Arc",22,24,18,this));
		skillList.Add(new Skill(2, Skill.MedDmg,"Poseidon",27,30,22,this));
		skillList.Add(new HealSkill(3, "Revitalize",35,35f,this) as Skill);
		skillList.Add(new ClearSkill(4, "Purifier Fountain",25,this) as Skill);
	}

}
