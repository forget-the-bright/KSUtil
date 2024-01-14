using System;
using System.Reflection;

namespace KSUtil.CommonUtils
{
    public static class ReflectUtil
    {

        /// <summary>
        /// 调用并执行指定类里面的函数
        /// </summary>
        /// <param name="className">需要调用的类名(包含其命名空间)</param>
        /// <param name="methodName">需要调用的方法名</param>
        /// <param name="parameters">传递的参数值</param>
        public static void GetAndExecuteMethod(string assemblyName, string className, string methodName, object[] parameters = null)
        {

            // 被引用的类库的程序集名称，根据实际情况修改
            // 加载程序集
            Assembly assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                throw new NullReferenceException("类库DLL: " + assemblyName + "不存在");
            Type type = assembly.GetType(className);
            if (type == null)
                throw new NullReferenceException("类: " + className + "不存在");
            var obj = type.Assembly.CreateInstance(className);
            //调用其方法
            var method = type.GetMethod(methodName);
            if (method == null)
                throw new NullReferenceException("方法: " + methodName + "不存在");
            //执行方法
            method.Invoke(obj, parameters);

        }

        /// <summary>
        /// 调用并执行指定类里面的函数
        /// </summary>
        /// <param name="assemblyName">类库DLL</param>
        /// <param name="className">需要调用的类名(包含其命名空间)</param>
        /// <param name="parameters">传递的参数值</param>
        public static Object GetInstance(string assemblyName,string className, params object[] parameters)
        {
            // 被引用的类库的程序集名称，根据实际情况修改
            // 加载程序集
            Assembly assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                throw new NullReferenceException("类库DLL: " + assemblyName + "不存在");
            // 通过反射加载窗体类型
            Type type = assembly.GetType(className);
            if (type == null)
                throw new NullReferenceException("类: " + className + "不存在");
            return Activator.CreateInstance(type, parameters);
        }

    }
}
