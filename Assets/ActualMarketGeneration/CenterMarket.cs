using UnityEngine;
using System.Collections;

public class CenterMarket : Market {

	public CenterMarket() : base((ActualMarketGeneration.bigGridSizeX/2)-2, (ActualMarketGeneration.bigGridSizeY/2)-4, 4, 8) {
		
		ActualMarketGeneration.bigGrid[x, y+(sizeY/2)-1] = 'g';
		ActualMarketGeneration.bigGrid[x+(sizeX/2)-1, y+sizeY-1] = 'g';
		ActualMarketGeneration.bigGrid[x+sizeX-1, y+(sizeY/2)] = 'g';
		ActualMarketGeneration.bigGrid[x+(sizeX/2), y] = 'g';
	}

	public override void buildRoads() {
		MonoBehaviour.print ("ITS ABOUT TO BUILD ROADS");
		RoadBuilder d1r1 = new RoadBuilder(new int[,] {{x-1, y+(sizeY/2)-1}, {x-3, y+(sizeY/2)-1}}, new char[] {'i', 'c'});
		MonoBehaviour.print ("d1r1 " + d1r1.mov[0] + ", " + d1r1.mov[1]);
		RoadBuilder d1r2 = new RoadBuilder(new int[,] {{x-3, y+(sizeY/2)-1}, {0, y+(sizeY/2)-1}}, new char[] {'i', 'x', 'b', 'g', 'c'});
		MonoBehaviour.print ("d1r2");
		RoadBuilder d2r1 = new RoadBuilder(new int[,] {{x+(sizeX/2)-1, y+sizeY}, {x+(sizeX/2)-1, y+sizeY+2}}, new char[] {'i', 'c'});
		MonoBehaviour.print ("d2r1");
		RoadBuilder d2r2 = new RoadBuilder(new int[,] {{x+(sizeX/2)-1, y+sizeY+2}, {x+(sizeX/2)-1, ActualMarketGeneration.bigGridSizeY-1}}, new char[] {'i', 'x', 'b', 'g', 'c'});
		MonoBehaviour.print ("d2r2");
		RoadBuilder d3r1 = new RoadBuilder(new int[,] {{x+sizeX, y+(sizeY/2)}, {x+sizeX+2, y+(sizeY/2)}}, new char[] {'i', 'c'});
		MonoBehaviour.print ("d3r1");
		RoadBuilder d3r2 = new RoadBuilder(new int[,] {{x+sizeX+2, y+(sizeY/2)}, {ActualMarketGeneration.bigGridSizeX-1, y+(sizeY/2)}}, new char[] {'i', 'x', 'b', 'g', 'c'});
		MonoBehaviour.print ("d3r3");
		RoadBuilder d4r1 = new RoadBuilder(new int[,] {{x+(sizeX/2), y-1}, {x+(sizeX/2), y-3}}, new char[] {'i', 'c'});
		MonoBehaviour.print ("d4r1");
		RoadBuilder d4r2 = new RoadBuilder(new int[,] {{x+(sizeX/2), y-3}, {x+(sizeX/2), 0}}, new char[] {'i', 'x', 'b', 'g', 'c'});
		MonoBehaviour.print ("d4r1");
	}
}