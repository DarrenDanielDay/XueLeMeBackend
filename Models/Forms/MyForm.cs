using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models.Forms
{
    public class MyForm
    {
        public static bool ReflectCheck(object form)
        {
            Type type = form.GetType();
            var attributes = type.GetProperties();
            return attributes.All(attribute => !(attribute.CanRead && attribute.GetValue(form) == null));
        }
    }
}
