using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
//RETURN
//using System.Environment;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;

namespace Campus
{
    class Program
    {
        //public static String URL = "https://campus.gov.il/";
        public static String URL = "https://stage.campus.gov.il/";
        public static int failed = 0, success = 0;
        //RETURN
        //public static String URL = Environment.GetEnvironmentVariable("CAMPUS_URL");
        static void Main(string[] args)
        {

            //RETURN
            //ChromeOptions options = new ChromeOptions();
            //options.AddArgument("--headless");
            //options.AddArgument("--whitelisted-ips");
            //options.AddArgument("--no-sandbox");
            //options.AddArgument("--disable-extensions");
            //options.AddArgument("--disable-dev-shm-usage");        
            //IWebDriver driver = new ChromeDriver(options);
            IWebDriver driver = new FirefoxDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(URL);
            if (CloseFirstPopup(driver)) success++; else failed++;
            if (Pagehome(driver)) success++; else failed++;
            if (PagesAcademicInstitution(driver)) success++; else failed++;
            FloorLearningObjectives(driver);
            if (CoursesSection(driver)) success++; else failed++;
            if (PagesCampusSchool(driver)) success++; else failed++;
            BlenPageTests(driver);
            if (driver.Url.Contains("https://stage.campus.gov.il/"))
            {
                associationPageTests(driver);
            }
            else
            {
                Console.WriteLine("fail or impossible! 404 - Association page doesn't exist in campus.gov");
            }
            CoursePage(driver);
            RegistrationAndeEnrollment(driver);
            CoursesPage(driver);
            /*if (CoursesPageEnAr(driver)) success++; else failed++;
            
            AnEventHasPassed(driver);
            EventsPage(driver);
            if (driver.Url.Contains("https://campus.gov.il/"))
            {
                AssimilationOrganization(driver);
            }
            else
            {
                Console.WriteLine("fail or impossible! 404 - Assimilation Organization page doesn't exist in stage.campus.gov");
            }*/

            Console.WriteLine("Final Mode A number of successful functions:" + success + " and a number of failed functions:" + failed);
            Quit(driver);
        }

        //messions functions
        private static bool CloseFirstPopup(IWebDriver driver)
        {
            try
            {
                driver.FindElements(By.CssSelector("[class='close-popup-course-button last-popup-element first-popup-element close-popup-button']"))[1].Click();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("fail or impossible! doesn't have popup event in front");
                return false;
            }
        }

        private static bool Pagehome(IWebDriver driver)
        {
            var class_floors = new List<string>() { "banner-wrapper", "academic-institution", "category-section", "courses-section", "faq-section", "testimonials-slider-section" };
            int existing_floors = 0, non_existing_floors = 0;
            foreach (var floor in class_floors)
            {
                if (ClassIsExists(floor, driver))
                    existing_floors++;
                else
                    non_existing_floors++;
            }
            var id_floors = new List<string>() { "hp_school_1st", "hp_school_2nd", "how-it-work" };
            foreach (var floor in id_floors)
            {
                if (IdIsExists(floor, driver))
                    existing_floors++;
                else
                    non_existing_floors++;
            }
            if (non_existing_floors == 0)
            {
                Console.WriteLine("success! Home Page have existing_floors:" + existing_floors + " non_existing_floors:" + non_existing_floors);
                return true;
            }
            else
            {
                Console.WriteLine("fail! Home Page have existing_floors:" + existing_floors + " non_existing_floors:" + non_existing_floors);
                return false;
            }

        }

