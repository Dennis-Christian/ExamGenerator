using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ExamGenerator
{
    public partial class App : Application
    {
        public App()
        {
            ExamGeneratorContext.Load();
        }

        ~App()
        {
            ExamGeneratorContext.Save();
        }
    }
}
