namespace Pulstar.Web.Extensions
{
    using System;

    public static class RazorExtensions
    {
        public static string ToBase64(this byte[] image)
        {
            var base64 = Convert.ToBase64String(image);
            var imgSrc = string.Format("data:image/gif;base64,{0}", base64);
            return imgSrc;
        }
    }
}
