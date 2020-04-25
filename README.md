
# nfa-to-dfa-conversion
FlyWeight Finite Automata Simulator.

## Objects

### FAState
The object used to operate States of the Automata.
#### Constructor
Has just one constructor. The constructor takes StateName, InitialState and Final State parameters. StateName is required parameter. InitialState and FinalState is optional and default values of they are False.

#### Properties

 - **IsInitialState:** Read only property. Returns  is this state marked as inital state or not.
 - **IsFinalState:** Read only property. Returns is this state marked as final state or not.
 - **StateName:** Read only property. Returns the name of the state.

#### Methods
No method defined yet.

### FATransition 
The object used to operate Transitions of the Automata.
#### Constructor
Has two constructors. The difference, one the constructor takes only one destination state, the other one is takes `IEnumerable` destination state. Constructor parameters are TransitionSymbol, FromState, ToState. All parameters are required.
#### Properties

- **TransitionSymbol:** Read only property. Returns the transition symbol.
- **FromState:** Read only property. Returns source state as `FAState` object.
- **ToState:** Read only property. Returns destination states as `IEnumerable<FaState>` object.
- **Direction:** Returns the transition direction is Left(false/0) or Right(true/1). (Added for 2DFA support). 
 #### Methods
- **CompareTo :** Compares states with another state.If there are equal that will be return 0, otherwise wiil be return -1
- 
### FiniteAutomata
Finite Automata Object can operate DFA or NFA automats.

#### Constructor
Has one constructor. It takes AutomataType and Alphabet parameters. All parameters are required.

#### Properties
- **InitialState:** Read only property. Returns the initial state of the automata as `FAState` object.
- **FinalState:** Read only property. Returns the final states of the automata as `IEnumerable<FaState>` object.
- **States:** Read only property. Returns the all of the states of the automata as `IEnumerable<FaState>` object.
- **Transitions:** Read only property. Returns the all of the transition declarations of the automata as `IEnumerable<FaTransition>` object.
- **AutomataType:** Read only property. Returns the automata type of the automata as `FiniteAutomataType`. Can only takes DFA or NFA.
- **Alphabet:** Read only property. Returns the alphabet of the automata as `IEnumerable<char>` object.
- **IsValid:** Read only property. Returns the current automata model is valid or invalid.
- 
#### Methods
- **AddState:** Creates new state to the automata. Returns the result of add operation is success or fail.
- **UpdateState:** Updates the state of the automata. Returns the result of update operation is success or fail.
- **AddTransition:** Creates new transition link between states. Returns the result of add operation is success or fail.
- **Run:** Runs the finite automata with input, then returns true if input is accepted.
 ### FiniteAutomataConverter
#### Constructor
There is no specific constructor declaration yet.
#### Properties
No property defined yet.
#### Methods
- **ConvertNFAToDFA:**  Converts NFA input to DFA. Returns DFA AS `FiniteAutomata` object.
- **Convert2DFAToDFA:**  Converts 2DFA input to DFA. Returns DFA AS `FiniteAutomata` object.
 
## Usage
### Sample DFA Build

