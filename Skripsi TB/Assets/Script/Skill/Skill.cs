using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill{

	public const string Default = "DEFAULT";
	public const string LowDmg = "LOW_DMG";
	public const string MedDmg = "MED_DMG";
	public const string HighDmg = "HIGH_DMG";
	public const string Heal = "HEAL";
	public const string Clear = "CLEAR";
	public const string Poison = "POISON";
	public const string Blind = "BLIND";
	public const string BuffAtk = "BUFF_ATK";
	public const string BuffDef = "BUFF_DEF";

	protected int skillId;

	protected int mana_needed,min_damage,max_damage,target_num,damageDeal;
	protected string category,skillname;
	protected Character skillMaster;

	protected bool skillActivationSuccess;

	public Skill(){
	
	}

	public string skillIdToStr(){
		return skillId + "";
	}

	public Skill(int id ,string category,string skillname,int min_damage,int max_damage,int mana,Character skillmaster){
		this.skillname = skillname;
		this.category = category;
		this.min_damage = min_damage;
		this.max_damage = max_damage;
		this.skillMaster = skillmaster;
		this.mana_needed = mana;
		skillId = id;
	}

	/*Properties getter and setter*/
	public int Mana_needed {
		get {
			return this.mana_needed;
		}
		set {
			mana_needed = value;
		}
	}
	public string Category {
		get {
			return this.category;
		}
		set {
			category = value;
		}
	}

	public Character SkillMaster {
		get {
			return this.skillMaster;
		}
	}

	public string Skillname {
		get {
			return this.skillname;
		}
		set {
			skillname = value;
		}
	}
	public int MaxDamage {
		get {
			return this.max_damage;
		}
		set {
			max_damage = value;
		}
	}
	public int MinDamage {
		get {
			return this.min_damage;
		}
		set {
			min_damage = value;
		}
	}
	public bool SkillActivationStatus {
		get {
			return this.skillActivationSuccess;
		}
		set {
			skillActivationSuccess = value;
		}
	}
	/*End of Properties getter and setter*/


	/*  Description: call this function to activate the skill.
	 *  Parameter Explanation:
			-targets: list of character that affected by this skill.
			-teamBelong: the team that skillMaster belongs to.
	*/
	public virtual IEnumerator unleash(Character target,Team teamBelong){

		if(target.Controller.isDead){
			skillActivationSuccess = false;
			yield return null;
		}
			

		int missModifier = 1;
		if(skillMaster.MissChance > Random.Range(0,100)){
			missModifier = 0;
			GameManager.ActionLog.ActionMiss = true;
		}

		//check wether it's default attack. Default attack doesn't need mana to unleash.
		if(category == Skill.Default){

			//calculate damage based on min_damage and max_damage of this skill and put effect on target.
			damageDeal = Random.Range(min_damage,max_damage+1) + Mathf.RoundToInt(0.1f * skillMaster.Strength);
			target.takeDamage(damageDeal * missModifier,skillMaster);

			//no mana decreasing because it's default attack
			//set result of skill activation to success.
			skillActivationSuccess = true;
			yield return null;//exit function
		}

		//not default skill do procedure below
		//check wether team mana is sufficient for executed this skill. Check with possible min_damage constraint.
		if(skillMaster.Mana < mana_needed){
			//Mana is not sufficient -> set result of skill activation to fail then exits from function.
			skillActivationSuccess = false;
			yield return null;
		}
			
		//Mana sufficient -> do procedure below
		//calculate damage based on min_damage and max_damage of this skill and put effect on target.
		damageDeal = Random.Range(min_damage,max_damage+1);
		int strdmg = Mathf.RoundToInt(0.1f * skillMaster.Strength);
		target.takeDamage((strdmg + damageDeal) * missModifier,skillMaster);

		//request mana decreased to class Team
		skillMaster.Mana -= mana_needed;
		if(skillMaster.Mana <= 0)
			skillMaster.Mana = 0;

		//update mana bar
		updateMana();

		//set result of skill activation to success.
		skillActivationSuccess = true;
		yield return null;//exit from function
	}

	public virtual void updateMana(){
		if(!skillMaster.isSimulationChara)
			skillMaster.Controller.updateManaBar();
	}

	/*helper method to query skill*/
	public static Skill getDefaultSkill(List<Skill> skillList){
		Skill result = null;
		foreach(Skill skill in skillList){
			if(skill.category == Skill.Default)
			{
				result = skill;
				break;
			}
		}
		return result;
	}

	public static Skill getHighDamageSkill(List<Skill> skillList,int caster_mana){
		Skill result = null;
		foreach(Skill skill in skillList){
			if(skill.category == Skill.HighDmg && caster_mana >= skill.mana_needed)
			{
				result = skill;
				break;
			}
		}
		return result;
	}

	public static Skill getMedDamageSkill(List<Skill> skillList,int caster_mana){
		Skill result = null;
		foreach(Skill skill in skillList){
			if(skill.category == Skill.MedDmg && caster_mana >= skill.mana_needed)
			{
				result = skill;
				break;
			}
		}
		return result;
	}

	public static Skill getLowDamageSkill(List<Skill> skillList,int caster_mana){
		Skill result = null;
		foreach(Skill skill in skillList){
			if(skill.category == Skill.LowDmg && caster_mana >= skill.mana_needed)
			{
				result = skill;
				break;
			}
		}
		return result;
	}

	public static Skill getStatusSkill(string statusType,List<Skill> skillList,int caster_mana){
		Skill result = null;
		foreach(Skill skill in skillList){
			if(skill is IStatusEffect)
			{
				IStatusEffect iStatusEffect = (IStatusEffect)skill;
				if(iStatusEffect.getEffectType() == statusType && caster_mana >= skill.mana_needed){
					result = skill;
					break;
				}
			}
		}
		return result;
	}
		
	public static Skill getHealSkill(List<Skill> skillList,int caster_mana){
		Skill result = null;
		foreach(Skill skill in skillList){
			if(skill.category == Skill.Heal && caster_mana >= skill.mana_needed)
			{
				result = skill;
				break;
			}
		}
		return result;
	}

	public static Skill getClearSkill(List<Skill> skillList,int caster_mana){
		Skill result = null;
		foreach(Skill skill in skillList){
			if(skill.category == Skill.Clear && caster_mana >= skill.mana_needed)
			{
				result = skill;
				break;
			}
		}
		return result;
	}

	public static Skill getSkillByName(List<Skill> skillList,string _skillname,int caster_mana){
		Skill result = null;
		foreach(Skill skill in skillList){
			if(skill.skillname == _skillname)
			{
				result = skill;
				break;
			}
		}
		return result;
	}
}