using System.Collections.Generic;

namespace AC.Tools.Models
{
    public class DbSaveModel
    {
        public int Language { get; set; }
        public string Author { get; set; }

        public string CommonNamespace { get; set; }

        public string DtoOrDomainNamespace { get; set; }

        public string DaoInterNamespace { get; set; }

        public string DaoNamespace { get; set; }

        public string ServiceInterNamespace { get; set; }

        public string ServiceNamespace { get; set; }
    }
    public class DbSettingModel
    {
        public static DbSettingModel Instance
        {
            get
            {
                DbSettingModel model = new DbSettingModel();
                model.JavaTemplate = GetJavaTemplate();
                model.CsharpTemplate = GetCsharpTemplate();
                if (model.JavaTemplate == null || model.CsharpTemplate == null)
                {
                    model.HasSetting = false;
                }
                else
                {
                    model.HasSetting = true;
                }

                if (model.JavaTemplate == null)
                {
                    model.JavaTemplate = GetDefaultOfJava();
                }
                
                if (model.CsharpTemplate == null)
                {
                    model.CsharpTemplate = GetDefaultOfCSharp();
                }
                return model;
            }
        }

        private const string JAVA_DB_SETTING_KEY = "JavaDbSettingKey_{0}";
        private const string CSHARP_DB_SETTING_KEY = "CSharpDbSettingKey_{0}";
        /// <summary>
        /// 1 c# 2 java
        /// </summary>
        public int Language { get; set; }
        /// <summary>
        /// 是否设置过？
        /// </summary>
        public bool HasSetting { get; set; }

        public Dictionary<string, string> Templates { get; set; }

        public LanguageTemplate JavaTemplate { get; set; }

        public LanguageTemplate CsharpTemplate { get; set; }

        public static LanguageTemplate GetJavaTemplate()
        {
            return null;
        }

        //public static void SetJavaTemplate(LanguageTemplate template)
        //{
        //    CookieHelper.Set(JAVA_DB_SETTING_KEY.format(ApplicationUser.Current.UserName), template,
        //        TimeSpan.FromDays(100));
        //}

        public static LanguageTemplate GetCsharpTemplate()
        {
            return null;
        }
        //public static void SetCsharpTemplate(LanguageTemplate template)
        //{
        //    CookieHelper.Set(CSHARP_DB_SETTING_KEY.format(ApplicationUser.Current.UserName), template,
        //        TimeSpan.FromDays(100));
        //}

        public void SaveAll()
        {
            //CookieHelper.Set(JAVA_DB_SETTING_KEY.format(ApplicationUser.Current.UserName), JavaTemplate,
            //    TimeSpan.FromDays(100));
            //CookieHelper.Set(CSHARP_DB_SETTING_KEY.format(ApplicationUser.Current.UserName), CsharpTemplate,
            //    TimeSpan.FromDays(100));
        }

        public void Save()
        {
            //if (Language == 1)
            //{
            //    CookieHelper.Set(CSHARP_DB_SETTING_KEY.format(ApplicationUser.Current.UserName), CsharpTemplate,
            //    TimeSpan.FromDays(100));
            //}
            //else if (Language == 2)
            //{
            //    CookieHelper.Set(JAVA_DB_SETTING_KEY.format(ApplicationUser.Current.UserName), JavaTemplate,
            //        TimeSpan.FromDays(100));
            //}
            //else
            //{
            //    SaveAll();
            //}
        }
        public static LanguageTemplate GetDefaultOfJava()
        {
            return new LanguageTemplate
            {
                //Author = ApplicationUser.Current.UserName,
                Author = "wangyuchuan",
                CommonNamespace = "com.edaisong",
                DtoOrDomainNamespace = "com.edaisong.entity.domain",
                DaoInterNamespace = "com.edaisong.dao.inter",
                DaoNamespace = "com.edaisong.dao.impl",
                ServiceInterNamespace = "com.edaisong.service.inter",
                ServiceNamespace = "com.edaisong.service.impl"
            };
        }
        public static LanguageTemplate GetDefaultOfCSharp()
        {
            return new LanguageTemplate
            {
                //Author = ApplicationUser.Current.UserName,
                Author = "wangyuchuan",
                CommonNamespace = "Eds",
                DtoOrDomainNamespace = "Eds.Service.DTO",
                DaoInterNamespace = "Eds.Dao.Inter",
                DaoNamespace = "Eds.Dao",
                ServiceInterNamespace = "Eds.Service",
                ServiceNamespace = "Eds.Service.Impl"
            };
        }
    }
}