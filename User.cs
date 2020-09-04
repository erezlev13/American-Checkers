using System;
using System.Collections.Generic;
using CheckerPiece;
using GameBoard;

namespace Player
{
    public struct User
    {
        // Constants:
        private const short k_RowIndex = 1;
        private const short k_ColIndex = 0;

        // Data members:
        private readonly string m_Name;
        private ushort m_Score;
        private List<CheckersPiece> m_CheckersPiece;
        private CheckersPiece.ePieceKind m_CheckerPieceKind;
        private CheckersPiece m_CurrentCheckerPiece;
        private readonly ePlayerType m_PlayerNumber;
        private Dictionary<string, List<string>> m_Moves;
        private readonly bool m_IsComputer;

        // Enums:
        public enum ePlayerType
        {
            MainPlayer = 0,
            RivalPlayer = 1
        }


        // Constructors:
        public User(string i_Name, ePlayerType i_PlayerNumber, bool i_IsComputer)
        {
            m_Name = i_Name;
            m_Score = 0;
            m_CheckersPiece = null;
            m_PlayerNumber = i_PlayerNumber;
            m_CheckerPieceKind = i_PlayerNumber == ePlayerType.MainPlayer ?
                CheckersPiece.ePieceKind.MainPlayerTool : CheckersPiece.ePieceKind.SecondPlayerTool;
            m_Moves = null;
            m_CurrentCheckerPiece = null;
            m_IsComputer = i_IsComputer;
        }

        // Properties:
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public ushort Score
        {
            get
            {
                return m_Score;
            }
            set
            {
                m_Score = value;
            }
        }

        public List<CheckersPiece> Pieces
        {
            get
            {
                return m_CheckersPiece;
            }
        }

        public CheckersPiece.ePieceKind CheckerKind
        {
            get
            {
                return m_CheckerPieceKind;
            }
        }

        public ePlayerType PlayerNumber
        {
            get
            {
                return m_PlayerNumber;
            }
        }

        public Dictionary<string, List<string>> Moves
        {
            get
            {
                return m_Moves;
            }
        }

        public bool IsComputer
        {
            get
            {
                return m_IsComputer;
            }
        }


        // Methods:
        public void InitializeCheckersArray(ushort i_BoardSize)
        {
            int sizeOfPieces = ((i_BoardSize / 2) * (i_BoardSize / 2 - 1));

            m_CheckersPiece = new List<CheckersPiece>(sizeOfPieces);
            initializePositions(i_BoardSize);
        }

        private void initializePositions(ushort i_BoardSize) // Place Checkers pieces on an empty board.
        {
            int toolIndex = 0;
            bool reachLastIndex = false;

            for (ushort i = 0; i < i_BoardSize; i++)
            {
                for (ushort j = 0; j < i_BoardSize; j++)
                {
                    if (PlayerNumber == ePlayerType.MainPlayer)
                    {
                        setMainToolPosition(i, j, ref toolIndex);
                    }
                    else
                    {
                        setSecondToolPosition(i_BoardSize, i, j, ref toolIndex);
                    }

                    if (toolIndex == (i_BoardSize / 2) * (i_BoardSize / 2 - 1))
                    {
                        reachLastIndex = true;
                        break;
                    }
                }

                if (reachLastIndex)
                {
                    break;
                }
            }
        }

        private void setMainToolPosition(ushort i_Row, ushort i_Col, ref int io_ToolIndex) // Place Checker pieces According to row and col index
        {
            if (i_Row % 2 == 0)
            {
                if (i_Col % 2 != 0)
                {
                    m_CheckersPiece.Add(new CheckersPiece(CheckersPiece.ePieceKind.MainPlayerTool, i_Row, i_Col));
                    io_ToolIndex++;
                }
            }
            else
            {
                if (i_Col % 2 == 0)
                {
                    m_CheckersPiece.Add(new CheckersPiece(CheckersPiece.ePieceKind.MainPlayerTool, i_Row, i_Col));
                    io_ToolIndex++;
                }
            }
        }

