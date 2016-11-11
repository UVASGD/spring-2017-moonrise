
class SideMarket extends Market {
  ArrayList<int[]> gates = new ArrayList<int[]>();
  
  SideMarket(int X, int Y, int SizeX, int SizeY) {
    super(X, Y, SizeX, SizeY);
    ArrayList<int[]> places = new ArrayList<int[]>();
    
    if (x > 15) {
      gates.add(new int[] {x, y+(sizeY/2), 1});
    }
    if (y < 25) {
      gates.add(new int[] {x+(sizeX/2), y+sizeY-1, 2});
    }
    if (x < 25) {
      gates.add(new int[] {x+sizeX-1, y+(sizeY/2), 3});
    }
    if (y > 15) {
      gates.add(new int[] {x+(sizeX/2), y, 4});
    }
    
    /*for (int i = 0; i < int(random(2, 5)); i++) {
      int[] place = places.get(int(random(places.size())));
      if (bigGrid[place[0]][place[1]] == 'b') {
        bigGrid[place[0]][place[1]] = 'g';
        gates.add(place);
      }
    }*/
  }
  
  void buildRoads() {
    for (int[] g : gates) {
      bigGrid[g[0]][g[1]] = 'g';
      switch (g[2]) {
        case 1: 
          RoadBuilder d1r1 = new RoadBuilder(new int[][] {{x-1, y+(sizeY/2)}, {x-3, y+(sizeY/2)}}, new char[] {'i', 'c'});
          RoadBuilder d1r2 = new RoadBuilder(new int[][] {{x-3, y+(sizeY/2)}, {0, y+(sizeY/2)}}, new char[] {'i', 'x', 'b', 'g', 'c'});
          break;
        case 2:
          RoadBuilder d2r1 = new RoadBuilder(new int[][] {{x+(sizeX/2), y+sizeY}, {x+(sizeX/2), y+sizeY+2}}, new char[] {'i', 'c'});
          RoadBuilder d2r2 = new RoadBuilder(new int[][] {{x+(sizeX/2), y+sizeY+2}, {x+(sizeX/2), bigGridSizeY-1}}, new char[] {'i', 'x', 'b', 'g', 'c'});
          break;
        case 3:
          RoadBuilder d3r1 = new RoadBuilder(new int[][] {{x+sizeX, y+(sizeY/2)}, {x+sizeX+2, y+(sizeY/2)}}, new char[] {'i', 'c'});
          RoadBuilder d3r2 = new RoadBuilder(new int[][] {{x+sizeX+2, y+(sizeY/2)}, {bigGridSizeX-1, y+(sizeY/2)}}, new char[] {'i', 'x', 'b', 'g', 'c'});
          break;
        case 4:
          RoadBuilder d4r1 = new RoadBuilder(new int[][] {{x+(sizeX/2), y-1}, {x+(sizeX/2), y-3}}, new char[] {'i', 'c'});
          RoadBuilder d4r2 = new RoadBuilder(new int[][] {{x+(sizeX/2), y-3}, {x+(sizeX/2), 0}}, new char[] {'i', 'x', 'b', 'g', 'c'});
          break;
      }
    }
  }

}