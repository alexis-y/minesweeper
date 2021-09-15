using System.Runtime.Serialization;

namespace Minesweeper.Model
{
    public enum GameResult
    {
        /// <summary>
        /// The player lost.
        /// </summary>
        [EnumMember(Value = "lose")]
        Lose,

        /// <summary>
        /// The player won.
        /// </summary>
        [EnumMember(Value = "win")]
        Win
    }
}
