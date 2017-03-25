using UnityEngine;
using System.Collections;

public class GenerateMarket : mapGenerator {
	public GameObject n, b, x, i, g, a, r, c, v, h;
	public GameObject tallBuilding1,tallBuilding2;
	// Use this for initialization
	public override int[,] init () {
		int[,] board = ActualMarketGeneration.Start (n, b, x, i, g, a, r, c, v, h,tallBuilding1,tallBuilding2);
		tileMap = ActualMarketGeneration.tileMap;
		return board;
	}
}
