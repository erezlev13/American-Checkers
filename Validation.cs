using System;
using CheckerPiece;
using CheckersBoard;

namespace Player
{
    public struct Validation
    {
        // Constants:
        private const short k_StartPositionTo = 0;
        private const short k_StartPositionFrom = 3;
        private const short k_SubStringLength = 2;
        private const short k_RowIndex = 1;
        private const short k_ColIndex = 0;
        private const char k_MoveUp = 'X';
        private const char k_MoveDown = 'O';
        private const char k_MoveUpKing = 'K';
        private const char k_MoveDownKing = 'U';

        // Methods:
        public static void CheckValidInput(string i_PlayerName, ref string io_PositionFrom, ref string io_PositionTo, int i_BoardSize)
        {
            string positionInput = null;
            ushort positionFromRow = (ushort) (io_PositionTo[k_RowIndex] - 'a');
            ushort positionFromCol = (ushort)(io_PositionTo[k_ColIndex] - 'A');
            ushort positionToRow = (ushort)(io_PositionTo[k_RowIndex] - 'a');
            ushort positionToCol = (ushort)(io_PositionTo[k_ColIndex] - 'A');

            // Checks if the given string's positions are valid. Means we got for exmaple 'Aa'. (A is column, a is row).
            while (!IsValidInput(i_BoardSize, io_PositionFrom) || !IsValidInput(i_BoardSize, io_PositionTo))
            {
                Console.WriteLine("Please enter valid indexes (for example: 'Aa').");
                UserTurnConversation(i_PlayerName, ref io_PositionFrom, ref io_PositionTo);
            }

            // Check if the given string's positions are in the border of the game board.
            while (!IsValidPosition(i_BoardSize, positionFromRow, positionFromCol) ||
                   !IsValidPosition(i_BoardSize, positionToRow, positionToCol))
            {
                Console.WriteLine("Please enter valid position.");
                UserTurnConversation(i_PlayerName, ref io_PositionFrom, ref io_PositionTo);
            }

            // Give the position params the right value, after validation check.
            ParsePositions(positionInput, ref io_PositionFrom, ref io_PositionTo);
        }

        public static bool IsValidInput(int i_BoardSize, string i_CharSequence)
        {
            // Check if the given input is A-H, a-h (for example).

            return isValidUpperCase(i_BoardSize, i_CharSequence[k_ColIndex]) &&
                   isValidLowerCase(i_BoardSize, i_CharSequence[k_RowIndex]);
        }

        private static bool isValidUpperCase(int i_BoardSize, char i_Char)
        {
            return i_Char >= 'A' && i_Char <= 'A' + i_BoardSize - 1;
        }

        private static bool isValidLowerCase(int i_BoardSize, char i_Char)
        {
            return i_Char >= 'a' && i_Char <= 'a' + i_BoardSize - 1;
        }

        public static bool IsValidPosition(int i_BoardSize, ushort i_RowIndex, ushort i_ColIndex)
        {
            return isValidIndex(i_BoardSize, i_RowIndex) && isValidIndex(i_BoardSize, i_ColIndex);
        }

        private static bool isValidIndex(int i_BoardSize, ushort i_Index)
        {
            return i_Index >= 0 && i_Index <= i_BoardSize - 1;
        }

        // Valid Move Methods:
        public static void CheckValidMoveRegularTool(Board i_GameBoard, User i_CurrentPlayer, CheckersPiece i_CurrentCheckerPiece,
                                                     ref string io_PositionFrom, ref string io_PositionTo)
        {
            while (!i_GameBoard.IsCheckerAvailable((ushort)(io_PositionTo[k_RowIndex] - 'a'),
                                                   (ushort)(io_PositionTo[k_ColIndex] - 'A')))
            {
                Console.WriteLine("The request position to move is not legal. Please enter position, which the cell is free to move.");
                UserTurnConversation(i_CurrentPlayer.Name, ref io_PositionFrom, ref io_PositionTo);
            }

            if (i_CurrentPlayer.CheckerKind == CheckersPiece.ePieceKind.SecondPlayerKing/*CheckersPiece.ePieceKind.O*/)
            {
                // The current player can move only down and in diagonal move.
                checkCorrectMoveWayDown(i_CurrentCheckerPiece, ref io_PositionFrom, ref io_PositionTo, i_CurrentPlayer.Name);
            }
            else
            // If the current player has tools from 'X' kind.
            {
                // The current player can move only up and in diagonal move.
                checkCorrectMoveWayUp(i_CurrentCheckerPiece, ref io_PositionFrom, ref io_PositionTo, i_CurrentPlayer.Name);
            }
        }

        private static void checkCorrectMoveWayDown(CheckersPiece i_CurrentCheckerPiece, ref string io_PositionFrom,
                                                    ref string io_PositionTo, string i_Name)
        {
            while (!isCorrectMoveDownWay(i_CurrentCheckerPiece, io_PositionTo))
            {
                Console.WriteLine("The request position to move is not legal. You can make only diagonal move.");
                UserTurnConversation(i_Name, ref io_PositionFrom, ref io_PositionTo);
            }
        }

        private static void checkCorrectMoveWayUp(CheckersPiece i_CurrentCheckerPiece, ref string io_PositionFrom,
                                                  ref string io_PositionTo, string i_Name)
        {
            while (!isCorrectMoveUpWay(i_CurrentCheckerPiece, io_PositionTo))
            {
                Console.WriteLine("The request position to move is not legal. You can make only diagonal move.");
                UserTurnConversation(i_Name, ref io_PositionFrom, ref io_PositionTo);
            }
        }

