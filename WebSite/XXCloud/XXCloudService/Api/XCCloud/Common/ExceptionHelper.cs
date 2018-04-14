using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;

namespace XXCloudService.Api.XCCloud.Common
{
    public static class ExceptionHelper
    {
        public static string ToErrors(this IEnumerable<DbEntityValidationResult> entityValidationErrors)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var eve in entityValidationErrors)
            {
                sb.Append(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    eve.Entry.Entity.GetType().Name, eve.Entry.State));
                foreach (var ve in eve.ValidationErrors)
                {
                    sb.Append(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                        ve.PropertyName, ve.ErrorMessage));
                }
            }
            return sb.ToString();
        }
    }
}