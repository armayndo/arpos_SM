using System;
using System.Collections.Generic;
using System.Text;

namespace arpos_SM.Asset
{
    public interface IDataViewer
    {
        void showPhoto(string AttachmentName, byte[] AttachmentBytes);
        string ImageExists(string Filename, byte[] ImageData);

        byte[] ReadAllByteS(string path);

        void Share(string subject, string message, string imgPath);
    }
}
