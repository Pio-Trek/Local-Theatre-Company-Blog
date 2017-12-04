using System;
using System.Web;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace LocalTheatre.Models
{
    public static class GetFromContext
    {
        private static readonly ApplicationDbContext Db = new ApplicationDbContext();

        public static string AuthorName(string id)
        {
            var authorName = Message.NoUserFound;

            try
            {
                authorName = HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindById(id)
                    .DisplayName;
            }
            catch
            {
                // ignored
            }

            return authorName;
        }

        public static string PostTitle(int id)
        {
            var item = Db.BlogPosts.Find(id)?.Title;

            return item;
        }

        public static string AccountStatus(bool isEnabled)
        {
            if (isEnabled)
            {
                return "Enabled";
            }
            else
            {
                return "Disabled";
            }
        }

        public static string ProfileUrl(string id)
        {
            string imageUrl = "";

            try
            {
                imageUrl = HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindById(id)
                    .ProfileUrl;
            }
            catch
            {
                // ignored
            }
            finally
            {
                if (imageUrl.IsNullOrWhiteSpace())
                {
                    imageUrl = "/images/default-avatar.png";
                }
            }

            return imageUrl;
        }

        public static string ProfileFileName(string id)
        {
            var imageUrl = HttpContext.Current.GetOwinContext()
                .GetUserManager<ApplicationUserManager>()
                .FindById(id)
                .ProfileFileName;

            if (imageUrl.IsNullOrWhiteSpace())
            {
                imageUrl = "/images/default-avatar.png";
            }

            return imageUrl;
        }

        public static double DayCount(string commentDate)
        {
            var startDate = DateTime.Parse(commentDate);
            var elapsed = DateTime.Now.Subtract(startDate);
            return elapsed.TotalDays;
        }
    }
}