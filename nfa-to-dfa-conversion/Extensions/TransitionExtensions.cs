﻿using nfa_to_dfa_conversion.Models;
using System.Collections.Generic;
using System.Linq;

namespace nfa_to_dfa_conversion.Extensions
{
    public static class TransitionExtensions
    {
        public static bool Compare(this FAState first, FAState second)
        {
            return first.StateName == second.StateName;
        }

        public static bool Compare(this IEnumerable<FAState> first, IEnumerable<FAState> second)
        {
            // TR: State sayıları eşit değilse aynı state olamazlar.
            if (first.Count() != second.Count())
            {
                return false;
            }

            Dictionary<FAState, bool> stateStatues = new Dictionary<FAState, bool>();
            foreach (FAState toState in first)
            {
                stateStatues.Add(toState, false);
                bool stateStatus = second.Any(state => state.StateName == toState.StateName);
                if (stateStatus)
                {
                    stateStatues[toState] = true;
                    continue;
                }
            }
            // TR: Eğer eşiti bulunamayan state varsa eşit değildir.
            bool isToStatesEqual = !stateStatues.Any(state => state.Value == false);

            return isToStatesEqual;
        }
    }

}
