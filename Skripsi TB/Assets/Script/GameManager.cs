using System.Collections;
using UnityEngine;

public class GameManager {

	private static Battle battleManager = null;
	private static CharaSpriteManager spriteManager = null;
	private static UIManager uiManager = null;
	private static ActionLog actionLog = null;

	public static Battle BattleManager(){
		if(battleManager == null)
		   battleManager = GameObject.FindObjectOfType<Battle>();
		return battleManager;
	}

	public static CharaSpriteManager SpriteManager {
		get {
			if(spriteManager == null)
				spriteManager = GameObject.FindObjectOfType<CharaSpriteManager>();
			return spriteManager;
		}
	}

	public static UIManager UiManager {
		get {
			if(uiManager == null)
				uiManager = GameObject.FindObjectOfType<UIManager>();
			return uiManager;
		}
	}

	public static ActionLog ActionLog {
		get {
			if(actionLog == null)
				actionLog = new ActionLog();
			return actionLog;
		}
	}
}
