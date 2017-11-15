using UnityEngine;
using System.Collections;

public interface IHasTurn {
	IEnumerator OnTurn();//called when team has it's own turn
	IEnumerator OnTurnEnd();//called to notify class Battle that team has finish it's turn
}
