using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard :Character{

	public Wizard(){
		this.profession = Character.Wizard;
		this.hp = 180;
		this.mana = 95;
		this.def = 5;
		this.maxMana = mana;
		this.maxHP = hp;
		this.strength = 30;
		maxStrength = strength;
		maxDef = def;
	}
		
	public override void initializeSkills ()
	{
		skillList.Add(new Skill(0,Skill.Default,"Attack",13,15,0,this));
		skillList.Add(new Skill(1,Skill.LowDmg,"Flare",22,26,20,this));
		skillList.Add(new Skill(2,Skill.MedDmg,"Phoenix Storm",27,34,24,this));
		skillList.Add(new DivineSkill(3,Skill.HighDmg,Skill.Blind,"Eclipse",35,39,40,2,32,this) as Skill);
		//skillList.Add(new BuffSkill(Skill.BuffAtk,"Temptation",20,5,this,1) as Skill);
	}
}
