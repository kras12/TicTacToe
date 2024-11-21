using TicTacToe.Game.Enums;

namespace TicTacToe.Game;

public class Player
{
    public Player(string name, PlayerType playerType)
    {
        Name = name;
        PlayerType = playerType;
    }

    public string Name { get; private init; } = "";

    public PlayerType PlayerType { get; private init; }
}
