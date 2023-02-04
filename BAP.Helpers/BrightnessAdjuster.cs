using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColorHelper;

namespace BAP.Helpers
{
    public static class BrightnessAdjuster
    {
        /// <summary>
        /// Darkens an BapColor
        /// </summary>
        /// <param name="bapColor">Color to adjust</param>
        /// <param name="darkenPercentage">Amount to darken - must be between 0 and 1</param>
        /// <returns></returns>
        public static BapColor Darken(BapColor bapColor, double darkenPercentage)
        {
            HSL h = ColorConverter.RgbToHsl(new RGB(bapColor.Red, bapColor.Green, bapColor.Blue));
            double newLuminosity = Convert.ToDouble(h.L) * darkenPercentage;
            newLuminosity = newLuminosity > 240 ? 240 : newLuminosity < 0 ? 0 : newLuminosity;
            h.L = Convert.ToByte(newLuminosity);
            RGB r = ColorConverter.HslToRgb(h);
            return new BapColor(r.R, r.G, r.B);
        }
        /// <summary>
        /// Darkens an BapColor
        /// </summary>
        /// <param name="bapColor">Color to adjust</param>
        /// <param name="lightenercentage">Amount to lighten - should be between 0 and 1 but in theory could be a little higher?</param>
        /// <returns></returns>
        public static BapColor Brighten(BapColor bapColor, double brightenPercentage)
        {
            HSL h = ColorConverter.RgbToHsl(new RGB(bapColor.Red, bapColor.Green, bapColor.Blue));
            double newLuminosity = Convert.ToDouble(h.L) * (1.0 + brightenPercentage);
            newLuminosity = newLuminosity > 240 ? 240 : newLuminosity < 0 ? 0 : newLuminosity;
            h.L = Convert.ToByte(newLuminosity);
            RGB r = ColorConverter.HslToRgb(h);
            return new BapColor(r.R, r.G, r.B);
        }
    }
}
