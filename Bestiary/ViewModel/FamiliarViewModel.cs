using Bestiary.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Bestiary.ViewModel
{
    enum ImageType
    {
        Icons,
        Images
    }
    
    class FamiliarViewModel : INotifyPropertyChanged
    {
        public FamiliarInfo Info { get; set; }
        public LocationTypes[] AvailableLocationTypes { get; private set; }

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
                if(m_Image ==  null)
                {
                    m_Image = LoadImage(ImageType.Images);
                }
                return m_Image;
            }
        }
        private LambdaCommand m_SetOwned;
        private LambdaCommand m_IncrementBondingLevel;

        public event PropertyChangedEventHandler PropertyChanged;

        public FamiliarViewModel(IModel model, FamiliarInfo info, LocationTypes[] availableLocationTypes)
        {
            Info = info;
            AvailableLocationTypes = availableLocationTypes;
            m_Model = model;
        }

        public ICommand SetOwned
        {
            get
            {
                if(m_SetOwned == null)
                {
                    m_SetOwned = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            var ownedFamiliar = new OwnedFamiliar(Info.Familiar.Id, BondingLevels.Wary, LocationTypes.InHoard);
                            m_Model.AddOwnedFamiliar(ownedFamiliar);
                            Info.OwnedFamiliar = m_Model.LookupOwnedFamiliar(Info.Familiar.Id);
                            Info.Owned = Info.OwnedFamiliar != null ? OwnershipStatus.Owned : OwnershipStatus.NotOwned;
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

        public ICommand IncrementBondingLevel
        {
            get
            {
                if(m_IncrementBondingLevel == null)
                {
                    m_IncrementBondingLevel = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            Info.BondLevel++;
                        },
                        onCanExecute: (p) =>
                        {
                            return Info.BondLevel < BondingLevels.Awakened;
                        }
                    );
                }
                return m_IncrementBondingLevel;
            }
        }

        private BitmapImage LoadImage(ImageType type)
        {
            string path = $"{type}/{Info.Familiar.Id}.png";

            try
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();

                return bitmapImage;
            }
            catch(FileNotFoundException)
            {
                // :(
            }

            return null;
        }
    }
}
