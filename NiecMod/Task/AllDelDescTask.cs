using System;
using System.Collections.Generic;
using System.Text;
using NiecMod.Nra;
using Sims3.SimIFace;
using System.Reflection;
using NiecMod.KillNiec;

namespace Sims3.NiecHelp.Tasks
{



    public class MethodStore
    {
        string mAssemblyName;
        string mClassName;
        string mRoutineName;
        Type[] mParameters;

        StringBuilder mError = new StringBuilder();

        public MethodInfo mMethod = null;
        public bool mChecked = false;

        public MethodStore(string assemblyName, string className, string routineName, Type[] parameters)
        {
            mAssemblyName = assemblyName;
            mClassName = className;
            mRoutineName = routineName;
            mParameters = parameters;
        }
        public MethodStore(string methodName, Type[] parameters)
        {
            if ((methodName != null) && (methodName.Contains(",")))
            {
                string[] strArray = methodName.Split(new char[] { ',' });

                mClassName = strArray[0];
                mAssemblyName = strArray[1];
                mRoutineName = strArray[2].Replace(" ", "");
            }

            mParameters = parameters;
        }

        public bool Valid
        {
            get { return LookupRoutine(); }
        }

        public string Error
        {
            get
            {
                LookupRoutine();
                return mError.ToString();
            }
        }

        public MethodInfo Method
        {
            get
            {
                LookupRoutine();
                return mMethod;
            }
        }

        public override string ToString()
        {
            return mMethod + " (" + mAssemblyName + "." + mClassName + "." + mRoutineName + ")";
        }

        protected bool LookupRoutine()
        {
            
            if (!mChecked)
            {
                //mError += "Assembly: " + mAssemblyName + NiecException.NewLine + "ClassName: " + mClassName + NiecException.NewLine + "RoutineName: " + mRoutineName;

                try
                {
                    mChecked = true;

                    if (!string.IsNullOrEmpty(mAssemblyName))
                    {
                        Assembly assembly = AssemblyCheckByNiec.FindAssembly(mAssemblyName);
                        if (assembly != null)
                        {
                            //mError += NiecException.NewLine + " Assembly Found: " + assembly.FullName;

                            Type type = assembly.GetType(mClassName);
                            if (type != null)
                            {
                                //mError += Common.NewLine + " Class Found: " + type.ToString();

                                if (mParameters != null)
                                {
                                    mMethod = type.GetMethod(mRoutineName, mParameters);
                                }
                                else
                                {
                                    mMethod = type.GetMethod(mRoutineName);
                                }

                                //if (mMethod != null)
                                //{
                                //    //mError += Common.NewLine + " Routine Found";
                                //}
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //mError += Common.NewLine + "Exception";
                    NiecException.WriteLog("Error " + e.ToString(), true, true);
                }
                finally
                {
                    //Common.WriteLog(mError);
                }
            }
            
            return (mMethod != null);
        }

        public T Invoke<T>(object[] parameters)
        {
            return Invoke<T>(null, parameters);
        }
        public T Invoke<T>(object obj, object[] parameters)
        {
            if (!Valid) return default(T);

            try
            {
                return (T)mMethod.Invoke(obj, parameters);
            }
            catch (Exception e)
            {
                NiecException.WriteLog("Error Invoke: " + e.ToString(), true, true);
                return default(T);
            }
        }
    }




    public class NiecNraTask
    {
        public delegate void NraFunction();
    }

    public class NiecTask : Task
    {
        NiecNraTask.NraFunction mFunction;

        protected NiecTask()
        {
            mFunction = OnPerform;
        }
        protected NiecTask(NiecNraTask.NraFunction func)
        {
            mFunction = func;
        }

        public static ObjectGuid Perform(NiecNraTask.NraFunction func)
        {
            return new NiecTask(func).AddToSimulator();
        }

        public ObjectGuid AddToSimulator()
        {
            return Simulator.AddObject(this);
        }

        protected virtual void OnPerform()
        { }

        public override void Simulate()
        {
            try
            {
                mFunction();
            }
            catch (Exception exception)
            {
                if (exception.StackTrace != null)
                    NiecException.WriteLog(ToString() + System.Environment.NewLine + System.Environment.NewLine + NiecException.LogException(exception), true, true, false);
            }
            finally
            {
                Simulator.DestroyObject(ObjectId);
            }
        }

        public override string ToString()
        {
            if (mFunction == null)
            {
                return "(NiecTask) NULL function";
            }
            else
            {
                return ("(NiecTask) Function method: " + this.mFunction.Method.ToString() + ", Declaring Type: " + this.mFunction.Method.DeclaringType.ToString());
            }
        }
    }
}
