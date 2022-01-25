namespace FlagsWarGame
{

    public class Gamelogic
    {
        
        static int turn_count;
        private  int playerhealthleft;
        private int[] grid = new int[37];
        private int[] gridPoint = new int[37];
        public int[] GridPoint { get => gridPoint; set => gridPoint = value; }
        private int flags_count;
        private int points;
        public Gamelogic()
        {
            flags_count = 5;
            points = 0;
            playerhealthleft = 5;
            for (int i=0; i < grid.Length; i++)
            {
                Grid[i] = 0;
                gridPoint[i] = 0;
            }
        }
        public int[] Grid { get => grid; set => grid = value; }
        
        private static int checkPlayerTurn()
        {
            if (turn_count % 2 == 0)
                return 1;
            else
                return 2;
        }
        static void passTurn()
        {
            turn_count++;
        }
        public void decreaseFlagsCount()
        {
            flags_count--;
        }
        public void increaseFlagsCount()
        {
            flags_count++;
        }
        public int Flags_count { get => flags_count; set => flags_count = value; }
        public int Points { get => points; set => points = value; }
        public int Playerhealthleft { get => playerhealthleft; set => playerhealthleft = value; }
    }
}