        private void setSecondToolPosition(ushort i_BoardSize, ushort i_Row, ushort i_Col, ref int io_ToolIndex) // Place Checker pieces According to row and col index
        {
            ushort rowIndex = (ushort)(i_BoardSize - i_Row - 1);

            if (i_Row % 2 == 0)
            {
                if (i_Col % 2 == 0)
                {
                    m_CheckersPiece.Add(new CheckersPiece(CheckersPiece.ePieceKind.SecondPlayerTool, rowIndex, i_Col));
                    io_ToolIndex++;
                }
            }
            else
            {
                if (i_Col % 2 != 0)
                {
                    m_CheckersPiece.Add(new CheckersPiece(CheckersPiece.ePieceKind.SecondPlayerTool, rowIndex, i_Col));
                    io_ToolIndex++;
                }
            }
        }

        public void GetAndCalculateScore()
        {
            foreach (CheckersPiece checkersPiece in Pieces)
            {
                if (checkersPiece.IsKing)
                {
                    m_Score += 4;
                }
                else
                {
                    m_Score++;
                }
            }
        }

        public void printScore()
        {
            Console.Write(Score);
        }

        public void MakeToolAKing(Board i_GameBoard, ref CheckersPiece io_CurrentCheckerPiece)
        {
            io_CurrentCheckerPiece.GotToOtherSideOfBoard(ref i_GameBoard);
        }

        public bool MakeMove(ref Board io_GameBoard, User i_RivalPlayer, string i_PositionFrom, string i_PositionTo)
        {
            bool isGameOver = false;

            if (GetMovesAndUpdate(io_GameBoard, i_RivalPlayer))
            {
                playCapture(io_GameBoard, ref i_PositionFrom, ref i_PositionTo, i_RivalPlayer.Pieces);
                if (createCaptureMoveList(io_GameBoard,i_RivalPlayer).Count == 0)
                {
                    m_CurrentCheckerPiece = null; 
                }
            }
            else
            {
                // If the player still have moves to do.
                if (Moves.Keys.Count == 0 || m_CheckersPiece.Count == 0)
                {
                    isGameOver = true;
                }
                else
                {
                    playMove(io_GameBoard, ref i_PositionFrom, ref i_PositionTo);
                }
            }

            return isGameOver;
        }


        public bool GetMovesAndUpdate(Board i_GameBoard, User i_RivalPlayer)
        {
            bool canCapture = true;
            Dictionary<string, List<string>> moves = new Dictionary<string, List<string>>();

            // If the current checker didn't make a move the turn before.
            if (m_CurrentCheckerPiece == null)
            {
                // If the user can eat, he must! the list will return as ref value. If not, the list will return the regular moves list.
                if (!CaptureUtils.CanUserCapture(i_GameBoard, this, i_RivalPlayer, ref moves))
                {
                    m_Moves = MoveUtils.CreateRegularMoves(this, i_GameBoard);
                    canCapture = false;
                }
                else
                {
                    m_Moves = moves;
                }
            }
            // If there's a "soldier" that can capture again.
            else
            {
                // If there's capture in a row.
                m_Moves = createCaptureMoveList(i_GameBoard, i_RivalPlayer);
            }

            return canCapture;
        }

        // Certain Tool's Capture Move List:
        private Dictionary<string, List<string>> createCaptureMoveList(Board i_GameBoard, User i_RivalPlayer)
        {
            Dictionary<string, List<string>> captureList = new Dictionary<string, List<string>>();

            if (m_PlayerNumber == ePlayerType.MainPlayer)
            {
                CaptureUtils.CanCaptureDown(i_GameBoard, m_CurrentCheckerPiece, i_RivalPlayer.Pieces, ref captureList);
                if (m_CurrentCheckerPiece.IsKing)
                {
                    CaptureUtils.CanCaptureUp(i_GameBoard, m_CurrentCheckerPiece, i_RivalPlayer.Pieces, ref captureList);
                }
            }
            else
            {
                CaptureUtils.CanCaptureUp(i_GameBoard, m_CurrentCheckerPiece, i_RivalPlayer.Pieces, ref captureList);
                if (m_CurrentCheckerPiece.IsKing)
                {
                    CaptureUtils.CanCaptureDown(i_GameBoard, m_CurrentCheckerPiece, i_RivalPlayer.Pieces, ref captureList);
                }
            }

            return captureList;
        }

