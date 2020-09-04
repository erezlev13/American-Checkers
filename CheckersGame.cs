using System;
using Player;
using GameBoard;
using CheckerPiece;
using UI;

namespace Game
{
    public class CheckersGame
    {
        public static void RunGame()
        {
            bool isComputer = false;
            string firstPlayerName = UserInterface.GetValidUserName();
            User firstPlayer = new User(firstPlayerName, User.ePlayerType.MainPlayer,isComputer);
            ushort boardSize = UserInterface.GetValidBoardSize();
            Board gameBoard = new Board(boardSize);
            char choice = UserInterface.GetRival();
            string rivalName = string.Empty;
           
            if (choice == '1')
            {
                rivalName = UserInterface.GetValidUserName();
            }
            else
            {
                isComputer = true;
                rivalName = "Computer";   
            }

            User rivalPlayer = new User(rivalName, User.ePlayerType.RivalPlayer, isComputer);
            playGame(firstPlayer, rivalPlayer, gameBoard);
        }

        private static void playGame(User i_FirstPlayer, User i_SecondPlayer, Board i_GameBoard)
        {
            i_GameBoard.InitializeBoard();
            i_FirstPlayer.InitializeCheckersArray(i_GameBoard.SizeOfBoard);
            i_SecondPlayer.InitializeCheckersArray(i_GameBoard.SizeOfBoard);
            bool isGameFinished = false;
            bool isFirstPlayerTurn = true;
            string currentMove = null;
            string currentLocation = null;
            string destinationPosition =string.Empty;

            while (!isGameFinished)
            {
                i_GameBoard.printBoard();
                if (isFirstPlayerTurn)
                {
                    currentMove = UserInterface.PlayerTurn(i_GameBoard, i_FirstPlayer.Name,CheckersPiece.ePieceKind.MainPlayerTool);
                    if (currentMove =="Q")
                    {
                        quitGame(i_FirstPlayer, i_SecondPlayer, ref isGameFinished); 
                    }
                    else
                    {
                        Validation.ParsePositions(currentMove, ref currentLocation, ref destinationPosition);
                        isGameFinished = i_FirstPlayer.MakeMove(ref i_GameBoard, i_SecondPlayer,currentLocation,destinationPosition);
                        if (i_FirstPlayer.HasAnotherTurn())
                        {
                            isFirstPlayerTurn = !isFirstPlayerTurn;
                        }
                    }
                }
                else
                {
                    currentMove = UserInterface.PlayerTurn(i_GameBoard, i_SecondPlayer.Name, CheckersPiece.ePieceKind.SecondPlayerTool);
                    Validation.ParsePositions(currentMove, ref currentLocation, ref destinationPosition);
                    if (currentMove == "Q")
                    {
                        quitGame(i_SecondPlayer, i_FirstPlayer, ref isGameFinished); 
                    }
                    else
                    {
                        isGameFinished = i_SecondPlayer.MakeMove(ref i_GameBoard, i_FirstPlayer, currentLocation, destinationPosition);
                        if (i_SecondPlayer.HasAnotherTurn())
                        {
                            isFirstPlayerTurn = !isFirstPlayerTurn;
                        }
                    }
                }
                isFirstPlayerTurn = !isFirstPlayerTurn;
            }

            printResult(i_FirstPlayer, i_SecondPlayer);
           if(UserInterface.WouldLikeToPlayAgain())
            { 
                playGame(i_FirstPlayer, i_SecondPlayer, i_GameBoard);
            }
        }

        private static void printResult(User i_firstPlayer, User i_AnotherPlayer)
        {
            string winner = i_AnotherPlayer.Name;
            int firstPlayerScore = i_firstPlayer.Score;
            int anotherPlayerScore = i_AnotherPlayer.Score;

            if(firstPlayerScore < anotherPlayerScore)
            {
                winner = i_AnotherPlayer.Name;
            }

            string ScoreMessage = string.Format(
                @"
{0}'s score is: {1} 
{2}'s score is: {3}
the winner is : {4}",
i_firstPlayer.Name,
i_firstPlayer.Score,
i_AnotherPlayer.Name,
i_AnotherPlayer.Score,
winner);
            Console.WriteLine(ScoreMessage);
        }

        private static void quitGame(User i_player,User i_RivalPlayer, ref bool isGameFinished)
        {
            i_player.Score = 0;
            i_RivalPlayer.Score = 0;
            UserInterface.PrintForfeitMessage(i_player.Name, i_RivalPlayer.Name);
            isGameFinished = true;
        }
    }
}
