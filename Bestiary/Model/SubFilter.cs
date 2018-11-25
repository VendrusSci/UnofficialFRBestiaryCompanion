using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.Model
{
    interface IAmSubFilter
    {
        IEnumerable<FamiliarInfo> Apply(IEnumerable<FamiliarInfo> toFilter);
    }

    public class SubFilter<T, SourceType> : IAmSubFilter, INotifyPropertyChanged
        where T : class
        where SourceType : class
    {
        public string Name { get; set; }
        public T[] AvailableOptions { get; set; }

        private T m_SelectedOption = null;
        public T SelectedOption
        {
            get { return m_SelectedOption; }
            set
            {
                m_SelectedOption = value;
                m_OnSet?.Invoke(m_SelectedOption);
            }
        }

        public SubFilter(string name, T[] availableOptions, T selectedOption, Func<SourceType, T> getKey, Func<T, T, bool> compare = null, Action<T> onSet = null)
        {
            Name = name;
            AvailableOptions = availableOptions;
            SelectedOption = selectedOption;
            m_GetKey = getKey;
            m_Compare = compare;
            m_OnSet = onSet;
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
        private Action<T> m_OnSet;

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class EnumSubFilter<T, SourceType> : IAmSubFilter, INotifyPropertyChanged
        where T : struct
        where SourceType : class
    {
        public string Name { get; set; }
        public T[] AvailableOptions { get; set; }
        private T? m_SelectedOption = null;
        public T? SelectedOption
        {
            get { return m_SelectedOption; }
            set
            {
                m_SelectedOption = value;
                m_OnSet?.Invoke(m_SelectedOption);
            }
        }

        public EnumSubFilter(string name, T[] availableOptions, T? selectedOption, Func<SourceType, T> getKey, Func<T, T, bool> compare = null, Action<T?> onSet = null)
        {
            Name = name;
            AvailableOptions = availableOptions;
            SelectedOption = selectedOption;
            m_GetKey = getKey;
            m_Compare = compare;
            m_OnSet = onSet;
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
        private Action<T?> m_OnSet;

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ListSubFilter<T, SourceType> : IAmSubFilter, INotifyPropertyChanged
    where T : struct
    where SourceType : class
    {
        public string Name { get; set; }
        public T[] AvailableOptions { get; set; }
        private T? m_SelectedOption = null;
        public T? SelectedOption
        {
            get { return m_SelectedOption; }
            set
            {
                m_SelectedOption = value;
                m_OnSet?.Invoke(m_SelectedOption);
            }
        }

        public ListSubFilter(string name, T[] availableOptions, T? selectedOption, Func<SourceType, List<T>> getKey, Action<T?> onSet = null)
        {
            Name = name;
            AvailableOptions = availableOptions;
            SelectedOption = selectedOption;
            m_GetKey = getKey;
            m_OnSet = onSet;
            if (m_Compare == null)
            {
                m_Compare = (a, b) => a.Contains(b);
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

        private Func<SourceType, List<T>> m_GetKey;
        private Func<List<T>, T, bool> m_Compare;
        private Action<T?> m_OnSet;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
