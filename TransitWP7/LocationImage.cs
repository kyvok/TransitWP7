using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace TransitWP7
{
    // This is a class that returns an image based on location
    public static class LocationImage
    {
        public static string GetImagePath(string StateOrProvince)
        {
            switch (StateOrProvince)
            {
                case "WA":
                    return @"Images\Seattle\seattle000.jpg";
                default:
                    break;
            }
            return "";
        }
    }
}