        private void playCapture(Board io_GameBoard, ref string io_PositionFrom, ref string io_PositionTo, List<CheckersPiece> i_RivalPlayerPieces)
        {
            CheckersPiece checkerPieceToMove = null;
            CheckersPiece rivalCheckerPiece = null;
            if (!IsComputer)
            {
                playerMustCapture(io_GameBoard, ref io_PositionFrom, ref io_PositionTo,
                    ref checkerPieceToMove, i_RivalPlayerPieces, ref rivalCheckerPiece);
            }
            else
            {
                computerMustCapture(io_GameBoard, ref io_PositionFrom, ref io_PositionTo,
                    ref checkerPieceToMove, i_RivalPlayerPieces, ref rivalCheckerPiece);
            }

            // Checks if there's an optional capture, and updating the data structure.
            MakeCapture(io_GameBoard, ref checkerPieceToMove, ref io_PositionTo, ref rivalCheckerPiece);
            checkerPieceToMove.GotToOtherSideOfBoard(ref io_GameBoard);
            // Remove checker piece from rival's soldiers.
            i_RivalPlayerPieces.Remove(rivalCheckerPiece);
        }

        private void playerMustCapture(Board i_GameBoard, ref string io_PositionFrom, ref string io_PositionTo,
            ref CheckersPiece io_CurrentChecker, List<CheckersPiece> i_RivalPieces, ref CheckersPiece io_RivalChecker)
        {
            while (!isValidCapture(i_GameBoard, io_PositionFrom, io_PositionTo, ref io_CurrentChecker, i_RivalPieces,
                ref io_RivalChecker))
            {
                Console.WriteLine("Invalid move. player must capture.");
                //string move = UserInterface.GetValidMove(i_GameBoard);
                //Validation.ParsePositions(move, ref io_PositionFrom, ref io_PositionTo);
                Validation.UserTurnConversation(Name, ref io_PositionFrom, ref io_PositionTo);
            }
        }

        public void MakeCapture(Board i_GameBoard, ref CheckersPiece io_CurrentCheckerPiece, ref string io_PositionTo,
            ref CheckersPiece io_RivalCheckerPiece)
        {
            CaptureUtils.CaptureRivalCheckerPiece(i_GameBoard, ref io_CurrentCheckerPiece,
                io_PositionTo, ref io_RivalCheckerPiece);
            m_CurrentCheckerPiece = io_CurrentCheckerPiece;
        }

        private bool isValidCapture(Board i_GameBoard, string io_PositionFrom, string io_PositionTo,
            ref CheckersPiece io_CurrentChecker, List<CheckersPiece> i_RivalPieces, ref CheckersPiece io_RivalChecker)
        {
            bool isValid = false;
            ushort rowIndex, colIndex;

            foreach (string positionFrom in Moves.Keys)
            {
                if (io_PositionFrom == positionFrom)
                {
                    if (isPositionInList(io_PositionTo, Moves[positionFrom]))
                    {
                        rowIndex = i_GameBoard.GetIndexInBoard(ref io_PositionFrom, out colIndex);
                        io_CurrentChecker = CaptureUtils.FindCheckerPiece(rowIndex, colIndex, Pieces);

                        colIndex += (ushort)(io_PositionTo[k_ColIndex] - io_PositionFrom[k_ColIndex]);
                        rowIndex += (ushort)(io_PositionTo[k_RowIndex] - io_PositionFrom[k_RowIndex]);
                        getRivalPosition(ref rowIndex, ref colIndex, io_PositionTo[k_RowIndex] < io_PositionFrom[k_RowIndex],
                            io_PositionTo[k_ColIndex] < io_PositionFrom[k_ColIndex]);
                        io_RivalChecker = CaptureUtils.FindCheckerPiece((ushort)rowIndex, (ushort)colIndex, i_RivalPieces);

                        isValid = true;
                        break;
                    }
                }
            }

            return isValid;
        }

