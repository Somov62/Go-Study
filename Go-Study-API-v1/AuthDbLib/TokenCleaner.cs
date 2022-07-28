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

            var expireTokens = db.UserTokens.Where(p => p.DateExpire < DateTime.Now && p.Token != string.Empty).ToList();
            try 
            {
                db.UserTokens.RemoveRange(expireTokens);
                db.SaveChanges(); 
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("error");
                #region log
                LoggerLib.Logger.GetContext().CreateLog(
                    new LoggerLib.LogModels.ExceptionLogModel()
                    {
                        Type = LoggerLib.LogType.Error,
                        Exception = ex,
                        CurrentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name,
                        Message = $"Ошибка при очистке устаревших токенов."
                    });
                #endregion
            }
        }

        public void StopChecking()
        {
            _timer.Stop();
        }

    }
}
