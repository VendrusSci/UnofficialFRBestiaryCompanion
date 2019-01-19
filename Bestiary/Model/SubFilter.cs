using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bestiary.Model
{
    public interface IAmSubFilter
    {
        IEnumerable<FamiliarInfo> Apply(IEnumerable<FamiliarInfo> toFilter);
        ICommand Clear { get; }
        bool Invert { get; set; }
    }

    public class SubFilter<T, SourceType> : IAmSubFilter, INotifyPropertyChanged
        where T : class
        where SourceType : class
    {
        public string Name { get; set; }
        public bool Invert { get; set; }
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

        private LambdaCommand m_Clear;
        public ICommand Clear
        {
            get
            {
                if(m_Clear == null)
                {
                    m_Clear = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            SelectedOption = null;
                        },
                        onCanExecute: (p) =>
                        {
                            return SelectedOption != null;
                        }
                    );
                }
                return m_Clear;
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
                    if(Invert)
                    {
                        matches = !matches;
                    }
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
        public bool Invert { get; set; }
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

        private LambdaCommand m_Clear;
        public ICommand Clear
        {
            get
            {
                if (m_Clear == null)
                {
                    m_Clear = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            SelectedOption = null;
                        },
                        onCanExecute: (p) =>
                        {
                            return SelectedOption != null;
                        }
                    );
                }
                return m_Clear;
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

                    if(Invert)
                    {
                        matches = !matches;
                    }

                    return matches;
                });
        }

        private Func<SourceType, T> m_GetKey;
        private Func<T, T, bool> m_Compare;
        private Action<T?> m_OnSet;

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class List<T, SourceType> : IAmSubFilter, INotifyPropertyChanged
    where T : struct
    where SourceType : class
    {
        public string Name { get; set; }
        public bool Invert { get; set; }
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

        public List(string name, T[] availableOptions, T? selectedOption, Func<SourceType, List<T>> getKey, Action<T?> onSet = null)
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
                    if(Invert)
                    {
                        matches = !matches;
                    }
                    return matches;
                });
        }

        private LambdaCommand m_Clear;
        public ICommand Clear
        {
            get
            {
                if (m_Clear == null)
                {
                    m_Clear = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            SelectedOption = null;
                        },
                        onCanExecute: (p) =>
                        {
                            return SelectedOption != null;
                        }
                    );
                }
                return m_Clear;
            }
        }

        private Func<SourceType, List<T>> m_GetKey;
        private Func<List<T>, T, bool> m_Compare;
        private Action<T?> m_OnSet;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
