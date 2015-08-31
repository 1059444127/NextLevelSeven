﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace NextLevelSeven.Cursors.Dividers
{
    /// <summary>
    /// A splitter which handles getting and setting delimited substrings within a parent string divider.
    /// </summary>
    sealed internal class StringSubDivider : IStringDivider
    {
        /// <summary>
        /// This event is raised whenever the value is changed. This event does not propagate to the parent string divider.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Create a subdivider for the specified string divider.
        /// </summary>
        /// <param name="baseDivider">Divider to reference.</param>
        /// <param name="delimiter">Delimiter to search for.</param>
        /// <param name="parentIndex">Index within the parent to reference.</param>
        public StringSubDivider(IStringDivider baseDivider, char delimiter, int parentIndex)
        {
            BaseDivider = baseDivider;
            Index = parentIndex;
            Delimiter = delimiter;
            DelimiterString = new string(delimiter, 1);
        }

        /// <summary>
        /// Create a subdivider for the specified string divider.
        /// </summary>
        /// <param name="baseDivider">Divider to reference.</param>
        /// <param name="baseDividerOffset">Index of the character to use as the delimiter from the subdivided value.</param>
        /// <param name="parentIndex">Index within the parent to reference.</param>
        public StringSubDivider(IStringDivider baseDivider, int baseDividerOffset, int parentIndex)
        {
            BaseDivider = baseDivider;
            Index = parentIndex;
            Delimiter = baseDivider.Value[baseDividerOffset];
            DelimiterString = new string(Delimiter, 1);
        }

        /// <summary>
        /// Get or set the substring at the specified index.
        /// </summary>
        /// <param name="index">Index of the string to get or set.</param>
        /// <returns>Substring.</returns>
        public string this[int index]
        {
            get
            {
                var splits = Divisions;
                if (index >= splits.Count || index < 0)
                {
                    return null;
                }

                var split = splits[index];
                return new string(BaseValue, split.Offset, split.Length);
            }
            set
            {
                if (index < 0)
                {
                    return;
                }

                List<StringDivision> divisions;
                var paddedString = StringDividerOperations.GetPaddedString(Value, index, Delimiter, out divisions);
                if (index >= divisions.Count)
                {
                    Value = (index > 0) ? string.Join(paddedString, value) : value;
                }
                else
                {
                    var d = divisions[index];
                    Value = StringDividerOperations.GetSplicedString(paddedString, d.Offset, d.Length, value);
                }

                if (ValueChanged != null)
                {
                    ValueChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Parent divider.
        /// </summary>
        IStringDivider BaseDivider
        {
            get;
            set;
        }

        /// <summary>
        /// String that is operated upon, as a character array. This points to the parent divider's BaseValue.
        /// </summary>
        public char[] BaseValue
        {
            get { return BaseDivider.BaseValue; }
        }

        /// <summary>
        /// Get the number of divisions.
        /// </summary>
        public int Count
        {
            get { return Divisions.Count; }
        }

        /// <summary>
        /// Get the delimiter character.
        /// </summary>
        public char Delimiter
        {
            get;
            private set;
        }

        /// <summary>
        /// [PERF] Get the delimiter character as a string.
        /// </summary>
        private string DelimiterString { get; set; }

        /// <summary>
        /// Create a subdivision.
        /// </summary>
        /// <param name="index">Index of the subdivider in the parent divider.</param>
        /// <param name="delimiter">Delimiter to be used by the subdivider.</param>
        /// <returns>String subdivider.</returns>
        public IStringDivider Divide(int index, char delimiter)
        {
            return new StringSubDivider(this, delimiter, index);
        }

        /// <summary>
        /// [PERF] Cached divisions list.
        /// </summary>
        private IReadOnlyList<StringDivision> _divisions;

        /// <summary>
        /// Get the division offsets in the string.
        /// </summary>
        public IReadOnlyList<StringDivision> Divisions
        {
            get
            {
                if (_divisions == null || Version != BaseDivider.Version)
                {
                    Update();
                }
                return _divisions;
            }
        }

        /// <summary>
        /// Get an enumerator for divided strings.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            return new StringDividerEnumerator(this);
        }

        /// <summary>
        /// Get an enumerator for divided strings.
        /// </summary>
        /// <returns>Enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new StringDividerEnumerator(this);
        }

        /// <summary>
        /// Get the subdivision in which this division's item at the specified index resides.
        /// </summary>
        /// <param name="index">Index of the item to get.</param>
        /// <returns>Subdivision location.</returns>
        public StringDivision GetSubDivision(int index)
        {
            if (index < 0)
            {
                return null;
            }

            var d = Divisions;
            return (index >= d.Count)
                ? null
                : d[index];
        }

        /// <summary>
        /// Index inside the parent. This will always be zero for a StringDivider because it is a root divider.
        /// </summary>
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Get the internal value as a string.
        /// </summary>
        /// <returns>Value as a string.</returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// [PERF] Refresh internal division cache.
        /// </summary>
        void Update()
        {
            Version = BaseDivider.Version;
            _divisions = StringDividerOperations.GetDivisions(BaseValue, Delimiter, BaseDivider.GetSubDivision(Index));
        }

        /// <summary>
        /// Calculated value of all divisions separated by delimiters.
        /// </summary>
        public string Value
        {
            get
            {
                var d = BaseDivider.GetSubDivision(Index);
                return (d == null)
                    ? null
                    : new string(BaseValue, d.Offset, d.Length);
            }
            set { BaseDivider[Index] = value; }
        }

        public int Version
        {
            get;
            private set;
        }
    }
}
