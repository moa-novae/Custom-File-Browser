using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q1
{
    class UserInterfaceViewModel
    {
        #region Public properties

        public DirectoryStructureViewModel FileBrowser { get; set; }

        #endregion
        #region Constructor

        public UserInterfaceViewModel ()
        {
            this.FileBrowser = new DirectoryStructureViewModel();
  
        }

        #endregion
    }
}
