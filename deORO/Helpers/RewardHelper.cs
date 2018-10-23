using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.Helpers
{
    public class RewardHelper
    {
        public static decimal GetRewardAmount(decimal refillAmount)
        {
            if (refillAmount >= 1 && refillAmount < 5)
            {
                return Convert.ToDecimal(Global.RefillRewardTier1, System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (refillAmount >= 5 && refillAmount < 10)
            {
                return Convert.ToDecimal(Global.RefillRewardTier2, System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (refillAmount >= 10 && refillAmount < 20)
            {
                return Convert.ToDecimal(Global.RefillRewardTier3, System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (refillAmount >= 20)
            {
                return Convert.ToDecimal(Global.RefillRewardTier4, System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                return 0;
            }
        }
    }
}
