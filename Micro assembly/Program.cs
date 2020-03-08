// #define DEBUG
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Micro_assembly
{
    public class Program
    {
        static void Main(string[] args)
        {
            var currentStage = 8;
#if (!DEBUG)
            string[] inputs = Console.ReadLine().Split(' ');
            int n = int.Parse(Console.ReadLine());

#else
            string[] inputs = InputData.GetInput(currentStage).Split(' ');
            int n = InputData.GetStage(currentStage)().Count;
#endif
            Console.Error.WriteLine($"inputs : {string.Join(" ", inputs)}");
            var variable = new Variable { A = int.Parse(inputs[0]), B = int.Parse(inputs[1]), C = int.Parse(inputs[2]), D = int.Parse(inputs[3]) };

            var instructions = Enumerable.Range(0, n)
                .Select(i =>
                {
#if (!DEBUG)
                    return Console.ReadLine().ParseInstruction(variable);
#else
                    return InputData.GetStage(currentStage)()[i].ParseInstruction(variable);
#endif
                }).ToList();
            int cursor = 0;
            while(cursor < instructions.Count)
            {
                Console.Error.WriteLine($"Processing ({cursor}) : {instructions[cursor].InstructionText}");
                // Console.Error.WriteLine($"Before: {variable.ToString()}");
                cursor = instructions[cursor].Process(cursor);
                // Console.Error.WriteLine($"After: {variable.ToString()}");

            }

            Console.WriteLine(variable.ToString());
        }
    }

    public class Variable
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int D { get; set; }

        public override string ToString()
        {
            return $"{A} {B} {C} {D}";
        }
    }

    public enum Actions
    {
        MOV, ADD, SUB, JNE
    }

    [DebuggerDisplay("Instruction = {InstructionText}")]
    public abstract class BaseInstruction

    {
        public Actions Action { get; set; }
        public string InstructionText { get; set; }
        public string Ope1 { get; set; }
        public string Ope2 { get; set; }
        public string Ope3 { get; set; }

        public Variable variables { get; set; }

        public BaseInstruction(string[] parts, Variable variable)
        {
            this.Ope1 = parts[1];
            this.Ope2 = parts[2];
            if (parts.Length > 3)
                this.Ope3 = parts[3];

            variables = variable;

        }

        public abstract int Process(int cursor);

    }

    public class OperationInstruction
        : BaseInstruction
    {
        public OperationInstruction(string[] parts, Variable variable) : base(parts, variable)
        { }

        public override int Process(int cursor)
        {
            //ADD DEST SRC | IMM SRC | IMM
            //add two register or immediate values and store the sum in destination register
            //Example: ADD b c d => b = c + d
            bool successOp2 = int.TryParse(this.Ope2, out var ope2Val);
            int valToPush2 = successOp2 ? ope2Val : (int)variables.GetType().GetProperty(this.Ope2.ToUpper()).GetValue(variables);

            bool successOp3 = int.TryParse(this.Ope3, out var ope3Val);
            int valToPush3 = successOp3 ? ope3Val : (int)variables.GetType().GetProperty(this.Ope3.ToUpper()).GetValue(variables);

            var result = this.Action == Actions.ADD ? (valToPush2 + valToPush3) : (valToPush2 - valToPush3);

            variables.GetType().GetProperty(this.Ope1.ToUpper()).SetValue(variables, result);
            return ++cursor;
        }
    }

    public class MoveInstruction
    : BaseInstruction
    {
        public MoveInstruction(string[] parts, Variable variable) : base(parts, variable)
        {
            this.Action = Actions.MOV;
        }
        public override int Process(int cursor)
        {
            //MOV DEST SRC|IMM
            //Example: MOV a 3 => a = 3
            //Example: MOV OPE1 OPE2 => OPE1 = OPE2
            bool success = int.TryParse(this.Ope2, out var ope2Val);
            var valToPush = success ? ope2Val : variables.GetType().GetProperty(this.Ope2.ToUpper()).GetValue(variables);
            variables.GetType().GetProperty(this.Ope1.ToUpper()).SetValue(variables, valToPush);
            return ++cursor;
        }
    }

    public class JumpInstruction
        : BaseInstruction
    {
        public JumpInstruction(string[] parts, Variable variable) : base(parts, variable)
        {
            this.Action = Actions.JNE;
        }

        public override int Process(int cursor)
        {
            // JNE IMM SRC SRC| IMM
            // jumps to instruction number IMM(zero - based) if the other two values are not equal
            // Example: JNE 0 a 0 => continue execution at line 0 if a is not zero
            bool successOp3 = int.TryParse(this.Ope3, out var ope2Val);
            int compare = successOp3 ? ope2Val : (int)variables.GetType().GetProperty(this.Ope3.ToUpper()).GetValue(variables);
            var restartFrom = int.Parse(this.Ope1);
            if ((int)variables.GetType().GetProperty(this.Ope2.ToUpper()).GetValue(variables) != compare)
            {
                cursor = restartFrom;
            } else
            {
                cursor++;
            }
            return cursor;
        }
    }



    public static class StringHelper
    {
        public static BaseInstruction ParseInstruction(this string instruction, Variable variables)
        {
            var parts = instruction.Split(' ');
            Enum.TryParse<Actions>(parts[0], out var action);

            switch (action)
            {
                case Actions.MOV:
                    return new MoveInstruction(parts, variables) { InstructionText = instruction };
                case Actions.ADD:
                case Actions.SUB:
                    return new OperationInstruction(parts, variables) { InstructionText = instruction, Action = action };
                case Actions.JNE:
                    return new JumpInstruction(parts, variables) { InstructionText = instruction };
                default:
                    return null;
            }

        }
    }
}