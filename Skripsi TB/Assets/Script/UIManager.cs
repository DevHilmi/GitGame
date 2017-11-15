using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public GameObject team0Mana,team1Mana,turnNumber,actionInfo;
	private Text manaTeam0info,manaTeam1info,turnNumberTxt,actionText;
	// Use this for initialization
	void Awake () {
		manaTeam0info = team0Mana.GetComponent<Text>();
		manaTeam1info = team1Mana.GetComponent<Text>();
		turnNumberTxt = turnNumber.GetComponent<Text>();
		actionText = actionInfo.GetComponent<Text>();
	}

	public void updateManaInfo(int mana,int team_id){
		if(team_id == 0){
			manaTeam0info.text = mana+"";
		}
		else if(team_id == 1){
			manaTeam1info.text = mana+"";
		}
	}

	public void updateTurnNumber(int _turnNumber){
		turnNumberTxt.text = "Turn "+_turnNumber;
	}

	public void updateActionInfo(string actionName){
		actionText.text = actionName;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
