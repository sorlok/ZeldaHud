using System;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.CSharp;
using System.Collections.Generic;

namespace EvalCSCode
{
    public class EvalCSCode
    {

        public static Tuple<object, MethodInfo> GetWithParamType<T1, T2, T3>(string decl, string sCSCode, string p1Name, string p2Name, string p3Name)
        {
            CSharpCodeProvider c = new CSharpCodeProvider();
            ICodeCompiler icc = c.CreateCompiler();
            CompilerParameters cp = new CompilerParameters();

            cp.ReferencedAssemblies.Add("system.dll");
            cp.ReferencedAssemblies.Add("system.xml.dll");
            cp.ReferencedAssemblies.Add("system.data.dll");
            cp.ReferencedAssemblies.Add("system.windows.forms.dll");
            cp.ReferencedAssemblies.Add("system.drawing.dll");
            //cp.ReferencedAssemblies.Add("EvalCSCode.exe");

            cp.CompilerOptions = "/t:library";
            cp.GenerateInMemory = true;
            StringBuilder sb = new StringBuilder("");

            sb.Append("using System;\n");
            sb.Append("using System.Xml;\n");
            sb.Append("using System.Data;\n");
            sb.Append("using System.Data.SqlClient;\n");
            sb.Append("using System.Windows.Forms;\n");
            sb.Append("using System.Drawing;\n");
            sb.Append("using System.Collections.Generic;\n");
            //sb.Append("using EvalCSCode;\n");

            sb.Append("namespace CSCodeEvaler{ \n");
            sb.Append("public class CSCodeEvaler{ \n");
            sb.Append(decl);
            sb.Append("public object EvalCode(" + typeof(T1).FullName + " " + p1Name + ", " + typeof(T2).FullName + " " + p2Name + ", " + typeof(T3).FullName + " " + p3Name + "){\n");
            //sb.Append("MessageBox.Show(oParam.ToString()); \n");
            //sb.Append("Func<bool, int, Tuple<bool, int>> T = Tuple.Create<bool, int>;\n");
            sb.Append("return new " + sCSCode + "; \n");
            sb.Append("} \n");
            sb.Append("} \n");
            sb.Append("}\n");
            //Debug.WriteLine(sb.ToString())// ' look at this to debug your eval string

            //CompilerResults cr = icc.CompileAssemblyFromSource(cp, sb.ToString());
            CompilerResults cr = c.CompileAssemblyFromSource(cp, sb.ToString());
            if (cr.Errors.Count > 0)
            {
                MessageBox.Show("ERROR: " + cr.Errors[0].ErrorText, "Error evaluating cs code", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            System.Reflection.Assembly a = cr.CompiledAssembly;
            object o = a.CreateInstance("CSCodeEvaler.CSCodeEvaler");
            Type t = o.GetType();
            MethodInfo mi = t.GetMethod("EvalCode");
            return Tuple.Create(o, mi);
        }
    }
}
//End Namespace