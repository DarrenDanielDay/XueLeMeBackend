using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static TForm Clone<TForm>(TForm form)
        {
            Type type = form.GetType();
            if (form is string || type.IsValueType)
            {
                return form;
            }
            TForm newForm = Activator.CreateInstance<TForm>();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (var field in fields)
            {
                try
                {
                    field.SetValue(newForm, Clone(field.GetValue(form)));
                }
                catch { }
            }

            return newForm;
        }
    }
}