        private void getRivalPosition(ref ushort io_RowIndex, ref ushort io_ColIndex, bool i_Up, bool i_Left)
        {
            changeRowIndex(ref io_RowIndex, i_Up);
            changeColIndex(ref io_ColIndex, i_Left);
        }

        private void changeRowIndex(ref ushort io_RowIndex, bool i_Up)
        {
            if (!i_Up)
            {
                io_RowIndex--;
            }
            else
            {
                io_RowIndex++;
            }
        }

        private void changeColIndex(ref ushort io_ColIndex, bool i_Left)
        {
            if (i_Left)
            {
                io_ColIndex++;
            }
            else
            {
                io_ColIndex--;
            }
        }

        private void playMove(Board io_GameBoard, ref string io_PositionFrom, ref string io_PositionTo)
        {
            CheckersPiece checkerPieceToMove = null;

            if (!IsComputer)
            {
                playerMustMoveValid(io_GameBoard, ref io_PositionFrom, ref io_PositionTo, ref checkerPieceToMove);
                MakeUserMove(ref io_GameBoard, ref checkerPieceToMove, io_PositionFrom, io_PositionTo);
            }
            else
            {
                MakeComputerMove(ref io_GameBoard, ref checkerPieceToMove);
            }
        }

        private void playerMustMoveValid(Board i_GameBoard, ref string io_PositionFrom, ref string io_PositionTo,
            ref CheckersPiece io_CurrentChecker)
        {
            while (!isValidMove(i_GameBoard, io_PositionFrom, io_PositionTo, ref io_CurrentChecker))
            {
                Console.WriteLine("Invalid move. player must move with to free positions.");
                //string move = UserInterface.GetValidMove(i_GameBoard);
                // Validation.ParsePositions(move, ref io_PositionFrom, ref io_PositionTo);
                Validation.UserTurnConversation(Name, ref io_PositionFrom, ref io_PositionTo);
            }
        }

        private bool isValidMove(Board i_GameBoard, string i_PositionFrom, string i_PositionTo, ref CheckersPiece io_CurrentChecker)
        {
            bool isValid = false;
            ushort rowIndex, colIndex;

            foreach (string positionFrom in Moves.Keys)
            {
                if (i_PositionFrom == positionFrom)
                {
                    if (isPositionInList(i_PositionTo, Moves[positionFrom]))
                    {
                        rowIndex = i_GameBoard.GetIndexInBoard(ref i_PositionFrom, out colIndex);
                        io_CurrentChecker = CaptureUtils.FindCheckerPiece(rowIndex, colIndex, Pieces);

                        isValid = true;
                        break;
                    }
                }
            }

            return isValid;
        }

        private bool isPositionInList(string i_PositionTo, List<string> i_Positions)
        {
            bool isFound = false;

            foreach (string positionsTo in i_Positions)
            {
                if (i_PositionTo == positionsTo)
                {
                    isFound = true;
                    break;
                }
            }

            return isFound;
        }

        public void MakeUserMove(ref Board io_GameBoard, ref CheckersPiece io_CurrentChecker, string i_PositionFrom, string i_PositionTo)
        {
            if (!io_CurrentChecker.IsKing)
            {
                MoveUtils.MoveTool(ref io_GameBoard, ref io_CurrentChecker, i_PositionTo);
                MakeToolAKing(io_GameBoard, ref io_CurrentChecker);
            }
            else
            {
                MoveUtils.MoveTool(ref io_GameBoard, ref io_CurrentChecker, i_PositionTo);
            }
        }


