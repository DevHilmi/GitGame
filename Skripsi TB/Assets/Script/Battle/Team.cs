using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team {

	protected static int id_increment = -1;

	/*team static status*/
	public int team_id;
	protected List<CharaController> member;
	protected string[] team_composition; //eg: {WARRIOR,WIZARD,HEALER},{WIZARD,HEALER,WIZARD}
	public bool isHuman;//indicate if this team controlled by human
	protected int team_size;
	/*team static status*/
		
	protected GameObject goTeamContainer = null;

	//member position
	protected Vector3[] pos_array = new Vector3[3]{
		new Vector3(0,0,0),
		new Vector3(0,0.89f,0),
		new Vector3(1.34f,0.35f,0)
	};

	//team dynamic status
	public bool hasTurn;//indicate if this team has turn
	protected int team_mana,max_team_mana,team_hp,max_team_hp,total_damage_received,total_damage_dealt,memberAlive,last_damage_received,last_damage_dealt;

	public Team(){}

	public Team(string[] teamComposition,bool isHuman){
		id_increment+=1;
		team_id = id_increment;
		this.team_size = teamComposition.Length;
		this.team_composition = teamComposition;
		member = new List<CharaController>();
		this.isHuman = isHuman;
		goTeamContainer = GameObject.Find("Team"+team_id);
		generateTeam();
	}
				
	public List<CharaController> Member{
		get{
			return member;
		}
		set{
			member = value;
		}
	}

	public int Team_mana {
		get {
			return this.team_mana;
		}
		set {
			team_mana = value;
		}
	}

	public int Max_team_mana {
		get {
			return this.max_team_mana;
		}
		set {
			max_team_mana = value;
		}
	}

	public int Team_hp {
		get {
			int totalHP = 0;
			foreach(CharaController _cont in member){
				totalHP += _cont.Character_controlled.Hp;
			}
			return this.team_hp;
		}
		set {
			team_hp = value;
		}
	}

	public int Max_team_hp {
		get {
			return this.max_team_hp;
		}
		set {
			max_team_hp = value;
		}
	}

	public int Total_damage_received {
		get {
			return this.total_damage_received;
		}
		set {
			total_damage_received = value;
		}
	}

	public int Total_damage_dealt {
		get {
			return this.total_damage_dealt;
		}
		set {
			total_damage_dealt = value;
		}
	}

	public int Last_damage_received {
		get {
			return this.last_damage_received;
		}
		set {
			last_damage_received = value;
		}
	}

	public int Last_damage_dealt {
		get {
			return this.last_damage_dealt;
		}
		set {
			last_damage_dealt = value;
		}
	}

	public int MemberAlive {
		get {
			return memberAlive;
		}
		set {
			memberAlive = value;
		}
	}



	protected virtual void generateTeam(){

		/*load prefab*/
		GameObject charaPrefab = Resources.Load("Prefab/Chara") as GameObject;
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

	public void resetStatusEffectNewTurn(){
		foreach(CharaController char_cont in member){
			if(char_cont!=null){
				Character character = char_cont.Character_controlled;
				foreach(StatusEffect effect in character.StatusEffectList){
					effect.OnNewTurn();
				}
			}
		}
	}

	public void checkAndApplyStatusEffect(){
		foreach(CharaController char_cont in member){
			if(char_cont!=null){
				Character character = char_cont.Character_controlled;
				for(int i=0; i<character.StatusEffectList.Count; i++){
					if(character.StatusEffectList[i] != null){
						character.StatusEffectList[i].checkStatusEffect(GameManager.BattleManager().turn_number);
					}
				}
			}
		}
	}

	public void requestMemberOut(CharaController deathChara){
		memberAlive -= 1;
		//member.Remove(deathChara);
		if(memberAlive <= 0)
			GameManager.BattleManager().OnBattleOver();
	}

	public virtual void resetTeamStat(){

		memberAlive = team_composition.Length;
		team_hp = team_mana = 0;
		total_damage_received = total_damage_dealt = 0;

		foreach(CharaController chara_cont in member){
			chara_cont.gameObject.SetActive(false);
			chara_cont.isDead = false;
			chara_cont.Character_controlled.removeStatusEffect ();
			chara_cont.Character_controlled.resetCharaStat();
			team_hp += chara_cont.Character_controlled.MaxHP;
			team_mana += chara_cont.Character_controlled.MaxMana;

			chara_cont.updateHPBar();
			chara_cont.updateManaBar();
			chara_cont.gameObject.SetActive(true);
		}
		max_team_hp = team_hp;
	}

	public void writeActionLog(){
		foreach(CharaController c in member){
			c.ActLog.writeActLog();
		}
	}

	public void addActionToLog(){
		foreach(CharaController c in member){
			c.addAgentActLog();
		}
	}

	public Team getEnemyTeam(){
		Team enemyTeam = null;
		foreach(Team team in GameManager.BattleManager().getFightTeam()){
			if(team != null && !team.Equals(this)){
				enemyTeam = team;
				break;
			}
		}
		return enemyTeam;
	}

	public List<Character> getAliveMembers(){
		List<Character> result = new List<Character>();
		foreach(CharaController cont in member){
			if(!cont.isDead)
				result.Add(cont.Character_controlled);
		}
		return result;
	}

	public Character getRandomCharacter(){
		Character target = null;
		List<Character> alive = getAliveMembers ();
		if (alive.Count > 0) {
			int index = Random.Range (0, alive.Count);
			target = alive [index];
			alive.Clear ();
			alive = null;
		}
		return target;
	}
			
	public CharaController currMemberTurnHuman = null;
	public virtual IEnumerator takeAction(){

		if(!isHuman){
			for(int i=0; i<member.Count; i++){			
				if(member[i] != null && member[i].gameObject.activeSelf){
					
					member[i].doAction();//do action each character

					if(!GameManager.BattleManager().hasEnd){
						yield return new WaitForSecondsRealtime(GameManager.BattleManager().battleDelay);
						if(GameManager.BattleManager().withVisual){
							member[i].toogleTurnCursor("",false);
							GameManager.ActionLog.TargetChar.Controller.toogleTurnCursor("",false);
						}
						GameManager.ActionLog.resetLog();
					}
					else
						break;
				}
			}
		}

		yield return null;
	}

//	public IEnumerator changeActor(int index){
//		Debug.Log("AAA");
//		index+=1;
//		if(index >= memberAlive+member[0].charaID)
//			yield return null;
//		else
//			member[index].charaSelected = true;
//	}

	/*Team specific data query*/
	//minHealth
	public int minHealthAlly(Character based,bool getAll){
		List<Character> alive = getAliveMembers();
		int min = int.MaxValue;
		foreach(Character chara in alive){
			if(!getAll){
				if(chara.Controller.charaID != based.Controller.charaID){
					if(chara.Hp < min)
						min = chara.Hp;
					}
			}
			else{
				if(chara.Hp < min)
					min = chara.Hp;
			}
		}

		alive = null;
		return min;
	}

	//getAllyMinHP
	public Character getMinHealthAlly(Character based,bool getAll){
		List<Character> alive = getAliveMembers();
		int min = int.MaxValue;
		Character charaMin = null;

		foreach(Character chara in alive){
			if(!getAll){
				if(chara.Controller.charaID != based.Controller.charaID){
					if(chara.Hp < min){
						min = chara.Hp;
						charaMin = chara;
					}
				}
			}
			else{
				if(chara.Hp < min){
					min = chara.Hp;
					charaMin = chara;
				}
			}
		}
		alive = null;
		return charaMin;
	}

	//maxHealth  getAllyMaxHP

	//getAllyMinDef 
	public Character getMinDef(Character based,bool getAll){
		List<Character> alive = getAliveMembers();
		int min = int.MaxValue;
		Character charaMin = null;

		foreach(Character chara in alive){
			if(!getAll){
				if(chara.Controller.charaID != based.Controller.charaID){
					if(chara.Def < min){
						min = chara.Def;
						charaMin = chara;
					}
				}
			}
			else{
				if(chara.Def < min){
					min = chara.Def;
					charaMin = chara;
				}
			}
		}
		alive = null;
		return charaMin;
	}
	//getAllyMaxDef
	public Character getMaxDef(Character based,bool getAll){
		List<Character> alive = getAliveMembers();
		int min = int.MinValue;
		Character charaMin = null;

		foreach(Character chara in alive){
			if(!getAll){
				if(chara.Controller.charaID != based.Controller.charaID){
					if(chara.Def > min){
						min = chara.Def;
						charaMin = chara;
					}
				}
			}
			else{
				if(chara.Def > min){
					min = chara.Def;
					charaMin = chara;
				}
			}
		}
		alive = null;
		return charaMin;
	}

	//getAllyMinAtk 
	public Character getMinAtk(Character based,bool getAll){
		List<Character> alive = getAliveMembers();
		int min = int.MaxValue;
		Character charaMin = null;

		foreach(Character chara in alive){
			if(!getAll){
				if(chara.Controller.charaID != based.Controller.charaID){
					if(chara.Strength < min){
						min = chara.Strength;
						charaMin = chara;
					}
				}
			}
			else{
				if(chara.Strength < min){
					min = chara.Strength;
					charaMin = chara;
				}
			}
		}
		alive = null;
		return charaMin;
	}

	//getAllyMaxAtk
	public Character getMaxAtk(Character based,bool getAll){
		List<Character> alive = getAliveMembers();
		int min = int.MinValue;
		Character charaMin = null;

		foreach(Character chara in alive){
			if(!getAll){
				if(chara.Controller.charaID != based.Controller.charaID){
					if(chara.Strength > min){
						min = chara.Strength;
						charaMin = chara;
					}
				}
			}
			else{
				if(chara.Strength > min){
					min = chara.Strength;
					charaMin = chara;
				}
			}
		}
		alive = null;
		return charaMin;
	}

	//containWarrior 
	public bool containWarrior(){
		return false;
	}

	//getWarrior
	public Character getWarrior(){
		return null;
	}
		
	//check if there debuffed allies of based 
	public bool allyHasDebuff(Character based,string debuffType){
		List<Character> alive = getAliveMembers();
		bool _allyHasDebuff = false;

		foreach(Character chara in alive){
			if(chara.Controller.charaID != based.Controller.charaID){
				foreach(StatusEffect effect in chara.StatusEffectList){
					if(effect != null && effect.effectName == debuffType){
						_allyHasDebuff = true;
						break;
					}
				}
			}
		}
		alive = null;
		return _allyHasDebuff;
	}
		
	//get debuffed ally of based
	public Character getDebuffedAlly(Character based,string debuffType){
		List<Character> alive = getAliveMembers();
		Character _allyDebuffed = null;

		foreach(Character chara in alive){
			if(chara.Controller.charaID != based.Controller.charaID){
				foreach(StatusEffect effect in chara.StatusEffectList){
					if(effect != null && effect.effectName == debuffType){
						_allyDebuffed = chara;
						break;
					}
				}
			}
		}
		alive = null;
		return _allyDebuffed;
	}

	//get debuffed member from team
	public Character getDebuffedMember(string debuffType){
		List<Character> alive = getAliveMembers();
		Character _member = null;

		foreach(Character chara in alive){
			foreach(StatusEffect effect in chara.StatusEffectList){
				if(effect != null && effect.effectName == debuffType){
					_member = chara;
					break;
				}
			}
		}
		alive = null;
		return _member;
	}

	//check if there buffed allies of based 
	public bool allyHasBuff(Character based,string buffType){
		List<Character> alive = getAliveMembers();
		bool _allyHasBuff = false;

		foreach(Character chara in alive){
			if(chara.Controller.charaID != based.Controller.charaID){
				foreach(StatusEffect effect in chara.StatusEffectList){
					if(effect != null && effect.effectName == buffType && effect is ParamBuff){
						_allyHasBuff = true;
						break;
					}
				}
			}
		}
		alive = null;
		return _allyHasBuff;
	}

	//get buffed ally of based
	public Character getBuffedAlly(Character based,string buffType){
		List<Character> alive = getAliveMembers();
		Character _allyBuffed = null;

		foreach(Character chara in alive){
			if(chara.Controller.charaID != based.Controller.charaID){
				foreach(StatusEffect effect in chara.StatusEffectList){
					if(effect != null && effect.effectName == buffType && effect is ParamBuff){
						_allyBuffed = chara;
						break;
					}
				}
			}
		}
		alive = null;
		return _allyBuffed;
	}

	//get buffed member from team
	public Character getBuffedMember(string buffType){
		List<Character> alive = getAliveMembers();
		Character _member = null;

		foreach(Character chara in alive){
			foreach(StatusEffect effect in chara.StatusEffectList){
				if(effect != null && effect.effectName == buffType && effect is ParamBuff){
					_member = chara;
					break;
				}
			}
		}
		alive = null;
		return _member;
	}
}