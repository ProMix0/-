using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace AutoComplete
{
    /// <summary>
    /// Struct that represents information about persons name
    /// </summary>
    public struct FullName
    {
        public string Name;
        public string Surname;
        public string Patronymic;
    }

    /// <summary>
    /// Class for persons names autocompletion suggestings
    /// </summary>
    public class AutoCompleter
    {
        private readonly SortedSet<string> searchable = new();

        /// <summary>
        /// Max length of <see cref="Search"/> method argument
        /// </summary>
        public int RequestLengthLimit { get; private set; }

        public AutoCompleter()
        {
            RequestLengthLimit = 100;
        }

        /// <summary>
        /// Adding range of persons to search
        /// </summary>
        /// <param name="fullNames">Persons to add</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="fullNames"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Throws if <paramref name="fullNames"/> is empty
        /// </exception>
        public void AddToSearch(List<FullName> fullNames)
        {
            if (fullNames == null)
                throw new ArgumentNullException($"{nameof(fullNames)} is null");

            if (fullNames.Count == 0)
                throw new ArgumentException($"{nameof(fullNames)} is empty");

            searchable
                .UnionWith(fullNames //Add new persons to existing
                .Select(ToString)); //Transform objects to string
        }

        /// <summary>
        /// Transform <see cref="FullName"/> to string view
        /// <br/>
        /// Don't add field to result if it is whitespace, empty or <see langword="null"/>
        /// </summary>
        /// <param name="person">Object to transform</param>
        /// <returns>Surname, name and patronymic separated by whitespace</returns>
        /// <exception cref="ArgumentException">
        /// Throws if all fields in <paramref name="person"/> is null, empty or whitespace
        /// </exception>
        private string ToString(FullName person)
        {
            List<string> items = new(); //List of filled fields

            if (!string.IsNullOrWhiteSpace(person.Surname))
                items.Add(person.Surname.Trim());

            if (!string.IsNullOrWhiteSpace(person.Name))
                items.Add(person.Name.Trim());

            if (!string.IsNullOrWhiteSpace(person.Patronymic))
                items.Add(person.Patronymic.Trim());


            if (items.Count == 0)
                throw new ArgumentException($"{nameof(person)} hasn't information in fields (is null, empty or whitespace)");

            return string.Join(' ', items); //Concat to string in predefined order
        }

        /// <summary>
        /// Suggest variants to autocomplete for entered prefix 
        /// </summary>
        /// <param name="prefix">Prefix for autocomplete suggestions</param>
        /// <returns><see cref="List{string}"/> of suggested variants</returns>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="prefix"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Throws if <paramref name="prefix"/> empty or whitespace or length exceed limit (<see cref="RequestLengthLimit"/>)
        /// </exception>
        public List<string> Search(string prefix)
        {
            if (prefix == null)
                throw new ArgumentNullException($"{nameof(prefix)} argument is null");

            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException($"{nameof(prefix)} argument is empty or contains only whitespaces");

            if (prefix.Length > RequestLengthLimit)
                throw new ArgumentException($"Length of {nameof(prefix)} argument exceed current limit ({RequestLengthLimit})");

            //Getting upper bound for answers range. Increase last char by 1
            string newPrefix = prefix[0..^1] + (char)(prefix[^1] + 1);
            //TODO rewrite system to prevent char overflow

            return searchable
                .GetViewBetween(prefix, newPrefix) //Get range
                .Except(new[] { newPrefix }) //Exclude upper bound
                .ToList();
        }
    }
}
