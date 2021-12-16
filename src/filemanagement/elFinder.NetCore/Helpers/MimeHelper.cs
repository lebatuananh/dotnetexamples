﻿using elFinder.NetCore.Models;

namespace elFinder.NetCore.Helpers
{
    public static class MimeHelper
    {
        public static MimeType GetMimeType(string extension)
        {
            // TODO:
            //  1. Ensure all values from MimeTypes.txt are in MimeTypeMap and then delete MimeTypes.txt.
            //  2. Compare MimeTypeMap with: https://codingislove.com/list-mime-types-2016/
            //  3. Compare MimeTypeMap with: https://slick.pl/kb/htaccess/complete-list-mime-types/
            //  4. Consider modifying MimeTypeMap to return our custom MimeType struct, so that we don't need this MimeHelper class anymore

            var mimeType = MimeTypeMap.GetMimeType(extension);
            var split = mimeType.Split('/');
            return new MimeType { Type = split[0], Subtype = split[1] };
        }
    }
}