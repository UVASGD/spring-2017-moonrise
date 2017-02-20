class Builder {
  int[][] area;
  char type;
  Builder(int[][] a, char t) {
    area = a;
    type = t;
    
    for (int i = area[0][0]; i < area[1][0]; i++) {
      for (int j = area[0][1]; j < area[1][1]; j++) {
        bigGrid[i][j] = type;
      }
    }
  }

}