int gridSizeX = 120;
int gridSizeY = 120;
int bigGridSizeX = 40;
int bigGridSizeY = 40;

char[][] grid = new char[gridSizeX][gridSizeY];
char[][] bigGrid = new char[bigGridSizeX][bigGridSizeY];

int gridTileSize = 5;
int bigGridTileSize = 15;
//x, y, xadd, yadd, xlim, ylim
int[][] zoneBounds = {{2, 2, 0, 10, 2, 30}, {2, 30, 10, 0, 30, 30}, {30, 30, 0, -10, 30, 2}, {30, 2, -10, 0, 2, 2}};

ArrayList<Market> marketList = new ArrayList<Market>();

Market cMarket;

ArrayList<int[]> roadSpaces = new ArrayList<int[]>();

void setup() {
  size(600, 600);
  background(255);
  
  for (int r = 0; r < gridSizeX; r ++) {
    for (int c = 0; c < gridSizeY; c++) {
      grid[r][c] = 'n';
      if (r % 3 == 0)
        bigGrid[r/3][c/3] = 'n';
    }
  }
  
  cMarket = new CenterMarket();
  
  for (int i = 0; i < zoneBounds.length; i++) {
    makeMarkets(2, i);
  }
  
  for (Market m : marketList) {
    m.buildRoads();
  }  
  
  //randomRoads(10);

  for (Market m : marketList) {
    m.checkCrosses();
  }

  for (int i = 0; i < bigGridSizeX; i++) {
    for (int j = 0; j < bigGridSizeY; j++) {
      if (bigGrid[i][j] == 'r') {
        roadSpaces.add(new int[] {i, j});
      }
      /*for (int[] r : roadSpaces) {
        int c = int(random(1, 50));
        if (c == 1) {
          
        }
      }*/
      fillSquares(i, j, bigGrid[i][j]);
    }
  }
  
  for (int r = 0; r < gridSizeX; r++) {
    for (int c = 0; c < gridSizeY; c++) {
      switch(grid[r][c]) {
        case 'n':
          noFill();
          break;
        case 'b':
          fill(50);
          break;
        case 'x':
          noFill();
          //fill(255, 0, 0);
          break;
        case 'i':
          noFill();
          //fill(150, 0, 150);
          break;
        case 'g':
          fill(255, 200, 0);
          break;
        case 'a':
          grid[r][c] = 'r';
        case 'r':
          fill(200);
          break;
        case 'c':
          fill(0, 150, 150);
          break;
        case 'v':
          noFill();
          //fill(0, 255, 0);
          break;
        case 'h':
          noFill();
          //fill(0, 0, 255);
          break;
      }
      rect(c*gridTileSize, r*gridTileSize, gridTileSize, gridTileSize);
    }
  }
}

void makeMarkets(int n, int j) {
  for (int i = 0; i < n; i++) {
    Market sMarket = new SideMarket(zoneBounds[j][0]+int(random(1,5)), zoneBounds[j][1]+int(random(1,5)), int(random(2, 5)),int(random(2, 5)));
    zoneBounds[j][0]+= zoneBounds[j][2];
    zoneBounds[j][1]+= zoneBounds[j][3];
  }
}

void randomRoads(int n) {
   for (int i = 0; i < n; i++) {
     //'i', 'x', 'b', 'g'
     int s = int(random(1, 5));
     int randomPos = int(random(5, 30));;
     if (s == 1) {
       RoadBuilder r = new RoadBuilder(new int[][] {{bigGridSizeX-1, randomPos}, {0, randomPos}}, new char[] {'i', 'x', 'b', 'g', 'c'});
     }
     else if (s == 2) {
       RoadBuilder r = new RoadBuilder(new int[][] {{randomPos, 0}, {randomPos, bigGridSizeY-1}}, new char[] {'i', 'x', 'b', 'g', 'c'});
     }
     else if (s == 3) {
       RoadBuilder r = new RoadBuilder(new int[][] {{0, randomPos}, {bigGridSizeX-1, randomPos}}, new char[] {'i', 'x', 'b', 'g', 'c'});
     }
     else if (s == 4) {
       RoadBuilder r = new RoadBuilder(new int[][] {{randomPos, bigGridSizeY-1}, {randomPos, 0}}, new char[] {'i', 'x', 'b', 'g', 'c'});
     }
   }
}

void fillSquares(int x, int y, char t) {
  for (int i = x*3; i < (x*3)+3; i++) {
    for (int j = y*3; j < (y*3)+3; j++) {
      if (grid[i][j] != 'a') grid[i][j] = t;
    }
  }
}