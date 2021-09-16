using System;

namespace Minesweeper.Model
{
    /// <summary>
    /// Represents that the action taken is not valid in a game.
    /// </summary>
    [Serializable]
    public class IllegalActionException : Exception
    {
        public IllegalActionException() { }
        public IllegalActionException(string message) : base(message) { }
        public IllegalActionException(string message, Exception inner) : base(message, inner) { }
        protected IllegalActionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
