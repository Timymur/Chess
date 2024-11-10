using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    enum Figure
    {
        none,

        whiteKing = 'K',
        whiteQueen = 'Q',
        whiteKnight = 'N',
        whiteBishop = 'B',
        whiteRook = 'R',
        whitePawn = 'P',

        blackKing = 'k',
        blackQueen = 'q',
        blackKnight = 'n',
        blackBishop = 'b',
        blackRook = 'r',
        blackePawn = 'p',


    }

    static class FigureMethods
    {
        public static Color GetColor(this Figure figure)
        {
            if (figure == Figure.none)
                return Color.none;
            return (figure == Figure.whiteKing ||
                    figure == Figure.whiteQueen ||
                    figure == Figure.whiteKnight ||
                    figure == Figure.whiteBishop ||
                    figure == Figure.whiteRook ||
                    figure == Figure.whitePawn)
                ? Color.white
                : Color.black;
        }
    }

}
