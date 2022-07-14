using System;
using System.Linq;
using System.Timers;
using DataBaseCore;

namespace AuthDbLib
{
    public class TokenCleaner
    {
        private readonly Timer _timer = new Timer();
        public TokenCleaner()
        {
            _timer.Interval = 5000;
            _timer.Elapsed += CheckExpireTokens;
            _timer.Start();
        }

        private void CheckExpireTokens(object sender, ElapsedEventArgs e)
        {
            var db = DbEntities.GetContext();

            var expireTokens = db.UserTokens.Where(p => p.DateExpire < DateTime.Now).ToList();
            foreach (var item in expireTokens)
            {
                ClearUserToken(item);
            }
        }

        public void ClearUserToken(UserToken user)
        {
            var db = DbEntities.GetContext();
            user.DateExpire = default;
            user.Token = string.Empty;
            user.RefreshToken = string.Empty;
            db.SaveChanges();
        }

        public void StopChecking()
        {
            _timer.Stop();
        }

    }
}
