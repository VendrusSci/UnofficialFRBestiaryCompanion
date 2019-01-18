using Bestiary.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Bestiary.ViewModel
{
    enum ImageType
    {
        Icons,
        Images
    }
    
    public class FamiliarViewModel : INotifyPropertyChanged
    {
        public FamiliarInfo Info { get; set; }
        public LocationTypes[] AvailableLocationTypes { get; private set; }
        public OwnershipStatus[] AvailableOwnershipStatuses { get; private set; }
        public BondingLevels[] AvailableBondingLevels { get; private set; }
        public BookmarkState[] AvailableBookmarkStates { get; private set; }

        private IModel m_Model;
        private BitmapImage m_Icon = null;
        public BitmapImage Icon
        {
            get
            {
                if(m_Icon == null)
                {
                    m_Icon = LoadImage(ImageType.Icons);
                }
                return m_Icon;
            }
        }
        private BitmapImage m_Image = null;
        public BitmapImage Image
        {
            get
            {
                if(m_Image == null)
                {
                    m_Image = LoadImage(ImageType.Images);
                }
                return m_Image;
            }
        }
 
        public event PropertyChangedEventHandler PropertyChanged;

        public FamiliarViewModel(IModel model, FamiliarInfo info, LocationTypes[] availableLocationTypes)
        {
            Info = info;
            AvailableLocationTypes = availableLocationTypes;
            AvailableOwnershipStatuses = ListEnumValues<OwnershipStatus>();
            AvailableBondingLevels = ListEnumValues<BondingLevels>();
            AvailableBookmarkStates = ListEnumValues<BookmarkState>();
            m_Model = model;

            SetResultsActions();

            PropertyChanged += (e, p) =>
            {
                MainViewModel.UserActionLog.Debug($"I, a familiar view model, just changed '{p.PropertyName}' ({Info.Familiar.Name})");
            };
        }

        private LambdaCommand m_SetOwned;
        public ICommand SetOwned
        {
            get
            {
                if(m_SetOwned == null)
                {
                    m_SetOwned = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            MainViewModel.UserActionLog.Info($"Setting familiar to owned: {Info.Familiar.Id}");
                            var ownedFamiliar = new OwnedFamiliar(Info.Familiar.Id, BondingLevels.Wary, LocationTypes.InHoard);
                            m_Model.AddOwnedFamiliar(ownedFamiliar);
                            Info.OwnedFamiliar = m_Model.LookupOwnedFamiliar(Info.Familiar.Id);
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Info"));
                        },
                        onCanExecute: (p) =>
                        {
                            return Info.Owned == OwnershipStatus.NotOwned;
                        }
                    );
                }
                return m_SetOwned;
            }
        }

        private LambdaCommand m_ToggleBookmark;
        public ICommand ToggleBookmark
        {
            get
            {
                if(m_ToggleBookmark == null)
                {
                    m_ToggleBookmark = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            if(Info.BookmarkedFamiliar == null)
                            {
                                MainViewModel.UserActionLog.Info($"Bookmarking familiar: {Info.Familiar.Id}");
                                m_Model.AddBookmarkedFamiliar(new BookmarkedFamiliar(Info.Familiar.Id));
                                Info.BookmarkedFamiliar = m_Model.LookupBookmarkedFamiliar(Info.Familiar.Id);
                                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Info"));
                            }
                            else
                            {
                                MainViewModel.UserActionLog.Info($"Removing bookmark from familiar: {Info.Familiar.Id}");
                                Info.BookmarkedFamiliar.Delete();
                                Info.BookmarkedFamiliar = null;
                                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Info"));
                            }
                        }
                    );
                }
                return m_ToggleBookmark;
            }
        }

        private BitmapImage LoadImage(ImageType type)
        {
            var resourcePath = ApplicationPaths.GetResourcesDirectory();
            var path = Path.Combine(resourcePath, $"{type}/{Info.Familiar.Id}.png");

            try
            {
                return ImageLoader.LoadImage(path);
            }
            catch(FileNotFoundException)
            {
                MainViewModel.UserActionLog.Info($"No {type} file found for familiar {Info.Familiar.Id}");
            }
            catch(DirectoryNotFoundException)
            {
                MainViewModel.UserActionLog.Info($"No resources folder, could not find {type} file for familiar {Info.Familiar.Id}");
            }

            return null;
        }

        public T[] ListEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>()
                .ToArray();
        }

        public ObservableCollection<ResultsAction> ResultsActions { get; private set; }
        private void SetResultsActions()
        {
            ResultsActions = new ObservableCollection<ResultsAction>();
            ResultsActions.Add(new ResultsAction("Set to Owned", SetOwned));
            ResultsActions.Add(new ResultsAction("Set to Not Owned", SetNotOwned));
            ResultsActions.Add(new ResultsAction("Set to Awakened", SetAwakened));
            ResultsActions.Add(new ResultsAction("Set to Wary", SetWary));
        }

        private LambdaCommand m_SetAwakened;
        public ICommand SetAwakened
        {
            get
            {
                if (m_SetAwakened == null)
                {
                    m_SetAwakened = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            if (Info.Owned == OwnershipStatus.Owned)
                            {
                                Info.BondLevel = BondingLevels.Awakened;
                            }
                        }
                    );
                }
                return m_SetAwakened;
            }
        }

        private LambdaCommand m_SetWary;
        public ICommand SetWary
        {
            get
            {
                if (m_SetWary == null)
                {
                    m_SetWary = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            if (Info.Owned == OwnershipStatus.Owned)
                            {
                                Info.BondLevel = BondingLevels.Wary;
                            }
                        }
                    );
                }
                return m_SetWary;
            }
        }

        private LambdaCommand m_SetNotOwned;
        public ICommand SetNotOwned
        {
            get
            {
                if (m_SetNotOwned == null)
                {
                    m_SetNotOwned = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            if (Info.Owned == OwnershipStatus.Owned)
                            {
                                Info.OwnedFamiliar.Delete();
                            }
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Info"));
                        }
                    );
                }
                return m_SetNotOwned;
            }
        }
    }

    public class ResultsAction : INotifyPropertyChanged
    {
        public string Name { get; private set; }
        public ICommand Action { get; private set; }

        public ResultsAction(string name, ICommand action)
        {
            Name = name;
            Action = action;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
