using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.Helpers
{
    public static class SessionHelper
    {
        public static void SetObjectAsJason(this ISession session ,string Key , object value)
        {
            session.SetString(Key, JsonConvert.SerializeObject(value));

        }

        public static T GetObjectfromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
       
    }
}
