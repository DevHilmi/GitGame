using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AI_Type{
	DynamicScripting,StaticScripting,MCTS
}

public class Static_AI_Script:MonoBehaviour,AI_Module {

	protected CharaController controller;
	protected Rule[] rules;
	protected string ruleBaseText;
	protected string rulebasePath = "Rulebases/";
	protected AI_Type type;
	protected Action decision = null;

	public virtual void initializeAI ()
	{
		type = AI_Type.StaticScripting;
		controller = GetComponent<CharaController>();
		ruleBaseText = Resources.Load<TextAsset>(rulebasePath+controller.Character_controlled.profession).text;
		assignRules();
	}

	public CharaController Controller {
		get {
			return this.controller;
		}
		set {
			controller = value;
		}
	}

	public virtual Action makeDecision ()
	{
		for(int i=0; i<rules.Length; i++){
			decision = rules[i].executeRule();
			if(Action.isValidAction(decision)){
				break;//get decision
			}
			else{
				decision = null;
			}
		}
		return decision;
	}

	public AI_Type getAIModuleType ()
	{
		return type;
	}

	public virtual void assignRules(){
		string[] lines = ruleBaseText.Split(new string[]{System.Environment.NewLine},System.StringSplitOptions.None);
		rules = new Rule[lines.Length];

		//assign rule here
		for(int i=0; i<lines.Length; i++){
			rules[i] = new Rule(lines[i],this);
		}
	}
}