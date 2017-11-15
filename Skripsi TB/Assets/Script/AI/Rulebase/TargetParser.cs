using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetType{


//	WARRIOR_E,WARRIOR_A,
//	WIZARD_E,WIZARD_A,
//	HEALER_E,HEALER_A,
//	POISONED_E,POISONED_A,
//	BLINDED_E,BLINDED_A,

	public const string self = "SELF";
	public const string rand_e = "RANDOM_E",rand_a = "RANDOM_A";
	public const string min_health_e = "MIN_HEALTH_E",min_health_a = "MIN_HEALTH_A";
	public const string min_def_e = "MIN_DEF_E",min_def_a = "MIN_DEF_A";
	public const string max_def_e = "MAX_DEF_E",max_def_a = "MAX_DEF_A";
	public const string min_atk_e = "MIN_ATK_E",min_atk_a = "MIN_ATK_A";
	public const string max_atk_e = "MAX_ATK_E",max_atk_a = "MAX_ATK_A";
	public const string warr_e = "WARRIOR_E",warr_a = "WARRIOR_A";
	public const string wiz_e = "WIZARD_E",wiz_a = "WIZARD_A";
	public const string hea_e = "HEALER_E",hea_a = "HEALER_A";
	public const string blinded_e = "BLINDED_E",blinded_a = "BLINDED_A";
	public const string poisoned_e = "POISONED_E",poisoned_a = "POISONED_A";
	public const string buffed_atk_a = "ATK_BUFFED_A",buffed_atk_e = "ATK_BUFFED_E";
	public const string buffed_def_a = "DEF_BUFFED_A",buffed_def_e = "DEF_BUFFED_E";
}

public class TargetParser {

	public static Character parse(string targetType,Character actor){
		/*build reference and knowledge*/
		Character target = null;
		Team actorTeam = actor.Controller.CharaTeam;
		Team enemyTeam = actorTeam.getEnemyTeam();
		List<Character> enemiesPossibleTarget = enemyTeam.getAliveMembers();

		List<Character> alliesTarget = new List<Character>();

		foreach(Character ally in actorTeam.getAliveMembers()){
			if(ally.Controller.charaID != actor.Controller.charaID){
				alliesTarget.Add(ally);
			}
		}
		/*build reference and knowledge*/

		/*switch target type*/
		switch(targetType){
		case TargetType.self:{
				target = actor;
				break;
			}
		case TargetType.rand_e:{
				if(enemiesPossibleTarget.Count > 0)
				{   
					target = enemyTeam.getRandomCharacter ();
				}
				break;
			}
		case TargetType.rand_a:{
				if(alliesTarget.Count>0){
					target = actorTeam.getRandomCharacter ();
				}
				break;
			}
		case TargetType.min_health_a:{
				target = actorTeam.getMinHealthAlly(actor,false);
				break;
			}
		case TargetType.min_health_e:{
				target = enemyTeam.getMinHealthAlly(null,true);
				break;
			}
		case TargetType.min_def_a:{
				target = actorTeam.getMinDef(actor,false);
				break;
			}
		case TargetType.min_def_e:{
				target = enemyTeam.getMinDef(null,true);
				break;
			}
		case TargetType.max_def_a:{
				target = actorTeam.getMaxDef(actor,false);
				break;
			}
		case TargetType.max_def_e:{
				target = enemyTeam.getMaxDef(null,true);
				break;
			}
		case TargetType.min_atk_a:{
				target = actorTeam.getMinAtk(actor,false);
				break;
			}
		case TargetType.min_atk_e:{
				target = enemyTeam.getMinAtk(null,true);
				break;
			}
		case TargetType.max_atk_a:{
				target = actorTeam.getMaxAtk(actor,false);
				break;
			}
		case TargetType.max_atk_e:{
				target = enemyTeam.getMaxAtk(null,true);
				break;
			}
		case TargetType.poisoned_a:{
				target = actorTeam.getDebuffedAlly(actor,Skill.Poison);
				break;
			}
		case TargetType.poisoned_e:{
				target = enemyTeam.getDebuffedMember(Skill.Poison);
				break;
			}
		case TargetType.blinded_a:{
				target = actorTeam.getDebuffedAlly(actor,Skill.Blind);
				break;
			}
		case TargetType.blinded_e:{
				target = enemyTeam.getDebuffedMember(Skill.Blind);
				break;
			}
		case TargetType.buffed_atk_a:{
				target = actorTeam.getBuffedAlly(actor,Skill.BuffAtk);
				break;
			}
		case TargetType.buffed_atk_e:{
				target = enemyTeam.getBuffedMember(Skill.BuffAtk);
				break;
			}
		case TargetType.buffed_def_a:{
				target = actorTeam.getBuffedAlly(actor,Skill.BuffDef);
				break;
			}
		case TargetType.buffed_def_e:{
				target = enemyTeam.getBuffedMember(Skill.BuffDef);
				break;
			}
		}
		/*switch target type*/

		/*free memories from temporary object*/
		alliesTarget = null;
		enemiesPossibleTarget = null;
		/*free memories from temporary object*/

		return target;
	}
}
