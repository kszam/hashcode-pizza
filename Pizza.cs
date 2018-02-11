using System;
using System.Collections.Generic;
using System.IO;

namespace Pizza_Slicing
{
    class Pizza
    {
        public int rows;
        public int columns;
        public int minIng;
        public int maxCPS;
        public string outputFileRoot = "";
        public int solutionNo;
        int score = 0;

        public char[,] pizzaBelag;
        public int[,] pizzaSlices;
        Stack<Slice> slices;

        int nextFreeRow=0;
        int nextFreeColumn = 0;

        public void findPieces()
        {
            // The big loop in the sky
            do
            {
                while(!addNewPiece())
                { // Neues Stück konnte nicht hinzugefügt werden
                    if (!findNextFreeCell())
                    { // nächste freie Zelle konnte nicht gefunden werden. Evaluiere Brett und suche dann weiter
                        scoreBoard();

                        while(!evolveLastPiece())
                        {
                            if (slices.Count == 0)
                            {
                                Console.WriteLine("=== No more solutions ==================");
                                return;
                            }
                        }
                    }
                }
            } while (true);
        }

        void scoreBoard()
        {
            int tempScore = 0;
            foreach (Slice s in slices)
            {
                tempScore = tempScore + s.rows * s.columns;
            }

             if (tempScore > score)
             {
                score = tempScore;
                Console.WriteLine("Solution number: " + ++solutionNo);
                //foreach (Slice s in slices)
                //{
                //    Console.WriteLine("(" + s.posRow + "," + s.posColumn + ") (" + (s.posRow + s.rows - 1) + "," + (s.columns + s.posColumn - 1) + ")");
                //}
                Console.WriteLine("Score: " + score);
                Console.WriteLine("========================================");
                writeSolutionToFile(outputFileRoot);
            }
            
        }

        public void readProblemFromFile(string filename)
        {
            slices = new Stack<Slice>();

            using (StreamReader reader = File.OpenText(filename))
            {
                {
                    nextFreeRow = 0;
                    nextFreeColumn = 0;
                    solutionNo=0;

                    char[] splitOnThese = {' '};

                    // Lese kopf
                    string readInputPizzaRow = reader.ReadLine();
                    string[] headerInf = readInputPizzaRow.Split(splitOnThese);
                    rows = int.Parse(headerInf[0]);
                    columns = int.Parse(headerInf[1]);
                    minIng = int.Parse(headerInf[2]);
                    maxCPS = int.Parse(headerInf[3]);

                    // Definiere Pizza-Größe
                    pizzaBelag = new char[rows, columns];
                    pizzaSlices = new int[rows, columns];

                    //lese Belag ein
                    for (int r = 0; r < rows; ++r)
                    {
                        readInputPizzaRow = reader.ReadLine();
                        for (int c = 0; c < columns; ++c)
                        {
                            pizzaBelag[r, c] = readInputPizzaRow.Substring(c, 1)[0];
                        }
                    }
                }
            }
        }

        public void writeSolutionToFile(string filename)
        {
            try
             {
                string filePath = filename + score.ToString();
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Anzahl Slices in Kopfzeile
                    writer.WriteLine(slices.Count.ToString());
                    foreach (Slice s in slices)
                    {
                        writer.WriteLine(s.posRow + " " + s.posColumn + " " + (s.posRow+s.rows - 1) + " " + (s.posColumn + s.columns - 1));
                    }
                }
            }
            catch (IOException e)
            {
               Console.WriteLine("A file error occurred: {0}", e.Message);
            }
        }

        bool findNextFreeCell()
        {
            int rr = nextFreeRow;
            int cc = nextFreeColumn;

            for (int r = rr; r < rows; ++r)
            {
                for (int c = cc; c < columns; ++c)
                {
                    if (rr != 0 || cc != 0)
                    {
                        rr = 0;
                        cc = 0;
                    }
                    else
                    if (pizzaSlices[r, c] == 0)
                    {
                        nextFreeRow = r;
                        nextFreeColumn = c;
                        return true;
                    }
                }
            }
            return false;
        }

