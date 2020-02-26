using System;
using System.Collections.Generic;
using System.Text;

namespace arpos_SM.Asset
{
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);

        string GetLocalDownloadPath(string filename);
    }
}
