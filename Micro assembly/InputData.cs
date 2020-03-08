using System;
using System.Collections.Generic;
using System.Text;

namespace Micro_assembly
{
    public static class InputData
    {

        public static Func<List<string>> GetStage(int i)
        {
            if (i == 1)
                return Stage1;
            else if(i == 2)
                return Stage2;
            else if (i == 4)
                return Stage4;
            else if (i == 8)
                return Stage8;
            else
                return null;
        }

        public static string GetInput(int i)
        {
            if (i == 1)
                return "1 2 3 -4";
            else if (i == 2)
                return "2 3 4 5";
            else if (i == 4)
                return "3 5 7 9";
            else if (i == 8)
                return "0 7 5 3";
            else 
                return null;
            
        }

        public static List<string> Stage1()
        {
            var r = new List<string> { "MOV b 3", "MOV c a" };
            return r;
        }
        public static List<string> Stage2()
        {
            var r = new List<string> { "ADD a b 1", "ADD b 2 7", "ADD c a b" };
            return r;
        }

        public static List<string> Stage4()
        {
            var r = new List<string> { "SUB b b 1", "JNE 0 b 0" };
            return r;
        }

        public static List<string> Stage8()
        {
            var r = new List<string> { "ADD a a b", "SUB c c 1", "JNE 0 c 0", "MOV b a", "SUB c d 1", "ADD a a b", "SUB c c 1", "JNE 5 c 0", "SUB b 0 d", "JNE 11 a -105", "MOV a 0", "ADD d d b", "SUB b b b" };
            return r;
        }

            

    }
}
