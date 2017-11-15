using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectIndicator : MonoBehaviour {

	public GameObject iconEffectPrefab;
	public GameObject parentGO;
	public Vector3 initPosIcon;
	public float marginRight;
	private List<GameObject> currentActiveEffect = new List<GameObject>();

	// Use this for initialization
	void Start () {
//		iconEffectPrefab = Resources.Load<GameObject>("Prefab/sEffect");
//		Debug.Log(iconEffectPrefab.name);
	}

	public void updateStatusEffectIndicator(List<StatusEffect> effects){
		foreach(GameObject effIcon in currentActiveEffect){
			Destroy(effIcon);
		}
		currentActiveEffect.Clear();
		foreach(StatusEffect effect in effects){
			if(effect!=null){
				displayStatusIcon(effect.effectName);
			}
		}
	}
		
	private void displayStatusIcon(string effectType){
		
		if(currentActiveEffect.Exists(x => x.name == effectType)){//effect icon has displayed
			return;
		}
		Sprite effectSprite = GameManager.SpriteManager.getStatusEffectSprite(effectType);
		GameObject sEffectIcon = Instantiate(iconEffectPrefab);
		sEffectIcon.name = effectType;
		sEffectIcon.transform.SetParent(parentGO.transform,false);
		sEffectIcon.transform.localPosition = initPosIcon + new Vector3(marginRight,0,0) * (currentActiveEffect.Count);
		currentActiveEffect.Add(sEffectIcon);

		sEffectIcon.GetComponent<Image>().sprite = effectSprite;
	}



	// Update is called once per frame
	void Update () {
		
	}
}
