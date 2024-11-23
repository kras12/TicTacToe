using TicTacToe.Game.Enums;

namespace TicTacToe.Game;

/// <summary>
/// Represents a player in a Tic Tac Toe Game.
/// </summary>
public class Player
{
    #region Constructors

    public Player(string name, PlayerType playerType)
    {
        Name = name;
        PlayerType = playerType;
    }

    #endregion


    #region Properties

    /// <summary>
    /// The name of the player.
    /// </summary>
    public string Name { get; private init; } = "";


    /// <summary>
    /// The player type.
    /// </summary>
    public PlayerType PlayerType { get; private init; }

    #endregion
}
