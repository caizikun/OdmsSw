using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neon.Aligner
{
    public class FormLogic<TMainForm> where TMainForm : Form
    {

        /// <summary>
        /// TForm 형식의 자식 창을 연다.
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <param name="showIfExist">true : Form 생성 후 Show() 호출</param>
        /// <returns></returns>
        public static TForm CreateAndShow<TForm>(bool showIfExist = true, bool createNew = true) where TForm : Form, new()
        {
            var mainForm = Application.OpenForms.OfType<TMainForm>().First();

            return (TForm)mainForm.Invoke((Func<TForm>)(() =>
            {
                var forms = Application.OpenForms.OfType<TForm>();

                TForm form;
                if (forms.Count() == 0)
                {
                    if (createNew) form = new TForm();
                    else return null;
                }
                else form = forms.First();

                form.MdiParent = mainForm;
                if (showIfExist) form.Show();
                return form;
            }));

        }//ShowOrNew

    }
}
