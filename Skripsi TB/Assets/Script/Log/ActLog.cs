using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ActLog {

	StreamWriter sWriter;
	string write_path,delim = "-",profession;
	int battleNumber;
	string[][] actHistory;

	public string[][] ActHistory {
		get {
			return this.actHistory;
		}
	}

	public ActLog(int battleNumber,string profession,int charID){
		this.battleNumber = battleNumber;
		this.profession = profession;
		write_path = "ActionLOG/"+profession+charID+"actions.txt";
		actHistory = new string[battleNumber][];
	}

	public void addRow (int rowIndex, List<string> actname)
	{
		int actNum = actname.Count;
		actHistory[rowIndex] = new string[actNum];
		//save rule id
		for(int i=0; i<actNum; i++){
			actHistory[rowIndex][i] = actname[i];
		}
		actname.Clear();
	}

	public void writeActLog ()
	{
		try{
			sWriter = new StreamWriter(write_path);
			string tempStr = "";

			for(int row=0; row<battleNumber; row++){
				if(actHistory[row] == null)
					continue;
				for(int col=0; col<actHistory[row].Length; col++){
					if(col == actHistory[row].Length-1)
						tempStr += (actHistory[row][col]+"");
					else
						tempStr += (actHistory[row][col]+delim);
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
