using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Character {

	public Warrior(){
		this.profession = Character.Warrior;
		this.hp = 190;
		this.mana = 90;
		this.def = 10;
		this.maxMana = mana;
		this.maxHP = hp;
		this.strength = 40;
		maxStrength = strength;
		maxDef = def;
	}
		
	public override void initializeSkills ()
	{
		skillList.Add(new Skill(0, Skill.Default,"Attack",14,17,0,this));
		skillList.Add(new Skill(1, Skill.LowDmg,"Cascade Strike",24,26,19,this));
		skillList.Add(new DivineSkill(2, Skill.LowDmg,Skill.Poison,"Demon's Blow",15,17,5,2,26,this) as Skill);//5% reduce HP 2 turn
		skillList.Add(new Skill(3, Skill.HighDmg,"Rains O'Blade",35,37,31,this));
		skillList.Add(new BuffSkill(4, Skill.BuffDef,"Demon's Zeal",22,5,this,1) as Skill);//+5 DEF 1 turn
	}
}
