using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.Model
{
    interface IAmSubFilter
    {
        IEnumerable<FamiliarInfo> Apply(IEnumerable<FamiliarInfo> toFilter);
    }

    class SubFilter<T, SourceType> : IAmSubFilter
        where T : class
        where SourceType : class
    {
        public string Name { get; set; }
        public T[] AvailableOptions { get; set; }
        public T SelectedOption { get; set; }

        public SubFilter(string name, T[] availableOptions, T selectedOption, Func<SourceType, T> getKey, Func<T, T, bool> compare = null)
        {
            Name = name;
            AvailableOptions = availableOptions;
            SelectedOption = selectedOption;
            m_GetKey = getKey;
            m_Compare = compare;
            if (m_Compare == null)
            {
                m_Compare = (a, b) => a.Equals(b);
            }
        }

        public IEnumerable<FamiliarInfo> Apply(IEnumerable<FamiliarInfo> toFilter)
        {
            if (SelectedOption == null)
            {
                return toFilter;
            }

            return toFilter
                .Where(f =>
                {
                    var source = f.Familiar.Source as SourceType;
                    if (source == null)
                    {
                        return false;
                    }

                    var key = m_GetKey(source);
                    var matches = m_Compare(key, SelectedOption);

                    return matches;
                });
        }

        private Func<SourceType, T> m_GetKey;
        private Func<T, T, bool> m_Compare;
    }

    class EnumSubFilter<T, SourceType> : IAmSubFilter
        where T : struct
        where SourceType : class
    {
        public string Name { get; set; }
        public T[] AvailableOptions { get; set; }
        public T? SelectedOption { get; set; }

        public EnumSubFilter(string name, T[] availableOptions, T? selectedOption, Func<SourceType, T> getKey, Func<T, T, bool> compare = null)
        {
            Name = name;
            AvailableOptions = availableOptions;
            SelectedOption = selectedOption;
            m_GetKey = getKey;
            m_Compare = compare;
            if (m_Compare == null)
            {
                m_Compare = (a, b) => a.Equals(b);
            }
        }

        public IEnumerable<FamiliarInfo> Apply(IEnumerable<FamiliarInfo> toFilter)
        {
            if (SelectedOption == null)
            {
                return toFilter;
            }

            return toFilter
                .Where(f =>
                {
                    var source = f.Familiar.Source as SourceType;
                    if (source == null)
                    {
                        return false;
                    }

                    var key = m_GetKey(source);
                    var matches = m_Compare(key, SelectedOption.Value);

                    return matches;
                });
        }

        private Func<SourceType, T> m_GetKey;
        private Func<T, T, bool> m_Compare;
    }
}
