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
        private const char STATE_SEPARATOR = '&';

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
                    string newStateName = string.Join(STATE_SEPARATOR, toStates);
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
                    string[] subStateNames = fromState.StateName.Split(STATE_SEPARATOR);

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
                    string newStateName = string.Join(STATE_SEPARATOR, newStateNames.OrderBy(x => x));

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
        /// <summary>
        /// Converts 2DFA to DFA 
        /// </summary>
        /// <param name="input">2DFA object</param>
        /// <returns>DFA object</returns>
        public FiniteAutomata Convert2DFAToDFA(FiniteAutomata input)
        {
            if (input is null)
            {
                throw new FAConverterException("Input automata cannot be NULL");
            }

            if (input.AutomataType != FiniteAutomataType.TwoWayDFA)
            {
                throw new FAConverterException($"Unexpected Automata Type {input.AutomataType}");
            }

            FiniteAutomata DFA = new FiniteAutomata(FiniteAutomataType.DFA, input.Alphabet.ToList());
            DFA = InsertRightDirectionStates(input, DFA);
            DFA = InsertLeftDirectionStates(input, DFA);

            List<FATransition> transitions = DFA.Transitions.Where(x => x.Direction == false).ToList();
            for (int i = 0; i < transitions.Count; i++)
            {
                transitions[i].Direction = true;
            }

            return DFA;
        }

        private FiniteAutomata InsertRightDirectionStates(FiniteAutomata TWDFA, FiniteAutomata DFA)
        {
            foreach (FAState state in TWDFA.States)
            {
                _ = DFA.AddState(state);
            }

            foreach (FATransition transition in TWDFA.Transitions)
            {
                if (transition.Direction == true)
                {
                    _ = DFA.AddTransition(transition);
                }
            }

            return DFA;
        }

        private FiniteAutomata InsertLeftDirectionStates(FiniteAutomata TWDFA, FiniteAutomata DFA)
        {
            IEnumerable<FATransition> leftTransitions = TWDFA.Transitions.Where(x => !x.Direction);
            Queue<FATransition> leftTransitionQueue = new Queue<FATransition>(leftTransitions);
            do
            {
                FATransition transition = leftTransitionQueue.Dequeue();
                FAState toState = transition.ToState.First();
                IEnumerable<FATransition> l0Transitions = TWDFA.Transitions.Where(x => x.FromState == toState);
                FAState targetState = null;
                if (l0Transitions.Any(x => !x.Direction))
                {
                    IEnumerable<FATransition> l1Transitions = l0Transitions.Where(x => !x.Direction);
                    if (l1Transitions.Count() > 1)
                    {
                        leftTransitionQueue.Enqueue(transition);
                    }
                    else
                    {
                        FATransition l1Transition = l1Transitions.First();
                        FAState l1State = l1Transition.FromState;

                        targetState = GetOptimalState(l1State, l0Transitions, DFA);
                    }
                }
                else
                {
                    targetState = GetOptimalState(toState, l0Transitions, DFA);
                }

                FATransition newTransition = new FATransition(transition.TransitionSymbol, transition.FromState, targetState);
                _ = DFA.AddTransition(newTransition);

            } while (leftTransitionQueue.Count > 0);

            return DFA;
        }

        private FAState GetOptimalState(FAState sourceState, IEnumerable<FATransition> filterTransition, FiniteAutomata DFA)
        {
            FAState selectedState = null;
            IEnumerable<FAState> finalStateFilter = filterTransition.SelectMany(x => x.ToState.Where(y => y.IsFinalState));
            if (finalStateFilter.Count() == 1)
            {
                selectedState = finalStateFilter.First();
            }
            else
            {
                bool xLoop = filterTransition.All(x => x.FromState == sourceState
                                                       && x.ToState.Any(y => y.StateName == sourceState.StateName)
                                                       && x.FromState.IsFinalState);
                if (xLoop)
                {
                    string emptySName = "Empty";
                    FAState emptySet = new FAState(emptySName);
                    if (!DFA.States.Any(x => x.StateName == emptySet.StateName))
                    {
                        _ = DFA.AddState(emptySet);

                        foreach (char symbol in DFA.Alphabet)
                        {
                            _ = DFA.AddTransition(symbol, emptySName, emptySName);
                        }
                    }
                    selectedState = emptySet;
                    return selectedState;
                }

                IEnumerable<FAState> selfRefStates = filterTransition.Where(x => x.ToState.Any(y => y == sourceState && x.FromState == sourceState) && !x.Direction)
                                                                     .Select(x => x.FromState);
                FAState selfRefSt = selfRefStates.FirstOrDefault();
                if (selfRefSt != null)
                {
                    FAState updState = new FAState(selfRefSt.StateName, selfRefSt.IsInitialState);
                    DFA.UpdateState(updState);
                    selectedState = updState;
                    return selectedState;
                }


                IEnumerable<FAState> selfReferenceFilter = filterTransition.SelectMany(x => x.ToState.Where(y => y != sourceState));
                if (selfReferenceFilter.Count() == 1)
                {
                    selectedState = selfReferenceFilter.First();
                }
                else
                {
                    selectedState = filterTransition.First().FromState;
                }
            }

            return selectedState;
        }
    }
}