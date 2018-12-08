﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Bestiary.Model
{
    public class FamiliarInfo : INotifyPropertyChanged
    {
        private ICRUD<Familiar> KnownFamiliar;

        private ICRUD<OwnedFamiliar> m_OwnedFamiliar;
        public ICRUD<OwnedFamiliar> OwnedFamiliar
        {
            get { return m_OwnedFamiliar; }
            set
            {
                if(m_OwnedFamiliar != value)
                {
                    m_OwnedFamiliar = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BondLevel"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Owned"));
                }
            }
        }

        public Familiar Familiar
        {
            get
            {
                return KnownFamiliar.Fetch();
            }
            private set{}
        }
        public OwnershipStatus Owned => OwnedFamiliar != null ? OwnershipStatus.Owned : OwnershipStatus.NotOwned;
        public BondingLevels? BondLevel
        {
            get
            {
                return OwnedFamiliar?.Fetch()?.BondingLevel;
            }
            set
            {
                if (value.HasValue)
                {
                    OwnedFamiliar?.Update(f => f.BondingLevel = value.Value);
                }
            }
        }
        public LocationTypes? Location
        {
            get
            {
                return OwnedFamiliar?.Fetch()?.Location;
            }
            set
            {
                if (value.HasValue)
                {
                    OwnedFamiliar?.Update(f => f.Location = value.Value);
                }
            }
        }

        public FamiliarInfo(ICRUD<Familiar> familiar, ICRUD<OwnedFamiliar> ownedFamiliar)
        {
            KnownFamiliar = familiar;
            OwnedFamiliar = ownedFamiliar;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class OwnedToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            OwnershipStatus status = (OwnershipStatus)value;
            if (status == OwnershipStatus.Owned)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    interface IFamiliarProvider
    {
        FamiliarInfo[] FetchFamiliars();
    }
}
