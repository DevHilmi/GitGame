using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaController : MonoBehaviour {

	public static int chara_id_counter = -1;
	Team charateam;
	public int charaID;
	Character character_controlled;
	public int hp,def,strength,miss_chance,numberStatEffect,mana;
	public bool isDead;

	/*Visual hud*/
	private GameObject chara_sprite,chara_hp_bar,chara_mana_bar,turnCursor;
	private StatusEffectIndicator sEffectIndicator;
	/*Visual hud*/

	//AI module
	AI_Module ai_module;

	public Character Character_controlled {
		get {
			return this.character_controlled;
		}
		set {
			character_controlled = value;
		}
	}

	public Team CharaTeam{
		get{
			return charateam;
		}
		set{
			charateam = value;
		}
	}

	public StatusEffectIndicator SEffectIndicator {
		get {
			return this.sEffectIndicator;
		}
		set {
			sEffectIndicator = value;
		}
	}

	public void initializeCharaController(Character char_controlled){
		/*initialize chara data*/
		chara_id_counter += 1;
		charaID = chara_id_counter;
		this.character_controlled = char_controlled;
		this.character_controlled.Controller = this;
		this.gameObject.name = char_controlled.profession;
		/*initialize chara data*/

		sEffectIndicator = GetComponent<StatusEffectIndicator>();//get status effect indicator manager
		chara_sprite = transform.GetChild(0).gameObject;
		chara_hp_bar = transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
		chara_mana_bar = transform.GetChild(1).GetChild(1).GetChild(0).gameObject;
		turnCursor = chara_sprite.transform.GetChild(0).gameObject;

		if(GameManager.BattleManager().withVisual){
			/*initialize chara sprite*/
			SpriteRenderer sRender = chara_sprite.GetComponent<SpriteRenderer>();
			sRender.sprite = GameManager.SpriteManager.getCharSprite(character_controlled.profession);
			if(charaID == 0 || charaID == 3)
				sRender.sortingOrder = 3;
			/*initialize chara sprite*/

			//initialize turn cursor
			toogleTurnCursor("",false);
		}
		else{
			chara_sprite.SetActive(false);
			chara_hp_bar.transform.parent.gameObject.SetActive(false);
			sEffectIndicator.enabled = false;
		}

		character_controlled.initializeSkills();//initialize chara skills
		character_controlled.createSimulationSelf(this,character_controlled.profession);
		//get AI module and initialize it
		ai_module = GetComponent<AI_Module>();
		ai_module.initializeAI();

		//setting act log
		actLog = new ActLog(GameManager.BattleManager().repeatNumber,character_controlled.profession,charaID);
	}

	//AI action
	List<string> acts = new List<string>();

	public virtual void doAction(){
		ActionLog log = GameManager.ActionLog;
		log.ActChar = character_controlled;

		if(GameManager.BattleManager().withVisual){
			toogleTurnCursor("turn",true);
		}

		Action action = ai_module.makeDecision();

		if(Action.isValidAction(action))
		{
			if (action.Target == null)
				return;
			//unleash action
			StartCoroutine(action.SkillSelected.unleash(action.Target,charateam));

			if(GameManager.BattleManager().withVisual){
				action.Target.Controller.toogleTurnCursor("target",true);
			}

			/*Update Action Log*/
			log.TargetChar = action.Target;
			log.ActionUsed = action.SkillSelected;	
			log.ActionStatus = action.SkillSelected.SkillActivationStatus;
			log.displayAction();
			/*Update Action Log*/

			//save log to temp act list.Wait till chara dead to save in log.
			acts.Add(action.SkillSelected.skillIdToStr());
		}
		else{
			Debug.Log("Cannot act. Decision error!");
		}

	}

	ActLog actLog;

	public ActLog ActLog {
		get {
			return this.actLog;
		}
	}

	public void addAgentActLog(){
		//save act list
		actLog.addRow(GameManager.BattleManager().currRepeatNumber,acts);
		acts.Clear();
	}

	public void onCharaDeath(){//this called when chara's hp reach zero or less
		isDead = true;
		this.gameObject.SetActive(false);
		charateam.requestMemberOut(this);
	}

	public void updateHPBar(){
		if(!GameManager.BattleManager().withVisual)
			return;
		float curr_fill_amount = character_controlled.Hp/(float)character_controlled.MaxHP;
		chara_hp_bar.GetComponent<Image>().fillAmount =  curr_fill_amount;
	}

	public void updateManaBar(){
		if(!GameManager.BattleManager().withVisual)
			return;
		float curr_fill_amount = character_controlled.Mana/(float)character_controlled.MaxMana;
		chara_mana_bar.GetComponent<Image>().fillAmount =  curr_fill_amount;
	}

	public void toogleTurnCursor(string type,bool displayed){
		if(!displayed)
			turnCursor.SetActive(false);
		else{
			if(type == "turn")
				turnCursor.GetComponent<SpriteRenderer>().color = Color.green;
			else if(type == "target")
				turnCursor.GetComponent<SpriteRenderer>().color = Color.red;
			turnCursor.SetActive(true);
		}
	}
		
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		this.hp = character_controlled.Hp;
		this.strength = character_controlled.Strength;
		this.def = character_controlled.Def;
		this.miss_chance = character_controlled.MissChance;
		this.mana = character_controlled.Mana;
		numberStatEffect = character_controlled.StatusEffectList.Count;
	}
}