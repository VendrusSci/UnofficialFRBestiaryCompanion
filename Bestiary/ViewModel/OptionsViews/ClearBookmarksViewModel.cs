using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Bestiary.Model;

namespace Bestiary.ViewModel
{
    public class ClearBookmarksViewModel : INotifyPropertyChanged
    {
        private IModel m_Model;
        public ClearBookmarksViewModel(IModel model)
        {
            m_Model = model;
        }

        private LambdaCommand m_ClearBookmarks;
        public ICommand ClearBookmarks
        {
            get
            {
                if(m_ClearBookmarks == null)
                {
                    m_ClearBookmarks = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            var tempFamList = m_Model.BookmarkedFamiliars.ToArray();
                            foreach(var id in tempFamList)
                            {
                                m_Model.LookupBookmarkedFamiliar(id).Delete();
                            }
                            File.Delete(Path.Combine(ApplicationPaths.GetDataDirectory(), "User Data/BookmarkData.xml"));

                        },
                        onCanExecute: (p) =>
                        {
                            return File.Exists(Path.Combine(ApplicationPaths.GetDataDirectory(), "User Data/BookmarkData.xml"));
                        }
                    );
                }
                return m_ClearBookmarks;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
