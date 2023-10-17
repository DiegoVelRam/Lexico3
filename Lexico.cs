using System.Security.Principal;
using System.Security.Cryptography;
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
            else if (c=='=')
                return 7;
             else if (c==';')
                return 8;
            else if (c=='&')
                return 9;
            else if (c=='|')
                return 10;
            else if (c=='!')
                return 11;
            else if (c=='>')
                return 12;
            else if (c=='<')
                return 13;
            else if (c=='%')
                return 14;    
            else if (c=='*')
                return 15;
            else if (c=='?')
                return 16;
            else if (c=='"')
                return 17;
            else if (c=='/')
                return 18;
            else if (c=='{')
                return 19;
            else if (c=='}')
                return 20;
            else if (c=='\n')
                return 0;
            else if (FinArchivo())
                return 22;
            else
                return 27;
            
        }
   private void clasificar(int estado)
        {
            switch (estado)
            {
                case 1: setClasificacion(Tipos.Identificador); break;
                case 2: setClasificacion(Tipos.Numero); break;
                case 8: setClasificacion(Tipos.Asignacion); break;
                case 9: setClasificacion(Tipos.OperadorRelacional); break;
                case 10: setClasificacion(Tipos.FinSentencia);break;
                case 11: setClasificacion(Tipos.Caracter); break;
                case 12: setClasificacion(Tipos.Caracter); break;
                case 13: setClasificacion(Tipos.OperadorLogico); break;
                case 14: setClasificacion(Tipos.OperadorLogico); break;
                case 15: setClasificacion(Tipos.OperadorRelacional); break;
                case 16: setClasificacion(Tipos.OperadorRelacional); break;
                case 17: setClasificacion(Tipos.OperadorRelacional); break;
                case 19: setClasificacion(Tipos.OperadorTermino); break;
                case 20: setClasificacion(Tipos.OperadorTermino); break;
                case 21: setClasificacion(Tipos.IncrementoTermino); break;
                case 22: setClasificacion(Tipos.OperadorFactor); break;
                case 23: setClasificacion(Tipos.IncrementoFactor); break;
                case 24: setClasificacion(Tipos.OperadorTernario); break;
                case 25: setClasificacion(Tipos.Cadena); break;
                case 26: setClasificacion(Tipos.Cadena); break;
                case 28: setClasificacion(Tipos.OperadorFactor); break;
                case 29: setClasificacion(Tipos.Comentario); break;
                case 30: setClasificacion(Tipos.Comentario); break;
                case 31: setClasificacion(Tipos.Comentario); break;
                case 32: setClasificacion(Tipos.IniLlave); break;
                case 33: setClasificacion(Tipos.FinLlave); break;
                case 27: setClasificacion(Tipos.Caracter); break;
                
            }
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
                clasificar(estado);
                
                if (estado >= 0)
                {
                    if (estado > 0)
                    {
                        buffer += c;    
                    }
                    archivo.Read();
                }
            }
            if (estado == E)
            {
              if(getClasificacion() == Tipos.Numero)
                {
                    throw new Error("Lexico: Se espera un digito",log);
                }
            }
            else
            {
                throw new Error("Lexico: Error no se cerro cadena",log);
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