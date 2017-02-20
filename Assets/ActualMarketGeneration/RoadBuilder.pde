class RoadBuilder {
  int x;
  int y;
  int[][] bounds;
  int[] mov = new int[2];
  boolean vert = false;
  boolean hor = false;
  ArrayList<Character> avoidList = new ArrayList<Character>();
  
  RoadBuilder(int[][] b, char[] aList) {
    bounds = b;
    x = bounds[0][0];
    y = bounds[0][1];
    mov[0] = int(Math.signum((bounds[1][0] - bounds[0][0])));
    mov[1] = int(Math.signum((bounds[1][1] - bounds[0][1])));
    if (Math.abs(mov[0]) == 1) {
      vert = true;
      avoidList.add('v');
    }
    else if (Math.abs(mov[1]) == 1) {
      hor = true;
      avoidList.add('h');
    }
    for (char c : aList) {
      avoidList.add(c);
    }
    
    if (canMove(x, y)) {
      bigGrid[x][y] = 'r';
      x += mov[0];
      y += mov[1];
    }
    else { return; }
    
    while (dist(x, y, bounds[1][0], bounds[1][1]) > 0) {
      if (canMove(x, y)) {
        bigGrid[x][y] = 'r';
        placeBlocks();
        x += mov[0];
        y += mov[1];
      }
      else { 
        if (bigGrid[x][y] == 'i' || bigGrid[x][y] == 'c') {
          bigGrid[x][y] = 'c';
        }
        return; 
      }  
    }
    
    if (canMove(x, y)) {
      bigGrid[x][y] = 'r';
      x += mov[0];
      y += mov[1];
    }  
  }
  
  
  void placeBlocks() {
    if (vert) {
      if (bigGrid[x][y-1] == 'n' || bigGrid[x][y-1] == 'i') {
        bigGrid[x][y-1] = 'v';
      }
      else if (bigGrid[x][y-1] == 'h') {
        bigGrid[x][y-1] = 'x';
      }
      
      if (bigGrid[x][y+1] == 'n' || bigGrid[x][y+1] == 'i') {
        bigGrid[x][y+1] = 'v';
      }
      else if (bigGrid[x][y+1] == 'h') {
        bigGrid[x][y+1] = 'x';
      }
    }
    if (hor) {
      if (bigGrid[x-1][y] == 'n' || bigGrid[x-1][y] == 'i') {
        bigGrid[x-1][y] = 'h';
      }
      else if (bigGrid[x-1][y] == 'v') {
        bigGrid[x-1][y] = 'x';
      }
      
      if (bigGrid[x+1][y] == 'n' || bigGrid[x+1][y] == 'i') {
        bigGrid[x+1][y] = 'h';
      }
      else if (bigGrid[x+1][y] == 'v') {
        bigGrid[x+1][y] = 'x';
      }
    }
  }
  
  boolean canMove(int X, int Y) {
    if (avoidList.indexOf(bigGrid[X][Y]) != -1) {
      return false;
    }
    return true;
  }
  
}