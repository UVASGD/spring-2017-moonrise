class Market {
  int x;
  int y;
  int sizeX;
  int sizeY;
  ArrayList<int[]> intersections = new ArrayList<int[]>();
  
  Market(int X, int Y, int SizeX, int SizeY) {
    x = X;
    y = Y;
    sizeX = SizeX;
    sizeY = SizeY;
    
    buildMarket();
    
    marketList.add(this);
  }
  
  void buildMarket() {
    Builder crossSection = new Builder(new int[][] {{(x-2), (y-2)}, {(x+sizeX+2), (y+sizeY+2)}}, 'i');
    Builder blockedSection = new Builder(new int[][] {{(x-1), (y-1)}, {(x+sizeX+1), (y+sizeY+1)}}, 'x');
    Builder base = new Builder(new int[][] {{x, y}, {(x+sizeX), (y+sizeY)}}, 'b');
  }
  
  void buildRoads() {}
  
  void checkCrosses() {
    for (int i = x-2; i < x+sizeX+2; i++) {
      for (int j = y-2; j < y+sizeY+2; j++) {
        if (bigGrid[i][j] == 'c') {
          intersections.add(new int[] {i, j});
        }
        //if (bigGrid[i][j] == 'i') {
          //bigGrid[i][j] = 'n';
        //}
      }
    }
    joinCrosses();
  }
  
  void joinCrosses() {
    for (int[] p : intersections) {
      boolean v = false;
      boolean h = false;
      if (p[0] == x-2 || p[0] == x+sizeX+1) {
        h = true;
      }
      if (p[1] == y-2 || p[1] == y+sizeY+1) {
        v = true;
      }
      if (h) {
        RoadBuilder r1 = new RoadBuilder(new int[][] {p, {p[0], y+sizeY+1}}, new char[] {'x', 'b', 'g'});
        RoadBuilder r2 = new RoadBuilder(new int[][] {p, {p[0], y-2}}, new char[] {'x', 'b', 'g'});  
        //RoadBuilder r3 = new RoadBuilder(new int[][] {{x-2,y-2}, {x+sizeX+1, y-2}}, new char[] {'x', 'b', 'g'});
        //RoadBuilder r4 = new RoadBuilder(new int[][] {{x-2, y+sizeY+1}, {x+sizeX+1, y+sizeY+1}}, new char[] {'x', 'b', 'g'});  

      }
      if (v) {
        //RoadBuilder r1 = new RoadBuilder(new int[][] {{x-2, y-2}, {x-2, y+sizeY+1}}, new char[] {'x', 'b', 'g'});
        //RoadBuilder r2 = new RoadBuilder(new int[][] {{x+sizeX+1, y-2}, {x+sizeX+1, y+sizeY+1}}, new char[] {'x', 'b', 'g'});  
        RoadBuilder r3 = new RoadBuilder(new int[][] {p, {x+sizeX+1, p[1]}}, new char[] {'x', 'b', 'g'});
        RoadBuilder r4 = new RoadBuilder(new int[][] {p, {x-2, p[1]}}, new char[] {'x', 'b', 'g'});        
      }
    }
  }
}