using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;

namespace LYA1_Lexico3
{
    public class Lexico : Token, IDisposable
    {
        const int F = -1;
        const int E = -2;
        private StreamReader archivo;
        private StreamWriter log;

        int[,] TRAND =  
        {
        //  WS, L, D,  ., E(^),  +,  -,  =,  ;,  &,  |,  !,  >,  <,  %,  *,  ?,  ",  /,  {,  }, EOL, EOF, Lamda
            {0,  1, 2, 27,    1, 19, 20,  8, 10, 11, 12, 13, 16, 17, 22, 22, 24, 25, 28, 32, 33,  27,  27,   27},           //0
            {F,  1, 1,  F,    F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //1
            {F,  F, 2,  3,    5,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //2
            {E,  E, 4,  E,    E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,   E,   E,    E},           //3
            {F,  F, 4,  F,    5,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //4
            {E,  E, 7,  E,    E,  6,  6,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,   E,   E,    E},           //5
            {E,  E, 7,  E,    E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,   E,   E,    E},           //6
            {F,  F, 7,  F,    F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //7
            {F,  F, F,  F,    F,  F,  F,  9,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //8
            {F,  F, F,  F,    F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //9
            {F,  F, F,  F,    F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //10
            {F,  F, F,  F,    F,  F,  F,  F,  F, 14,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //11
            {F,  F, F,  F,    F,  F,  F,  F,  F,  F, 14,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //12
            {F,  F, F,  F,    F,  F,  F, 15,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //13
            {F,  F, F,  F,    F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //14
            {F,  F, F,  F,    F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //15
            {F,  F, F,  F,    F,  F,  F, 18,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //16
            {F,  F, F,  F,    F,  F,  F, 18,  F,  F,  F,  F, 18,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //17
            {F,  F, F,  F,    F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //18
            {F,  F, F,  F,    F, 21,  F, 21,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //19
            {F,  F, F,  F,    F,  F, 21, 21,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //20
            {F,  F, F,  F,    F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //21
            {F,  F, F,  F,    F,  F,  F, 23,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //22
            {F,  F, F,  F,    F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //23
            {F,  F, F,  F,    F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //24
            {25,25,25, 25,   25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 26, 25, 25, 25,  25,   E,   25},           //25
            {F,  F, F,  F,    F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //26
            {F,  F, F,  F,    F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //27
            {F,  F, F,  F,    F,  F,  F, 23,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, 29,  F,  F,   F,   F,    F},           //28
            {29,29,29, 29,   29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 30, 29, 29, 29, 29, 29,   0,  29,    F},           //29
            {30,30,30, 30,   30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 31, 30, 30, 30, 30, 30,  30,   E,   30},           //30
            {30,30,30, 30,   30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 31, 30, 30,  0, 30, 30,  30,   E,   30},           //31
            {F,  F, F,  F,    F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //32
            {F,  F, F,  F,    F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,   F,   F,    F},           //33
        };
        public Lexico()
        {
            archivo = new StreamReader("prueba.cpp");
            log = new StreamWriter("prueba.log");
            log.AutoFlush = true;
        }
        public Lexico(string nombre)
        {
            archivo = new StreamReader(nombre);
            log = new StreamWriter("prueba.log");
            log.AutoFlush = true;
        }
        public void Dispose()
        {
            archivo.Close();
            log.Close();
        }
        private int columna(char c)
        {
            if (char.IsWhiteSpace(c))
                return 0;
            else if (char.ToLower(c) == 'e')
                return 4;
            else if (char.IsLetter(c))
                return 1;
            else if (char.IsAsciiDigit(c))
                return 2;
            else if (c=='.')
                return 3;
            else if (c=='+')
                return 5;
            else if (c=='-')
                return 6;
            else
                return 7;
            
        }
        public void nextToken()
        {
            char c;
            string buffer = "";

            int estado = 0;

            while (estado >= 0)
            {
                c = (char)archivo.Peek();
                
                estado = TRAND[estado,columna(c)];

                if (estado >= 0)
                {
                    if (estado > 0)
                    {
                        buffer += c;    
                    }
                    archivo.Read();
                }
            }
            setContenido(buffer);
            log.WriteLine(getContenido() + " = " + getClasificacion());
        }
        public bool FinArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}