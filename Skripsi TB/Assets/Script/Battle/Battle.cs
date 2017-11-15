using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour,IHasTurn {

	//public int team_number;
	public int turn_number,manaPerTurn;
	public float battleDelay;
	public int current_turn;//id of team that has current turn
	public Team currentTurnTeam;
	static Team[] fightTeams;
	public bool hasEnd = true,withVisual = true;

	//for testing
	public bool repeatBattle = true;
	public int repeatNumber,currRepeatNumber;
	BattleLog bLog;

	// Use this for initialization
	void Start () {
		initializeBattle();
	}

	public Team[] getFightTeam(){
		return fightTeams;
	}

	public static void assignCharaToTeam(GameObject chara,Character profession,Team team){

		CharaController char_cont = chara.GetComponent<CharaController>();
		team.Member.Add(char_cont);
		char_cont.CharaTeam = team;
		char_cont.initializeCharaController(profession);
		chara.SetActive(true);
	}

	public virtual void initializeBattle(){
		//set Battle Log manager
		bLog = new BattleLog(repeatNumber,3);

		/*set fight team*/
		fightTeams = new Team[2];//number of team that fight in this battle
		fightTeams[0] = new MCTSTeam(new string[]{Character.Warrior,Character.Healer,Character.Wizard},false);
		fightTeams[1] = new Team(new string[]{Character.Warrior,Character.Healer,Character.Wizard},false);
		/*set fight team*/

		/*set first turn team*/
		hasEnd = false;
		currentTurnTeam = fightTeams[0];
		GameManager.UiManager.updateTurnNumber(turn_number);
		StartCoroutine(OnTurn());

		//start battle simulation
		StartCoroutine(BattleUpdate());
	}

	public void restartBattle(){
		fightTeams[0].resetTeamStat();
		fightTeams[1].resetTeamStat();
		turn_number = 0;
		currentTurnTeam = fightTeams[1];
		hasEnd = false;
	}
		
	public virtual IEnumerator changeTurn(){

		int last_team_id = currentTurnTeam.team_id; 
		if(currentTurnTeam.team_id < fightTeams.Length - 1){
			currentTurnTeam = fightTeams[last_team_id + 1];
		}
		else{
			currentTurnTeam = fightTeams[0];
		}

		//update turn number
		turn_number += 1;
		GameManager.UiManager.updateTurnNumber(turn_number);

		yield return null;
	}

	public virtual IEnumerator OnTurn ()
	{
		currentTurnTeam.hasTurn = true;

		foreach(Team team in fightTeams){
			team.resetStatusEffectNewTurn();
			team.checkAndApplyStatusEffect();
		}
		yield return StartCoroutine(currentTurnTeam.takeAction());
		yield return StartCoroutine(OnTurnEnd());
	}

	public virtual IEnumerator OnTurnEnd ()
	{
		currentTurnTeam.hasTurn = false;
		yield return null;
	}
		
	public void OnBattleOver(){
		hasEnd = true;
		int[] rowData = new int[]{currRepeatNumber,fightTeams[0].MemberAlive,fightTeams[1].MemberAlive};
		bLog.addRow(currRepeatNumber,rowData);
		fightTeams[0].addActionToLog();

		//write log
		if(currRepeatNumber == repeatNumber-1){
			bLog.writeBattleLog();//write battle data log
			fightTeams[0].writeActionLog();//write action log
		}
	}

	//Battle simulation's main loop
	protected virtual IEnumerator BattleUpdate () {

		while(this.gameObject.activeSelf){ 
			if(hasEnd){
				if(repeatBattle && currRepeatNumber < repeatNumber-1){
					//store battle info and evaluate dynamic AI fitness

					//prepare new battle
					restartBattle();
					currRepeatNumber += 1;
					yield return new WaitForSeconds(2f);
				}
			}

			else{
				if(currentTurnTeam != null && !currentTurnTeam.hasTurn){
					yield return StartCoroutine(changeTurn());
					if(!currentTurnTeam.isHuman)
						yield return StartCoroutine(OnTurn());
				}
			}
			yield return null;
		}
	}
}