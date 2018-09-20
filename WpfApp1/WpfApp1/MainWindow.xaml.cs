using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            EntityInfos infos = new EntityInfos();
            infos.EntityList.Add(new EntityInfo() {A = 1, B = "a"});
            infos.EntityList.Add(new EntityInfo() { A = 2, B = "B" });
            EntityInfos infos1 = DeepCopyByReflect(infos);
            infos.EntityList[0].A = 3;
            infos.EntityList[0].B = "C";
        } 

        public static T DeepCopyByBin<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                //序列化成流
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                //反序列化成对象
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }
        public static T DeepCopyByReflect<T>(T obj)
        {
            //如果是字符串或值类型则直接返回
            if (obj is string || obj.GetType().IsValueType) return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopyByReflect(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }
    }

    //[Serializable]
    public class EntityInfos
    {
        public EntityInfos()
        {
            EntityList = new List<EntityInfo>();
        }
        public List<EntityInfo> EntityList { get; set; }

        public EntityInfos Clone()
        {
            return (EntityInfos)MemberwiseClone();
        }
    }

    //[Serializable]
    public class EntityInfo
    {
        public int A { get; set; }
        public string B { get; set; }
    }
}
