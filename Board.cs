using System;
using System.Text;

namespace CheckersBoard
{
    public class Board
    {
        // Constants:
        private const char k_BlankChecker = ' ';
        private const int K_smallestBoardSize = 6;

        // Data members:
        private readonly char[,] m_CheckersBoard;
        private readonly ushort m_SizeOfBoard;
        
        // Enums:
        public enum eBoardSize
        {
            small = 6,
            medium = 8, 
            large = 10
        }

        private static string createLineString(ushort i_SizeOfBoard) // Creates the line that seperates between two checker's rows.
        {
            StringBuilder EqualsLine = new StringBuilder(" ========================");

            // Adding equal signs According to the size of the board.
            for (int i = K_smallestBoardSize; i < i_SizeOfBoard; i++)
            {
                EqualsLine.Append("====");
            }

            return EqualsLine.ToString();
        }

        private static void correctTopindex(ref StringBuilder checkerBoard, ushort i_SizeOfBoard) // prints top indexes of the board 
        {
            char letterIndex = 'G';

            // Adding Top Indexes According to the size of the board.
            for (int i = K_smallestBoardSize; i < i_SizeOfBoard; i++)
            {
                checkerBoard.AppendFormat("{0}   ", letterIndex);
                letterIndex++;
            }
        }

        public Board(ushort i_SizeOfBoard) // Constructor.
        {   
            m_CheckersBoard = new char[i_SizeOfBoard, i_SizeOfBoard];
            m_SizeOfBoard = i_SizeOfBoard;
            this.InitializeBoard();
        }

        // Properties
        public ushort SizeOfBoard
        {
            get
            {
                return m_SizeOfBoard;
            }
        }

        public char[,] CheckersBoard
        {
            get
            {
                return m_CheckersBoard;
            }
        }

        public void InitializeBoard() // Initialize start board.
        {
            makeEmptyBoard();
            placePlayersAtStartPoint();
        }

        private void makeEmptyBoard() // Creates an empty board.
        {
            for (int i = 0; i < m_SizeOfBoard; i++)
            {
                for (int j = 0; j < m_SizeOfBoard; j++)
                {
                    m_CheckersBoard[i, j] = k_BlankChecker;
                }
            }
        }

        private void placePlayersAtStartPoint() // Place Checkers pieces on an empty board.
        {
            ushort rowIndex = 0;
            while ((uint)m_SizeOfBoard - 1 - rowIndex != rowIndex + 1)
            {
                for (ushort colIndex = 0; colIndex < m_SizeOfBoard; colIndex++)
                {
                    placePlayerAccordingToToRowAndCol(rowIndex, colIndex);
                }

                rowIndex++;
            }
        }

        private void placePlayerAccordingToToRowAndCol(ushort i_Row, ushort i_Col) // Place Checker pieces According to row and col index
        {
            if (i_Row % 2 == 0)
            {
                if (i_Col % 2 != 0)
                {
                    m_CheckersBoard[i_Row, i_Col] = 'O';
                }
                else
                {
                    m_CheckersBoard[m_SizeOfBoard - 1 - i_Row, i_Col] = 'X';
                }
            }
            else
            {
                if (i_Col % 2 == 0)
                {
                    m_CheckersBoard[i_Row, i_Col] = 'O';
                }
                else
                {
                    m_CheckersBoard[m_SizeOfBoard - 1 - i_Row, i_Col] = 'X';
                }
            }
        }

        public void printBoard() // Prints game Board.
        {
            string lineString = createLineString(m_SizeOfBoard); // creates the string that seperates between lines.  
            StringBuilder CheckersBoard = new StringBuilder("   A   B   C   D   E   F   ");

            correctTopindex(ref CheckersBoard, m_SizeOfBoard); // Prints the top indexes for the Board.

            // Add to StringBuilder the board with it's content.
            addAllCheckersToBoard(ref CheckersBoard, ref lineString);

            Console.WriteLine(CheckersBoard);
        }

        private void addAllCheckersToBoard(ref StringBuilder CheckersBoard, ref string lineString) // Add lines and checker pieces to checker Board.
        {
            char leftIndex = 'a';
            for (int i = 0; i < m_SizeOfBoard; i++)
            {
                CheckersBoard.AppendFormat(
                    @"
{0}
{1}|",
lineString,
leftIndex);

                for (int j = 0; j < m_SizeOfBoard; j++)
                {
                    CheckersBoard.AppendFormat(" {0} |", m_CheckersBoard[i, j]); // Add to stringBuilder Checker with it's content
                }

                leftIndex++; // Increases the row index. 
            }
        }

        public bool IsCheckerAvailable(ushort i_RowIndex, ushort i_ColIndex) // Checks if a checker is possible to go to.
        {
            return IsCheckerValidPosition(i_RowIndex, i_ColIndex) && m_CheckersBoard[i_RowIndex, i_ColIndex] == k_BlankChecker;
        }

        public ushort GetIndexInBoard(ref string i_DestinationChecker, out ushort o_colIndex) // Gets an index according to the name of checker. 
        {
            o_colIndex = (ushort)(i_DestinationChecker[0] - 'A');
            return (ushort)(i_DestinationChecker[1] - 'a');
        }

        public bool IsCheckerValidPosition(ushort i_ColIndex, ushort i_RowIndex) // Checks if checker is in bound.
        {
            return (i_RowIndex < m_SizeOfBoard && i_RowIndex >= 0) && (i_ColIndex < m_SizeOfBoard && i_ColIndex >= 0);
        }

        // Update the board according to player move
        public void UpdateBoardAccordingToPlayersMove(ushort i_Row, ushort i_Col, ushort i_NewRow, ushort i_NewCol)
        {
            m_CheckersBoard[i_NewRow, i_NewCol] = m_CheckersBoard[i_Row, i_Col];
            m_CheckersBoard[i_Row, i_Col] = k_BlankChecker;
        }

        // Clears a checker of a checker piece that was eaten.
        public void DeleteCheckerPieceFromBoard(int i_Row, int i_Col)
        {
            m_CheckersBoard[i_Row, i_Col] = k_BlankChecker;
        }

        // Update the board after a checker piece eat another.
        public void UpdateAfterEating(ushort i_row, ushort i_Col, ushort i_newRow, ushort i_NewCol, ushort i_RowTokill, ushort i_ColTokill)
        {
            UpdateBoardAccordingToPlayersMove(i_row, i_Col, i_newRow, i_NewCol);
            DeleteCheckerPieceFromBoard(i_RowTokill, i_ColTokill);
        }
        
        public static bool isValidBoardSize( ushort i_SizeOfBoard)
        {
            return i_SizeOfBoard == 6 || i_SizeOfBoard == 8 || i_SizeOfBoard == 10;
        }
    }
}
