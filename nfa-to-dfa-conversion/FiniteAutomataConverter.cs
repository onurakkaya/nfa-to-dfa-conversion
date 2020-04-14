using nfa_to_dfa_conversion.Exceptions;
using nfa_to_dfa_conversion.Extensions;
using nfa_to_dfa_conversion.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace nfa_to_dfa_conversion
{
    public class FiniteAutomataConverter
    {
        private const char _stateSeparator_ = '&';

        /// <summary>
        /// Converts NFA to DFA 
        /// </summary>
        /// <param name="input">NFA object</param>
        /// <returns>DFA object</returns>
        public FiniteAutomata ConvertNFAToDFA(FiniteAutomata input)
        {
            if (input is null)
            {
                throw new FAConverterException("Input automata cannot be NULL.");
            }

            // TR: Parametrede gelen tür DFA ise dönüştürmeye gerek olmadığından aynısı geri döndürülür.
            if (input.AutomataType == FiniteAutomataType.DFA)
            {
                return input;
            }

            FiniteAutomata DFA = new FiniteAutomata(FiniteAutomataType.DFA, input.Alphabet.ToList());

            // TR: İlk durum olduğu gibi eklenir.
            DFA = InsertInitalState(input, DFA);

            // TR: Dönüştürme işlemi yapılır.
            DFA = Convert(input, DFA);

            return DFA;
        }
        /// <summary>
        ///  Inserts initial state to DFA from NFA.
        /// </summary>
        /// <param name="NFA"></param>
        /// <param name="DFA"></param>
        /// <returns>Updated DFA object.</returns>
        private FiniteAutomata InsertInitalState(FiniteAutomata NFA, FiniteAutomata DFA)
        {
            _ = DFA.AddState(NFA.InitialState);

            // TR: Sadece initial state'in yaptığı geçişleri NFA dan getirilir.
            IEnumerable<FATransition> transitions = NFA.Transitions.Where(x => x.FromState == NFA.InitialState);
            foreach (FATransition transition in transitions)
            {
                if (transition.ToState.Count() == 1)
                {
                    // TR: Hedef state yoksa oluşturulur. 
                    FAState toState = transition.ToState.First();
                    _ = DFA.AddState(toState);

                    // TR: 1-1 geçiş olduğundan DFA'ya uyumludur. Geçişi olduğu gibi eklenir.
                    _ = DFA.AddTransition(transition);
                }
                else if (transition.ToState.Count() > 1) // 1 den fazla to state varsa
                {
                    IEnumerable<FAState> toStates = transition.ToState;
                    // TR: Stateleri aralarına ayraç koyarak birleştir.
                    string newStateName = string.Join(_stateSeparator_, toStates);
                    // TR: Herhangi bir state final state ise bu state final olmalıdır.
                    bool isFinalState = toStates.Any(state => state.IsFinalState);

                    // TR: Hedef state yoksa oluşturulur.
                    FAState aState = new FAState(newStateName, isFinalState: isFinalState);
                    _ = DFA.AddState(aState);

                    _ = DFA.AddTransition(transition.TransitionSymbol, transition.FromState.StateName, aState.StateName);
                }
                else // 0 olma durumu
                {

                }
            }

            return DFA;
        }
        /// <summary>
        /// Converts NFA transitions into the DFA transition, except initial state.
        /// </summary>
        /// <param name="NFA"></param>
        /// <param name="DFA"></param>
        /// <returns>Updated DFA object.</returns>
        private FiniteAutomata Convert(FiniteAutomata NFA, FiniteAutomata DFA)
        {
            Queue<FAState> statesQueue = new Queue<FAState>();

            // TR: Initial state tarafından eklenen tüm stateleri bulup kuyruğa ekler.
            IEnumerable<FAState> states = DFA.States.Where(x => !x.IsInitialState);
            foreach (FAState state in states)
            {
                statesQueue.Enqueue(state);
            }

            // TR: Kuyruktaki tüm nesneler için çalışır.
            do
            {
                // TR: Kuyruktan sıradaki eleman getirilir.
                FAState fromState = statesQueue.Dequeue();

                // TR: DFA olduğu için tüm geçişler kullanılmalıdır.
                foreach (char symbol in DFA.Alphabet)
                {
                    string[] subStateNames = fromState.StateName.Split(_stateSeparator_);

                    List<string> newStateNames = new List<string>();
                    bool isFinalState = false;

                    foreach (string subStateName in subStateNames)
                    {
                        FAState subState = NFA.States.First(x => x.StateName == subStateName);
                        FATransition subTransition = NFA.Transitions.FirstOrDefault(x => x.FromState == subState && x.TransitionSymbol == symbol);

                        IEnumerable<FAState> subToStates = subTransition.ToState;
                        newStateNames.AddNotExists(subToStates.Select(x => x.StateName));

                        // TR: Herhangi bir state final state ise bu state final olmalıdır.
                        if (!isFinalState)
                        {
                            isFinalState = subToStates.Any(state => state.IsFinalState);
                        }
                    }

                    // TR: Stateleri aralarına ayraç koyarak birleştirir.
                    string newStateName = string.Join(_stateSeparator_, newStateNames.OrderBy(x => x));

                    // TR: Hedef state yoksa oluşturulur.
                    FAState targetState;
                    if (!DFA.States.Any(x => x.StateName == newStateName))
                    {
                        targetState = new FAState(newStateName, isFinalState: isFinalState);
                        _ = DFA.AddState(targetState);

                        // TR: Olmayan item kuyruğa da eklenir.
                        statesQueue.Enqueue(targetState);
                    }
                    else
                    {
                        targetState = DFA.States.First(x => x.StateName == newStateName);
                    }

                    _ = DFA.AddTransition(symbol, fromState.StateName, targetState.StateName);
                }

            } while (statesQueue.Count > 0);

            return DFA;
        }
    }
}