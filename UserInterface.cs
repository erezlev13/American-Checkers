using System;
using CheckersBoard;
using CheckerPiece;

namespace UI
{
   public class UserInterface
    {
        public static string GetValidUserName()
        {
            Console.WriteLine("Please enter player's name:");
            return getValidPlayerName();
        }

        private static string getValidPlayerName()
        {
            string playerName = Console.ReadLine();

            while (!IsValidUserName(playerName))
            {
                Console.WriteLine("Invalid Input please type your name again.");
                playerName = Console.ReadLine();
            }

            return playerName;
        }

            private static bool IsValidUserName(string i_NameOfPlayer)
            {
                bool isValidName = i_NameOfPlayer.Length <= 20 && i_NameOfPlayer.Length > 0;

                if (isValidName)
                {
                    for (int i = 0; i < i_NameOfPlayer.Length; i++)
                    {
                        isValidName = isValidName && char.IsLetter(i_NameOfPlayer[i]);
                    }
                }

                return isValidName;
            }

        public static ushort GetValidBoardSize()
        {
            Console.WriteLine("Please enter the size of the Board");
            return getValidBoardSize();
        }

        private static ushort getValidBoardSize()
        {
            ushort sizeOfBoard;
            while (!ushort.TryParse(Console.ReadLine(), out sizeOfBoard) || !CheckersBoard.Board.isValidBoardSize(sizeOfBoard))
            {
                Console.WriteLine("Invalid input please insert size again.");
            }

            return sizeOfBoard;
        }

        public static char GetRival()
        {
            Console.WriteLine(@"Please pick the game kind
1. Two players.
2. VS computer.");

            return getCompetitor();
        }

        private static char getCompetitor()
        {
            char choice = getValidchoice(Console.ReadLine());

            return choice;
        }

        private static char getValidchoice(string i_PlayerChoice)
        {
            char choice;
            while (!char.TryParse(i_PlayerChoice, out choice) || (choice != '1' && choice != '2'))
            {
                Console.WriteLine("invalid choice. please try again:");
                i_PlayerChoice = Console.ReadLine();
            }

            return choice;
        }

        public static bool WouldLikeToPlayAgain()
        {
            Console.WriteLine("To play Again press '1' otherwise press '2'.");
            char WouldLikeToPlayAgain = getValidchoice(Console.ReadLine());

            return WouldLikeToPlayAgain == '1';
        }

        public static string GetValidMove(Board i_Board)
        {
            Console.WriteLine("please enter Q if you like to quit otherwise insert a move");
            string move = Console.ReadLine();
            while (!IsValidMove(move,i_Board))
            {
                Console.WriteLine("Invalid Input");
                move = Console.ReadLine();
            }

            return move;
        }

        public static bool IsValidMove(string i_MoveToPreform, Board i_Board)
        {
            return i_MoveToPreform == "Q" || isLegalMove(i_MoveToPreform,i_Board);
        }
        public static bool isLegalMove(string i_MoveToPreform,Board i_Board)
        {
            return (isLegalMovePattern(i_MoveToPreform) && IsValidBoardMove(i_MoveToPreform, i_Board)); 

        }
        public static bool isLegalMovePattern(string i_MoveToPreform)
        {
                return i_MoveToPreform.Length == 5 && 
                char.IsUpper(i_MoveToPreform[0]) &&
                char.IsLower(i_MoveToPreform[1]) &&
                i_MoveToPreform[2] == '>' &&
                char.IsUpper(i_MoveToPreform[3]) &&
                char.IsLower(i_MoveToPreform[4]);
        }

       public static bool  IsValidBoardMove(string i_MoveToPreform, Board i_Board)
        {
            string location = string.Empty;
            string destination = string.Empty;
            // Validation.ParsePositions(i_MoveToPreform, ref location, ref destination);
            
            return checkIndexes(i_Board, location, destination);
        }

       public static bool checkIndexes(Board i_Board, string i_Location, string i_Destination)
        {
            ushort colIndex = (ushort) (i_Location[0] - 'A');
            ushort rowIndex = (ushort)(i_Location[1] - 'a');
            ushort destinationRowIndex = (ushort) (i_Destination[0] -'A');
            ushort destinationColIndex = (ushort)(i_Destination[1] - 'a');
            bool isValidIndexesMove = colIndex >= 0 && colIndex < i_Board.SizeOfBoard && rowIndex >= 0 && rowIndex < i_Board.SizeOfBoard
                && destinationRowIndex >= 0 && destinationRowIndex < i_Board.SizeOfBoard && destinationRowIndex >= 0 && destinationRowIndex < i_Board.SizeOfBoard;

            return isValidIndexesMove;
        }

        public static void PrintForfeitMessage(string i_playerName, string i_RivalPlayerName)
        {
            string forfeitMessage = string.Format("{0} has quit, {1} is the winner", i_playerName,i_RivalPlayerName);
            Console.WriteLine(forfeitMessage);
        }

        public static string  PlayerTurn( Board i_GameBoard, string i_PlayerName,CheckersPiece.ePieceKind pieceKind)
        {
            Console.Write(i_PlayerName + "'s turn");
            if(pieceKind == CheckerPiece.CheckersPiece.ePieceKind.MainPlayerTool)
            {
                Console.WriteLine("(O)");
            }
            else
            {
                Console.WriteLine("(X)");

            }
            string currentMove = UserInterface.GetValidMove(i_GameBoard);

            return currentMove;
        }
    }
}
