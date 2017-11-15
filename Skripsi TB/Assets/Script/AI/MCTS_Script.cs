using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCTS_Script: MonoBehaviour,AI_Module {

	AI_Type ai_type = AI_Type.MCTS;
	const int C = 2;
	public int LOOP_MCTS = 10;
	public int LOOP_SIMULATION = 20;//5 turn simulation

	Node root;
	Node current;

	CharaController controller;
	Team enemyTeam;
	Team mctsTeam;

	#region AI_Module interfacing
	public void initializeAI (){
		controller = GetComponent<CharaController> ();
	}
		
	public Action makeDecision (){
		Initialize ();
		Result ();//result proses MCTS dan nentukan skill aja.
		if(Action.isValidAction(current.actionSelected))
			current.actionSelected.Target = current.actionSelected.Target.Controller.Character_controlled;
		return current.actionSelected;//hasil return akan diekseskusi chara asli(bukan simulasi)
	}
		
	public AI_Type getAIModuleType (){
		return ai_type;
	}
	#endregion

	public void resetPoton(){
		if (root!=null) {
			root.child.Clear ();
			current = null;
			root = null;
		}
	}

	public void Result(){
		
		UnityEngine.Profiling.Profiler.BeginSample ("MCTSResult");
		for (int a=0; a<LOOP_MCTS; a++){
			UnityEngine.Profiling.Profiler.BeginSample ("Selection"); Selection(); UnityEngine.Profiling.Profiler.EndSample ();
			UnityEngine.Profiling.Profiler.BeginSample ("Simulation"); Simulation(); UnityEngine.Profiling.Profiler.EndSample ();
			UnityEngine.Profiling.Profiler.BeginSample ("Backpropagation");Backpropagation();UnityEngine.Profiling.Profiler.EndSample();
		}
		UnityEngine.Profiling.Profiler.EndSample ();
		Selection ();
	}

	#region MAIN FUNCTION MCTS
	void Initialize(){
		resetPoton();
		root = new Node(controller,null,null);
		current = root;
		enemyTeam = current.controller.CharaTeam.getEnemyTeam ();
		mctsTeam = current.controller.CharaTeam;
		/*synchronize all character simulation data with current data of actual character to prepare simulation*/
		synchronizeCharacterData();

		root.updateScore(root.score);
		root.setChild();
	}

	void Selection(){
		if (current.getUnVisittedChild() == null){			//TODO: UCB formula
			//current = Node with best score UCB in Selection;
			current = getBestUCB(current.child);
		
		} else{ 												//TODO: Exploration
			Exploration();
		}
	}

	void Exploration(){
		current = current.getUnVisittedChild();
	}

	void Simulation(){
		int numberChara = enemyTeam.Member.Count+mctsTeam.Member.Count; 	//TODO: 3 for Player and 3 for enemy
		int numberCurrent = 0;	//TODO: if numberCurrent is 0 mean, Chara 0 Do Action. then, if 1 mean Chara 1 Do Action etc
		int turnSimulationNumber = GameManager.BattleManager().turn_number - 1;//dimulai dari turn number Battle aslinya - 1
		Action selectedAction = null;

		for (int a=0; a<LOOP_SIMULATION; a++)
		{
			if (numberCurrent % (numberChara/2) == 0) {//new turn disini
				turnSimulationNumber += 1;
				#region check status effect
				//check status effect pada semua character enemy atau mcts simulasi tiap ganti turn. 
				/*Hint untuk check status effect pada satu character di enemy team:*/
				UnityEngine.Profiling.Profiler.BeginSample ("Simulation mcts status");
				foreach(CharaController chara in mctsTeam.Member){
					if (chara.isDead || chara.Character_controlled.simulChara.Hp <= 0)
						continue;
					List<StatusEffect> effects = chara.Character_controlled.simulChara.StatusEffectList;

					for(int i=0; i<effects.Count; i++){
						if(effects[i] != null) 
							effects[i].checkStatusEffect (turnSimulationNumber);
					}
				}
				UnityEngine.Profiling.Profiler.EndSample ();

				UnityEngine.Profiling.Profiler.BeginSample ("Simulation enemy status");
				foreach(CharaController chara in enemyTeam.Member){
					if (chara.isDead || chara.Character_controlled.simulChara.Hp <= 0)
						continue;
					List<StatusEffect> effects = chara.Character_controlled.simulChara.StatusEffectList;
					for(int i=0; i<effects.Count; i++){
						if(effects[i] != null) 
							effects[i].checkStatusEffect (turnSimulationNumber);
					}
				}
				UnityEngine.Profiling.Profiler.EndSample ();
				#endregion
			}

			int indexChara = numberCurrent % (numberChara / 2);
			if (turnSimulationNumber % 2 != 0) {
				
				//check if chara is MCTS team
				
				//check if controller is Dead or simulation character has died in previous simulation
				if (mctsTeam.Member [indexChara].isDead || mctsTeam.Member[indexChara].Character_controlled.simulChara.Hp <= 0 ||
					aliveNumber(enemyTeam.Member) <= 0)
					continue;
				
				Character chara = mctsTeam.Member[indexChara].Character_controlled.simulChara;//get simulation chara

				//jika turn pertama simulasi dan chara yg dapat turn adalah chara yg pake script poton ini.
				if (chara.Controller.charaID == controller.charaID && turnSimulationNumber == GameManager.BattleManager ().turn_number) {
					Skill skill = Skill.getSkillByName (chara.SkillList, current.actionSelected.SkillSelected.Skillname,0);
					Action action = new Action ();
					action.SkillSelected = skill;
					action.Target = current.actionSelected.Target;
					selectedAction = action;
				} else {
					//jika turn simulasi>2 atau bukan giliran controller 
					//gunakan aksi random
					List<Action> actList = ruleForMcts(chara);
					int randIndex = Random.Range (0, actList.Count);
					selectedAction = actList [randIndex];
				}
				if (selectedAction.SkillSelected.Mana_needed > chara.Mana) {
					selectedAction.SkillSelected = chara.SkillList [0];
					selectedAction.Target = getRandomTarget (enemyTeam);
				}
			
				doAction1(selectedAction,turnSimulationNumber);
			}

			else {
				//check if chara is enemy team
				
				//check if controller is Dead or simulation character has died in previous simulation
				if (enemyTeam.Member [indexChara].isDead || enemyTeam.Member[indexChara].Character_controlled.simulChara.Hp <= 0 || 
					aliveNumber(mctsTeam.Member) <= 0)
					continue;

				Character chara = enemyTeam.Member[indexChara].Character_controlled.simulChara;//get simulation chara
	
				List<Action> actList = ruleForMcts(chara);
				int randIndex = Random.Range (0, actList.Count);
				selectedAction = actList [randIndex];
				if (selectedAction.SkillSelected.Mana_needed > chara.Mana) {
					selectedAction.SkillSelected = chara.SkillList [0];
					selectedAction.Target = getRandomTarget (enemyTeam);
				}	
			}
			numberCurrent+=1;
		}

		int healthEnemy = totalHealth(enemyTeam.Member);
		int numberEnemy = aliveNumber(enemyTeam.Member);
		int healthMcts = totalHealth(mctsTeam.Member);
		int numberMcts = aliveNumber(mctsTeam.Member);

		//sinkronisasi semua karakter simulasi sesuai stat dan kondisi karakter aslinya
		synchronizeCharacterData();

		//final value of helath and count enemy after one simulation
		current.addScore(healthEnemy, numberEnemy, healthMcts, numberMcts);
	}
		
	void Backpropagation(){
		float score = current.getCalculationScore();
		Node point = current;

		while(point != root)
		{
			point.updateScore(score);
			point = point.parent;
		}
		current = point;
	}
	#endregion

	#region HELPER FUNCTION

	public static List<Action> ruleForMcts (Character chara){

		List<Action> action = new List<Action>();

		switch (chara.profession) {
		case Character.Healer:
			{
				if (minHealth (chara.Controller.CharaTeam) <= (float)(getMinHealth (chara.Controller.CharaTeam).MaxHP) * 0.25f) {
					Action tempAction = new Action ();
					tempAction.SkillSelected = chara.SkillList [3];
					tempAction.Target = getMinHealth (chara.Controller.CharaTeam);
					action.Add (tempAction);
					Action defAction = new Action ();
					defAction.SkillSelected = chara.SkillList [0];
					defAction.Target = getMinHealth (chara.Controller.CharaTeam.getEnemyTeam());
					action.Add (defAction);
				}
				else if (isAllyPoisoned (chara.Controller.CharaTeam)) {
					Action tempAction = new Action ();
					tempAction.SkillSelected = chara.SkillList [4];
					tempAction.Target = getAllyPoisoned (chara.Controller.CharaTeam);
					action.Add(tempAction);
					Action defAction = new Action ();
					defAction.SkillSelected = chara.SkillList [0];
					defAction.Target = getMinHealth (chara.Controller.CharaTeam.getEnemyTeam());
					action.Add (defAction);
				} else {
					Character target = getRandomTarget (chara.Controller.CharaTeam.getEnemyTeam());
					for(int i=0; i<3; i++){
						Action act = new Action ();
						act.SkillSelected = chara.SkillList [i];
						act.Target = target;
						action.Add (act);
					}
				}
				break;
			}
		case Character.Warrior:
			{
				if (minHealth (chara.Controller.CharaTeam) <= (float)(getMinHealth (chara.Controller.CharaTeam).MaxHP) * 0.4f) {
					Action tempAction = new Action ();
					tempAction.SkillSelected = chara.SkillList [4];
					tempAction.Target = getMinHealth (chara.Controller.CharaTeam);
					action.Add (tempAction);
					Action defAction = new Action ();
					defAction.SkillSelected = chara.SkillList [0];
					defAction.Target = getMinHealth (chara.Controller.CharaTeam.getEnemyTeam());
					action.Add (defAction);
				}
				if (isBlinded (chara)) {
					Action tempAction = new Action ();
					tempAction.SkillSelected = chara.SkillList [4];
					tempAction.Target = chara;
					action.Add (tempAction);
					Action defAction = new Action ();
					defAction.SkillSelected = chara.SkillList [0];
					defAction.Target = getMinHealth (chara.Controller.CharaTeam.getEnemyTeam());
					action.Add (defAction);
				} else {
					Character target = getRandomTarget (chara.Controller.CharaTeam.getEnemyTeam());
					for(int i=0; i<4; i++){
						Action act = new Action ();
						act.SkillSelected = chara.SkillList [i];
						act.Target = target;
						action.Add (act);
					}
				}
				break;
			}
		case Character.Wizard:
			{
				Character target = getRandomTarget (chara.Controller.CharaTeam.getEnemyTeam());
				for(int i=0; i<4; i++){
					Action act = new Action ();
					act.SkillSelected = chara.SkillList [i];
					act.Target = target;
					action.Add (act);
				}
			}
			break;
		}
		return action;
	}

	#region condition for MCTS Rule
	private static int minHealth (Team teamSopo){
		int min = int.MaxValue;
		foreach(CharaController chara in teamSopo.Member){
			
			if(chara.Character_controlled.simulChara.Hp > 0){
				if(chara.Character_controlled.simulChara.Hp < min)
					min = chara.Character_controlled.simulChara.Hp;
				}
		}
		return min;
	}

	private static Character getMinHealth (Team teamSopo){
		Character result = null;
		int min = int.MaxValue;
		foreach(CharaController chara in teamSopo.Member){

			if(chara.Character_controlled.simulChara.Hp > 0){
				if (chara.Character_controlled.simulChara.Hp < min) {
					min = chara.Character_controlled.simulChara.Hp;
					result = chara.Character_controlled.simulChara;
				}
			}
		}
		return result;
	}

	private static bool isAllyPoisoned(Team teamSopo){
		bool status = false;
		foreach(CharaController chara in teamSopo.Member){

			if(chara.Character_controlled.simulChara.Hp > 0){
				if (chara.Character_controlled.simulChara.OnPoisoned) {
					status = true;
				}
			}
		}
		return status;
	}

	private static Character getAllyPoisoned (Team teamSopo){
		Character result = null;
		foreach(CharaController chara in teamSopo.Member){

			if(chara.Character_controlled.simulChara.Hp > 0){
				if (chara.Character_controlled.simulChara.OnPoisoned) {
					result = chara.Character_controlled.simulChara;
					break;
				}
			}
		}
		return result;
	}

	private static bool isBlinded(Character chara){
		bool status = false;
		if (chara.OnBlinded)
			status = true;
		return status;
	}
	#endregion
		
	private void doAction1(Action action,int turn){
		Skill skill = action.SkillSelected;
		Character target = action.Target;

		if (skill is ClearSkill) {
			target.removeStatusEffect ();
		} else if (skill is HealSkill) {
			HealSkill healSkill = (HealSkill)skill;
			target.Hp += Mathf.RoundToInt(healSkill.HealPercentage/100f * target.MaxHP);
			if(target.Hp > target.MaxHP)
				target.Hp = target.MaxHP;
		} else {
			if (!(skill is BuffSkill)) {
				int missModifier = 1;
				if (skill.SkillMaster.MissChance > 0)
					missModifier = 0;
				target.Hp -= missModifier*skill.MaxDamage;
				if (target.Hp <= 0)
					target.Hp = 0;
			}
	
			if (skill is IStatusEffect) {
				IStatusEffect skillStatus = skill as IStatusEffect;
				skillStatus.addStatusEffect (target, skillStatus.getEffectType (), turn);
			}
		}
		skill.SkillMaster.Mana -= skill.Mana_needed;
	}

	public static Character getRandomTarget(Team team){
		Character charRandom = null;
		List<Character> temp = new List<Character>();
		//check simulchara that still alive and add them to temp list
		foreach(CharaController charaCont in team.Member){
			Character _simulChara = charaCont.Character_controlled.simulChara;
			if (_simulChara.Hp > 0)
				temp.Add(_simulChara);
		}
		if(temp.Count > 0){
			int index = Random.Range(0,temp.Count);
			charRandom = temp[index];
			temp.Clear(); temp = null; //clear object temp after used
		}
		return charRandom;
	}

	float getUCB(Node node){
		float pVisit = (float) node.parent.Visit;
		float cVisit = (float) node.Visit;

		float UCB = 0;
		float visit = Mathf.Sqrt (pVisit / cVisit);

		//write your UCB formula here by calculate node score and visit
		UCB = node.score + 2 * visit;

		return UCB;
	}

	public void synchronizeCharacterData(){
		foreach(CharaController _chara in enemyTeam.Member){
			_chara.Character_controlled.synchronizeSimulationChara();
		}
		foreach(CharaController _chara in mctsTeam.Member){
			_chara.Character_controlled.synchronizeSimulationChara();
		}
	}

	Node getBestUCB(List<Node> nodes){
		float bestUCBvalue = getUCB(nodes[0]);
		Node bestNode = nodes[0];

		for(int i=1; i<nodes.Count; i++){
			float temp = getUCB(nodes[i]);
			if(bestUCBvalue <= temp){
				bestUCBvalue = temp;
				bestNode = nodes[i];
			}
		}
		return bestNode;
	}

	int totalHealth(List<CharaController> controllers){
		int total = 0;
		foreach(CharaController _cont in controllers){
			total += _cont.Character_controlled.simulChara.Hp;
		}
		return total;
	}

	int aliveNumber(List<CharaController> controllers){
		int num = 0;
		foreach(CharaController _cont in controllers){
			if(_cont.Character_controlled.simulChara.Hp > 0)
				num += 1;
		}
		return num;
	}
	#endregion
}

