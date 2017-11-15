using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action{
	Skill skillSelected;
	Character target;

	public Skill SkillSelected {
		get {
			return this.skillSelected;
		}
		set {
			skillSelected = value;
		}
	}

	public Character Target {
		get {
			return this.target;
		}
		set {
			target = value;
		}
	}

	public static bool isValidAction(Action action){
		if(action != null && action.SkillSelected != null && action.Target != null)
			return true;
		else
			return false;
	}
}

namespace DynamicScripting{
	public class RuleCategoryType{
		public const string SUPPORT = "Sup";
		public const string SURVIVAL = "Sur";
		public const string OFFENSIVE = "Off";
		public const string DEFAULT = "Def";
	}
}

public class Rule {

	protected string ruleCategory;
	protected Static_AI_Script associatedScript;
	protected int weight;
	protected int priority;

	protected string[] condition_part;
	protected string action;
	protected string target;
	protected string ruleLine;

	bool execStatus = false;

	public Rule(string _ruleLine,Static_AI_Script associatedScript){
		this.associatedScript = associatedScript;
		ruleLine = _ruleLine;
		extractRule();
	}

	public Rule(string _ruleLine,Static_AI_Script associatedScript,int initialWeight){
		this.associatedScript = associatedScript;
		ruleLine = _ruleLine;
		this.weight = initialWeight;
		extractRule();
	}

	public int Weight {
		get {
			return this.weight;
		}
		set {
			weight = value;
		}
	}

	public int Priority {
		get {
			return this.priority;
		}
		set {
			priority = value;
		}
	}

	public bool executeStatus(){
		return execStatus;
	}

	private void extractRule(){
		//1.split condition and action
		string[] ruleLineParts = ruleLine.Split(new char[]{'-'});

		//1. Split condition into sub-condition and logicOperator
		condition_part = ruleLineParts[0].Split(new string[]{" "},System.StringSplitOptions.None);
		string[] actionTargetParts = ruleLineParts[1].Split(new char[]{','});
		action = actionTargetParts[0];
		target = actionTargetParts[1];
		priority = int.Parse(actionTargetParts[2]);
	}

	public bool conditionValid(){
		bool status = false;
		/*parse string condition to bool*/
		if(condition_part.Length <= 1)
			status = true;
		else{
			for(int i=0; i<condition_part.Length; i++){
				if(i%2 == 0 && i != 0)
					continue;
				else if(i%2 == 0 && i == 0){
					status = ConditionParser.parse(condition_part[i],associatedScript.Controller.Character_controlled);
				}
				else if(i%2 != 0){
					switch(condition_part[i]){
					case "&":{
							status = status && ConditionParser.parse(condition_part[i+1],associatedScript.Controller.Character_controlled);
							break;
						}
					case "|":{
							status = status || ConditionParser.parse(condition_part[i+1],associatedScript.Controller.Character_controlled);
							break;	
						}
					}
				}
			}
		}
		return status;
	}

	public string Action {
		get {
			return this.action;
		}
		set {
			action = value;
		}
	}

	public Action executeRule(){
		if(!conditionValid())
			return null;
		
		Action decision = new Action();
		/*parse action to decision object*/
		//first parse skill
		decision.SkillSelected = ActionParser.parse(action,associatedScript.Controller.Character_controlled);
		//second parse target
		decision.Target = TargetParser.parse(target,associatedScript.Controller.Character_controlled);

		return decision;	
	}
}
