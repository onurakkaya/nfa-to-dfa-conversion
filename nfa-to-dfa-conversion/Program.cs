using nfa_to_dfa_conversion.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace nfa_to_dfa_conversion
{
    public static class Program
    {
        public static Stopwatch Timer = new Stopwatch();
        public static List<string> BenchmarkResults = new List<string>();
        private static void Main(string[] args)
        {
            string inputString = "10001";
            ConsoleOperations.WriteTitle("Input String");
            Console.WriteLine($">> {inputString}");

            NFAFunction(inputString);

            TWDFAFunction(inputString);
        }

        private static void NFAFunction(string inputString)
        {
            Timer.Start();
            FiniteAutomata automata = NFABuilder();
            ConsoleOperations.WriteBMarkReset("NFA Creation");

            ConsoleOperations.WriteTitle("NFA Info");
            ConsoleOperations.WriteAutomataInfo(automata);

            ConsoleOperations.WriteTitle("NFA Trace");
            Timer.Restart();
            bool result = automata.Run(inputString);
            ConsoleOperations.WriteBMarkReset("NFA Run");

            Timer.Restart();
            FiniteAutomataConverter dfaConverter = new FiniteAutomataConverter();
            FiniteAutomata DFAVersion = dfaConverter.ConvertNFAToDFA(automata);
            ConsoleOperations.WriteBMarkReset("Automata Conversion");

            Console.WriteLine("\n>>>NFA is converted to DFA<<<\n");
            ConsoleOperations.WriteTitle("DFA Info");
            ConsoleOperations.WriteAutomataInfo(DFAVersion);

            ConsoleOperations.WriteTitle("DFA Trace");
            Timer.Restart();
            bool resultDFA = DFAVersion.Run(inputString);
            ConsoleOperations.WriteBMarkReset("DFA Run");
            Timer.Stop();

            ConsoleOperations.WriteTitle("Automata Results");
            Console.WriteLine("NFA Response: Input is " + (result ? "Accepted" : "Rejected"));
            Console.WriteLine("DFA Response: Input is " + (resultDFA ? "Accepted" : "Rejected"));

            ConsoleOperations.WriteTitle("Benchmark Results");
            BenchmarkResults.ForEach(x => Console.WriteLine(x));
        }
        private static void TWDFAFunction(string inputString)
        {
            Timer.Start();
            FiniteAutomata automata1 = TWDFABuilder3();
            ConsoleOperations.WriteBMarkReset("2DFA Creation");

            ConsoleOperations.WriteTitle("2DFA Info");
            ConsoleOperations.WriteAutomataInfo(automata1);

            Timer.Restart();
            FiniteAutomataConverter dfaConverter1 = new FiniteAutomataConverter();
            FiniteAutomata resultX = dfaConverter1.Convert2DFAToDFA(automata1);
            ConsoleOperations.WriteBMarkReset("Automata Conversion");

            Console.WriteLine("\n>>>2DFA is converted to DFA<<<\n");
            ConsoleOperations.WriteTitle("DFA Info");
            ConsoleOperations.WriteAutomataInfo(resultX);

            ConsoleOperations.WriteTitle("DFA Trace");
            Timer.Restart();
            bool result = resultX.Run(inputString);
            ConsoleOperations.WriteBMarkReset("DFA Run");
            Timer.Stop();

            ConsoleOperations.WriteTitle("Automata Results");
            Console.WriteLine("DFA Response: Input is " + (result ? "Accepted" : "Rejected"));

            ConsoleOperations.WriteTitle("Benchmark Results");
            BenchmarkResults.ForEach(x => Console.WriteLine(x));
        }

        private static FiniteAutomata NFABuilder()
        {
            List<char> alphabet = new List<char> { '0', '1' };
            FiniteAutomata nfaTest = new FiniteAutomata(FiniteAutomataType.NFA, alphabet);

            _ = nfaTest.AddState("A", isInitialState: true); //q0
            _ = nfaTest.AddState("B");                       //q1
            _ = nfaTest.AddState("C", isFinalState: true);   //q2

            _ = nfaTest.AddTransition('0', "A", "A");     // TR: q0 '0' geçişiyle q0'a gider.
            _ = nfaTest.AddTransition('1', "A", "B,C");   // TR: q0 '1' geçişiyle q1 ya da q2'ye gider.

            _ = nfaTest.AddTransition('0', "B", "B");     // TR: q1 '0' geçişiyle q1'e gider.
            _ = nfaTest.AddTransition('1', "B", "A,C");   // TR: q1 '1' geçişiyle q0 ya da q2'ye gider.

            _ = nfaTest.AddTransition('0', "C", "A,B");   // TR: q2 '0' geçişiyle q0 ya da q1'e gider.
            _ = nfaTest.AddTransition('1', "C", "C");     // TR: q2 '1' geçişiyle q2'ye gider.

            return nfaTest;
        }

        private static FiniteAutomata TWDFABuilder1()
        {
            List<char> alphabet = new List<char> { '0', '1' };
            FiniteAutomata twdfaTest = new FiniteAutomata(FiniteAutomataType.TwoWayDFA, alphabet);

            _ = twdfaTest.AddState("q0", isInitialState: true);
            _ = twdfaTest.AddState("q1");
            _ = twdfaTest.AddState("q2");
            _ = twdfaTest.AddState("q3", isFinalState: true);

            _ = twdfaTest.AddTransition('0', "q0", "q1", 1);
            _ = twdfaTest.AddTransition('1', "q0", "q2", 1);

            _ = twdfaTest.AddTransition('0', "q1", "q3", 0);
            _ = twdfaTest.AddTransition('1', "q1", "q2", 0);

            _ = twdfaTest.AddTransition('0', "q2", "q2", 1);
            _ = twdfaTest.AddTransition('1', "q2", "q3", 1);

            _ = twdfaTest.AddTransition('0', "q3", "q1", 1);
            _ = twdfaTest.AddTransition('1', "q3", "q2", 0);

            return twdfaTest;
        }

        private static FiniteAutomata TWDFABuilder2()
        {
            List<char> alphabet = new List<char> { '0', '1' };
            FiniteAutomata twdfaTest = new FiniteAutomata(FiniteAutomataType.TwoWayDFA, alphabet);

            _ = twdfaTest.AddState("q0", isInitialState: true, isFinalState: true);
            _ = twdfaTest.AddState("q1", isFinalState: true);
            _ = twdfaTest.AddState("q2", isFinalState: true);

            _ = twdfaTest.AddTransition('0', "q0", "q0", 1);
            _ = twdfaTest.AddTransition('1', "q0", "q1", 1);

            _ = twdfaTest.AddTransition('0', "q1", "q1", 1);
            _ = twdfaTest.AddTransition('1', "q1", "q2", 0);

            _ = twdfaTest.AddTransition('0', "q2", "q0", 1);
            _ = twdfaTest.AddTransition('1', "q2", "q2", 0);

            return twdfaTest;
        }

        private static FiniteAutomata TWDFABuilder3()
        {
            List<char> alphabet = new List<char> { '0', '1' };
            FiniteAutomata twdfaTest = new FiniteAutomata(FiniteAutomataType.TwoWayDFA, alphabet);

            _ = twdfaTest.AddState("q0", isInitialState: true);
            _ = twdfaTest.AddState("q1");
            _ = twdfaTest.AddState("q2");
            _ = twdfaTest.AddState("q3");
            _ = twdfaTest.AddState("q4", isFinalState: true);

            _ = twdfaTest.AddTransition('0', "q0", "q1", 1);
            _ = twdfaTest.AddTransition('1', "q0", "q2", 1);

            _ = twdfaTest.AddTransition('0', "q1", "q4", 1);
            _ = twdfaTest.AddTransition('1', "q1", "q2", 0);

            _ = twdfaTest.AddTransition('0', "q2", "q3", 1);
            _ = twdfaTest.AddTransition('1', "q2", "q2", 1);

            _ = twdfaTest.AddTransition('0', "q3", "q1", 0);
            _ = twdfaTest.AddTransition('1', "q3", "q2", 0);

            _ = twdfaTest.AddTransition('0', "q4", "q4", 0);
            _ = twdfaTest.AddTransition('1', "q4", "q4", 1);

            return twdfaTest;
        }
    }
}