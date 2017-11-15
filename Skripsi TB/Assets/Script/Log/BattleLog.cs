using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BattleLog{
	protected int totalBattle;
	int totalColumn = 3;//column: Battle-i | Dynamic Team Result Fitness
	int[][] datasetBattle;
	protected StreamWriter sWriter;

	public string write_path = "BattleLog.txt";

	public BattleLog(int totalBattle,int column){
		
		this.totalBattle = totalBattle;
		this.totalColumn = column;
		datasetBattle = new int[this.totalBattle][];
	}

	public virtual void addRow(int rowIndex,int[] rowData){ 
		datasetBattle[rowIndex] = new int[rowData.Length];

		for(int i=0; i<rowData.Length; i++){
			datasetBattle[rowIndex][i] = rowData[i];
		}
		rowData = null;
	}

	public virtual void writeBattleLog(){
		try{
			sWriter = new StreamWriter(write_path);
			string tempStr = "";

			for(int row=0; row<totalBattle; row++){
				for(int col=0; col<totalColumn; col++){
					if(col==totalColumn-1)
						tempStr += (datasetBattle[row][col]+"");
					else
						tempStr += (datasetBattle[row][col]+",");
				}
				sWriter.WriteLine(tempStr);
				tempStr = "";
			}
			sWriter.Flush();
			sWriter.Close();
		}
		catch(IOException e){
			Debug.Log(e.Message);
		}
	}
		

}
