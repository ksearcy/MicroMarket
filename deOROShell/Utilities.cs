using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using deOROMonitor.Assistant.Utils;

namespace deOROMonitor
{
    class Utilities
    {
        private static Regex _gtinRegex;

        private static Regex validUpcRegex;

        static Utilities()
        {
            Utilities._gtinRegex = new Regex("^(\\d{8}|\\d{12,14})$");
            Utilities.validUpcRegex = new Regex("^(\\d|\\w){4,20}$");
        }

        public Utilities()
        {
        }

        internal static void AddFileToZip(string zipFilename, string fileToAdd)
        {
            using (Package package = Package.Open(zipFilename, FileMode.OpenOrCreate))
            {
                Uri uri = PackUriHelper.CreatePartUri(new Uri(string.Concat(".\\", Path.GetFileName(fileToAdd)), UriKind.Relative));
                if (package.PartExists(uri))
                {
                    package.DeletePart(uri);
                }
                PackagePart packagePart = package.CreatePart(uri, "", CompressionOption.Normal);
                using (FileStream fileStream = new FileStream(fileToAdd, FileMode.Open, FileAccess.Read))
                {
                    using (Stream stream = packagePart.GetStream())
                    {
                        Utilities.CopyStream(fileStream, stream);
                    }
                }
            }
        }



    }
}
