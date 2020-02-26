//using System;
using System.IO;
using Android.Content;
using Android.Database;
using Android.Net;
using Android.OS;
using Android.Provider;
using arpos_SM.Asset;
using arpos_SM.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]
namespace arpos_SM.Droid
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            //string path = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

            return Path.Combine(path, filename);

            
        }

        //public string GetLocalDownloadPath(string filename)
        //{


        //    string root = Android.OS.Environment.DirectoryDownloads;
        //    string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        //    return Path.Combine(root, filename);
        //}

        public void CopyFile(Context context, Uri uri)
        {
            const int bufferSize = 1024;

            using (Stream inputStream = context.ContentResolver.OpenInputStream(uri))
            {
                using (FileStream outputStream = File.Create(GetLocalFilePath("DataImport.csv")))
                {
                    var buffer = new byte[bufferSize];
                    while (true)
                    {
                        var count = inputStream.Read(buffer, 0, bufferSize);
                        if (count > 0)
                        {
                            outputStream.Write(buffer, 0, count);
                        }

                        if (count < bufferSize) break;
                    }
                }
            }
        }



        public string GetLocalDownloadPath(string uriString)
        {
            bool isKitKat = Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat;

            Android.Net.Uri uri = Android.Net.Uri.Parse(uriString);
            //Uri uri = data.Data;
            string path = "";
            

            if (!isKitKat)
            {
                //the usual function
                //path = GetPathToImage(uri);

            }
            else
            {
                

                //Android.App.Application.Contex
                //bool isdoc = DocumentsContract.IsDocumentUri(this, uri);
                bool isdoc = DocumentsContract.IsDocumentUri(Android.App.Application.Context, uri);
                if (isdoc)
                {
                    if(IsGoogleDrive(uri))
                    {
                        CopyFile(Android.App.Application.Context, uri);
                        return uri.Path;
                    }

                    if (IsExternalStorageDocument(uri))
                    {
                        CopyFile(Android.App.Application.Context, uri);
                        ////Actually Here i don t know how to handle all possibility.......
                        //string docId = DocumentsContract.GetDocumentId(uri);
                        //string[] split = docId.Split(':');
                        //string type = split[0];

                        //if ("primary".Equals(type))
                        //{
                        //    //return Android.OS.Environment.GetExternalStoragePublicDirectory() + "/" + split[1];
                        //    return Environment.GetExternalStoragePublicDirectory().ToString()
                        //        + "/"
                        //        + split[1];
                        //}


                        return uri.Path;

                    }
                    else if (IsDownloadsDocument(uri))
                    {
                        CopyFile(Android.App.Application.Context, uri);
                        string id = DocumentsContract.GetDocumentId(uri);
                        
                        ////content://com.android.providers.downloads.documents/document/21
                        ////Uri contentUri = ContentUris.WithAppendedId(Uri.Parse("content://downloads/public_downloads"), System.Convert.ToInt64(id));
                        //Uri contentUri = ContentUris.WithAppendedId(Uri.Parse("content://downloads/my_downloads"), System.Convert.ToInt64(id));
                        ////Uri contentUri = ContentUris.WithAppendedId(Uri.Parse("content://com.android.providers.downloads.documents/document"), System.Convert.ToInt64(id));

                        //path = GetDataColumn(Android.App.Application.Context, contentUri, null, null);

                        string[] contentUriPrefixesToTry = new string[]
                        {
                            "content://downloads/public_downloads",
                            "content://downloads/my_downloads"
                        };

                        foreach (string contentUriPrefix in contentUriPrefixesToTry)
                        {
                            Android.Net.Uri contentUri = ContentUris.WithAppendedId(
                                Android.Net.Uri.Parse(contentUriPrefix), long.Parse(id));

                            try
                            {
                                path = GetDataColumn(Android.App.Application.Context, contentUri, null, null);
                                if (path != null)
                                {
                                    return path;
                                }
                            }
                            catch (System.Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine("Image Exception : " + ex.Message);
                                // ignore exception; path can't be retrieved using ContentResolver
                            }
                        }

                    }
                    else if (IsMediaDocument(uri))
                    {

                        string docId = DocumentsContract.GetDocumentId(uri);
                        string[] split = docId.Split(':');

                        string type = split[0];

                        Uri contentUri = null;
                        if ("image".Equals(type))
                        {
                            contentUri = MediaStore.Images.Media.ExternalContentUri;
                        }
                        else if ("video".Equals(type))
                        {
                            contentUri = MediaStore.Video.Media.ExternalContentUri;
                        }
                        else if ("audio".Equals(type))
                        {
                            contentUri = MediaStore.Audio.Media.ExternalContentUri;
                        }

                        string selection = "_id=?";
                        string[] selectionArgs = new string[] { split[1] };

                        path = GetDataColumn(Android.App.Application.Context, contentUri, selection, selectionArgs);

                    }

                }
            }
            return path;

        }

        private string GetDataColumn(Context context, Uri uri, string selection, string[] selectionArgs)
        {

            ICursor cursor = null;
            string column = "_data";
            string[] projection = { column };

            try
            {
                //Error : 'Unknown URI: content://downloads/public_downloads/21'
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(index);
                }
            }
            finally
            {
                if (cursor != null)
                    cursor.Close();
            }
            return null;
        }

        private bool IsExternalStorageDocument(Uri uri)
        {
            return "com.android.externalstorage.documents".Equals(uri.Authority);
        }

        private bool IsDownloadsDocument(Uri uri)
        {
            return "com.android.providers.downloads.documents".Equals(uri.Authority);
        }

        private bool IsMediaDocument(Uri uri)
        {
            return "com.android.providers.media.documents".Equals(uri.Authority);
        }

        private bool IsGooglePhotosUri(Uri uri)
        {
            return "com.google.android.apps.photos.content".Equals(uri.Authority);
        }

        private bool IsGoogleDrive(Uri uri)
        {
            return "com.google.android.apps.docs.storage".Equals(uri.Authority);
        }

    }
}