class Node{
	public int MAX_ENEMY;
	public int MAX_HEALTH; 	//TODO: max health tiap enemy. sesuaikan
	public int MAX_MCTS_HEALTH;
	public int MAX_MCTS_NUMBER;

	public List<Node> child = new List<Node>();		//TODO: child is action enemy. eg: attac, buff, debuf
	public Node parent;
	public float score;

	float enemyTotalHealth,mctsHealth;
	float enemyNumber,mctsNumber;

	int visit = 0;
	public CharaController controller;
	public Action actionSelected;

	public int Visit {
		get {
			return this.visit;
		}
	}

	public Node(CharaController controller,Node parent,Action action){
		this.controller = controller;
		this.parent = parent;
		MAX_ENEMY = controller.CharaTeam.getEnemyTeam().Member.Count;
		MAX_HEALTH = controller.CharaTeam.getEnemyTeam().Max_team_hp;
		MAX_MCTS_HEALTH = controller.CharaTeam.Max_team_hp;
		MAX_MCTS_NUMBER = controller.CharaTeam.Member.Count;
		this.actionSelected = action;
	}

	public void setChild(){
		List<Action> actionList =  MCTS_Script.ruleForMcts(controller.Character_controlled);

		for (int a=0; a<actionList.Count; a++){
			if(actionList[a].SkillSelected.Mana_needed <= controller.Character_controlled.Mana)
				child.Add(new Node(controller,this,actionList[a]));
		}
	}

	public void updateScore(float _score){
		score += _score;
		visit++;
	}

	public void addScore(int curEnemyHealth,int enemyNumber,int mctsHealth,int mctsNumber){
		enemyTotalHealth = curEnemyHealth;
		this.enemyNumber = enemyNumber;
		this.mctsHealth = mctsHealth;
		this.mctsNumber = mctsNumber;
	}

	public float getCalculationScore(){//hitung node score berdsarkan 4 parameter state curEnemyHealth,enemyNumber,mctsHealth,mctsNumber

		return ((6f*(mctsHealth/(float)MAX_MCTS_HEALTH - enemyTotalHealth/(float)MAX_HEALTH)) + (4f*(mctsNumber/(float)MAX_MCTS_NUMBER - enemyNumber/(float)MAX_ENEMY))) / 10f;
	}

	public Node getUnVisittedChild(){
		foreach (Node c in child)
		{
			if (c.visit == 0)
				return c;
		}
		return null;
	}
}