        private static bool isCorrectMoveUpWay(CheckersPiece i_CurrentCheckerPiece, string i_PositionTo)
        {
            return (i_CurrentCheckerPiece.ColIndex + 1 == i_PositionTo[k_ColIndex] - 'A' &&
                    i_CurrentCheckerPiece.RowIndex - 1 == i_PositionTo[k_RowIndex] - 'a') ||
                   (i_CurrentCheckerPiece.ColIndex - 1 == i_PositionTo[k_ColIndex] - 'A' &&
                    i_CurrentCheckerPiece.RowIndex - 1 == i_PositionTo[k_RowIndex] - 'a');
        }

        private static bool isCorrectMoveDownWay(CheckersPiece i_CurrentCheckerPiece, string i_PositionTo)
        {
            return (i_CurrentCheckerPiece.ColIndex + 1 == i_PositionTo[k_ColIndex] - 'A' &&
                    i_CurrentCheckerPiece.RowIndex + 1 == i_PositionTo[k_RowIndex] - 'a') ||
                   (i_CurrentCheckerPiece.ColIndex - 1 == i_PositionTo[k_ColIndex] - 'A' &&
                    i_CurrentCheckerPiece.RowIndex + 1 == i_PositionTo[k_RowIndex] - 'a');
        }

        public static void CheckValidMoveKingTool(Board i_GameBoard, User i_CurrentPlayer, CheckersPiece i_CurrentCheckerPiece,
                                                  ref string io_PositionFrom, ref string io_PositionTo)
        {
            while (!i_GameBoard.IsCheckerAvailable((ushort)(io_PositionTo[k_RowIndex] - 'a'),
                (ushort)(io_PositionTo[k_ColIndex] - 'A')))
            {
                Console.WriteLine("The request position to move is not legal. Please enter position, which the cell is free to move.");
                UserTurnConversation(i_CurrentPlayer.Name, ref io_PositionFrom, ref io_PositionTo);
            }

            // The current player can move only down and in diagonal move.
            checkCorrectMoveKingWay(i_CurrentCheckerPiece, ref io_PositionFrom, ref io_PositionTo, 
                                    i_GameBoard.SizeOfBoard, i_CurrentPlayer.Name);
            }

        private static void checkCorrectMoveKingWay(CheckersPiece i_CurrentCheckerPiece, ref string io_PositionFrom,
                                                    ref string io_PositionTo, ushort i_BoardSize, string i_Name)
        {
            while (!isCorrectMoveKingWay(i_CurrentCheckerPiece, i_BoardSize, io_PositionTo))
            {
                Console.WriteLine("The request position to move is not legal. You can make only diagonal move.");
                UserTurnConversation(i_Name, ref io_PositionFrom, ref io_PositionTo);
            }
        }

        private static bool isCorrectMoveKingWay(CheckersPiece i_CurrentCheckerPiece, ushort i_BoardSize, string i_PositionTo)
        {
            return isCorrectMoveKingDownWay(i_CurrentCheckerPiece, i_BoardSize, i_PositionTo) &&
                   isCorrectMoveKingUpWay(i_CurrentCheckerPiece, i_BoardSize, i_PositionTo);
        }

        private static bool isCorrectMoveKingUpWay(CheckersPiece i_CurrentCheckerPiece, ushort i_BoardSize, string i_PositionTo)
        {
            return ((i_CurrentCheckerPiece.ColIndex + 1 == i_PositionTo[k_ColIndex] - 'A' &&
                    i_CurrentCheckerPiece.RowIndex - 1 == i_PositionTo[k_RowIndex] - 'a')) ||
                   (i_CurrentCheckerPiece.ColIndex - 1 == i_PositionTo[k_ColIndex] - 'A' &&
                    i_CurrentCheckerPiece.RowIndex - 1 == i_PositionTo[k_RowIndex] - 'a');
        }

        private static bool isCorrectMoveKingDownWay(CheckersPiece i_CurrentCheckerPiece, ushort i_BoardSize, string i_PositionTo)
        {
            return (i_CurrentCheckerPiece.ColIndex + 1 == i_PositionTo[k_ColIndex] - 'A' &&
                    i_CurrentCheckerPiece.RowIndex + 1 == i_PositionTo[k_RowIndex] - 'a') ||
                   (i_CurrentCheckerPiece.ColIndex - 1 == i_PositionTo[k_ColIndex] - 'A' &&
                    i_CurrentCheckerPiece.RowIndex + 1 == i_PositionTo[k_RowIndex] - 'a');
        }

        public static void UserTurnConversation(string i_Name, ref string io_PositionFrom, ref string io_PositionTo)
        {
            string move = null;

            // Pass position from parameter, and change msg.
            Console.WriteLine("Please enter where you want to move.");
            Console.Write(i_Name + "'s turn: ");
            move = Console.ReadLine();
            ParsePositions(move, ref io_PositionFrom, ref io_PositionTo);
        }

        public static void ParsePositions(string i_StrInput, ref string io_PositionFrom, ref string io_PositionTo)
        {
            io_PositionTo = i_StrInput.Substring(k_StartPositionFrom, k_SubStringLength);
            io_PositionFrom = i_StrInput.Substring(k_StartPositionTo, k_SubStringLength);
        }
    }
}
