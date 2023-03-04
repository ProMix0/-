using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Morphology
{
    public class SentenceMorpher
    {
        /// <summary>
        ///     Creates <see cref="SentenceMorpher"/> from lines enumerable.
        /// </summary>
        /// <param name="dictionaryLines">
        ///     Lines of source dictionary OpenCorpora in plain-text format.
        ///     <code> WORD(\t)PART_OF_SPEECH[, ]lemmAttribute1[, ]lemmAttributeN( )formAttribute1[, ]formAttributeN </code>
        /// </param>
        public static SentenceMorpher Create(IEnumerable<string> dictionaryLines)
        {

            List<List<string>> blocks = new();
            //Block of consecutive lines with text
            List<string> block = new();
            //Key - hash, value - lemms
            SortedDictionary<int, SortedSet<Lemm>> lemms = new();
            //Index of lemm in input order
            int index = 0;

            foreach (var line in dictionaryLines)
            {
                //Close block if new line empty or hasn't words
                if (line.Length == 0 || !char.IsLetter(line[0]))
                {
                    if (block.Count == 0) continue;
                    blocks.Add(block);
                    //And add new
                    block.Clear();
                }
                else
                    block.Add(line);
            }
            //Add last block
            if (block.Count != 0)
                blocks.Add(block);

            foreach (var item in blocks)
            {
                Lemm lemm = new(item, index++);

                int lemmHash = lemm.GetHashCode();
                if (lemms.ContainsKey(lemmHash))
                    lemms[lemmHash].Add(lemm);
                else
                    lemms[lemmHash] = new(comparer) { lemm };
            }

            return new SentenceMorpher(lemms);
        }

        /// <summary>
        /// Comparer for sort lemms with equal hash by input order
        /// </summary>
        private static readonly Comparer<Lemm> comparer = Comparer<Lemm>.Create((left, right) => left.Index.CompareTo(right.Index));

        /// <summary>
        /// Hash-table of loaded lemms
        /// </summary>
        private readonly SortedDictionary<int, SortedSet<Lemm>> lemms;

        /// <summary>
        /// Create new instance from dictionary with lemms
        /// </summary>
        /// <param name="lemms">Hash-table dictionary</param>
        private SentenceMorpher(SortedDictionary<int, SortedSet<Lemm>> lemms)
        {
            this.lemms = lemms;
        }

        /// <summary>
        /// Declance sentence to specified format
        /// </summary>
        /// <param name="sentence">
        /// Input sentence <para/>
        /// Format - whitespace-separated sequence of words.
        /// Word can have attributes after it.
        /// Format: <code>{PART_OF_SPEECH[, ]attribute1[, ]attribute2[, ]attributeN}</code>
        /// Use first matching form of word
        /// </param>
        /// <returns>
        /// Declanced sentence in lower case
        /// </returns>
        public virtual string Morph(string sentence)
        {
            List<string> words = new();

            foreach (var word in SplitToWords(sentence))
                words.Add(MorphWord(word));

            return string.Join(' ', words);
        }

        /// <summary>
        /// Split sentence to words by whitespaces and string newlines
        /// </summary>
        /// <param name="sentence">Sentence to split</param>
        /// <returns>Sequence of words</returns>
        private IEnumerable<string> SplitToWords(string sentence)
        {
            List<int> whitespaceIndexes = new();

            int brackets = 0;//Brackets balance
            for (int i = 0; i < sentence.Length; i++)
            {
                switch (sentence[i])
                {
                    case '{':
                        brackets++;
                        break;
                    case '}':
                        brackets--;
                        break;
                    //Words separators
                    case '\n':
                    case ' ':
                        if (brackets == 0)
                            whitespaceIndexes.Add(i);
                        break;
                }
            }
            whitespaceIndexes.Add(sentence.Length);

            //Separation
            int prev = 0;
            foreach (var index in whitespaceIndexes)
            {
                string result = sentence[prev..index];
                prev = index + 1;
                if (!string.IsNullOrWhiteSpace(result))
                    yield return result;
            }
        }

        /// <summary>
        /// Morph single word by attribytes
        /// </summary>
        /// <param name="word">Word and it's attributes</param>
        /// <returns>Morphed word</returns>
        private string MorphWord(string word)
        {
            string[] content = word.Split('{', '}'); //Split to word and attributes

            if (content.Length == 1) //If have not attributes
                return content[0];

            ushort[] grammemas = content[1].Split(',', ' ').Where(str => !string.IsNullOrWhiteSpace(str)).Select(Grammema.GetIndex).ToArray();

            if (grammemas.Length == 0) //If attributes count is zero
                return content[0];

            int hash = content[0].ToUpper().GetHashCode();
            if (lemms.ContainsKey(hash))
                foreach (var lemma in lemms[hash])//Find first match
                {
                    if (lemma.TryGetForm(content[0], grammemas, out string morphed))
                        return morphed;
                }

            //If no one match
            return content[0];
        }

        /// <summary>
        /// Presentation of lemma and it's forms<para/>
        /// Use lazy initialization
        /// </summary>
        private class Lemm
        {
            /// <summary>
            /// Normal form
            /// </summary>
            public string Word { get; }

            /// <summary>
            /// Index in input order
            /// </summary>
            public int Index { get; }

            /// <summary>
            /// All available forms
            /// </summary>
            private Form[] forms;

            /// <summary>
            /// Create <see cref="Lemm"/> from list of lines
            /// </summary>
            /// <param name="block">List of input lines</param>
            public Lemm(List<string> block, int index)
            {
                this.block = block;
                Index = index;

                //Get normal form for calculating hash
                Word = block[0].Substring(0, block[0].IndexOf('\t')).ToUpper();
            }

            /// <summary>
            /// Find forms by attributes and return first if finded
            /// </summary>
            /// <param name="toMorph">Normal form of word to morph</param>
            /// <param name="grammemas">Sequence of grammemas to morph</param>
            /// <param name="textForm">Return form if it finded</param>
            /// <returns>
            /// Return <see langword="true"> if requested form exist
            /// </returns>
            public bool TryGetForm(string toMorph, IEnumerable<ushort> grammemas, out string textForm)
            {
                EnsureInitialized();

                textForm = "";

                //Exit if normal forms in request and lemm are different (can happend with equals hashs)
                if (!toMorph.Equals(Word, StringComparison.OrdinalIgnoreCase))
                    return false;

                SortedSet<ushort> toCompare = new(grammemas);

                //First form with all requested attributes
                foreach (var form in forms)
                {
                    if (form.grammemas.IsSupersetOf(toCompare))
                    {
                        textForm = form.word;
                        return true;
                    }
                }
                return false;
            }

            public override int GetHashCode()
            {
                //Lemm's hash equals normal form hash for finding in dictionary
                return Word.GetHashCode();
            }

            #region LazyInitialization

            private List<string> block;
            private bool initialized = false;

            private void EnsureInitialized()
            {
                if (initialized) return; ;
                //And forms of lemm
                forms = block.Select(line => new Form(line)).ToArray();

                initialized = true;
                block.Clear();
                block = null;
            }

            #endregion

            /// <summary>
            /// Form of lemma
            /// </summary>
            private class Form
            {
                /// <summary>
                /// Create form of lemma from line of dictionary
                /// </summary>
                /// <param name="line">Line with text form and set of attributes</param>
                public Form(string line)
                {
                    //Get word
                    word = line.Substring(0, line.IndexOf('\t'));
                    //And attributes
                    string attrs = line.Substring(line.IndexOf('\t') + 1, line.Length - line.IndexOf('\t') - 1);

                    if (string.IsNullOrWhiteSpace(attrs))//Hasn't attributes
                        grammemas = new();
                    else
                        grammemas = new(attrs.Split(',', ' ').Select(Grammema.GetIndex));
                }

                /// <summary>
                /// Text of form
                /// </summary>
                public readonly string word;
                /// <summary>
                /// Set of form's grammemas
                /// </summary>
                public readonly SortedSet<ushort> grammemas;
            }
        }

    }

    /// <summary>
    /// Transform <see langword="string"/> attributes to <see langword="ushort"/> with dictionary<para/>
    /// Purpose - save memory
    /// </summary>
    static class Grammema
    {
        private static readonly SortedDictionary<string, ushort> grammemas = new();
        private static ushort index = 0;

        /// <summary>
        /// Transform <see langword="string"/> attribute to <see langword="ushort"/> view
        /// </summary>
        /// <param name="grammema">Attribute</param>
        /// <returns><see langword="ushort"/> view</returns>
        public static ushort GetIndex(string grammema)
        {
            grammema = grammema.ToLower();
            if (grammemas.ContainsKey(grammema))
                return grammemas[grammema];
            else
                return grammemas[grammema] = index++;
        }
    }

}