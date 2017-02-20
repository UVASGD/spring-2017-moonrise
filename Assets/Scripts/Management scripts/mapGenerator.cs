//Abstract parent class for generators
using UnityEngine;

public abstract class mapGenerator : MonoBehaviour{
	public virtual int[,] init(){
		return null;
	}

}
