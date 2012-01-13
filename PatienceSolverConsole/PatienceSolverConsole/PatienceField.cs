﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;

namespace PatienceSolverConsole
{
    [Serializable]
    public class PatienceField
    {
        /// <summary>
        /// Gets valid destination stacks, from least to most likely to lead to a solution
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CardStack> GetDestinationStacks()
        {
            return PlayStacks.Cast<CardStack>()
                .Concat(FinishStacks.ToArray());
        }

        /// <summary>
        /// Gets valid origin stacks, from least to most likely to lead to a solution
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CardStack> GetOriginStacks()
        {
            return (new CardStack[] { Stock })
                .Concat(PlayStacks.ToArray());
        }


        public List<PlayStack> PlayStacks { get; private set; }
        public List<FinishStack> FinishStacks { get; private set; }
        public Stock Stock { get; private set; }

        private int _hash;
        protected bool _hashDirty;

        public PatienceField()
        {
            PlayStacks = new List<PlayStack>();
            FinishStacks = new List<FinishStack>();
            InvalidateHash();
        }

        public void FillWithRandomCards(Random random)
        {
            var cards = GetStock().ToList();
            Util.Shuffle(cards, random);
            var stackless = cards.Where(c => c.Stack == null);
            for (int playstack = 1; playstack <= 7; playstack++)
            {
                var stack = new PlayStack(stackless.Take(playstack));
                PlayStacks.Add(stack);
            }
            for (int finishstack = 1; finishstack <= 4; finishstack++)
            {
                var stack = new FinishStack();
                FinishStacks.Add(stack);
            }
            Stock = new Stock(stackless);
            InvalidateHash();
        }

        public void InvalidateHash()
        {
            _hashDirty = true;
        }

        private IEnumerable<Card> GetStock()
        {
            foreach (var suit in Util.GetValues<Suit>())
                foreach (var value in Util.GetValues<Value>())
                    yield return new Card(suit, value);
        }

        public void DumpToConsole()
        {
            var toprow = FinishStacks.Cast<CardStack>()
                .Concat(new[] { Stock });
            DumpRows(toprow);
            DumpRows(PlayStacks.ToArray());
        }

        private static void DumpRows(IEnumerable<CardStack> toprow)
        {
            int i = 0;
            bool morerows;
            do
            {
                morerows = false;
                foreach (var row in toprow)
                {
                    morerows |= row.WriteLine(i);
                }
                Console.WriteLine();
                i++;
            } while (morerows);
        }

        /// <summary>
        /// Two fields equal if all the stacks are the same.
        /// But, if the location of the stacks are rearranged, the fields are equal too.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as PatienceField;
            if (other == null) return false;
            if (other.GetHashCode() != this.GetHashCode())
                return false; // this should be performant
            if (!Stock.Equals(other.Stock))
                return false;
            var mystacksOrdered = PlayStacks.OrderByDescending(s => s.GetHashCode());
            var hisstacksOrdered = other.PlayStacks.OrderByDescending(s => s.GetHashCode());

            return mystacksOrdered.SequenceEqual(hisstacksOrdered);
            // The finish stacks are not checked, because only the cards not in stock or on the play stacks are there.
            // There is only one significant order possible, so if the cards in stock and on the play stacks are equal, so must be the finish stacks.
        }

        public override int GetHashCode()
        {
            if (_hashDirty)
            {
                _hash = DoGetHashCode();
                _hashDirty = false;
            }
            return _hash;
        }

        private int DoGetHashCode()
        {
            var mystacksOrdered = PlayStacks.OrderByDescending(s => s.GetHashCode());
            var hashcode = 0;
            foreach (var stack in mystacksOrdered)
                hashcode = hashcode * 81 + stack.GetHashCode();
            return hashcode;
        }

        public bool IsDone()
        {
            return
                //new[] { (CardStack)Stock }.Concat(PlayStacks.Cast<CardStack>())
                PlayStacks
                .SelectMany(s => s)
                .All(card => card.Visible);
        }

    }

    static class Util
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        internal static void Shuffle<T>(IList<T> cards, Random random)
        {
            var number = cards.Count;
            for (int i = 0; i < number; i++)
            {
                var toshuffle = cards[i];
                var newplace = random.Next(number);
                cards[i] = cards[newplace];
                cards[newplace] = toshuffle;
            }
        }
    }
}