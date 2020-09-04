using System.Collections.Generic;
using GameBoard;
using CheckerPiece;

namespace Player
{
    public struct MoveUtils
    {
        // Constants:
        private const short k_RowIndex = 1;
        private const short k_ColIndex = 0;

        // Regular Move List:
        public static Dictionary<string, List<string>> CreateRegularMoves(User i_CurrentPlayer, Board i_GameBoard)
        {
            Dictionary<string, List<string>> optionsToMove = new Dictionary<string, List<string>>();

            foreach (CheckersPiece checkerPiece in i_CurrentPlayer.Pieces)
            {
                string checkerPiecePosition = GetStringIndexes(checkerPiece.RowIndex, checkerPiece.ColIndex);

                if (i_CurrentPlayer.PlayerNumber == User.ePlayerType.MainPlayer)
                {
                    getDownCellsPosition(ref optionsToMove, i_GameBoard, checkerPiece);
                    if (checkerPiece.IsKing)
                    {
                        getUpCellsPosition(ref optionsToMove, i_GameBoard, checkerPiece);
                    }
                }
                else
                {
                    getUpCellsPosition(ref optionsToMove, i_GameBoard, checkerPiece);
                    if (checkerPiece.IsKing)
                    {
                        getDownCellsPosition(ref optionsToMove, i_GameBoard, checkerPiece);
                    }
                }
            }

            return optionsToMove;
        }

        private static void getUpCellsPosition(ref Dictionary<string, List<string>> io_OptionsMove, Board i_GameBoard,
                                               CheckersPiece i_CurrentCheckerPiece)
        {
            ushort newRowIndex = (ushort)(i_CurrentCheckerPiece.RowIndex - 1);
            ushort newColRightIndex = (ushort)(i_CurrentCheckerPiece.ColIndex + 1);
            ushort newColLeftIndex = (ushort)(i_CurrentCheckerPiece.ColIndex - 1);
            string positionRightStr = GetStringIndexes(newRowIndex, newColRightIndex);
            string positionLeftStr = GetStringIndexes(newRowIndex, newColLeftIndex);

            if (isAvailableCellUpRightWay(i_GameBoard, i_CurrentCheckerPiece))
            {
                AddToDict(ref io_OptionsMove, i_CurrentCheckerPiece, positionRightStr);
            }

            if (isAvailableCellUpLeftWay(i_GameBoard, i_CurrentCheckerPiece))
            {
                AddToDict(ref io_OptionsMove, i_CurrentCheckerPiece, positionLeftStr);
            }
        }

        private static bool isAvailableCellUpRightWay(Board i_GameBoard, CheckersPiece i_CurrentCheckerPiece)
        {
            ushort newRowIndex = (ushort)(i_CurrentCheckerPiece.RowIndex - 1);
            ushort newColRightIndex = (ushort)(i_CurrentCheckerPiece.ColIndex + 1);

            return i_GameBoard.IsCheckerAvailable(newRowIndex, newColRightIndex);
        }

        private static bool isAvailableCellUpLeftWay(Board i_GameBoard, CheckersPiece i_CurrentCheckerPiece)
        {
            ushort newRowIndex = (ushort)(i_CurrentCheckerPiece.RowIndex - 1);
            ushort newColLeftIndex = (ushort)(i_CurrentCheckerPiece.ColIndex - 1);

            return i_GameBoard.IsCheckerAvailable(newRowIndex, newColLeftIndex);
        }

        private static void getDownCellsPosition(ref Dictionary<string, List<string>> io_OptionsMove, Board i_GameBoard, CheckersPiece i_CurrentCheckerPiece)
        {
            ushort newRowIndex = (ushort)(i_CurrentCheckerPiece.RowIndex + 1);
            ushort newColRightIndex = (ushort)(i_CurrentCheckerPiece.ColIndex + 1);
            ushort newColLeftIndex = (ushort)(i_CurrentCheckerPiece.ColIndex - 1);
            string positionRightStr = GetStringIndexes(newRowIndex, newColRightIndex);
            string positionLeftStr = GetStringIndexes(newRowIndex, newColLeftIndex);

            if (isAvailableCellDownRightWay(i_GameBoard, i_CurrentCheckerPiece))
            {
                AddToDict(ref io_OptionsMove, i_CurrentCheckerPiece, positionRightStr);
            }

            if (isAvailableCellDownLeftWay(i_GameBoard, i_CurrentCheckerPiece))
            {
                AddToDict(ref io_OptionsMove, i_CurrentCheckerPiece, positionLeftStr);
            }
        }

        private static bool isAvailableCellDownRightWay(Board i_GameBoard, CheckersPiece i_CurrentCheckerPiece)
        {
            ushort newRowIndex = (ushort)(i_CurrentCheckerPiece.RowIndex + 1);
            ushort newColRightIndex = (ushort)(i_CurrentCheckerPiece.ColIndex + 1);

            return i_GameBoard.IsCheckerAvailable(newRowIndex, newColRightIndex);
        }

        private static bool isAvailableCellDownLeftWay(Board i_GameBoard, CheckersPiece i_CurrentCheckerPiece)
        {
            ushort newRowIndex = (ushort)(i_CurrentCheckerPiece.RowIndex + 1);
            ushort newColLeftIndex = (ushort)(i_CurrentCheckerPiece.ColIndex - 1);

            return i_GameBoard.IsCheckerAvailable(newRowIndex, newColLeftIndex);
        }

        public static void AddToDict(ref Dictionary<string, List<string>> io_Options, CheckersPiece i_CurrentChecker, string i_OptionPosition)
        {
            string currentPosition = GetStringIndexes(i_CurrentChecker.RowIndex, i_CurrentChecker.ColIndex);
            if (!io_Options.ContainsKey(currentPosition))
            {
                io_Options.Add(currentPosition, new List<string>());
            }
            io_Options[currentPosition].Add(i_OptionPosition);
        }

        public static string GetStringIndexes(ushort i_RowIndex, ushort i_ColIndex)
        {
            char[] indexes = new char[2];
            indexes[k_RowIndex] = (char)(i_RowIndex + 'a');
            indexes[k_ColIndex] = (char)(i_ColIndex + 'A');

            return new string(indexes);
        }


        // Move Tools Methods:
        public static void MoveTool(ref Board io_GameBoard, ref CheckersPiece io_CurrentChecker, string i_PositionTo)
        {
            ushort nextRowIndex = (ushort)(i_PositionTo[k_RowIndex] - 'a');
            ushort nextColIndex = (ushort)(i_PositionTo[k_ColIndex] - 'A');

            // If there is not(!) a rival checker piece in the way.
            // Check valid move - include: inborder, valid input, is empty cell.
            // Validation.CheckValidMoveRegularTool(io_GameBoard, i_CurrentPlayer, io_CurrentChecker, ref i_PositionFrom, ref i_PositionTo);

            // Update board - new tool position.
            io_GameBoard.UpdateBoardAccordingToPlayersMove(
                io_CurrentChecker.RowIndex,
                io_CurrentChecker.ColIndex,
                nextRowIndex,
                nextColIndex);

            io_CurrentChecker.ChangePosition(nextRowIndex, nextColIndex);
        }
    }
}
