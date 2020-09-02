using System.Collections.Generic;
using CheckerPiece;
using CheckersBoard;

namespace Player
{
    public struct CaptureUtils
    {
        public static bool CanUserCapture(Board i_GameBoard, User i_CurrentPlayer, User i_RivalPlayer,
                                          ref Dictionary<string, List<string>> io_CapturePositions)
        {
            bool canCapture = false;

            foreach (CheckersPiece checkerPiece in i_CurrentPlayer.Pieces)
            {
                if (i_CurrentPlayer.PlayerNumber == User.ePlayerType.MainPlayer)
                {
                    if (CanCaptureDown(i_GameBoard, checkerPiece, i_RivalPlayer.Pieces, ref io_CapturePositions))
                    {
                        canCapture = true;
                    }

                    if (checkerPiece.IsKing)
                    {
                        canCapture = canCapture || CanCaptureUp(i_GameBoard, checkerPiece, i_RivalPlayer.Pieces, ref io_CapturePositions);
                    }

                }
                else
                {
                    if (CanCaptureUp(i_GameBoard, checkerPiece, i_RivalPlayer.Pieces, ref io_CapturePositions))
                    {
                        canCapture = true;
                    }

                    if (checkerPiece.IsKing)
                    {
                        canCapture = canCapture || CanCaptureDown(i_GameBoard, checkerPiece, i_RivalPlayer.Pieces, ref io_CapturePositions);
                    }
                }
            }

            return canCapture;
        }

        public static bool CanCaptureUp(Board i_GameBoard, CheckersPiece i_Current, List<CheckersPiece> i_RivalCheckersPiece,
                                        ref Dictionary<string, List<string>> io_CapturePositions)
        {
            bool canCaptureLeft, canCaptureRight;
            ushort rowIndex, colIndex;
            // ushort newRowIndex, newColIndex;
            CheckersPiece rivalCheckerPieceUpRight, rivalCheckerPieceUpLeft;

            // Check if can capture up-right rival.
            rowIndex = (ushort)(i_Current.RowIndex - 1);
            colIndex = (ushort)(i_Current.ColIndex + 1);
            rivalCheckerPieceUpRight = FindCheckerPiece(rowIndex, colIndex, i_RivalCheckersPiece);
            canCaptureRight = TryInsertCapturePosition(
                i_GameBoard, i_Current,
                (ushort)(i_Current.RowIndex - 2), (ushort)(i_Current.ColIndex + 2),
                rivalCheckerPieceUpRight, ref io_CapturePositions);

            // Check if can capture up-left rival.
            rowIndex = (ushort)(i_Current.RowIndex - 1);
            colIndex = (ushort)(i_Current.ColIndex - 1);
            rivalCheckerPieceUpLeft = FindCheckerPiece(rowIndex, colIndex, i_RivalCheckersPiece);
            canCaptureLeft = TryInsertCapturePosition(
                i_GameBoard, i_Current,
                (ushort)(i_Current.RowIndex - 2), (ushort)(i_Current.ColIndex - 2),
                rivalCheckerPieceUpLeft, ref io_CapturePositions);

            return canCaptureRight || canCaptureLeft;
        }

        public static bool CanCaptureDown(Board i_GameBoard, CheckersPiece i_Current, List<CheckersPiece> i_RivalCheckersPiece,
                                          ref Dictionary<string, List<string>> io_CapturePositions)
        { 
            bool canCaptureRight, canCaptureLeft;
            ushort rowIndex, colIndex;
            // ushort newRowIndex, newColIndex;
            CheckersPiece rivalCheckerPieceDownRight, rivalCheckerPieceDownLeft;

            // Check if can capture down-right rival.
            rowIndex = (ushort)(i_Current.RowIndex + 1);
            colIndex = (ushort)(i_Current.ColIndex + 1);
            rivalCheckerPieceDownRight = FindCheckerPiece(rowIndex, colIndex, i_RivalCheckersPiece);
            canCaptureRight = TryInsertCapturePosition(
                i_GameBoard, i_Current,
                (ushort)(i_Current.RowIndex + 2), (ushort)(i_Current.ColIndex + 2),
                rivalCheckerPieceDownRight, ref io_CapturePositions);

            // Check if can capture down-left rival.
            rowIndex = (ushort)(i_Current.RowIndex + 1);
            colIndex = (ushort)(i_Current.ColIndex - 1);
            rivalCheckerPieceDownLeft = FindCheckerPiece(rowIndex, colIndex, i_RivalCheckersPiece);
            canCaptureLeft = TryInsertCapturePosition(
                i_GameBoard, i_Current,
                (ushort)(i_Current.RowIndex + 2), (ushort)(i_Current.ColIndex - 2),
                rivalCheckerPieceDownLeft, ref io_CapturePositions);

            return canCaptureLeft || canCaptureRight;
        }

        private static bool TryInsertCapturePosition(
            Board i_GameBoard, CheckersPiece i_CurrentChecker,
            ushort i_RowIndex, ushort i_ColIndex,
            CheckersPiece i_RivalChecker, ref Dictionary<string, List<string>> io_CapturePositions)
        {
            bool canInsert = false;

            if (i_RivalChecker != null && isAvailableCaptureCell(i_GameBoard, i_RowIndex, i_ColIndex))
            {
                string captureIndex = MoveUtils.GetStringIndexes(i_RowIndex, i_ColIndex);
                MoveUtils.addToDict(ref io_CapturePositions, i_CurrentChecker, captureIndex);
                canInsert = true;
            }

            return canInsert;
        }

        private static bool isAvailableCaptureCell(Board i_GameBoard, ushort i_RowIndex, ushort i_ColIndex)
        {
            return i_GameBoard.IsCheckerAvailable(i_RowIndex, i_ColIndex);
        }


        public static CheckersPiece FindCheckerPiece(ushort i_RowIndex, ushort i_ColIndex, List<CheckersPiece> i_RivalChckersPiece)
        {
            CheckersPiece currentCheckerPiece = null;

            foreach (CheckersPiece piece in i_RivalChckersPiece)
            {
                if (isSamePosition(piece, i_RowIndex, i_ColIndex))
                {
                    currentCheckerPiece = piece;
                    break;
                }
            }

            return currentCheckerPiece;
        }

        public static bool isSamePosition(CheckersPiece i_ChckerPiece, ushort i_RowIndex, ushort i_ColIndex)
        {
            return i_ChckerPiece.ColIndex == i_ColIndex && i_ChckerPiece.RowIndex == i_RowIndex;
        }

        public static void CaptureRivalCheckerPiece(Board i_GameBoard, ref CheckersPiece io_CurrentCheckerPiece, string i_PositionTo,
                                                    ref CheckersPiece io_RivalCheckerPiece)
        {
            ushort nextColIndex;
            ushort nextRowIndex = i_GameBoard.GetIndexInBoard(ref i_PositionTo, out nextColIndex);

            // Update board after eating, and move the current checker to his next place.
            i_GameBoard.UpdateAfterEating(io_CurrentCheckerPiece.RowIndex, io_CurrentCheckerPiece.ColIndex,
                nextRowIndex, nextColIndex,
                io_RivalCheckerPiece.RowIndex, io_RivalCheckerPiece.ColIndex);
            // Update current checker position.
            io_CurrentCheckerPiece.ChangePosition(nextRowIndex, nextColIndex);
            // Update rival's checker status (dead).
            io_RivalCheckerPiece.Die();
        }
    }
}