        // Computer Player Methods:
        public void MakeComputerMove(ref Board io_GameBoard, ref CheckersPiece io_CurrentChecker)
        {
            string[] positions = getRandomMove(io_GameBoard, ref io_CurrentChecker);

            if (!io_CurrentChecker.IsKing)
            {
                MoveUtils.MoveTool(ref io_GameBoard, ref io_CurrentChecker, positions[1]);
                MakeToolAKing(io_GameBoard, ref io_CurrentChecker);
            }
            else
            {
                MoveUtils.MoveTool(ref io_GameBoard, ref io_CurrentChecker, positions[1]);
            }
        }

        private string[] getRandomMove(Board i_GameBoard, ref CheckersPiece io_CurrentChecker)
        {
            int keyIndex = new Random().Next(Moves.Keys.Count);
            string[] positions = new string[2];

            while (!isCheckerFoundAndUpdate(i_GameBoard, keyIndex, out positions[0],
                out positions[1], ref io_CurrentChecker))
            {
                keyIndex = new Random().Next(Moves.Keys.Count);
            }

            return positions;
        }

        private bool isCheckerFoundAndUpdate(Board i_GameBoard, int i_KeyIndex,
            out string o_PositionFrom, out string o_PositionTo, ref CheckersPiece io_CheckersPiece)
        {
            int i = 0;
            bool isFound = false;
            o_PositionFrom = null;
            o_PositionTo = null;

            foreach (string checkerPosition in Moves.Keys)
            {
                if (i == i_KeyIndex)
                {
                    o_PositionFrom = checkerPosition;
                    int randomValue = new Random().Next(Moves[checkerPosition].Count);
                    o_PositionTo = Moves[checkerPosition][randomValue];
                    ushort colIndex;
                    ushort rowIndex = i_GameBoard.GetIndexInBoard(ref o_PositionFrom, out colIndex);
                    io_CheckersPiece = updateCurrentCheckerPiece(rowIndex, colIndex);
                    isFound = true;
                    break;
                }

                i++;
            }

            return isFound;
        }

        private CheckersPiece updateCurrentCheckerPiece(ushort i_RowIndex, ushort i_ColIndex)
        {
            CheckersPiece foundCheckerPiece = null;

            foreach (CheckersPiece checkerPiece in m_CheckersPiece)
            {
                if (CaptureUtils.IsSamePosition(checkerPiece, i_RowIndex, i_ColIndex))
                {
                    foundCheckerPiece = checkerPiece;
                    break;
                }
            }

            return foundCheckerPiece;
        }

        private void computerMustCapture(Board i_GameBoard, ref string io_PositionFrom, ref string io_PositionTo,
            ref CheckersPiece io_CurrentChecker, List<CheckersPiece> i_RivalPieces, ref CheckersPiece io_RivalChecker)
        {
            while (!isValidCapture(i_GameBoard, io_PositionFrom, io_PositionTo, ref io_CurrentChecker,
                i_RivalPieces,
                ref io_RivalChecker))
            {
                int randomKey = new Random().Next(Moves.Keys.Count);
                io_PositionFrom = getRandomKey(randomKey);
                int randomValue = new Random().Next(Moves[io_PositionFrom].Count);
                io_PositionTo = Moves[io_PositionFrom][randomValue];
            }
        }

        private string getRandomKey(int i_RandomIndex)
        {
            int i = 0;
            string getPositionFrom = null;

            foreach (string positionFrom in Moves.Keys)
            {
                if (i == i_RandomIndex)
                {
                    getPositionFrom = positionFrom;
                    break;
                }

                i++;
            }

            return getPositionFrom;
        }

        public bool HasAnotherTurn()
        {
            return m_CurrentCheckerPiece != null;
        }
    }
}