[![](https://mermaid.ink/img/eyJjb2RlIjoic3RhdGVEaWFncmFtXG5bKl0gLS0-IHEwXG4gIHEwIC0tPiBxMTogYVxuICBxMCAtLT4gcTI6IGJcbiAgcTAgLS0-IHEwOiBjXG4gIHExIC0tPiBxMDogYVxuICBxMSAtLT4gcTE6IGJcbiAgcTEgLS0-IHEyOiBjXG4gIHEyIC0tPiBxMDogYVxuICBxMiAtLT4gcTI6IGIgLGNcbiAgcTIgLS0-IFsqXSIsIm1lcm1haWQiOnsidGhlbWUiOiJkZWZhdWx0In0sInVwZGF0ZUVkaXRvciI6ZmFsc2V9)](https://mermaid-js.github.io/mermaid-live-editor/#/edit/eyJjb2RlIjoic3RhdGVEaWFncmFtXG5bKl0gLS0-IHEwXG4gIHEwIC0tPiBxMTogYVxuICBxMCAtLT4gcTI6IGJcbiAgcTAgLS0-IHEwOiBjXG4gIHExIC0tPiBxMDogYVxuICBxMSAtLT4gcTE6IGJcbiAgcTEgLS0-IHEyOiBjXG4gIHEyIC0tPiBxMDogYVxuICBxMiAtLT4gcTI6IGIgLGNcbiAgcTIgLS0-IFsqXSIsIm1lcm1haWQiOnsidGhlbWUiOiJkZWZhdWx0In0sInVwZGF0ZUVkaXRvciI6ZmFsc2V9)

```
            // Alphabet of the automata is a, b and c.
            List<char> alphabet = new List<char> { 'a', 'b', 'c' };
            FiniteAutomata dfaTest = new FiniteAutomata(FiniteAutomataType.DFA, alphabet);

            _ = dfaTest.AddState(true);               // Uses default naming convention.So q0 state created.
            _ = dfaTest.AddState();                   // Uses default naming convention.So q1 state created.
            _ = dfaTest.AddState(isFinalState: true); // Uses default naming convention.So q2 state created.

            _ = dfaTest.AddTransition('a', "q0", "q1"); // Declaration: q0, transits to q1 with 'a' transition.
            _ = dfaTest.AddTransition('b', "q0", "q2"); // Declaration: q0, transits to q2 with 'b' transition.
            _ = dfaTest.AddTransition('c', "q0", "q0"); // Declaration: q0, transits to q0 with 'c' transition.
            
            _ = dfaTest.AddTransition('a', "q1", "q0"); // Declaration: q1, transits to q0 with 'a' transition.
            _ = dfaTest.AddTransition('b', "q1", "q1"); // Declaration: q1, transits to q1 with 'b' transition.
            _ = dfaTest.AddTransition('c', "q1", "q2"); // Declaration: q1, transits to q2 with 'c' transition.

            _ = dfaTest.AddTransition('a', "q2", "q0"); // Declaration: q0, transits to q2 with 'a' transition.
            _ = dfaTest.AddTransition('b', "q2", "q2"); // Declaration: q2, transits to q2b with 'b' transition.
            _ = dfaTest.AddTransition('c', "q2", "q2"); // Declaration: q2, transits to q2 with 'c' transition.
```

### Sample NFA Build

[![](https://mermaid.ink/img/eyJjb2RlIjoic3RhdGVEaWFncmFtXG4gIFsqXSAtLT4gQVxuICBBIC0tPiBBOiAwXG4gIEEgLS0-IEI6IDFcbiAgQSAtLT4gQzogMVxuICBCIC0tPiBCOiAwXG4gIEIgLS0-IEE6IDFcbiAgQiAtLT4gQzogMVxuICBDIC0tPiBBOiAwXG4gIEMgLS0-IEI6IDBcbiAgQyAtLT4gQzogMVxuICBDIC0tPiBbKl0iLCJtZXJtYWlkIjp7InRoZW1lIjoiZGVmYXVsdCJ9LCJ1cGRhdGVFZGl0b3IiOmZhbHNlfQ)](https://mermaid-js.github.io/mermaid-live-editor/#/edit/eyJjb2RlIjoic3RhdGVEaWFncmFtXG4gIFsqXSAtLT4gQVxuICBBIC0tPiBBOiAwXG4gIEEgLS0-IEI6IDFcbiAgQSAtLT4gQzogMVxuICBCIC0tPiBCOiAwXG4gIEIgLS0-IEE6IDFcbiAgQiAtLT4gQzogMVxuICBDIC0tPiBBOiAwXG4gIEMgLS0-IEI6IDBcbiAgQyAtLT4gQzogMVxuICBDIC0tPiBbKl0iLCJtZXJtYWlkIjp7InRoZW1lIjoiZGVmYXVsdCJ9LCJ1cGRhdGVFZGl0b3IiOmZhbHNlfQ)

```
            private static FiniteAutomata NFABuilder()
        {
            // Alphabet of the automata is 0 and 1.
            List<char> alphabet = new List<char> { '0', '1' }; 
            // Create new Automata NFA with Alphabet above.
            FiniteAutomata nfaTest = new FiniteAutomata(FiniteAutomataType.NFA, alphabet);

            _ = nfaTest.AddState("A", isInitialState: true); // A state created.
            _ = nfaTest.AddState("B");                       // B state created.
            _ = nfaTest.AddState("C", isFinalState: true);   // C state created.

            _ = nfaTest.AddTransition('0', "A", "A");     // Declaration: A, transits to A with '0' transition.
            _ = nfaTest.AddTransition('1', "A", "B,C");   // Declaration: A, transits to B or C with '1' transition.

            _ = nfaTest.AddTransition('0', "B", "B");     // Declaration: B, transits to B with '0' transition.
            _ = nfaTest.AddTransition('1', "B", "A,C");   // Declaration: B, transits to A or C with '1' transition.
            _ = nfaTest.AddTransition('0', "C", "A,B");   // Declaration: C, transits to A or B with '0' transition.
            _ = nfaTest.AddTransition('1', "C", "C");     // Declaration: C, transits to C with '1' transition.
            return nfaTest;
```
Don't forget check automata with IsValid property. :)

        FiniteAutomata automata = NFABuilder();
        if (automata.IsValid)
        {
            // Some codes
        }

You can run input on the automata with Run method. 
**Note:** Run  method contains IsValid check, so you don't need addinationally check that.

    FiniteAutomata automata = NFABuilder();
    bool result = automata.Run(inputString);

You can convert your NFA to DFA easily.

    FiniteAutomata automata = NFABuilder();
    
    FiniteAutomataConverter dfaConverter = new FiniteAutomataConverter();
    FiniteAutomata ConvertedAutomata = dfaConverter.ConvertNFAToDFA(automata);

## Benchmarks
### Some test results:

    [Time To Completion] NFA Creation: 57 ms
    [Time To Completion] NFA Validation: 0 ms
    [Time To Completion] NFA Run: 35 ms
    [Time To Completion] Automata Conversion: 16 ms
    [Time To Completion] DFA Validation: 1 ms
    [Time To Completion] DFA Run: 28 ms