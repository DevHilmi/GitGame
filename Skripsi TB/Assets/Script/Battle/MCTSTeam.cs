using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCTSTeam : Team {

	public MCTSTeam(string[] teamComposition,bool isHuman){
		id_increment+=1;
		team_id = id_increment;
		this.team_size = teamComposition.Length;
		this.team_composition = teamComposition;
		member = new List<CharaController>();
		this.isHuman = isHuman;
		goTeamContainer = GameObject.Find("Team"+team_id);
		generateTeam();
	}

	protected override void generateTeam ()
	{
		/*load prefab*/
		GameObject charaPrefab = Resources.Load("Prefab/CharaMCTS") as GameObject;
		charaPrefab.SetActive(false); //prevent auto invoke Start() after instantiate this prefab
		/*load prefab*/

		for(int i=0; i<team_size; i++){
			//check character class
			GameObject charaInstance = null;
			switch(team_composition[i]){
			case Character.Warrior:{
					//instantiate the character prefab first
					charaInstance = GameObject.Instantiate(charaPrefab);
					Battle.assignCharaToTeam(charaInstance,new Warrior(),this);
					break;
				}
			case Character.Wizard:{
					//instantiate the character prefab first
					charaInstance = GameObject.Instantiate(charaPrefab);
					Battle.assignCharaToTeam(charaInstance,new Wizard(),this);
					break;}
			case Character.Healer:{
					//instantiate the character prefab first
					charaInstance = GameObject.Instantiate(charaPrefab);
					Battle.assignCharaToTeam(charaInstance,new Healer(),this);
					break;}
			}
			charaInstance.SetActive(true);
			charaInstance.transform.SetParent(goTeamContainer.transform);
			charaInstance.transform.localPosition = pos_array[i];
			if(team_id == 0){
				charaInstance.transform.GetChild(0).transform.Rotate(0,180f,0);
			}
		}
		resetTeamStat();
	}
}