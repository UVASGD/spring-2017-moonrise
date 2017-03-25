//Abstract parent class for generators
using UnityEngine;

public abstract class mapGenerator : MonoBehaviour{
	[HideInInspector]
	public char[,] tileMap;
	public virtual int[,] init(){
		return null;
	}

}
