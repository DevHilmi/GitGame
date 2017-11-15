using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionParam{
	public const string enemy_num = "enemy_num";
	public const string selfMana = "selfMana",selfHP = "selfHP",selfPoisoned = "selfPoisoned",selfBlinded = "selfBlinded",selfAtkBuffed = "selfAtkBuffed"
		,selfDefBuffed = "selfDefBuffed";
	public const string allyMinHP = "allyMinHP",allyPoisoned = "allyPoisoned",allyBlinded = "allyBlinded",allyAtkBuffed = "allyAtkBuffed"
		,allyDefBuffed = "allyDefBuffed",enemyPoisoned = "enemyPoisoned",enemyBlinded = "enemyBlinded";
}

public class ConditionParser {

	public static bool parse(string condition,Character actor){
		bool status = false;
		string param = null,value = null;
		string[] part = condition.Split(new char[]{'=','<','>'});
		param = part[0];
		value = part[1];

		switch(param){

		/*Start parsing by parameter. This is so fucking healthy :v*/
		case ConditionParam.selfHP:{
				int _value = int.Parse(value);
				_value = Mathf.RoundToInt(((float)_value/100f)*(float)actor.MaxHP);

				if(condition.Contains("=")){
					status = actor.Hp == _value;
				}
				else if(condition.Contains("<")){
					status = actor.Hp < _value;
				}
				else if(condition.Contains(">")){
					status = actor.Hp > _value;
				}
				break;
			}
	
		case ConditionParam.allyMinHP:{
				int _value = int.Parse(value);
				Team actorTeam = actor.Controller.CharaTeam;
				if(actorTeam.getMinHealthAlly(actor,false) != null){
					_value = Mathf.RoundToInt(((float)_value/100f)*(float)actorTeam.getMinHealthAlly(actor,false).MaxHP);

					if(condition.Contains("=")){
						status = actorTeam.minHealthAlly(actor,false) == _value;
					}
					else if(condition.Contains("<")){
						status = actorTeam.minHealthAlly(actor,false) < _value;
					}
					else if(condition.Contains(">")){
						status = actorTeam.minHealthAlly(actor,false) > _value;
						}
				}
				break;
			}

		case ConditionParam.selfMana:{
				int _value = int.Parse(value);
				_value = Mathf.RoundToInt(((float)_value/100f)*(float)actor.MaxMana);

				if(condition.Contains("=")){
					status = actor.Mana == _value;
				}
				else if(condition.Contains("<")){
					status = actor.Mana < _value;
				}
				else if(condition.Contains(">")){
					status = actor.Mana > _value;
				}
				break;
			}

		case ConditionParam.selfAtkBuffed:{
				if(condition.Contains("=")){
					if(value == "true")
						status = actor.OnAtkBuff == true;
					else if(value == "false")
						status = actor.OnAtkBuff == false;
				}
				break;
			}

		case ConditionParam.allyAtkBuffed:{
				if(condition.Contains("=")){
					if(value == "true")
						status = actor.Controller.CharaTeam.allyHasBuff(actor,Skill.BuffAtk) == true;
					else if(value == "false")
						status = actor.Controller.CharaTeam.allyHasBuff(actor,Skill.BuffAtk) == false;
				}
				break;
			}

		case ConditionParam.selfDefBuffed:{
				if(condition.Contains("=")){
					if(value == "true")
						status = actor.OnDefBuff == true;
					else if(value == "false")
						status = actor.OnDefBuff == false;
				}
				break;
			}

		case ConditionParam.allyDefBuffed:{
				if(condition.Contains("=")){
					if(value == "true")
						status = actor.Controller.CharaTeam.allyHasBuff(actor,Skill.BuffDef) == true;
					else if(value == "false")
						status = actor.Controller.CharaTeam.allyHasBuff(actor,Skill.BuffDef) == false;
				}
				break;
			}

		case ConditionParam.selfBlinded:{
				if(condition.Contains("=")){
					if(value == "true")
						status = actor.hasDebuff(Skill.Blind) == true;
					else if(value == "false")
						status = actor.hasDebuff(Skill.Blind) == false;
				}
				break;
			}

		case ConditionParam.allyBlinded:{
				if(condition.Contains("=")){
					if(value == "true")
						status = actor.Controller.CharaTeam.allyHasDebuff(actor,Skill.Blind) == true;
					else if(value == "false")
						status = actor.Controller.CharaTeam.allyHasDebuff(actor,Skill.Blind) == false;
				}

				break;
			}

		case ConditionParam.enemyBlinded:{
				if(condition.Contains("=")){
					if(value == "true")
						status = actor.Controller.CharaTeam.getEnemyTeam().allyHasDebuff(actor,Skill.Blind) == true;
					else if(value == "false")
						status = actor.Controller.CharaTeam.getEnemyTeam().allyHasDebuff(actor,Skill.Blind) == false;
				}
				break;
			}
				
		case ConditionParam.selfPoisoned:{
				if(condition.Contains("=")){
					if(value == "true")
						status = actor.hasDebuff(Skill.Poison) == true;
					else if(value == "false")
						status = actor.hasDebuff(Skill.Poison) == false;
				}
				break;
			}

		case ConditionParam.allyPoisoned:{
				if(condition.Contains("=")){
					if(value == "true")
						status = actor.Controller.CharaTeam.allyHasDebuff(actor,Skill.Poison) == true;
					else if(value == "false")
						status = actor.Controller.CharaTeam.allyHasDebuff(actor,Skill.Poison) == false;
				}
				break;
			}

		case ConditionParam.enemyPoisoned:{
				if(condition.Contains("=")){
					if(value == "true")
						status = actor.Controller.CharaTeam.getEnemyTeam().allyHasDebuff(actor,Skill.Poison) == true;
					else if(value == "false")
						status = actor.Controller.CharaTeam.getEnemyTeam().allyHasDebuff(actor,Skill.Poison) == false;
				}
				break;
			}

		case ConditionParam.enemy_num:{
				Team actorTeam = actor.Controller.CharaTeam;
				if(condition.Contains("=")){
					if(value == "LESS")
						status = actorTeam.MemberAlive > actorTeam.getEnemyTeam().MemberAlive;
					else if(value == "SAME")
						status = actorTeam.MemberAlive == actorTeam.getEnemyTeam().MemberAlive;
					else if(value == "MORE")
						status = actorTeam.MemberAlive < actorTeam.getEnemyTeam().MemberAlive;
				}
				break;
			}
			/*End parsing by parameter. Finally...oh yeah..it's over :v*/
		}
		part = null;
		param = value = null;
		return status;
	}
}