        public void blockCells(Slice s){
            for (int r = 0; r < s.rows; ++r)
            {
                for (int c = 0; c < s.columns; ++c)
                {
                    pizzaSlices[s.posRow + r, s.posColumn + c] = 1;
                }

            }
        }

        public void unblockCells(Slice s)
        {
            for (int r = 0; r < s.rows; ++r)
            {
                for (int c = 0; c < s.columns; ++c)
                {
                    pizzaSlices[s.posRow + r, s.posColumn +  c] = 0;
                }
            }
        }

        bool pieceFitsOnBoard(Slice s) 
        {
            if (!(s.posRow + s.rows <= rows && s.posColumn + s.columns <= columns) || pieceCoversOtherPiece(s))
            {
                return false;  // Slice geht über die Pizza hinaus
            }
            else
            {
                return true;
            }
        }

        /*
         * true: other piece is covered
         * false: no other pieces covered
         */
        bool pieceCoversOtherPiece(Slice s)
        {
            for (int r = 0; r < s.rows; ++r)
            {
                for (int c = 0; c < s.columns; ++c)
                {
                    if (pizzaSlices[s.posRow + r, s.posColumn + c] == 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /*
         * true = new piece added
         * false = piece not added
         * 
         */
        public bool addNewPiece() 
        {
            Slice s = new Slice();
            s.posRow = nextFreeRow;
            s.posColumn = nextFreeColumn;

            for (int r = 1; r <= maxCPS; ++r)
            {
                for (int c = 1; c <= maxCPS/r; ++c)
                {
                    s.rows = r;
                    s.columns = c;
                    if (pieceFitsOnBoard(s))
                    {
                        if (evaluatePiece(s))
                        {
                            goto weiter;
                        }
                    }
                }
             }
            return false;

            weiter:  blockCells(s);
            findNextFreeCell();
           slices.Push(s);
           return true;
        }

        // true = ok, false = not ok
        bool evaluatePiece(Slice s)
        {
            // Slice s = slices.Peek();
            int m = 0;
            int t = 0;
            int totalCells = s.rows * s.columns;

            //fits on board?
            //if (!pieceFitsOnBoard(s))
            //{
            //    return false; // Slice geht über die Pizza hinaus
            //}

            for (int r = 0; r < s.rows; ++r)
            {
                for (int c = 0; c < s.columns; ++c)
                {
                    if (pizzaBelag[s.posRow + r, s.posColumn + c] == 'T')
                    {
                        t += 1;
                    }

                    if (pizzaBelag[s.posRow + r, s.posColumn + c] == 'M')
                    {
                        m += 1;
                    }
                }
            }

            if (t >= minIng && m >= minIng && totalCells <= maxCPS)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /*
         * When the last piece couldn't be evolved, it is taken from the board and I return false!
         * 
         */
        public bool evolveLastPiece()
        {
            Slice s = removeLastPiece();
            int rstart = s.rows;
            int cstart = s.columns;
            bool firstCall = true;

            for (int r = rstart; r <= maxCPS; ++r)
            {
                for (int c = cstart; c <= maxCPS / r; ++c)
                {
                    if (!firstCall)
                    {
                        s.rows = r;
                        s.columns = c;
                        if (pieceFitsOnBoard(s) && evaluatePiece(s))
                        {
                            goto weiter;
                        }
                    }
                    else
                    {   // Dieses ist nur um quasi in die ehemalige Generierungsschleife wieder hineinzuspringen
                        rstart = 1;
                        cstart = 1;
                        firstCall = false;
                    }
                }
            }
            return false;

        weiter: blockCells(s);

            nextFreeRow = s.posRow;
            nextFreeColumn = s.posColumn;

            findNextFreeCell();
            slices.Push(s);
            return true;
        }

        public Slice removeLastPiece()
        {
            Slice s = slices.Pop();
            // belegte Felder wieder freigeben
            unblockCells(s);
            return s;
        }
    }
}
