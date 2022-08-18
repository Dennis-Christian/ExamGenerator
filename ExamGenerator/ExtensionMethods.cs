using Nancy.Json;

namespace ExamGenerator
{

    static public class ExtensionMethods
    {
        static public object FromJSON<T>(string s)
        {
            var JSS = new JavaScriptSerializer();
            var k = JSS.Deserialize<T>(s);
            return k;
        }
    }
}
