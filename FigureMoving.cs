using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class FigureMoving
    {
        public Figure figure {  get; private set; }
        public Square from { get; private set; }
        public Square to { get; private set; }

       

        public FigureMoving(FigureOnSquare fs, Square to)
        {
            this.figure = fs.figure;
            this.from = fs.square;
            this.to = to;
           
        }

        public FigureMoving(string move) // Pe2e4     
        {                                // 01234     
            this.figure = (Figure)move[0];  //Сразу меняем тип данных с чар на Figure
            this.from = new Square(move.Substring(1 , 2));
            this.to = new Square(move.Substring(3, 2));

        }

        public int DeltaX { get { return to.x - from.x; } }
        public int DeltaY { get { return to.y - from.y; } }

        public int AbsDeltaX { get { return Math.Abs(DeltaX); } }
        public int AbsDeltaY { get { return Math.Abs(DeltaY); } }

        public int SignX { get { return Math.Sign(DeltaX); } }
        public int SignY { get { return Math.Sign(DeltaY); } }

        public override string ToString()
        {
            string text = (char)figure + from.Name + to.Name;
            return text;
        }

    }
}
