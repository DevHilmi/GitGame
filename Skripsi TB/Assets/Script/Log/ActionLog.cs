using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionLog{

	Character actChar,targetChar;
	Skill actionUsed;
	bool actionStatus = true,actionMiss = false;

	public Character ActChar {
		get {
			return this.actChar;
		}
		set {
			actChar = value;
		}
	}

	public Character TargetChar {
		get {
			return this.targetChar;
		}
		set {
			targetChar = value;
		}
	}

	public Skill ActionUsed {
		get {
			return this.actionUsed;
		}
		set {
			actionUsed = value;
		}
	}

	public bool ActionStatus {
		get {
			return this.actionStatus;
		}
		set {
			actionStatus = value;
		}
	}

	public bool ActionMiss {
		get {
			return this.actionMiss;
		}
		set {
			actionMiss = value;
		}
	}

	public void resetLog(){
		actChar = targetChar = null;
		actionUsed = null;
		actionMiss = false;
		actionStatus = true;
	}
				
	public void displayAction(){
		if(!actionStatus){
			//Debug.Log("Action is not success");
			return;
		}
		if(actionMiss){
			if(GameManager.BattleManager().withVisual){
				GameManager.UiManager.updateActionInfo("Attack Missed!");
			}
		}
		else{
			if(GameManager.BattleManager().withVisual){
				GameManager.UiManager.updateActionInfo(actionUsed.Skillname);
			}
		}
	}
}