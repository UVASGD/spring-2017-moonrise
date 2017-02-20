class CenterMarket extends Market {

  CenterMarket() {
    super((bigGridSizeX/2)-2, (bigGridSizeY/2)-4, 4, 8);
    bigGrid[x][y+(sizeY/2)-1] = 'g';
    bigGrid[x+(sizeX/2)-1][y+sizeY-1] = 'g';
    bigGrid[x+sizeX-1][y+(sizeY/2)] = 'g';
    bigGrid[x+(sizeX/2)][y] = 'g';
  }

  void buildRoads() {
    RoadBuilder d1r1 = new RoadBuilder(new int[][] {{x-1, y+(sizeY/2)-1}, {x-3, y+(sizeY/2)-1}}, new char[] {'i', 'c'});
    RoadBuilder d1r2 = new RoadBuilder(new int[][] {{x-3, y+(sizeY/2)-1}, {0, y+(sizeY/2)-1}}, new char[] {'i', 'x', 'b', 'g', 'c'});
    
    RoadBuilder d2r1 = new RoadBuilder(new int[][] {{x+(sizeX/2)-1, y+sizeY}, {x+(sizeX/2)-1, y+sizeY+2}}, new char[] {'i', 'c'});
    RoadBuilder d2r2 = new RoadBuilder(new int[][] {{x+(sizeX/2)-1, y+sizeY+2}, {x+(sizeX/2)-1, bigGridSizeY-1}}, new char[] {'i', 'x', 'b', 'g', 'c'});
    
    RoadBuilder d3r1 = new RoadBuilder(new int[][] {{x+sizeX, y+(sizeY/2)}, {x+sizeX+2, y+(sizeY/2)}}, new char[] {'i', 'c'});
    RoadBuilder d3r2 = new RoadBuilder(new int[][] {{x+sizeX+2, y+(sizeY/2)}, {bigGridSizeX-1, y+(sizeY/2)}}, new char[] {'i', 'x', 'b', 'g', 'c'});
    
    RoadBuilder d4r1 = new RoadBuilder(new int[][] {{x+(sizeX/2), y-1}, {x+(sizeX/2), y-3}}, new char[] {'i', 'c'});
    RoadBuilder d4r2 = new RoadBuilder(new int[][] {{x+(sizeX/2), y-3}, {x+(sizeX/2), 0}}, new char[] {'i', 'x', 'b', 'g', 'c'});
  }
}