        private static bool PagesAcademicInstitution(IWebDriver driver)
        {
            for (int i = 0; i < 2; i++)
            {
                Thread.Sleep(1000);
                try
                {
                    IWebElement page = driver.FindElement(By.Id("academic-institution-slider")).FindElements(By.CssSelector("div[aria-hidden='false']"))[i].FindElement(By.TagName("a"));
                    string title_page = page.FindElement(By.TagName("img")).GetAttribute("alt");
                    page?.Click();
                    Thread.Sleep(6000);

                    if (driver.Title.Contains(title_page))
                    {
                        Console.WriteLine("success! in Academic Institutions: " + title_page + " page");

                        var languages = driver.FindElement(By.CssSelector("div[class='lang d-none d-lg-inline-block languages_menu_wrap']")).FindElements(By.TagName("a"));
                        List<string> links = new List<string>(); List<string> titles = new List<string>();
                        foreach (var item in languages)
                        {
                            links.Add(item.GetAttribute("href"));
                            titles.Add(item.GetAttribute("title"));
                        }

                        for (int index_link = links.Count - 1; index_link >= 0; index_link--)
                        {
                            driver.Url = links[index_link].ToString();
                            Thread.Sleep(100);
                            string lang = driver.FindElement(By.TagName("HTML")).GetAttribute("lang");

                            switch (titles[index_link].ToString())
                            {
                                case "עב":
                                    {
                                        if (lang == "he-IL")
                                            PageInstitution(driver, lang, "Hebrew");
                                    }
                                    break;
                                case "العر":
                                    {
                                        if (lang == "ar")
                                            PageInstitution(driver, lang, "Arabic");
                                    }
                                    break;
                                case "En":
                                    {
                                        if (lang == "en-US")
                                            PageInstitution(driver, lang, "English");
                                    }
                                    break;
                                default:
                                    Console.WriteLine("fail! can not change " + titles[index_link].ToString() + " language");
                                    break;
                            }
                        }
                    }

                    else
                        Console.WriteLine("fail! Institution page: " + title_page + " was not found.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("fail! " + e.Message);
                    return false;
                }

                IWebElement go_back = driver.FindElement(By.ClassName("above-banner")).FindElement(By.ClassName("campus_logo"));
                go_back?.Click();
                Thread.Sleep(500);
            }
            return true;

        }

        private static void FloorLearningObjectives(IWebDriver driver)
        {
            driver.Url = URL;
            Console.WriteLine("go to Learning Objectives page");
            //הכנה לבגרויות
            if (PreparationForMatriculation(driver)) success++; else failed++;

            Thread.Sleep(100);
            //בית ספר
            //driver.Url = URL;
            if (School(driver)) success++; else failed++;
            Thread.Sleep(200);
            //השכלה
            //driver.Url = URL;
            if (Education(driver)) success++; else failed++;

        }

        private static bool PagesCampusSchool(IWebDriver driver)
        {
            driver.Url = URL;
            Thread.Sleep(1000);
            Console.WriteLine("go to campus schools");
            try
            {
                int count_page = driver.FindElement(By.XPath("//*[@id='hp_school_1st']")).FindElements(By.TagName("a")).Count;
                for (int i = 0; i < count_page; i++)
                {
                    Console.WriteLine("go to campus school number " + (i + 1));
                    //string title_page = driver.FindElement(By.Id("hp_school_1st")).FindElements(By.ClassName("school-item"))[i].FindElement(By.TagName("h4")).Text;  
                    Thread.Sleep(1000);
                    var page = driver.FindElement(By.XPath("//*[@id='hp_school_1st']")).FindElements(By.TagName("a"))[i];
                    page.Click();
                    Thread.Sleep(10000);
                    if (driver.Title.Contains("קורסים"))
                    {
                        Console.WriteLine("success! course page");
                        //בודק אם מספר הקורסים המוצג בכותרת שווה לכמות הקורסים המוצגים
                        if (CountCourse(driver)) success++; else failed++;
                        // בודק אם יש אפשרות של סינון שפה -> אם כן אז שולף את השפה ובודק תקינות סינון של השפה שנבחרה
                        LanguagesOptions(driver);
                        if (ChangeLanguageEn(driver, "Courses Page")) success++; else failed++;
                        if (ChangeLanguageAr(driver, "Courses Page")) success++; else failed++;
                        if (ChangeLanguageHe(driver, "Courses Page")) success++; else failed++;

                    }
                    else
                    {
                        Console.WriteLine("fail! title isn't match " + driver.Title);
                        return false;
                    }

                    driver.Url = URL;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! Pages Campus School " + e.Message);
                return false;
            }
        }

        private static bool CoursesSection(IWebDriver driver)
        {
            Thread.Sleep(20);
            Console.WriteLine("go to Courses Section");
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    IWebElement course_link = driver.FindElements(By.ClassName("course-item-details"))[i];
                    string course_title = course_link.FindElement(By.ClassName("course-item-title")).Text;
                    course_link.Click();
                    Thread.Sleep(100);
                    if (driver.Title.Contains(course_title))
                        Console.WriteLine("success! go to course");

                    else
                    {
                        Console.WriteLine("fail! go to course");
                        return false;
                    }
                    driver.Url = URL;
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! Courses Section " + e.Message);
                return false;
            }
        }

        private static void BlenPageTests(IWebDriver driver)
        {
            string url = URL + "h_course/tester/";
            driver.Navigate().GoToUrl(url);
            Console.WriteLine("go to Blen Course Page");
            if (ChangeLanguageEn(driver, "Blen Page")) success++; else failed++;
            if (ChangeLanguageAr(driver, "Blen Page")) success++; else failed++;
            if (ChangeLanguageHe(driver, "Blen Page")) success++; else failed++;
            if (TitleInBannerById(driver, "hybrid_banner_h1")) success++; else failed++;
            MoreInfo(driver);
            if (BlendPageRegistrationButton(driver)) success++; else failed++;
            if (associationButton(driver)) success++; else failed++;
            if (institutionButton(driver)) success++; else failed++;
            if (AboutLecturer(driver)) success++; else failed++;
            if (MoreCourses(driver)) success++; else failed++;
            if (ButtonForCourse(driver, url)) success++; else failed++;
            if (BlenMoreInfo(driver, url)) success++; else failed++;
            if (NavigateCoursesPage(driver, url)) success++; else failed++;
            if (NavigatesEventsPage(driver, url)) success++; else failed++;
            if (NavigatesAboutPage(driver, url)) success++; else failed++;
            if (NavigatesSupportPage(driver, url)) success++; else failed++;
            if (ChatBotAvatar(driver, url)) success++; else failed++;
        }

        private static void associationPageTests(IWebDriver driver)
        {
            string url = URL + "hybrid_institution/tester/";
            Console.WriteLine("go to Association Page");
            if (ButtonForCourse(driver, url)) success++; else failed++;
            if (NavigateCoursesPage(driver, url)) success++; else failed++;
            if (NavigatesEventsPage(driver, url)) success++; else failed++;
            if (NavigatesAboutPage(driver, url)) success++; else failed++;
            if (NavigatesSupportPage(driver, url)) success++; else failed++;
        }

        private static void CoursePage(IWebDriver driver)
        {
            driver.Url = URL + "course/course-v1-mse-gov_psychometry/";
            Console.WriteLine("go to course page");
            CourseLecturer(driver);
            if (PlayVideo(driver)) success++; else failed++;
            MoreInfo(driver);
            if (FloodedPosters(driver)) success++; else failed++;

            if (ChangeLanguageEn(driver, "Course Page")) success++; else failed++;
            if (ChangeLanguageAr(driver, "Course Page")) success++; else failed++;
            if (ChangeLanguageHe(driver, "Course Page")) success++; else failed++;

            CoursePageRegistrationButton(driver);
        }

        private static void RegistrationAndeEnrollment(IWebDriver driver)
        {
            driver.Url = URL;
            Console.WriteLine("go to Registration And Enrollment");
            Thread.Sleep(300);
            HeaderLoginButton(driver);
            HeaderRegistrationButton(driver);
            LoginButton(driver);
            HeaderRegistrationUserName(driver);
            PrivateArea(driver);
            CoursePageRegistration(driver);
            BlendPageRegistration(driver);
            LogOut(driver);
            Thread.Sleep(2000);
            LoginButton(driver);
            HeaderRegistrationUserName(driver);
            ToCoursePage(driver);
            ToBlendCoursePage(driver);
        }

        private static void CoursesPage(IWebDriver driver)
        {
            Console.WriteLine("go to courses page");
            if (NavigateCoursesPage(driver, URL)) success++; else failed++;
            FilterByInstitution(driver);
            FilterByWhatIsInteresting(driver);
            FilterByTech(driver);
            FilterByLanguage(driver);
            //אותם בדיקות לאתר הנוכחי באנגלית + ערבית
        }

        private static bool CoursesPageEnAr(IWebDriver driver)
        {
            NavigateCoursesPage(driver, URL);

            ChangeLanguageEn(driver, "Courses Page");
            FilterByInstitutionEnAr(driver, "institution_1362");
            FilterByWhatIsInterestingEnAr(driver, "areas_of_knowledge_887");
            FilterByTechEnAr(driver, "subject_937");
            FilterByLanguageEnAr(driver, "language_29");

            ChangeLanguageAr(driver, "Courses Page");
            FilterByInstitutionEnAr(driver, "institution_4730");
            FilterByWhatIsInterestingEnAr(driver, "areas_of_knowledge_884");
            FilterByTechEnAr(driver, "subject_1029");
            FilterByLanguageEnAr(driver, "language_401");
            return true;
        }

        private static void AnEventHasPassed(IWebDriver driver)
        {
            driver.Url = URL + "event/live-prof-david-passig/";
            Console.WriteLine("go to an event has passed ");
            EventProducerLogo(driver);
            ImageInBanner(driver);
            TitleInBanner(driver, "title-course");
            SubtitleInBanner(driver);
            PriceInBottomOfBanner(driver);
            DateInBottomOfBanner(driver);
            SharingComponent(driver);
            Participants(driver);
            PopupsInParticipants(driver);
        }

        private static void EventsPage(IWebDriver driver)
        {
            driver.Url = URL + "event/";
            Console.WriteLine("go to events page");
            CountEvents(driver);
            FilterByPastEvents(driver);
            FilterByMastersInTheLivingRoom(driver);
            PastEvent(driver);
            FutureEvent(driver);
            ChangeLanguageEn(driver, "events page");
            ChangeLanguageAr(driver, "events page");
            ChangeLanguageHe(driver, "events page");
            GoToEventPage(driver);

        }

        private static void AssimilationOrganization(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(URL + "hybrid_institution/test_ao/");
            Console.WriteLine("go to Assimilation Organization");
            OrganizationLogo(driver);
            OrganizationDescription(driver);
            FoundCourse(driver);
            FoundLecturer(driver);
            FoundTrainers(driver);
            HybridMoreCoursesBtn(driver);
            HybridCourseMoreEight(driver);
            CourseTrackAHybridCourse(driver);
            ButtonForCourse(driver, URL + "hybrid_institution/test_ao/");
        }

        //shortcut functions

        private static bool PreparationForMatriculation(IWebDriver driver)
        {
            IWebElement link_learning_Objectives = driver.FindElement(By.ClassName("category-section")).FindElement(By.CssSelector("a[href='" + URL + "areas_of_knowledge/prepartion_for_matriculation_exams/']"));
            link_learning_Objectives?.Click();
            string title = driver.Title;
            if (title.Contains("לבגרויות"))
            {
                var class_floors = new List<string>() { "banner-image", "academic-institution", "courses-section", "more-info-lobby", "faq-section" };

                int existing_floors = 0, non_existing_floors = 0;
                foreach (var floor in class_floors)
                {
                    if (ClassIsExists(floor, driver))
                        existing_floors++;
                    else
                        non_existing_floors++;
                }

                var id_floors = new List<string>() { "how-it-work" };
                foreach (var floor in id_floors)
                {
                    if (IdIsExists(floor, driver))
                        existing_floors++;
                    else
                        non_existing_floors++;
                }

                if (non_existing_floors == 0)
                {
                    Console.WriteLine("success! Preparation for matriculation have existing_floors:" + existing_floors + " non_existing_floors:" + non_existing_floors);
                }
                else
                {
                    Console.WriteLine("fail! Preparation for matriculation have existing_floors:" + existing_floors + " non_existing_floors:" + non_existing_floors);
                    return false;
                }

            }
            else
            {
                Console.WriteLine("fail! go to Preparation for matriculation ");
                return false;
            }

            IWebElement go_back = driver.FindElement(By.ClassName("above-banner")).FindElement(By.ClassName("campus_logo"));
            go_back?.Click();
            return true;

        }

        private static bool School(IWebDriver driver)
        {
            IWebElement link_learning_Objectives = driver.FindElement(By.ClassName("category-section")).FindElement(By.CssSelector("a[href='" + URL + "areas_of_knowledge/career-and-high-tech-school/']"));
            link_learning_Objectives?.Click();
            string title = driver.Title;
            if (title.Contains("בית ספר לקריירה והייטק"))
            {
                var class_floors = new List<string>() { "banner-image", "academic-institution", "courses-section", "more-info-lobby", "testimonials-lobby-knowledge", "faq-section" };
                int existing_floors = 0, non_existing_floors = 0;
                foreach (var floor in class_floors)
                {
                    if (ClassIsExists(floor, driver))
                        existing_floors++;
                    else
                        non_existing_floors++;
                }

                var id_floors = new List<string>() { "how-it-work" };
                foreach (var floor in id_floors)
                {
                    if (IdIsExists(floor, driver))
                        existing_floors++;
                    else
                        non_existing_floors++;
                }

                if (non_existing_floors == 0)
                {
                    Console.WriteLine("success! High-tech-school have existing_floors:" + existing_floors + " non_existing_floors:" + non_existing_floors);
                }
                else
                {
                    Console.WriteLine("fail! High-tech-school have existing_floors:" + existing_floors + " non_existing_floors:" + non_existing_floors);
                    return false;
                }

            }
            else
            {
                Console.WriteLine("fail! go to  school");
                return false;
            }

            IWebElement go_back = driver.FindElement(By.ClassName("above-banner")).FindElement(By.ClassName("campus_logo"));
            go_back?.Click();
            return true;

        }

        private static bool Education(IWebDriver driver)
        {
            IWebElement link_learning_Objectives = driver.FindElement(By.ClassName("category-section")).FindElement(By.CssSelector("a[href='" + URL + "areas_of_knowledge/academic-education-and-broadening-horizons/']"));
            link_learning_Objectives?.Click();
            string title = driver.Title;
            if (title.Contains("השכלה אקדמית"))
            {
                var class_floors = new List<string>() { "banner-image", "academic-institution", "courses-section", "more-info-lobby", "faq-section" };
                int existing_floors = 0, non_existing_floors = 0;
                foreach (var floor in class_floors)
                {
                    if (ClassIsExists(floor, driver))
                        existing_floors++;
                    else
                        non_existing_floors++;
                }

                var id_floors = new List<string>() { "how-it-work" };
                foreach (var floor in id_floors)
                {
                    if (IdIsExists(floor, driver))
                        existing_floors++;
                    else
                        non_existing_floors++;
                }

                if (non_existing_floors == 0)
                    Console.WriteLine("success! Academic-education-and-broadening-horizons have existing_floors:" + existing_floors + " non_existing_floors:" + non_existing_floors);
                else
                {
                    Console.WriteLine("fail! Academic-education-and-broadening-horizons have existing_floors:" + existing_floors + " non_existing_floors:" + non_existing_floors);
                    return false;
                }
            }
            else
            {
                Console.WriteLine("fail! go to education");
                return false;
            }

            IWebElement go_back = driver.FindElement(By.ClassName("above-banner")).FindElement(By.ClassName("campus_logo"));
            go_back?.Click();
            return true;
        }

        private static bool CountCourse(IWebDriver driver)
        {
            try
            {
                int sum_course_title = Int16.Parse(driver.FindElement(By.Id("add-sum-course")).Text);
                int sum_list_course = driver.FindElement(By.ClassName("output-courses")).FindElements(By.ClassName("item_post_type_course")).Count;
                if (sum_course_title == sum_list_course)
                {
                    Console.WriteLine("success! the amount of courses is compatible ");
                }
                else
                {
                    Console.WriteLine(sum_course_title + " != " + sum_list_course + " :( ");
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! Count Course " + e.Message);
                return false;
            }

        }

        private static void LanguagesOptions(IWebDriver driver)
        {
            try
            {
                IWebElement myElement;
                if (driver.Url.Contains("https://stage.campus.gov.il/"))
                    myElement = driver.FindElement(By.XPath("//h2[text() = 'שפה']"));
                else
                    myElement = driver.FindElement(By.XPath("//h5[text() = 'שפה']"));
                IWebElement parent = myElement.FindElement(By.XPath("./.."));
                var labels = parent.FindElements(By.TagName("label"));
                foreach (var lable in labels)
                {
                    var forLang = lable.GetAttribute("for");
                    switch (forLang)
                    {
                        case "language_111":
                            {
                                if (CheckChoosingLanguageOfPosts(driver, "Arabic", "language_111", "language_111")) success++; else failed++; ;
                                break;
                            }

                        case "language_110":
                            {
                                if (CheckChoosingLanguageOfPosts(driver, "English", "language_110", "language_110")) success++; else failed++;
                                break;
                            }

                        case "language_109":
                            {
                                if (CheckChoosingLanguageOfPosts(driver, "Hebrew", "language_109", "language_109")) success++; else failed++;
                                break;
                            }

                        default:
                            {
                                Console.WriteLine("fail! language of posts"); break;
                            }
                    }
                }
            }
            catch
            {
                Console.WriteLine("fail or impossible! do not have option of language in posts");
            }
        }

        private static bool CheckChoosingLanguageOfPosts(IWebDriver driver, string language, string label_language, string input_language)
        {
            try
            {
                IWebElement element = driver.FindElement(By.ClassName("wrap-all-tags-filter")).FindElement(By.CssSelector("label[for='" + label_language + "']"));
                IWebElement input = element.FindElement(By.Id(input_language));
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                jse.ExecuteScript("arguments[0].click();", input);
                string str_sum = element.FindElement(By.ClassName("sum")).Text.Replace(")", "").Replace("(", "");
                int sum = Int16.Parse(str_sum);
                int sum_course_title = Int16.Parse(driver.FindElement(By.Id("add-sum-course")).Text);

                if (sum == sum_course_title)
                {
                    Console.WriteLine("success! show all " + language + " posts");
                }
                else
                {
                    Console.WriteLine("fail! not show all " + language + " posts :( ");
                    return false;
                }
                ////click on checkbox language to unchecked
                jse.ExecuteScript("arguments[0].click();", input);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! Check Choosing Language Of Posts " + e.Message);
                return false;
            }
        }

        private static void PageInstitution(IWebDriver driver, string lang, string language_page)
        {
            Console.WriteLine("success! change language to " + language_page);
            IsAmountOfCoursesEqual(driver);
            IsAmountOfLecturersEqual(driver);
            //יבודק אם הקורסים הולכים לאתרים שלהם בשפה הנבחרת
            GoToCourseInInstatution(driver, lang);
        }

        private static void GoToCourseInInstatution(IWebDriver driver, string lang_language)
        {
            for (int i = 0; i < 2; i++)
            {
                IWebElement course_link = driver.FindElements(By.ClassName("course-item-details"))[i];
                string course_title = course_link.FindElement(By.ClassName("course-item-title")).Text;
                course_link?.Click();

                string current_language = driver.FindElement(By.TagName("HTML")).GetAttribute("lang");
                if (current_language == lang_language && driver.Title.Contains(course_title))
                {
                    Console.WriteLine("success! go to course");
                    driver.Navigate().Back();
                }
                else Console.WriteLine("fail! go to course");

            }
        }

        private static bool TitleInBannerById(IWebDriver driver, string id_title)
        {
            try
            {
                string title = driver.FindElement(By.Id(id_title)).Text;
                if (title != "")
                    Console.WriteLine("success! have title in banner");
                else
                {
                    Console.WriteLine("fail! don't have title in banner");
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! Title In Banner By Id " + e.Message);
                return true;
            }

        }


        private static bool BlendPageRegistrationButton(IWebDriver driver)
        {
            driver.Url = URL + "h_course/tester/";
            try
            {
                IWebElement a = driver.FindElement(By.Id("hybrid_banner_btn"));
                if (a.Text == "הרשמה לcampusIL")
                    Console.WriteLine("success! registration button have the correct text");
                else
                    Console.WriteLine("fail! registration button  doesn't have the correct text");

                a.Click();
                var browserTabs = driver.WindowHandles;
                driver.SwitchTo().Window(browserTabs[1]);

                //check is it correct page opened or not 
                if (driver.Title.Contains("היכנס או צור חשבון"))
                {
                    Console.WriteLine("success! button registration send user to registration page");
                }
                else
                {
                    Console.WriteLine("registration button doesn't send to registration page");
                    return false;
                }

                //close tab and get back
                driver.Close();
                driver.SwitchTo().Window(browserTabs[0]);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! BlendPageRegistrationButton" + e.Message);
                return false;
            }

        }

        private static bool associationButton(IWebDriver driver)
        {
            try
            {
                IWebElement a = driver.FindElement(By.XPath("//div[@class='uni-logo2 col-6 col-sm-2']")).FindElement(By.TagName("a"));
                a.Click();

                if (driver.Title.Contains("משרד החינוך"))
                {
                    Console.WriteLine("success! association button send to association page");
                }
                //close tab and get back
                else
                {
                    Console.WriteLine("association button doesn't send to association page");
                    return false;
                }
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! association Button" + e.Message);
                return false;
            }

        }

        private static bool institutionButton(IWebDriver driver)
        {
            driver.Url = URL + "h_course/tester/";
            try
            {
                IWebElement a = driver.FindElement(By.XPath("//div[@class='uni-logo1 col-6 col-sm-2 align-self-center']"));
                a.Click();

                if (driver.Title.Contains("אוניברסיטת חיפה"))
                    Console.WriteLine("success! Institution button send to institution page");

                else
                {
                    Console.WriteLine("fail! Institution button doesn't send to institution page");
                    return false;
                }
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! institutionButton " + e.Message);
                return false;
            }

        }

        private static bool AboutLecturer(IWebDriver driver)
        {
            driver.Url = URL + "h_course/tester/";
            try
            {
                driver.FindElement(By.ClassName("lecturer-little-about")).Click();
                Thread.Sleep(100);
                IWebElement a = driver.FindElement(By.XPath("//div[@class='single-lecturer-popup dialog active']"));
                driver.FindElement(By.XPath("//div[@class='single-lecturer-popup dialog active']"));
                Console.WriteLine("success! Popup about lecturer apear");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! AboutLecturer" + e.Message);
                return false;

            }

        }

        private static bool MoreCourses(IWebDriver driver)
        {
            try
            {
                driver.Url = URL + "h_course/tester/";
                driver.FindElement(By.ClassName("for-all-courses-link")).Click();
                if (driver.Title.Contains("משרד החינוך"))
                {
                    Console.WriteLine("success! The More Courses button takes to the Assimilation Body page");
                }
                else
                {
                    Console.WriteLine("fail! The More Courses button doesn't take to the Assimilation Body page");
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! MoreCourses" + e.Message);
                return false;
            }

        }

        //בדיקת הגעה לקורס מכפתור "לעמוד הקורס" בדף ארגון מטמיע
        private static bool ButtonForCourse(IWebDriver driver, string url)
        {
            try
            {
                driver.Url = url;
                driver.FindElement(By.ClassName("course-item-link")).Click();
                Console.WriteLine("success! the button navigate user to course page");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! ButtonForCourse " + e.Message);
                return false;
            }

        }

        private static bool BlenMoreInfo(IWebDriver driver, string url)
        {
            try
            {
                driver.Url = url;
                IWebElement wrap_info = driver.FindElement(By.ClassName("content-info-wrap"));
                Console.WriteLine("success! get organisation " + wrap_info.FindElement(By.XPath("//span[@class='org info-course-list-bold']/./following-sibling::span")).Text);
                Console.WriteLine("success! get institution " + wrap_info.FindElement(By.ClassName("item_corporation_institution")).Text);
                Console.WriteLine("success! get duration " + wrap_info.FindElement(By.XPath("//span[@class='duration info-course-list-bold']/./following-sibling::span")).Text);
                Console.WriteLine("success! get start date " + wrap_info.FindElement(By.XPath("//span[@class='start info-course-list-bold']/./following-sibling::span")).Text);
                Console.WriteLine("success! get end date " + wrap_info.FindElement(By.XPath("//span[@class='end info-course-list-bold']/./following-sibling::span")).Text);
                Console.WriteLine("success! get price " + wrap_info.FindElement(By.XPath("//span[@class='price info-course-list-bold']/./following-sibling::span")).Text);
                Console.WriteLine("success! get language " + wrap_info.FindElement(By.XPath("//span[@class='language info-course-list-bold']/./following-sibling::span")).Text);
                var info_langs_spans = wrap_info.FindElements(By.ClassName("info_lang_span"));
                foreach (var item in info_langs_spans)
                    Console.WriteLine("success! get subtitle_lang " + item.Text);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! BlenMoreInfo " + e.Message);
                return false;
            }



        }

        /*private static void Navigates(IWebDriver driver, string url)
         {
             NavigateCoursesPage(driver, url);
             NavigatesEventsPage(driver, url);
             NavigatesAboutPage(driver, url);
             NavigatesSupportPage(driver, url);
         }*/

        private static bool NavigateCoursesPage(IWebDriver driver, string url)
        {
            try
            {
                driver.Url = url;
                driver.FindElement(By.Id("menu-item-6449")).Click();
                Thread.Sleep(400);
                if (driver.Title.Contains("קורסים"))
                    Console.WriteLine("success! the button navigate to courses page");

                else
                {
                    Console.WriteLine("fail! can't navigate to courses page");
                    return false;
                }
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! NavigateCoursesPage " + e.Message);
                return false;
            }



        }

        private static bool NavigatesAboutPage(IWebDriver driver, string url)
        {
            try
            {
                driver.Url = url;
                driver.FindElement(By.Id("menu-item-36401")).Click();
                if (driver.Title.Contains("החזון"))
                    Console.WriteLine("success! the button navigate to about page");
                else
                {
                    Console.WriteLine("fail! can't navigate to about page");
                    return false;
                }
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! NavigatesAboutPage " + e.Message);
                return false;
            }
        }

        private static bool NavigatesEventsPage(IWebDriver driver, string url)
        {
            try
            {
                driver.Url = url;
                driver.FindElement(By.Id("menu-item-36418")).Click();
                if (driver.Title.Contains("אירועים - קמפוס IL"))
                    Console.WriteLine("success! the button navigate to events page");
                else
                {
                    Console.WriteLine("fail! can't navigate to events page");
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! NavigatesEventsPage " + e.Message);
                return false;
            }
        }

        private static bool NavigatesSupportPage(IWebDriver driver, string url)
        {
            try
            {
                driver.Url = url;
                driver.FindElement(By.Id("menu-item-21855")).Click();
                if (driver.Title.Contains("צור קשר"))
                    Console.WriteLine("success! the button navigate to support page");
                else
                {
                    Console.WriteLine("fail! can't navigate to support page");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! NavigatesSupportPage " + e.Message);
                return false;
            }
        }

        private static bool ChatBotAvatar(IWebDriver driver, string url)
        {
            try
            {
                driver.Url = url;
                driver.FindElement(By.Id("ChatBotAvatar")).Click();
                Thread.Sleep(50);
                IWebElement a = driver.FindElement(By.CssSelector("iframe[id='ChatBotFrame']"));
                if (a.Displayed)
                    Console.WriteLine("success to open chat with campus!");
                else
                {
                    Console.WriteLine("fail to open chat with campus :(");
                    return false;
                }
                return true;

            }
            catch (Exception e)
            {

                Console.WriteLine("fail! ChatBotAvatar " + e.Message);
                return false;
            }

        }

        private static void CoursePageRegistrationButton(IWebDriver driver)
        {
            try
            {
                IWebElement a = driver.FindElement(By.CssSelector("a[class='signup-course-button con_to_course ']"));
                if (a.Text == "הרשמה לcampusIL")
                {
                    Console.WriteLine("success! registration button have the correct text");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! registration button  doesn't have the correct text");
                    failed++;
                }


                a.Click();
                var browserTabs = driver.WindowHandles;
                driver.SwitchTo().Window(browserTabs[1]);

                //check is it correct page opened or not 
                if (driver.Title.Contains("היכנס או צור חשבון"))
                {
                    Console.WriteLine("success! button registration send user to registration page in course page");
                    success++;
                }
                else
                {
                    Console.WriteLine("fail! registration button doesn't send to registration page in course page");
                    failed++;
                }

                //close tab and get back
                driver.Close();
                driver.SwitchTo().Window(browserTabs[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! CoursePageRegistrationButton " + e.Message);
                failed++;
            }

        }

        private static void CourseLecturer(IWebDriver driver)
        {

            int count_lecturer = driver.FindElements(By.ClassName("content-lecturer")).Count;
            if (count_lecturer == 0)
            {
                Console.WriteLine("fail! Course Lecturer. class name:content-lecturer does not exists");
                failed++;
            }


            else
            {
                Console.WriteLine("success! count lecturers " + count_lecturer);
                success++;
            }


        }

        private static bool PlayVideo(IWebDriver driver)
        {
            try
            {
                IWebElement play_video = driver.FindElement(By.CssSelector("div[class='banner-image about-course gray-part d-none d-lg-inline-block']")).FindElement(By.TagName("a"));
                play_video?.Click();
                Thread.Sleep(500);
                string opacity = driver.FindElement(By.Id("popup_overlay_2020")).GetCssValue("opacity");
                if (opacity == "1")
                {
                    Console.WriteLine("success! play course video");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! can not play course video");
                    return false;
                }

                IWebElement close_video = driver.FindElement(By.ClassName("close-popup-button-2020"));
                close_video?.Click();
                Console.WriteLine("success! close play course video");

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! PlayVideo " + e.Message);
                return false;
            }

        }

        private static void MoreInfo(IWebDriver driver)
        {

            try
            {
                string start_date = driver.FindElement(By.ClassName("start-bar-info")).FindElement(By.ClassName("text-bar-course")).Text;
                Console.WriteLine("start date: " + start_date);
                if (start_date != "")
                {
                    Console.WriteLine("success! get start date");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! can not get start date");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! MoreInfo start_date " + e.Message);
                failed++;

            }

            try
            {
                string price = driver.FindElement(By.ClassName("price-bar-info")).FindElement(By.ClassName("text-bar-course")).Text;
                Console.WriteLine("price: " + price);
                if (price != "")
                {
                    Console.WriteLine("success! get price");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! can not get price");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! MoreInfo price " + e.Message);
                failed++;

            }

            try
            {
                string duration_of_course = driver.FindElement(By.ClassName("duration-bar-info")).FindElement(By.ClassName("text-bar-course")).Text;
                Console.WriteLine("duration of course: " + duration_of_course);
                if (duration_of_course != "")
                {
                    Console.WriteLine("success! get duration of the course");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! can not get duration of the course");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! MoreInfo duration_of_course " + e.Message);
                failed++;

            }
        }

        private static bool FloodedPosters(IWebDriver driver)
        {
            var posts = driver.FindElements(By.ClassName("course-item-title"));
            int flag = 0;
            if (posts.Count == 4)
            {
                Console.WriteLine("success! 4 flooded posters");
                success++;
                for (int i = 0; i < posts.Count; i++)
                {
                    string current_poster = posts[i].Text;
                    for (int j = i + 1; j < posts.Count; j++)
                    {
                        if (current_poster == posts[j].Text)
                        {
                            flag = 1;
                        }
                    }
                }
                if (flag == 1)
                {
                    Console.WriteLine("fail! have two of the same post");
                    return false;
                }
                else
                {
                    Console.WriteLine("success! there are different posts");
                    return true;
                }
            }
            else
            {
                Console.WriteLine("fail! do not have flooded posters or class name: course-item-title does not exists");
                return false;
            }


        }

        private static void HeaderLoginButton(IWebDriver driver)
        {
            try
            {
                IWebElement a = driver.FindElements(By.ClassName("login-item"))[1];
                if (a.Text == "התחברות")
                {
                    Console.WriteLine("success! button login text is login");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! button login text is not login");
                    failed++;
                }


                a?.Click();
                if (driver.Title.Contains("היכנס או צור חשבון") && driver.Url.Contains("login"))
                {
                    Console.WriteLine("success! button login send user to login page");
                    success++;
                }


                else
                {
                    Console.WriteLine("login button doesn't send to login page");
                    failed++;
                }

                driver.Navigate().Back();
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! HeaderLoginButton " + e.Message);
                failed++;
            }

        }

        private static void HeaderRegistrationButton(IWebDriver driver)
        {
            try
            {
                IWebElement a = driver.FindElements(By.ClassName("signin"))[1];
                if (a.Text == "הרשמה")
                {
                    Console.WriteLine("success! button registration text is registration");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! button registration text is not registration");
                    failed++;
                }

                a.Click();

                //check is it correct page opened or not 
                if (driver.Title.Contains("היכנס או צור חשבון") && driver.Url.Contains("register"))
                {
                    Console.WriteLine("success! button registration send user to registration page");
                    success++;
                }


                else
                {
                    Console.WriteLine("registration button doesn't send to registration page");
                    failed++;
                }

                driver.Navigate().Back();
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! HeaderRegistrationButton " + e.Message);
                failed++;
            }

        }

        private static void LoginButton(IWebDriver driver)
        {
            try
            {
                IWebElement a = driver.FindElements(By.ClassName("login-item"))[1];
                a?.Click();
                if (driver.Title.Contains("היכנס או צור חשבון") && driver.Url.Contains("login"))
                {
                    Console.WriteLine("success! button login send user to login page for Sign up");
                    IWebElement input_user_mail = driver.FindElement(By.Id("login-email"));
                    input_user_mail.SendKeys("ravitc@daatsolutions.co.il");

                    IWebElement input_user_password = driver.FindElement(By.Id("login-password"));
                    input_user_password.SendKeys("ravitc");

                    IWebElement enter_button = driver.FindElement(By.CssSelector("button[class='action action-primary action-update js-login login-button']"));
                    enter_button?.Click();

                    //בגלל שבלינק בסטאג הזינו לינק של קמפוס השארתי את זה כהה:  
                    if (driver.Url != URL)
                        driver.Url = URL;

                    success++;
                }

                else
                {
                    Console.WriteLine("login button doesn't send to login page");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! LoginButton" + e.Message);
                failed++;
            }


        }

        private static void HeaderRegistrationUserName(IWebDriver driver)
        {
            try
            {
                string user_name = driver.FindElements(By.CssSelector("[class='user-information show_for_connected_user']"))[1].Text;
                if (user_name.Contains("שלום"))
                {
                    Console.WriteLine("success! have user name after registration");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! doesn't have user name after registration");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! HeaderRegistrationUserName " + e.Message);
                failed++;
            }

        }

        private static void PrivateArea(IWebDriver driver)
        {
            try
            {
                IWebElement a = driver.FindElement(By.CssSelector("div[class='d-block d-md-none d-lg-inline-block user-connect show_for_connected_user']")).FindElement(By.TagName("a"));
                a?.Click();

                var browserTabs = driver.WindowHandles;
                driver.SwitchTo().Window(browserTabs[1]);

                //check is it correct page opened or not 
                if (driver.Title.Contains("אזור אישי") && driver.Url.Contains("dashboard") || driver.Title.Contains("לוח בקרה") && driver.Url.Contains("dashboard"))
                {
                    Console.WriteLine("success! button private area send user to dashboard page");
                    success++;
                }


                else
                {
                    Console.WriteLine("fail! button private area doesn't send to dashboard page");
                    failed++;
                }


                //close tab and get back
                driver.Close();
                driver.SwitchTo().Window(browserTabs[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! PrivateArea " + e.Message);
                failed++;
            }


        }

        private static void CoursePageRegistration(IWebDriver driver)
        {
            try
            {
                driver.Url = URL + "course/course-v1-molsa-gov-eng101/";
                IWebElement green_button = driver.FindElement(By.CssSelector("[class='signup-course-button register_api user_not_con_to_course ']"));
                Console.WriteLine(green_button.Text);
                if (green_button.Text == "הרשמה לקורס")
                {
                    Console.WriteLine("success! green button text in course page is Sign up for a course");
                    //שמתי בהערה כי הקליק עובד ואז בפעם הבא שאני אריץ את הקורס הזה לא יהיה כתוב לי 
                    //להרשמה לקורס אלא לעמוד הקורס
                    //לכן שמתי לינק פה - לבדיקה להרשמה לקורס
                    //ולינק אחר לעמוד הקורס - קורס שכבר נרשמתי אליו ע"י סלניום
                    //green_button.Click();
                    //Console.WriteLine("success! registration to this course");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! green button text in course page is not Sign up for a course");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! CoursePageRegistration " + e.Message);
                failed++;
            }


        }

        private static void BlendPageRegistration(IWebDriver driver)
        {
            try
            {
                driver.Url = URL + "h_course/tester/";
                IWebElement green_button = driver.FindElement(By.Id("hybrid_banner_btn"));
                Console.WriteLine(green_button.Text);
                //בודק אם הטקסט בכפתור לא ריק
                if (green_button.Text != "")
                {
                    Console.WriteLine("success! green button text in blend page is Sign up for a course");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! green button text in blend page is not Sign up for a course");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! BlendPageRegistration" + e.Message);
                failed++;
            }


        }

        private static void LogOut(IWebDriver driver)
        {
            try
            {
                IWebElement a = driver.FindElement(By.CssSelector("div[class='d-block d-md-none d-lg-inline-block user-connect show_for_connected_user']")).FindElement(By.TagName("a"));
                a?.Click();

                var browserTabs = driver.WindowHandles;
                driver.SwitchTo().Window(browserTabs[1]);
                Thread.Sleep(100);
                //check is it correct page opened or not 
                if (driver.Title.Contains("אזור אישי") && driver.Url.Contains("dashboard") || driver.Title.Contains("לוח בקרה") && driver.Url.Contains("dashboard"))
                {
                    //Console.WriteLine("success! button dashboard send user to dashboard page");
                    IWebElement li_logout = driver.FindElement(By.CssSelector("span[class='user-name']"));
                    li_logout?.Click();
                    IWebElement a_logout = driver.FindElement(By.CssSelector("a[href='/logout']"));
                    a_logout?.Click();
                    Console.WriteLine("success! Log out");
                    success++;
                }
                else
                {
                    Console.WriteLine("fail! Log out");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! LogOut " + e.Message);
                failed++;
            }

        }

        private static void ToCoursePage(IWebDriver driver)
        {
            try
            {
                driver.Url = URL + "course/course-v1-mse-gov_psychometry/";
                IWebElement green_button = driver.FindElement(By.CssSelector("[class='signup-course-button con_to_course ']"));
                Console.WriteLine(green_button.Text);
                if (green_button.Displayed && green_button.Text == "לעמוד הקורס")
                {
                    Console.WriteLine("success! green button text in course page is to course page");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! green button text in course page is to course page. The current course may have been deleted from my personal area");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! ToCoursePage " + e.Message);
                failed++;
            }

        }

        private static void ToBlendCoursePage(IWebDriver driver)
        {
            try
            {
                driver.Url = URL + "h_course/%d7%9c%d7%91%d7%93%d7%99%d7%a7%d7%94-%d7%91%d7%9c%d7%91%d7%93-2-%d7%90%d7%99%d7%9f-%d7%9c%d7%91%d7%a6%d7%a2-%d7%a9%d7%99%d7%9e%d7%95%d7%a9/";
                IWebElement green_button = driver.FindElement(By.CssSelector("[class='signup-course-button con_to_course ']"));

                if (green_button.Text == "משתמש מחובר")
                {
                    Console.WriteLine("success! green button text in course page is to blend course page");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! green button text in course page is to blend course page");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! ToBlendCoursePage " + e.Message);
                failed++;
            }

        }

        private static void FilterByInstitution(IWebDriver driver)
        {
            try
            {
                IWebElement institution = driver.FindElement(By.CssSelector("button[class='filter_main_button dropdown_open']"));
                institution?.Click();

                IWebElement input = driver.FindElement(By.Id("institution_1128"));
                Filters(driver, input, "institution");
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FilterByInstitution " + e.Message);
                failed++;
            }
        }

        private static void Filters(IWebDriver driver, IWebElement input, string filter_name = "")
        {

            try
            {
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                jse.ExecuteScript("arguments[0].click();", input);

                Thread.Sleep(800);
                driver.FindElements(By.CssSelector("a[class='ajax_filter_btn']"))[1].Click();

                Thread.Sleep(20000);
                try
                {
                    while (driver.FindElement(By.Id("course_load_more")).Displayed)
                    {
                        driver.FindElement(By.Id("course_load_more")).Click();
                        Thread.Sleep(600);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("fail! click on load more button " + e.Message);
                    failed++;
                }


                Console.WriteLine("success! filtering of " + filter_name);
                success++;

                int sum_course_text = Int16.Parse(driver.FindElement(By.Id("add-sum-course")).Text);
                int sum_course_list = driver.FindElements(By.CssSelector("div[class='item_post_type_course course-item col-xs-12 col-md-6 col-xl-4 course-item-with-border']")).Count;
                if (sum_course_text == sum_course_list)
                {
                    Console.WriteLine("success! text sum is equal to courses list");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! text sum is not equal to courses list");
                    failed++;
                }


                jse.ExecuteScript("arguments[0].click();", input);
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! filter " + e.Message);
                failed++;
            }


        }

        private static void FilterByWhatIsInteresting(IWebDriver driver)
        {
            try
            {
                IWebElement input = driver.FindElement(By.Id("areas_of_knowledge_876"));
                Filters(driver, input, "what is interesting");
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FilterByWhatIsInteresting " + e.Message);
                failed++;
            }

        }

        private static void FilterByTech(IWebDriver driver)
        {
            try
            {
                IWebElement input = driver.FindElement(By.Id("subject_715"));
                Filters(driver, input, "Technology and computers");
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FilterByTech " + e.Message);
                failed++;
            }

        }

        private static void FilterByLanguage(IWebDriver driver)
        {
            try
            {
                IWebElement input = driver.FindElement(By.Id("language_111"));
                Filters(driver, input, "languages");
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FilterByLanguage " + e.Message);
                failed++;
            }

        }

        private static void FilterByInstitutionEnAr(IWebDriver driver, string input_checkbox)
        {
            try
            {
                IWebElement institution = driver.FindElement(By.CssSelector("button[class='filter_main_button dropdown_open']"));
                institution?.Click();

                IWebElement input = driver.FindElement(By.Id(input_checkbox));
                FiltersEnAr(driver, input, "institution");
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FilterByInstitutionEnAr " + e.Message);
            }


        }

        private static void FiltersEnAr(IWebDriver driver, IWebElement input, string filter_name = "")
        {
            try
            {
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                jse.ExecuteScript("arguments[0].click();", input);

                if (driver.FindElement(By.CssSelector("div[class='row wrap-top-bar-search']")).FindElement(By.CssSelector("[class='filter_dynamic_tag']")) != null)
                {
                    while (driver.FindElement(By.Id("course_load_more")).Displayed)
                    {
                        driver.FindElement(By.Id("course_load_more")).Click();
                        Thread.Sleep(15);
                    }

                    Console.WriteLine("success! filtering of " + filter_name);
                    int sum_course_text = Int16.Parse(driver.FindElement(By.Id("add-sum-course")).Text);
                    int sum_course_list = driver.FindElements(By.CssSelector("div[class='item_post_type_course course-item col-xs-12 col-md-6 col-xl-4 course-item-with-border']")).Count;
                    if (sum_course_text == sum_course_list)
                        Console.WriteLine("success! text sum is equal to courses list");
                    else
                        Console.WriteLine("fail! text sum is not equal to courses list");
                }
                else
                    Console.WriteLine("fail! can not filtering of " + filter_name);

                jse.ExecuteScript("arguments[0].click();", input);
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FiltersEnAr " + e.Message);
            }

        }

        private static void FilterByWhatIsInterestingEnAr(IWebDriver driver, string input_checkbox)
        {
            try
            {
                IWebElement input = driver.FindElement(By.Id(input_checkbox));
                FiltersEnAr(driver, input, "what is interesting");
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FilterByWhatIsInterestingEnAr " + e.Message);
            }

        }

        private static void FilterByTechEnAr(IWebDriver driver, string input_checkbox)
        {
            try
            {
                IWebElement input = driver.FindElement(By.Id(input_checkbox));
                FiltersEnAr(driver, input, "Technology and computers");
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FilterByTechEnAr " + e.Message);
            }

        }

        private static void FilterByLanguageEnAr(IWebDriver driver, string input_checkbox)
        {
            try
            {
                IWebElement input = driver.FindElement(By.Id(input_checkbox));
                FiltersEnAr(driver, input, "languages");
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FilterByLanguageEnAr " + e.Message);
            }

        }

        private static void EventProducerLogo(IWebDriver driver)
        {
            string url_bgImage = driver.FindElement(By.ClassName("academic-course-image")).GetCssValue("background-image");
            string bgImage = url_bgImage.Replace("url(", "").Replace(")", "");
            if (bgImage != "about:invalid")
                Console.WriteLine("success! event producer logo");
            else
                Console.WriteLine("fail! don't event producer logo");
        }

        private static void ImageInBanner(IWebDriver driver)
        {
            String url_bgImage = driver.FindElement(By.CssSelector("div[class='banner-image about-course gray-part d-none d-lg-inline-block']")).GetCssValue("background-image");
            string bgImage = url_bgImage.Replace("url(", "").Replace(")", "");
            if (bgImage != "about:invalid")
                Console.WriteLine("success! have image in banner");
            else
                Console.WriteLine("fail! don't have image in banner");
        }

        private static void TitleInBanner(IWebDriver driver, string class_title)
        {
            string title = driver.FindElement(By.ClassName(class_title)).Text;
            if (title != "")
                Console.WriteLine("success! have title in banner");
            else
                Console.WriteLine("fail! don't have title in banner");
        }

        private static void SubtitleInBanner(IWebDriver driver)
        {
            string subtitle = driver.FindElement(By.ClassName("excerpt-course")).Text;
            if (subtitle != "")
                Console.WriteLine("success! have subtitle in banner");
            else
                Console.WriteLine("fail! don't have subtitle in banner");
        }

        private static void PriceInBottomOfBanner(IWebDriver driver)
        {
            string price = driver.FindElement(By.ClassName("price-bar-info")).FindElement(By.ClassName("text-bar-course")).Text;
            if (price == "חינם")
                Console.WriteLine("success! price : free");
            else
                Console.WriteLine("fail! price isn't free");
        }

        private static void DateInBottomOfBanner(IWebDriver driver)
        {
            string price = driver.FindElement(By.ClassName("start-bar-info")).FindElement(By.ClassName("text-bar-course")).Text;
            if (price == "האירוע עבר")
                Console.WriteLine("success! the event has passed");
            else
                Console.WriteLine("fail! the event didn't passed");
        }

        private static void SharingComponent(IWebDriver driver)
        {
            var sharing_component = driver.FindElement(By.ClassName("sharing"));

            if (sharing_component != null)
            {
                var links_sharing_component = sharing_component.FindElements(By.TagName("a"));
                foreach (var link in links_sharing_component)
                {

                    if (link.GetAttribute("href").Contains("live-prof-david-passig"))
                        Console.WriteLine("success! have links in sharing component");
                    else
                        Console.WriteLine("fail! have links in sharing component");

                }
            }
            else
                Console.WriteLine("fail! don't have a sharing component");

        }

        private static void Participants(IWebDriver driver)
        {
            int count = driver.FindElements(By.ClassName("single-lecturer")).Count;
            if (count > 0)
                Console.WriteLine("success! there is at least one participant");
            else
                Console.WriteLine("fail!There isn't at least one participant");
        }

        private static void PopupsInParticipants(IWebDriver driver)
        {
            driver.FindElement(By.ClassName("lecturer-little-about")).Click();
            if (driver.FindElement(By.Id("popup_lecturer")).Displayed)
            {
                if (
                    driver.FindElement(By.CssSelector("div[class='img-lecturer-popup circle-image-lecturer']")).GetCssValue("background-image").Replace("url(", "").Replace(")", "") != "about:invalid" &
                    driver.FindElement(By.ClassName("lecturer-content")).Text != "" &
                    driver.FindElement(By.ClassName("lecturer-title-popup")).Text != "")
                    Console.WriteLine("success! Popup about participant apear with image & text");
                else
                    Console.WriteLine("fail! Popup about participant don't apear with image & text");

            }
            else
                Console.WriteLine("fail! Popup about participant don't apear");

        }

        private static void PastEvent(IWebDriver driver)
        {
            //TODO change findelements of 1 to findelemt
            IWebElement element = driver.FindElement(By.CssSelector("div[data-status=',past,']"));
            if (element.FindElement(By.ClassName("course-item-title")).Text != "" &
                element.FindElement(By.CssSelector("[class='course-item-image has_background_image open-popup-button donthaveyoutube']")).GetCssValue("background-image").Replace("url(", "").Replace(")", "") != "about:invalid" &
                element.FindElement(By.TagName("img")).GetAttribute("src") != null
                )
            {
                Console.WriteLine("success! past event has title & text past & image & icon");
            }
            else
                Console.WriteLine("success! past event don't has title & text past & image & icon");


        }

        private static void FutureEvent(IWebDriver driver)
        {
            var course_load_more = driver.FindElement(By.Id("course_load_more"));

            while (course_load_more.Displayed)
            {
                course_load_more.Click();
            }

            var label = driver.FindElement(By.CssSelector("label[for='status_future'"));
            int text_sum = Int16.Parse(label.FindElement(By.ClassName("sum")).Text);
            if (text_sum != 0)
            {
                IWebElement element = driver.FindElement(By.CssSelector("div[data-status=',future,']"));
                if (element.FindElement(By.ClassName("course-item-title")).Text != "" &
                    element.FindElement(By.CssSelector("[class='course-item-image has_background_image open-popup-button donthaveyoutube']")).GetCssValue("background-image").Replace("url(", "").Replace(")", "") != "about:invalid" &
                    element.FindElement(By.TagName("img")).GetAttribute("src") != null
                    )
                {
                    Console.WriteLine("success! future event has title & text past & image & icon");
                }

                else
                    Console.WriteLine("fail! future event don't has title & text past & image & icon");
            }
            else
                Console.WriteLine("impossible! don't have futer events");


        }

        private static void CountEvents(IWebDriver driver)
        {
            var course_load_more = driver.FindElement(By.Id("course_load_more"));
            while (course_load_more.Displayed)
            {
                course_load_more.Click();
            }
            Thread.Sleep(300);
            int count_list_events = driver.FindElements(By.ClassName("item_post_type_event")).Count;
            int text_count_events = Int16.Parse(driver.FindElement(By.Id("add-sum-course")).Text);

            if (count_list_events == text_count_events)
                Console.WriteLine("success! Checking the number of events that appear is equal");
            else
                Console.WriteLine("fail! Checking the number of events that appear isn't equal");

        }

        private static void FilterByPastEvents(IWebDriver driver)
        {
            var label = driver.FindElement(By.CssSelector("label[for='status_past'"));
            int text_sum = Int16.Parse(label.FindElement(By.ClassName("sum")).Text);
            var input = label.FindElement(By.TagName("input"));
            FilterAutomatic(driver, input, text_sum, "past events");

        }

        private static void FilterAutomatic(IWebDriver driver, IWebElement input, int text_sum, string filter_name = "")
        {
            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
            jse.ExecuteScript("arguments[0].click();", input);

            while (driver.FindElement(By.Id("course_load_more")).Displayed)
            {
                driver.FindElement(By.Id("course_load_more")).Click();
            }

            int text_count_events = Int16.Parse(driver.FindElement(By.Id("add-sum-course")).Text);
            if (text_sum == text_count_events)
                Console.WriteLine("success! filtering of " + filter_name);
            else
                Console.WriteLine("fail! can't filtering of " + filter_name);

            jse.ExecuteScript("arguments[0].click();", input);
        }

        private static void FilterByMastersInTheLivingRoom(IWebDriver driver)
        {
            var label = driver.FindElement(By.CssSelector("label[for='event_type_871'"));
            int text_sum = Int16.Parse(label.FindElement(By.ClassName("sum")).Text);
            var input = label.FindElement(By.TagName("input"));
            FilterAutomatic(driver, input, text_sum, "masters in the living room");

        }

        private static void GoToEventPage(IWebDriver driver)
        {
            IWebElement first_event = driver.FindElement(By.ClassName("course-item-details"));
            string course_item_title = first_event.FindElement(By.ClassName("course-item-title")).Text;
            first_event.Click();
            if (driver.Title.Contains(course_item_title))
                Console.WriteLine("success! go to event page");

            else
                Console.WriteLine("fail! don't go to event page");
        }

        private static void OrganizationLogo(IWebDriver driver)
        {
            string img = driver.FindElement(By.XPath("//div[@id='hybrid_inst_banner_img']/img")).GetAttribute("src");
            if (img != "")
                Console.WriteLine("success! image organization logo");
            else
                Console.WriteLine("fail! don't hame image organization logo");
        }

        private static void OrganizationDescription(IWebDriver driver)
        {
            if (driver.FindElement(By.Id("h_inst_content")).Text != "")
                Console.WriteLine("success! have content organization");
            else
                Console.WriteLine("fail! doesn't content organization");
        }

        private static void FoundCourse(IWebDriver driver)
        {
            string sum = driver.FindElement(By.ClassName("found-course-number")).Text;
            string count = driver.FindElements(By.ClassName("item_post_type_course")).Count.ToString();
            if (sum == count)
                Console.WriteLine("success! Introduces the courses");
            else
                Console.WriteLine("fail! doesn't introduces the courses");
        }

        private static void FoundLecturer(IWebDriver driver)
        {
            string sum = driver.FindElements(By.ClassName("found-lecturer-number"))[0].Text;
            if (sum != "")
                Console.WriteLine("success! Introduces the lecturers " + sum);
            else
                Console.WriteLine("fail! doesn't introduces the lecturers " + sum);
        }

        private static void FoundTrainers(IWebDriver driver)
        {
            string sum = driver.FindElements(By.ClassName("found-lecturer-number"))[1].Text;
            if (sum != "")
                Console.WriteLine("success! Introduces the trainers " + sum);
            else
                Console.WriteLine("fail! doesn't introduces the trainers " + sum);
        }

        private static void HybridMoreCoursesBtn(IWebDriver driver)
        {
            int count = driver.FindElements(By.ClassName("item_post_type_course")).Count;
            if (count > 8)
            {
                Console.WriteLine("success! courses more 8");
                if (driver.FindElement(By.ClassName("hybrid_more_courses_btn")).Displayed)
                {
                    Console.WriteLine("success! more courses button is display");
                }
                else
                    Console.WriteLine("fail! more courses button isn't display");
            }
            else
            {
                if (count > 0)
                    Console.WriteLine("success! courses less 8");
                else
                    Console.WriteLine("fail! doesn't introduces the courses");
            }

        }

        private static void HybridCourseMoreEight(IWebDriver driver)
        {
            IWebElement hybrid_more_courses_btn = driver.FindElement(By.ClassName("hybrid_more_courses_btn"));
            if (hybrid_more_courses_btn.Displayed)
            {
                hybrid_more_courses_btn.Click();
                if (driver.FindElements(By.CssSelector(".item_post_type_course > [style=display:none]")).Count > 0)
                    Console.WriteLine("success! Introduces all courses");
                else
                    Console.WriteLine("fail! doesn't introduces all courses");
            }
            else
                Console.WriteLine("impossible! doesn't have courses more 8");

        }

        private static void CourseTrackAHybridCourse(IWebDriver driver)
        {
            IWebElement target = driver.FindElement(By.ClassName("course-item-details"));
            ((IJavaScriptExecutor)driver).ExecuteScript(
            "arguments[0].scrollIntoView();", target);
            Actions actions = new Actions(driver);
            actions.MoveToElement(target).Perform();
            IWebElement course_item_link = driver.FindElement(By.ClassName("course-item-link"));
            if (course_item_link.Displayed && course_item_link.Text == "לעמוד הקורס")
                Console.WriteLine("success! show course_item_link on mouse over");
            else
                Console.WriteLine("fail! doesn't show course_item_link on mouse over");
        }

        //global functions

        private static void Quit(IWebDriver driver)
        {
            //wait
            System.Threading.Thread.Sleep(6000);
            Console.WriteLine("Test Passed");
            driver.Quit();
        }

        private static bool ClassIsExists(string classNameFloor, IWebDriver driver)
        {
            try
            {
                IWebElement element = driver.FindElement(By.ClassName(classNameFloor));
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("fail! class " + classNameFloor + " not exists.");
                return false;
            }

        }

        private static bool IdIsExists(string idFloor, IWebDriver driver)
        {
            try
            {
                IWebElement element = driver.FindElement(By.Id(idFloor));
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("fail! id " + idFloor + " not exists.");
                return false;
            }
        }

        /*public static void WaitForLoad(IWebDriver driver, int timeoutSec = 15)
        {
            //IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(1));
            //wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, timeoutSec));
            wait.Until(wd => js.ExecuteScript("return document.readyState").ToString() == "complete");
            Console.WriteLine("finish WaitForLoad");
        }*/

        private static void IsAmountOfCoursesEqual(IWebDriver driver)
        {
            int sum = Int16.Parse(driver.FindElement(By.ClassName("found-course-number")).Text);
            int sum_list_course = driver.FindElements(By.ClassName("item_post_type_course")).Count;
            if (sum == sum_list_course)
            {
                Console.WriteLine("success! courses equal");
            }
            else
            {
                Console.WriteLine("fail! courses not equal");
            }
        }

        private static void IsAmountOfLecturersEqual(IWebDriver driver)
        {
            int sum = Int16.Parse(driver.FindElement(By.ClassName("found-lecturer-number")).Text);
            int sum_list_lecturers = driver.FindElements(By.ClassName("single-lecturer")).Count;
            if (sum == sum_list_lecturers)
            {
                Console.WriteLine("success! lecturers equal");
            }
            else
            {
                Console.WriteLine("fail! lecturers not equal");
            }
        }

        /*
        private static void SelectLanguage(IWebDriver driver, string page)
        {
            Console.WriteLine("success! in " + page + " change languages");
            var languages = driver.FindElement(By.CssSelector("div[class='lang d-none d-lg-inline-block languages_menu_wrap']")).FindElements(By.TagName("a"));
            List<string> links = new List<string>();
            List<string> titles = new List<string>();
            foreach (var item in languages)
            {
                links.Add(item.GetAttribute("href"));
                titles.Add(item.GetAttribute("title"));
            }

            for (int i = links.Count - 1; i >= 0; i--)
            {
                driver.Url = links[i].ToString();
                string lang = driver.FindElement(By.TagName("html")).GetAttribute("lang");
                if (titles[i] == "עב" && lang == "he-IL")
                    Console.WriteLine("success! change language to hebrew");

                else
                {
                    if (titles[i] == "العر" && lang == "ar")
                        Console.WriteLine("success! change language to arabic");

                    else
                    {
                        if (titles[i] == "En" && lang == "en-US")
                            Console.WriteLine("success! change language to En");
                        else
                            Console.WriteLine("fail! can not change language");

                    }
                }
            }
        }
*/

        private static bool ChangeLanguageAr(IWebDriver driver, string page)
        {
            try
            {
                var language = driver.FindElement(By.CssSelector("div[class='lang d-none d-lg-inline-block languages_menu_wrap']")).FindElement(By.ClassName("wpml-ls-item-ar")).FindElement(By.TagName("a"));

                string links = language.GetAttribute("href");

                driver.Url = links.ToString();
                string lang = driver.FindElement(By.TagName("html")).GetAttribute("lang");
                if (lang == "ar")
                    Console.WriteLine("success! change language Ar in " + page);

                else
                {
                    Console.WriteLine("fail! can not change Ar in " + page);
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! Change Language Ar " + e.Message);
                return false;
            }
        }

        private static bool ChangeLanguageEn(IWebDriver driver, string page)
        {
            try
            {
                var language = driver.FindElement(By.CssSelector("div[class='lang d-none d-lg-inline-block languages_menu_wrap']")).FindElement(By.ClassName("wpml-ls-item-en")).FindElement(By.TagName("a"));

                string links = language.GetAttribute("href");

                driver.Url = links.ToString();
                string lang = driver.FindElement(By.TagName("html")).GetAttribute("lang");
                if (lang == "en-US")
                    Console.WriteLine("success! change language En in " + page);

                else
                {
                    Console.WriteLine("fail! can not change En in " + page);
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! Change Language En " + e.Message);
                return false;
            }
        }

        private static bool ChangeLanguageHe(IWebDriver driver, string page)
        {
            try
            {
                var language = driver.FindElement(By.CssSelector("div[class='lang d-none d-lg-inline-block languages_menu_wrap']")).FindElement(By.ClassName("wpml-ls-item-he")).FindElement(By.TagName("a"));

                string links = language.GetAttribute("href");

                driver.Url = links.ToString();
                string lang = driver.FindElement(By.TagName("html")).GetAttribute("lang");
                if (lang == "he-IL")
                    Console.WriteLine("success! change language He in " + page);

                else
                {
                    Console.WriteLine("fail! can not change He in " + page);
                    return false;
                }
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! Change Language He " + e.Message);
                return false;
            }
        }

    }
}