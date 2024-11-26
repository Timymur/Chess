using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Board
    {
        public string fen { get; private set; }

        Figure[,] figures;

        public Color moveColor { get; private set; }

        public int moveNumber { get; private set; }

        public string castling {  get;  set; }

        public Board(string fen)
        {
            this.fen = fen;
            figures = new Figure[8, 8];
            Init();

        }

        void Init()
        {
            //"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
            //0                                            1 2    3 4 5  
            string[] parts = fen.Split();
            if (parts.Length != 6) return;
            InitFigures(parts[0]);
            moveColor = parts[1] == "b" ? Color.black : Color.white;
            castling = parts[2];
            moveNumber = int.Parse(parts[5]);


        }

        void InitFigures(string data)
        {
            for (int j = 8; j >= 2; j--)
                data = data.Replace(j.ToString(), (j - 1).ToString() + ".");

            data = data.Replace('1', '.');
            string[] lines = data.Split('/');
            for (int y = 7; y >= 0; y--)
                for (int x = 0; x < 8; x++)
                    figures[x, y] = lines[7 - y][x] == '.' ? Figure.none : (Figure)lines[7 - y][x];


        }

        public void GenerateFEN() // Собираем фен 
        {
            fen = FenFigures() + " " +  // Расстановка фигур
                  (moveColor == Color.white ? "w " : "b ") + // цвет фигуры, чей ход
                  castling + // рокировка
                   " - 0 " + moveNumber.ToString(); // опускаем взятие на проходе, 50 ходов ничьи 
        }

        string FenFigures()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 7; y >= 0; y--)
            {
                for (int x = 0; x < 8; x++)
                    sb.Append(figures[x, y] == Figure.none ? '1' : (char)figures[x, y]);

                if (y > 0)
                    sb.Append("/");
            }

            string eight = "11111111";
            for (int j = 8; j >= 2; j--)
                sb.Replace(eight.Substring(0, j), j.ToString()); // Переводим обратно к фену. 111 будет равно 3

            return sb.ToString();
        }

        public Figure GetFigureAt(Square square)
        {
            if (square.OnBoard())
            {
                return figures[square.x, square.y];

            }
            return Figure.none;
        }

        void SetFigureAt(Square square, Figure figure)
        {
            if (square.OnBoard())
            {
                figures[square.x, square.y] = figure;
            }
        }

        public Board Move(FigureMoving fm) // движение
        {
            
            Board next = new Board(fen); // создаем новую доску
            next.SetFigureAt(fm.from, Figure.none); // удаляем фигуру с предыдущего места
            next.SetFigureAt(fm.to, fm.promotion == Figure.none ? fm.figure : fm.promotion);// ставим ее на новую клетку/ Если нет превращения фигуры, а если есть то устанавливаем промоушн  в тот который есть

            if ((fm.figure == Figure.whiteKing || fm.figure == Figure.blackKing ) && (fm.AbsDeltaX == 2)) // Ход ладьей при рокировке
            {
                FigureMoving fmRook;
                fmRook = CastlingMove(fm);
                next.SetFigureAt(fmRook.from, Figure.none);
                next.SetFigureAt(fmRook.to, fmRook.promotion == Figure.none ? fmRook.figure : fmRook.promotion);
            }
                
                    
            if (moveColor == Color.black)
                next.moveNumber++;

            next.moveColor = moveColor.FlipColor();
           
            next.GenerateFEN();
            return next;
        }

        public IEnumerable<FigureOnSquare> YieldFigures()
        {
            foreach (Square square in Square.YieldSquares())
            {
                if (GetFigureAt(square).GetColor() == moveColor)
                    yield return new FigureOnSquare(GetFigureAt(square), square );
            }

        }

        public bool isCheck()
        {
            Board after = new Board(fen);
            after.moveColor = moveColor.FlipColor();
            return after.CanEatKing();
        }

        private bool CanEatKing()
        {
            Square badKing = FindBadKing();
            Moves moves = new Moves(this);
            foreach(FigureOnSquare fs in YieldFigures())
            {
                FigureMoving fm = new FigureMoving(fs, badKing);
                if(moves.CanMove(fm))
                    return true;
            }
            return false;
        }

        private Square FindBadKing()
        {
            Figure badKing = moveColor == Color.black ? Figure.whiteKing : Figure.blackKing;
            foreach(Square square in Square.YieldSquares())
                if(GetFigureAt(square) == badKing)
                    return square;
            return Square.none;
        }

        public bool IsCheckAfterMove(FigureMoving fm)
        {
            Board after = Move(fm);
            return after.CanEatKing();

        }


        public bool CanCastling( int signX)
        {
            string[] parts = fen.Split();

            Square whiteKnight = new Square("b1"); // Кони мешающие при длинной рокировке
            Square blackKnight = new Square("b8");

            if (moveColor == Color.white)
            {
                if (signX == 1)
                    if (parts[2].Contains("K"))
                        return true;

                if (signX == -1)
                    if (parts[2].Contains("Q"))
                        if(GetFigureAt(whiteKnight) == Figure.none)
                            return true;

            }
           
            if (moveColor == Color.black)
            {
                if (signX == 1)
                    if (parts[2].Contains("k"))
                        return true;

                if (signX == -1)
                    if (parts[2].Contains("q"))
                        if(GetFigureAt(whiteKnight) == Figure.none)
                            return true;
            }

            return false;
        }

        public string ChangeCastling(FigureMoving fm)
        {
            if (fm.figure == Figure.whiteKing)
            {
                castling = castling.Replace('K', '.');
                castling = castling.Replace('Q', '.');
            }

            if (fm.figure == Figure.blackKing)
            {
                castling = castling.Replace('k', '.');
                castling = castling.Replace('q', '.');
            }

            Square QWRook = new Square("a1");
            Square KWRook = new Square("h1");
            Square QBRook = new Square("a8");
            Square KBRook = new Square("h8");

            if (fm.figure == Figure.whiteRook)
            {
                if(fm.from == QWRook)
                    castling = castling.Replace('Q', '.');

                if (fm.from == KWRook)
                    castling = castling.Replace('K', '.');
            }

            if (fm.figure == Figure.blackRook)
            {
                if (fm.from == QBRook)
                    castling = castling.Replace('q', '.');

                if (fm.from == KBRook)
                    castling = castling.Replace('k', '.');
            }

            return castling;


        }

        FigureMoving CastlingMove(FigureMoving fm)
        {
            if (fm.figure == Figure.whiteKing)
            {
                if(fm.SignX == 1)               
                    return new FigureMoving("Rh1f1");
                
                if (fm.SignX == -1)               
                    return new FigureMoving("Ra1d1");
            }

            if (fm.figure == Figure.blackKing)
            {
                if (fm.SignX == 1)
                    return new FigureMoving("rh8f8");
                
                if (fm.SignX == -1)
                    return new FigureMoving("ra8d8");    
            }

            return null;
        }



    }
}
