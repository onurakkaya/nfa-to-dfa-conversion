using nfa_to_dfa_conversion.Exceptions;
using nfa_to_dfa_conversion.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace nfa_to_dfa_conversion.Tests
{
    public class FiniteAutomataConverterTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Should_ReturnsDFAAutomata_When_ConvertNFAToDFA_WithValidNFA()
        {
            List<char> alphabet = new List<char> { '0', '1' };
            FiniteAutomata nfaTest = new FiniteAutomata(FiniteAutomataType.NFA, alphabet);

            _ = nfaTest.AddState("A", isInitialState: true); //A
            _ = nfaTest.AddState("B");                       //B
            _ = nfaTest.AddState("C", isFinalState: true);   //C

            _ = nfaTest.AddTransition('0', "A", "A");     // TR: A '0' geçişiyle A'ya gider.
            _ = nfaTest.AddTransition('1', "A", "B,C");   // TR: A '1' geçişiyle B ya da C'ye gider.

            _ = nfaTest.AddTransition('0', "B", "A");     // TR: B '0' geçişiyle B'ye gider.
            _ = nfaTest.AddTransition('1', "B", "A,C");   // TR: B '1' geçişiyle A ya da C'ye gider.

            _ = nfaTest.AddTransition('0', "C", "A,B");   // TR: C '0' geçişiyle A ya da B'ye gider.
            _ = nfaTest.AddTransition('1', "C", "C");     // TR: C '1' geçişiyle C'ye gider.

            FiniteAutomata dfaTest = new FiniteAutomata(FiniteAutomataType.DFA, alphabet);
            _ = dfaTest.AddState("A", isInitialState: true);
            _ = dfaTest.AddState("B&C", isFinalState: true);
            _ = dfaTest.AddState("A&B");
            _ = dfaTest.AddState("A&C", isFinalState: true);
            _ = dfaTest.AddState("A&B&C", isFinalState: true);

            _ = dfaTest.AddTransition('0', "A", "A");
            _ = dfaTest.AddTransition('1', "A", "B&C");

            _ = dfaTest.AddTransition('0', "B&C", "A&B");
            _ = dfaTest.AddTransition('1', "B&C", "A&C");

            _ = dfaTest.AddTransition('0', "A&B", "A");
            _ = dfaTest.AddTransition('1', "A&B", "A&B&C");

            _ = dfaTest.AddTransition('0', "A&C", "A&B");
            _ = dfaTest.AddTransition('1', "A&C", "B&C");

            _ = dfaTest.AddTransition('0', "A&B&C", "A&B");
            _ = dfaTest.AddTransition('1', "A&B&C", "A&B&C");


            FiniteAutomataConverter automataConverter = new FiniteAutomataConverter();
            FiniteAutomata converterDFA = automataConverter.ConvertNFAToDFA(nfaTest);

            if (converterDFA.InitialState.StateName == dfaTest.InitialState.StateName
                && converterDFA.FinalState.Count() == dfaTest.FinalState.Count()
                && converterDFA.States.Count() == dfaTest.States.Count()
                && converterDFA.Transitions.Count() == dfaTest.Transitions.Count()
                && converterDFA.States.Count() == dfaTest.States.Count()
                && converterDFA.Transitions.Last().TransitionSymbol == dfaTest.Transitions.Last().TransitionSymbol)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }
        [Test]
        public void Should_ThrowException_When_ConvertNFAToDFA_WithNullParameter()
        {
            _ = Assert.Throws<FAConverterException>(delegate
              {
                  FiniteAutomataConverter converter = new FiniteAutomataConverter();
                  _ = converter.ConvertNFAToDFA(null);
              });
        }
        [Test]
        public void Should_ReturnsItSelf_When_ConvertNFAToDFA_WithDFAParameter()
        {
            List<char> alphabet = new List<char> { 'a', 'b', 'c' };
            FiniteAutomata dfaTest = new FiniteAutomata(FiniteAutomataType.DFA, alphabet);

            _ = dfaTest.AddState(true); // q0
            _ = dfaTest.AddState(); // q1
            _ = dfaTest.AddState(isFinalState: true); // q2

            _ = dfaTest.AddTransition('a', "q0", "q1"); // TR: q0 'a' geçişi ile q1'e gider
            _ = dfaTest.AddTransition('b', "q0", "q2"); // TR: q0 'b' geçişi ile q2'ye gider.
            _ = dfaTest.AddTransition('c', "q0", "q0"); // TR: q0 'c' geçişi ile q0'a gider.

            _ = dfaTest.AddTransition('a', "q1", "q0"); // TR: q1 'a' geçişi ile q0'a gider.
            _ = dfaTest.AddTransition('b', "q1", "q1"); // TR: q1 'b' geçişi ile q1'e gider.
            _ = dfaTest.AddTransition('c', "q1", "q2"); // TR: q1 'c' geçişi ile q2'ye gider.

            _ = dfaTest.AddTransition('a', "q2", "q0"); // TR: q2 'a' geçişi ile q0'a gider.
            _ = dfaTest.AddTransition('b', "q2", "q2"); // TR: q2 'b' geçişi ile q2'ye gider.
            _ = dfaTest.AddTransition('c', "q2", "q2"); // TR: q2 'c' geçişi ile q2'ye gider.

            FiniteAutomataConverter converter = new FiniteAutomataConverter();
            FiniteAutomata converterDFA = converter.ConvertNFAToDFA(dfaTest);

            if (converterDFA.InitialState.StateName == dfaTest.InitialState.StateName
               && converterDFA.FinalState.Count() == dfaTest.FinalState.Count()
               && converterDFA.States.Count() == dfaTest.States.Count()
               && converterDFA.Transitions.Count() == dfaTest.Transitions.Count()
               && converterDFA.States.Count() == dfaTest.States.Count()
               && converterDFA.Transitions.Last().TransitionSymbol == dfaTest.Transitions.Last().TransitionSymbol)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }
    }
}