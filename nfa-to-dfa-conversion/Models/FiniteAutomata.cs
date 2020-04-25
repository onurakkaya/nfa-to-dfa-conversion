using nfa_to_dfa_conversion.Exceptions;
using nfa_to_dfa_conversion.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nfa_to_dfa_conversion.Models
{
    /// <summary>
    /// Finite Automata object
    /// </summary>
    public class FiniteAutomata
    {

        private readonly FiniteAutomataType _automataType;
        private int _stateCounter = 0;

        private IList<char> _alphabet;
        private IList<FAState> _states = new List<FAState>();
        private IList<FATransition> _transitions = new List<FATransition>();

        /// <summary>
        /// Hidden constructor of finite automata.
        /// </summary>
        private FiniteAutomata()
        {

        }

        /// <summary>
        /// Validates DFA model and if validation pass, returns true.
        /// </summary>
        private bool ValidateDFA
        {
            get
            {
                bool hasInitialState = this.InitialState != null;
                bool hasFinalState = this.FinalState.Count() > 0;

                // TR: Tüm durumlar için gez.
                foreach (FAState state in this.States)
                {
                    // TR: Tüm geçişler için kontrol et.

                    foreach (char letter in this.Alphabet)
                    {
                        // TR: Bu alfabe elemanı için seçilen durumdan herhangi bir noktaya transition yapılmış mı?
                        // TR: Bir alfabe elemanı için hiç transition tanımlanmamış ya da aynı elemandan birden fazla noktaya transition tanımlanmış ise DFA geçersizdir.
                        bool hasTransition = this.Transitions.Where(x => x.FromState.StateName == state.StateName && x.TransitionSymbol == letter).Count() == 1;

                        if (!hasTransition)
                        {
                            // TR: Eksik transition var geçerli bir otomata değil.
                            return false;
                        }
                    }
                }

                // TR: Başlangıç durumu ya da bitiş durumu tanımlanmamışsa DFA geçersizdir.
                if (!hasInitialState || !hasFinalState)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Validates NFA model and if validation pass, returns true.
        /// </summary>
        private bool ValidateNFA
        {
            get
            {


                bool hasInitialState = this.InitialState != null;
                bool hasFinalState = this.FinalState.Count() > 0;

                // TR: Başlangıç durumu ya da bitiş durumu tanımlanmamışsa NFA geçersizdir.
                if (!hasInitialState || !hasFinalState)
                {
                    return false;
                }

                return true;
            }
        }


        /// <summary>
        /// Gets automata state by state name.
        /// </summary>
        /// <param name="name">A state name which will be get.</param>
        /// <returns>Returns FAState object by state name.</returns>
        private FAState GetStateByName(string name)
        {
            FAState state = this.States.FirstOrDefault(x => x.StateName.ToLower() == name.ToLower());
            return state;
        }
        /// <summary>
        ///  Returns Inital State of Automata
        /// </summary>
        public FAState InitialState
        {
            // TR: Başlangıç durumu sadece NFA da birden fazla olabilir. Bu durumda her bir başlangıç durumu ile ayrı bir otomata gibi tek tek ele alınır.
            /* EN :Sometimes, NFAs are defined with a set of initial states. 
             * There is an easy construction that translates a NFA with multiple initial states to a NFA with single initial state, which provides a convenient notation. 
             * (https://en.wikipedia.org/wiki/Nondeterministic_finite_automaton)*/

            // TR: Başlangıç durumu olarak işaretlenmiş ilk durum geri dönülür. 
            get
            {
                FAState initialState = this._states.FirstOrDefault(node => node.IsInitialState);
                return initialState;
            }
        }
        public IEnumerable<FAState> FinalState
        {
            // TR: Sonlanma durumu olarak işaretlenmiş tüm durumlar kümesi geri dönülür
            get
            {
                IEnumerable<FAState> finalState = this._states.Where(state => state.IsFinalState);
                return finalState;
            }
        }
        public IEnumerable<FAState> States
        {
            // TR: Tüm durumlar kümesi geri dönülür.
            get
            {
                return this._states;
            }
        }
        public IEnumerable<FATransition> Transitions
        {
            // TR: Tüm durum geçişleri geri döndürülür.
            get
            {
                return this._transitions;
            }
        }
        public FiniteAutomataType AutomataType
        {
            get
            {
                return this._automataType;
            }
        }
        public IEnumerable<char> Alphabet
        {
            get
            {
                return this._alphabet;
            }
        }

        /// <summary>
        /// Checks the finite automata model and returns the model is valid or not. 
        /// </summary>
        public bool IsValid
        {
            get
            {
                switch (this._automataType)
                {
                    case FiniteAutomataType.DFA:
                        return ValidateDFA;
                    default:
                    case FiniteAutomataType.NFA:
                        return ValidateNFA;
                }
            }
        }

        /// <summary>
        /// Initializes Finite Automata Object.
        /// </summary>
        /// <param name="automataType">Automata type, can be take DFA or NFA.</param>
        /// <param name="alphabet">Automata Alphabet, e.g: new List() { 'a','b','c' }; </param>
        public FiniteAutomata(FiniteAutomataType automataType, IList<char> alphabet)
        {
            if (alphabet is null)
            {
                throw new FAAlphabetException("Alphabet input cannot be NULL.");
            }
            else if (alphabet.Count < 1)
            {
                throw new FAAlphabetException("Alphabet input cannot be Empty.");
            }

            this._automataType = automataType;
            this._alphabet = alphabet;

        }

        /// <summary>
        /// Creates new state to the automata.
        /// </summary>
        /// <param name="isInitialState">Is the new state inital state?</param>
        /// <param name="isFinalState">Is the new state final state?</param>
        /// <returns>Returns the result of add operation is success or fail.</returns>
        public bool AddState(bool isInitialState = false, bool isFinalState = false)
        {
            bool hasInitialState = this.States.Any(x => x.IsInitialState);
            // TR: Önceden eklenmiş bir başlangıç durumu varsa ve bu durumda başlangıç durumu olarak eklenmek isteniyorsa buradan geri dön.
            if ((hasInitialState && isInitialState))
            {
                return false;
                throw new MultipleFAInitalStateException();
            }

            string stateName = string.Concat("Q", _stateCounter++);
            FAState state = new FAState(stateName, isInitialState, isFinalState);
            this._states.Add(state);

            return true;
        }
        /// <summary>
        /// Creates new state to the automata.
        /// </summary>
        /// <param name="stateName">Name of the state.</param>
        /// <param name="isInitialState">Is the new state inital state?</param>
        /// <param name="isFinalState">Is the new state final state?</param>
        /// <returns></returns>
        public bool AddState(string stateName, bool isInitialState = false, bool isFinalState = false)
        {
            bool hasInitialState = this.States.Any(x => x.IsInitialState);
            // TR: Önceden eklenmiş bir başlangıç durumu varsa ve bu durumda başlangıç durumu olarak eklenmek isteniyorsa buradan geri dön.
            if ((hasInitialState && isInitialState))
            {
                return false;
                throw new MultipleFAInitalStateException();
            }

            if (this.GetStateByName(stateName) != null)
            {
                return false;
                throw new DuplicateFAStateException();
            }

            FAState state = new FAState(stateName, isInitialState, isFinalState);
            this._states.Add(state);

            return true;
        }

        /// <summary>
        /// Creates new state to the automata. ( This version takes State object as a parameter.)
        /// </summary>
        /// <param name="state">Previously created automata state object.</param>
        /// <returns>Returns the result of add operation is success or fail.</returns>
        public bool AddState(FAState state)
        {
            if (state is null)
            {
                return false;
            }

            bool hasInitialState = this.States.Any(x => x.IsInitialState);
            // TR: Önceden eklenmiş bir başlangıç durumu varsa ve bu durumda başlangıç durumu olarak eklenmek isteniyorsa buradan geri dön.
            if ((hasInitialState && state.IsInitialState))
            {
                return false;
                throw new MultipleFAInitalStateException();
            }

            if (States.Contains(state))
            {
                return false;
                throw new DuplicateFAStateException();
            }

            this._states.Add(state);

            return true;

        }

        public bool UpdateState(FAState state)
        {
            FAState previousState = this.GetStateByName(state.StateName);
            this._states.Remove(previousState);
            this._states.Add(state);
            return true;
        }

        /// <summary>
        /// Creates new transition link between states.
        /// </summary>
        /// <param name="symbol">Transition Symbol</param>
        /// <param name="fromStateName">Where will be transited from? State Name</param>
        /// <param name="toStateName">Where will be transited to? State Name ( Multiple values must be separated by comma: ",")</param>
        /// <returns>Returns the result of add operation is success or fail.</returns>
        public bool AddTransition(char symbol, string fromStateName, string toStateName, int direction = -1)
        {
            string[] toStateArray = toStateName.Split(',');

            if (AutomataType == FiniteAutomataType.DFA && toStateArray.Length > 1)
            {
                return false;
            }

            // TR: Yapılacak geçiş, geçiş alfabesinde yoksa buradan geri dön.
            if (!_alphabet.Contains(symbol))
            {
                return false;
                throw new InvalidFALetterException();
            }


            // TR: Kaynak durumu adına göre bul.
            FAState fromState = this.GetStateByName(fromStateName);
            if (fromState == null)
            {
                return false;
            }

            // TR: Hedef durumları adına göre bul.
            List<FAState> toStatesList = new List<FAState>();
            foreach (string selectedState in toStateArray)
            {
                FAState toState = this.GetStateByName(selectedState);
                if (toState == null)
                {
                    return false;
                }

                toStatesList.Add(toState);
            }

            FATransition transitionModel = null;

            if (direction == 0 || direction == 1)
            {
                transitionModel = new FATransition(symbol, fromState, toStatesList, direction == 1);
            }
            else
            {
                // TR: Geçiş nesnesini oluştur.
                transitionModel = new FATransition(symbol, fromState, toStatesList);
            }

            // TR: Geçiş daha önce tanımlanmışsa hata fırlat.
            bool hasTransition = this._transitions.Any(x => x.CompareTo(transitionModel) == 0);
            if (hasTransition)
            {
                return false;
                throw new DuplicateFATransactionException();
            }

            this._transitions.Add(transitionModel);
            return true;
        }

        /// <summary>
        /// Creates new transition link between states.
        /// </summary>
        /// <param name="transition"> Finite automata transition object.</param>
        /// <returns></returns>
        public bool AddTransition(FATransition transition)
        {
            if (transition is null)
            {
                return false;
            }

            if (this.AutomataType == FiniteAutomataType.DFA && transition.ToState.Count() > 1)
            {
                return false;
            }

            // TR: Yapılacak geçiş, geçiş alfabesinde yoksa buradan geri dön.
            if (!this.Alphabet.Contains(transition.TransitionSymbol))
            {
                return false;
                throw new InvalidFALetterException();
            }

            // TR: Geçiş daha önce tanımlanmışsa hata fırlat.
            bool hasTransition = this.Transitions.Any(x => x.CompareTo(transition) == 0);
            if (hasTransition)
            {
                return false;
                throw new DuplicateFATransactionException();
            }

            this._transitions.Add(transition);
            return true;
        }

        /// <summary>
        /// Runs the finite automata with input, then returns true if input is accepted.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>If input is valid for the finite automata, that will be return true otherwise false.</returns>
        public bool Run(string input)
        {
            if (!IsValid)
            {
                return false;
            }

            FAState currentState = InitialState;
            FATransition currentTransition;
            foreach (char letter in input)
            {
                Console.Write("Transition {0} - {1} >>> ", letter, currentState.StateName);

                currentTransition = this.Transitions.FirstOrDefault(x => x.FromState.StateName == currentState.StateName && x.TransitionSymbol == letter);
                currentState = currentTransition.ToState.RandomState();
                Console.WriteLine(currentState.StateName);
            }

            Console.WriteLine("Current State is {0}.(Final State: {1})", currentState.StateName, currentState.IsFinalState ? "Yes" : "No");
            if (currentState.IsFinalState)
            {
                return true;
            }

            return false;
        }
    }

    public enum FiniteAutomataType
    {
        DFA = 0, NFA = 1, TwoWayDFA = 2
    }
}
