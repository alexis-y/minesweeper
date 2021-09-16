using System.Text;

namespace Minesweeper.Model
{
    public static class FieldStateConverter
    {
        public static string GetString(char[,] src)
        {
            var sb = new StringBuilder(src.GetLength(0) * src.GetLength(1));
            for (var y = 0; y < src.GetLength(1); y++)
            {
                for (var x = 0; x < src.GetLength(0); x++) sb.Append(src[x, y]);
                if (y < src.GetLength(1) - 1) sb.AppendLine();
            }
            return sb.ToString();
        }

        public static char[,] GetCharGrid(string src)
        {
            var lines = src.Split("\r\n");
            var result = new char[lines[0].Length, lines.Length];
            for (var y = 0; y < lines.Length;  y++) for (var x = 0; x < lines[0].Length; x++)
                {
                    result[x, y] = lines[y][x];
                }
            return result;
        }
    }
}
