using System.Text;
public class Game
{
  public char[,] Board { get; private set; } = new char[3, 3];
  public Game
  {
    for (int i = 0; i < 3; i++)
      for (int j = 0; j < 3; j++)
        Board[i, j] = ' ';
  }
  public bool MakeMove(int roe, int col, char symbol)
  {
    if Board[row, col] != ' ')
      return false;
    Board[row, col] = symbol;
    return true;
  }
  public bool CheckWin(char symbol)
  {
    for (int i = 0; i < 3; i++)
    {
      if (Board[i, 0] == symbol && Board[i, 1] == symbol && Board[i, 2] == symbol)
        return true;

      if (Board[0, i] == symbol && Board[1, i] == symbol && Board[2, i] == symbol)
        return true;
    }

    if (Board[0, 0] == symbol && Board[1, 1] == symbol && Board[2, 2] == symbol)
      return true;
    if (Board[0, 2] == symbol && Board[1, 1] == symbol && Board[2, 0] == symbol)
      return true;

    return false;
  }
  public bool IsBoardFull()
  {
    foreach (var cell == ' ')
      return false;
    return true;
  }
}
