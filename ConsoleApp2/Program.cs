using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
//RETURN
using System.Environment;

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
        //public static String URL = "https://stage.campus.gov.il/";
        public static int failed = 0, success = 0;
        //RETURN
        public static String URL = Environment.GetEnvironmentVariable("CAMPUS_URL");

        static void Main(string[] args)
        {

            //RETURN
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--whitelisted-ips");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-dev-shm-usage");        
            IWebDriver driver = new ChromeDriver(options);

            //IWebDriver driver = new FirefoxDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(URL);
            CloseFirstPopup(driver);
            Pagehome(driver);
            PagesAcademicInstitution(driver);
            FloorLearningObjectives(driver);
            CoursesSection(driver);
            PagesCampusSchool(driver);
            BlenPageTests(driver);
            if (driver.Url.Contains("https://stage.campus.gov.il/"))
            {
                associationPageTests(driver);

            }
            else
            {
                Console.WriteLine("fail or impossible! 404 - Association page doesn't exist in campus.gov");
                failed++;
            }
            CoursePage(driver);
            RegistrationAndeEnrollment(driver);
            CoursesPage(driver);
            CoursesPageEnAr(driver);
            AnEventHasPassed(driver);
            EventsPage(driver);
            if (driver.Url.Contains("https://campus.gov.il/"))
            {
                AssimilationOrganization(driver);
            }
            else
            {
                Console.WriteLine("fail or impossible! 404 - Assimilation Organization page doesn't exist in stage.campus.gov");
                failed++;
            }
            ProjectPage(driver);

            Console.WriteLine("Final Mode A number of successful functions:" + success + " and a number of failed functions:" + failed);


            Quit(driver);
        }

        //messions functions
        private static void CloseFirstPopup(IWebDriver driver)
        {
            try
            {
                driver.FindElements(By.CssSelector("[class='close-popup-course-button last-popup-element first-popup-element close-popup-button']"))[1].Click();
                success++;
            }
            catch (Exception)
            {
                Console.WriteLine("fail or impossible! doesn't have popup event in front");
                failed++;
            }
        }

        private static void Pagehome(IWebDriver driver)
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
                success++;
            }
            else
            {
                Console.WriteLine("fail! Home Page have existing_floors:" + existing_floors + " non_existing_floors:" + non_existing_floors);
                failed++;
            }

        }

        private static void PagesAcademicInstitution(IWebDriver driver)
        {
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    Thread.Sleep(1000);

                    IWebElement page = driver.FindElement(By.Id("academic-institution-slider")).FindElements(By.CssSelector("div[aria-hidden='false']"))[i].FindElement(By.TagName("a"));
                    string title_page = page.FindElement(By.TagName("img")).GetAttribute("alt");
                    page?.Click();
                    Thread.Sleep(6000);

                    if (driver.Title.Contains(title_page))
                    {
                        Console.WriteLine("success! in Academic Institutions: " + title_page + " page");
                        success++;

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
                    {
                        Console.WriteLine("fail! Institution page: " + title_page + " was not found.");
                        failed++;
                    }

                    IWebElement go_back = driver.FindElement(By.ClassName("above-banner")).FindElement(By.ClassName("campus_logo"));
                    go_back?.Click();
                    Thread.Sleep(500);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! PagesAcademicInstitution  " + e.Message);
                failed++;
            }

        }

        private static void FloorLearningObjectives(IWebDriver driver)
        {
            driver.Url = URL;
            Console.WriteLine("go to Learning Objectives page");
            //הכנה לבגרויות
            PreparationForMatriculation(driver);

            Thread.Sleep(100);
            //בית ספר
            //driver.Url = URL;
            School(driver);
            Thread.Sleep(200);
            //השכלה
            //driver.Url = URL;
            Education(driver);

        }

        private static void PagesCampusSchool(IWebDriver driver)
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
                        success++;
                        //בודק אם מספר הקורסים המוצג בכותרת שווה לכמות הקורסים המוצגים
                        TitleAndCountCourses(driver, "108.2");
                        // בודק אם יש אפשרות של סינון שפה -> אם כן אז שולף את השפה ובודק תקינות סינון של השפה שנבחרה
                        LanguagesOptions(driver);
                        if (ChangeLanguageEn(driver, "Courses Page")) success++; else failed++;
                        if (ChangeLanguageAr(driver, "Courses Page")) success++; else failed++;
                        if (ChangeLanguageHe(driver, "Courses Page")) success++; else failed++;

                    }
                    else
                    {
                        Console.WriteLine("fail! title isn't match " + driver.Title);
                        failed++;
                    }

                    driver.Url = URL;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! Pages Campus School " + e.Message);
                failed++;
            }
        }

        private static void CoursesSection(IWebDriver driver)
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
                    {
                        Console.WriteLine("success! go to course");
                        success++;
                    }


                    else
                    {
                        Console.WriteLine("fail! go to course");
                        failed++;
                    }
                    driver.Url = URL;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! Courses Section " + e.Message);
                failed++;
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
            TitleInBannerById(driver, "hybrid_banner_h1");
            MoreInfo(driver);
            BlendPageRegistrationButton(driver);
            associationButton(driver);
            institutionButton(driver);
            if (AboutLecturer(driver)) success++; else failed++;
            MoreCourses(driver);
            if (ButtonForCourse(driver, url)) success++; else failed++;
            BlenMoreInfo(driver, url);
            if (NavigateCoursesPage(driver, url)) success++; else failed++;
            if (NavigatesEventsPage(driver, url)) success++; else failed++;
            if (NavigatesAboutPage(driver, url)) success++; else failed++;
            if (NavigatesSupportPage(driver, url)) success++; else failed++;
            ChatBotAvatar(driver, url);
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
            Thread.Sleep(3000);
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

        private static void CoursesPageEnAr(IWebDriver driver)
        {
            if (NavigateCoursesPage(driver, URL)) success++; else failed++;

            if (ChangeLanguageEn(driver, "Courses Page")) success++; else failed++;
            FilterByInstitutionEnAr(driver, "institution_1362");
            FilterByWhatIsInterestingEnAr(driver, "areas_of_knowledge_887");
            FilterByTechEnAr(driver, "subject_937");
            FilterByLanguageEnAr(driver, "language_29");

            if (ChangeLanguageAr(driver, "Courses Page")) success++; else failed++;
            FilterByInstitutionEnAr(driver, "institution_4730");
            FilterByWhatIsInterestingEnAr(driver, "areas_of_knowledge_884");
            FilterByTechEnAr(driver, "subject_1029");
            FilterByLanguageEnAr(driver, "language_401");
        }

        private static void AnEventHasPassed(IWebDriver driver)
        {
            driver.Url = URL + "event/live-prof-david-passig/";
            Console.WriteLine("go to an event has passed ");
            EventProducerLogo(driver);
            ImageInBanner(driver);
            TitleInBanner(driver, "//[@class='title-course']");
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
            if (ChangeLanguageEn(driver, "events page")) success++; else failed++;
            if (ChangeLanguageAr(driver, "events page")) success++; else failed++;
            if (ChangeLanguageHe(driver, "events page")) success++; else failed++;
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
            if (ButtonForCourse(driver, URL + "hybrid_institution/test_ao/")) success++; else failed++;
        }

        //shortcut functions

        private static void PreparationForMatriculation(IWebDriver driver)
        {
            try
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
                        success++;
                    }
                    else
                    {
                        Console.WriteLine("fail! Preparation for matriculation have existing_floors:" + existing_floors + " non_existing_floors:" + non_existing_floors);
                        failed++;
                    }

                }
                else
                {
                    Console.WriteLine("fail! go to Preparation for matriculation ");
                    failed++;
                }

                IWebElement go_back = driver.FindElement(By.ClassName("above-banner")).FindElement(By.ClassName("campus_logo"));
                go_back?.Click();
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! PreparationForMatriculation " + e.Message);
                failed++;
            }

        }

        private static void School(IWebDriver driver)
        {
            try
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
                        success++;
                    }
                    else
                    {
                        Console.WriteLine("fail! High-tech-school have existing_floors:" + existing_floors + " non_existing_floors:" + non_existing_floors);
                        failed++;
                    }

                }
                else
                {
                    Console.WriteLine("fail! go to  school");
                    failed++;
                }

                IWebElement go_back = driver.FindElement(By.ClassName("above-banner")).FindElement(By.ClassName("campus_logo"));
                go_back?.Click();
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! School " + e.Message);
                failed++;
            }

        }

        private static void Education(IWebDriver driver)
        {
            try
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
                    {
                        Console.WriteLine("success! Academic-education-and-broadening-horizons have existing_floors:" + existing_floors + " non_existing_floors:" + non_existing_floors);
                        success++;
                    }

                    else
                    {
                        Console.WriteLine("fail! Academic-education-and-broadening-horizons have existing_floors:" + existing_floors + " non_existing_floors:" + non_existing_floors);
                        failed++;
                    }
                }
                else
                {
                    Console.WriteLine("fail! go to education");
                    failed++;
                }

                IWebElement go_back = driver.FindElement(By.ClassName("above-banner")).FindElement(By.ClassName("campus_logo"));
                go_back?.Click();

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! Education " + e.Message);
                failed++;
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
                                CheckChoosingLanguageOfPosts(driver, "Arabic", "language_111", "language_111");
                                break;
                            }

                        case "language_110":
                            {
                                CheckChoosingLanguageOfPosts(driver, "English", "language_110", "language_110");
                                break;
                            }

                        case "language_109":
                            {
                                CheckChoosingLanguageOfPosts(driver, "Hebrew", "language_109", "language_109");
                                break;
                            }

                        default:
                            {
                                Console.WriteLine("fail! language of posts");
                                failed++;
                                break;
                            }
                    }
                }
            }
            catch
            {
                Console.WriteLine("fail or impossible! do not have option of language in posts");
                failed++;
            }
        }

        private static void CheckChoosingLanguageOfPosts(IWebDriver driver, string language, string label_language, string input_language)
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
                    success++;
                }
                else
                {
                    Console.WriteLine("fail! not show all " + language + " posts :( ");
                    failed++;
                }
                ////click on checkbox language to unchecked
                jse.ExecuteScript("arguments[0].click();", input);


            }
            catch (Exception e)
            {
                Console.WriteLine("fail! Check Choosing Language Of Posts " + e.Message);
                failed++;
            }
        }

        private static void PageInstitution(IWebDriver driver, string lang, string language_page)
        {
            Console.WriteLine("success! change language to " + language_page);
            success++;
            IsAmountOfCoursesEqual(driver);
            IsAmountOfLecturersEqual(driver);
            //יבודק אם הקורסים הולכים לאתרים שלהם בשפה הנבחרת
            GoToCourseInInstatution(driver, lang);
        }

        private static void GoToCourseInInstatution(IWebDriver driver, string lang_language)
        {
            try
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
                        success++;
                        driver.Navigate().Back();
                    }
                    else
                    {
                        Console.WriteLine("fail! go to course");
                        failed++;
                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! GoToCourseInInstatution " + e.Message);
                failed++;
            }

        }

        private static void TitleInBannerById(IWebDriver driver, string id_title)
        {
            try
            {
                string title = driver.FindElement(By.Id(id_title)).Text;
                if (title != "")
                {
                    Console.WriteLine("success! have title in banner");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! don't have title in banner");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! Title In Banner By Id " + e.Message);
                failed++;
            }

        }


        private static void BlendPageRegistrationButton(IWebDriver driver)
        {
            driver.Url = URL + "h_course/tester/";
            try
            {
                IWebElement a = driver.FindElement(By.Id("hybrid_banner_btn"));
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
                    Console.WriteLine("success! button registration send user to registration page");
                    success++;
                }
                else
                {
                    Console.WriteLine("fail! registration button doesn't send to registration page");
                    failed++;
                }

                //close tab and get back
                driver.Close();
                driver.SwitchTo().Window(browserTabs[0]);

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! BlendPageRegistrationButton" + e.Message);
                failed++;
            }

        }

        private static void associationButton(IWebDriver driver)
        {
            try
            {
                IWebElement a = driver.FindElement(By.XPath("//div[@class='uni-logo2 col-6 col-sm-2']")).FindElement(By.TagName("a"));
                a.Click();

                if (driver.Title.Contains("משרד החינוך"))
                {
                    Console.WriteLine("success! association button send to association page");
                    success++;
                }
                //close tab and get back
                else
                {
                    Console.WriteLine("fail! association button doesn't send to association page");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! association Button" + e.Message);
                failed++;
            }

        }

        private static void institutionButton(IWebDriver driver)
        {
            driver.Url = URL + "h_course/tester/";
            try
            {
                IWebElement a = driver.FindElement(By.XPath("//div[@class='uni-logo1 col-6 col-sm-2 align-self-center']"));
                a.Click();

                if (driver.Title.Contains("אוניברסיטת חיפה"))
                {
                    Console.WriteLine("success! Institution button send to institution page");
                    success++;
                }


                else
                {
                    Console.WriteLine("fail! Institution button doesn't send to institution page");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! institutionButton " + e.Message);
                failed++;
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

        private static void MoreCourses(IWebDriver driver)
        {
            try
            {
                driver.Url = URL + "h_course/tester/";
                driver.FindElement(By.ClassName("for-all-courses-link")).Click();
                if (driver.Title.Contains("משרד החינוך"))
                {
                    Console.WriteLine("success! The More Courses button takes to the Assimilation Body page");
                    success++;
                }
                else
                {
                    Console.WriteLine("fail! The More Courses button doesn't take to the Assimilation Body page");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! MoreCourses" + e.Message);
                failed++;
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

        private static void BlenMoreInfo(IWebDriver driver, string url)
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
                success += 7;
                var info_langs_spans = wrap_info.FindElements(By.ClassName("info_lang_span"));
                foreach (var item in info_langs_spans)
                {
                    Console.WriteLine("success! get subtitle_lang " + item.Text);
                    success++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! BlenMoreInfo " + e.Message);
                failed++;
            }



        }

        private static bool NavigateCoursesPage(IWebDriver driver, string url)
        {
            try
            {
                driver.Url = url;
                driver.FindElement(By.Id("menu-item-6449")).Click();
                Thread.Sleep(500);
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

        private static void ChatBotAvatar(IWebDriver driver, string url)
        {
            try
            {
                driver.Url = url;
                driver.FindElement(By.Id("ChatBotAvatar")).Click();
                Thread.Sleep(50);
                IWebElement a = driver.FindElement(By.CssSelector("iframe[id='ChatBotFrame']"));
                if (a.Displayed)
                {
                    Console.WriteLine("success to open chat with campus!");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail to open chat with campus :(");
                    failed++;
                }

            }
            catch (Exception e)
            {

                Console.WriteLine("fail! ChatBotAvatar " + e.Message);
                failed++;
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
                Thread.Sleep(550);
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
                    Console.WriteLine("fail! registration button doesn't send to registration page");
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
                try
                {
                    IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                    jse.ExecuteScript("arguments[0].click();", input);

                    Thread.Sleep(900);
                    driver.FindElements(By.CssSelector("a[class='ajax_filter_btn']"))[1].Click();

                    try
                    {
                        while (driver.FindElement(By.Id("course_load_more")).Displayed)
                        {
                            driver.FindElement(By.Id("course_load_more")).Click();
                            Thread.Sleep(2000);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("fail! click on load more button " + e.Message);
                        failed++;
                    }


                    Console.WriteLine("success! filtering of institution");
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

                Thread.Sleep(900);
                driver.FindElements(By.CssSelector("a[class='ajax_filter_btn']"))[1].Click();

                Thread.Sleep(50000);
                try
                {
                    while (driver.FindElement(By.Id("course_load_more")).Displayed)
                    {
                        driver.FindElement(By.Id("course_load_more")).Click();
                        Thread.Sleep(2000);
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
                int sum_of_courses_in_parentheses = Int16.Parse(input.FindElement(By.XPath("following-sibling::*[1]")).FindElement(By.ClassName("sum")).Text.Replace("(", "").Replace(")", ""));
                if (sum_course_text == sum_course_list && sum_course_text == sum_of_courses_in_parentheses)
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
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                jse.ExecuteScript("arguments[0].click();", input);

                if (driver.FindElement(By.CssSelector("div[class='row wrap-top-bar-search']")).FindElement(By.CssSelector("[class='filter_dynamic_tag']")) != null)
                {
                    while (driver.FindElement(By.Id("course_load_more")).Displayed)
                    {
                        driver.FindElement(By.Id("course_load_more")).Click();
                        Thread.Sleep(15);
                    }

                    Console.WriteLine("success! filtering of institution");
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

                }
                else
                {
                    Console.WriteLine("fail! can not filtering of institution");
                    failed++;
                }


                jse.ExecuteScript("arguments[0].click();", input);
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FilterByInstitutionEnAr " + e.Message);
                failed++;
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
                    success++;
                    int sum_course_text = Int16.Parse(driver.FindElement(By.Id("add-sum-course")).Text);
                    int sum_course_list = driver.FindElements(By.CssSelector("div[class='item_post_type_course course-item col-xs-12 col-md-6 col-xl-4 course-item-with-border']")).Count;
                    int sum_of_courses_in_parentheses = Int16.Parse(input.FindElement(By.XPath("following-sibling::*[1]")).FindElement(By.ClassName("sum")).Text.Replace("(", "").Replace(")", ""));
                    if (sum_course_text == sum_course_list && sum_course_text == sum_of_courses_in_parentheses)
                    {
                        Console.WriteLine("success! text sum is equal to courses list");
                        success++;
                    }

                    else
                    {
                        Console.WriteLine("fail! text sum is not equal to courses list");
                        failed++;
                    }

                }
                else
                {
                    Console.WriteLine("fail! can not filtering of " + filter_name);
                    failed++;
                }


                jse.ExecuteScript("arguments[0].click();", input);
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FiltersEnAr " + e.Message);
                failed++;
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
                failed++;
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
                failed++;
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
                failed++;
            }

        }

        private static void EventProducerLogo(IWebDriver driver)
        {
            try
            {
                string url_bgImage = driver.FindElement(By.ClassName("academic-course-image")).GetCssValue("background-image");
                string bgImage = url_bgImage.Replace("url(", "").Replace(")", "");
                if (bgImage != "about:invalid")
                {
                    Console.WriteLine("success! event producer logo");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! don't event producer logo");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! EventProducerLogo " + e.Message);
                failed++;
            }

        }

        private static void ImageInBanner(IWebDriver driver)
        {
            try
            {

                String url_bgImage = driver.FindElement(By.CssSelector("div[class='banner-image about-course gray-part d-none d-lg-inline-block']")).GetCssValue("background-image");
                string bgImage = url_bgImage.Replace("url(", "").Replace(")", "");
                if (bgImage != "about:invalid")
                {
                    Console.WriteLine("success! have image in banner");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! don't have image in banner");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! ImageInBanner " + e.Message);
                failed++;
            }

        }

        private static void TitleInBanner(IWebDriver driver, string class_title, string IdScript = "")
        {
            try
            {
                string title = driver.FindElement(By.XPath(class_title)).Text;
                if (title != "")
                {
                    Console.WriteLine("success! have title in banner " + IdScript);
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! don't have title in banner " + IdScript);
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! TitleInBanner " + e.Message);
                failed++;
            }

        }

        private static void SubtitleInBanner(IWebDriver driver)
        {
            try
            {
                string subtitle = driver.FindElement(By.ClassName("excerpt-course")).Text;
                if (subtitle != "")
                {
                    Console.WriteLine("success! have subtitle in banner");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! don't have subtitle in banner");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! SubtitleInBanner " + e.Message);
                failed++;
            }


        }

        private static void PriceInBottomOfBanner(IWebDriver driver)
        {
            try
            {
                string price = driver.FindElement(By.ClassName("price-bar-info")).FindElement(By.ClassName("text-bar-course")).Text;
                if (price == "חינם")
                {
                    Console.WriteLine("success! price : free");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! price isn't free");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! PriceInBottomOfBanner " + e.Message);
                failed++;
            }


        }

        private static void DateInBottomOfBanner(IWebDriver driver)
        {
            try
            {
                string price = driver.FindElement(By.ClassName("start-bar-info")).FindElement(By.ClassName("text-bar-course")).Text;
                if (price == "האירוע עבר")
                {
                    Console.WriteLine("success! the event has passed");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! the event didn't passed");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! DateInBottomOfBanner " + e.Message);
                failed++;
            }


        }

        private static void SharingComponent(IWebDriver driver)
        {
            try
            {
                var sharing_component = driver.FindElement(By.ClassName("sharing"));
                var links_sharing_component = sharing_component.FindElements(By.TagName("a"));
                foreach (var link in links_sharing_component)
                {

                    if (link.GetAttribute("href").Contains("live-prof-david-passig"))
                    {
                        Console.WriteLine("success! have links in sharing component");
                        success++;
                    }

                    else
                    {
                        Console.WriteLine("fail! don't have links in sharing component");
                        failed++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! SharingComponent " + e.Message);
                failed++;
            }


        }

        private static void Participants(IWebDriver driver)
        {
            try
            {
                int count = driver.FindElements(By.ClassName("single-lecturer")).Count;
                if (count > 0)
                {
                    Console.WriteLine("success! there is at least one participant");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! There isn't at least one participant");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! Participants " + e.Message);
                failed++;
            }


        }

        private static void PopupsInParticipants(IWebDriver driver)
        {
            try
            {
                driver.FindElement(By.ClassName("lecturer-little-about")).Click();
                if (driver.FindElement(By.Id("popup_lecturer")).Displayed)
                {
                    if (
                        driver.FindElement(By.CssSelector("div[class='img-lecturer-popup circle-image-lecturer']")).GetCssValue("background-image").Replace("url(", "").Replace(")", "") != "about:invalid" &
                        driver.FindElement(By.ClassName("lecturer-content")).Text != "" &
                        driver.FindElement(By.ClassName("lecturer-title-popup")).Text != "")
                    {
                        Console.WriteLine("success! Popup about participant apear with image & text");
                        success++;
                    }

                    else
                    {
                        Console.WriteLine("fail! Popup about participant don't apear with image & text");
                        failed++;
                    }


                }
                else
                {
                    Console.WriteLine("fail! Popup about participant don't apear");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! PopupsInParticipants " + e.Message);
                failed++;
            }


        }

        private static void PastEvent(IWebDriver driver)
        {
            try
            {
                IWebElement element = driver.FindElement(By.CssSelector("div[data-status=',past,']"));
                if (element.FindElement(By.ClassName("course-item-title")).Text != "" &
                    element.FindElement(By.CssSelector("[class='course-item-image has_background_image open-popup-button donthaveyoutube']")).GetCssValue("background-image").Replace("url(", "").Replace(")", "") != "about:invalid" &
                    element.FindElement(By.TagName("img")).GetAttribute("src") != null
                    )
                {
                    Console.WriteLine("success! past event has title & text past & image & icon");
                    success++;
                }
                else
                {
                    Console.WriteLine("success! past event don't has title & text past & image & icon");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! PastEvent " + e.Message);
                failed++;
            }


        }

        private static void FutureEvent(IWebDriver driver)
        {
            try
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
                        success++;
                    }

                    else
                    {
                        Console.WriteLine("fail! future event don't has title & text past & image & icon");
                        failed++;
                    }

                }
                else
                {
                    Console.WriteLine("fail impossible! don't have futer events");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FutureEvent " + e.Message);
                failed++;
            }


        }

        private static void CountEvents(IWebDriver driver)
        {
            try
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
                {
                    Console.WriteLine("success! Checking the number of events that appear is equal");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! Checking the number of events that appear isn't equal");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! CountEvents " + e.Message);
                failed++;
            }


        }

        private static void FilterByPastEvents(IWebDriver driver)
        {
            try
            {
                var label = driver.FindElement(By.CssSelector("label[for='status_past'"));
                int text_sum = Int16.Parse(label.FindElement(By.ClassName("sum")).Text);
                var input = label.FindElement(By.TagName("input"));
                FilterAutomatic(driver, input, text_sum, "past events");
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FilterByPastEvents " + e.Message);
                failed++;
            }


        }

        private static void FilterAutomatic(IWebDriver driver, IWebElement input, int text_sum, string filter_name = "")
        {
            try
            {
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                jse.ExecuteScript("arguments[0].click();", input);

                while (driver.FindElement(By.Id("course_load_more")).Displayed)
                {
                    driver.FindElement(By.Id("course_load_more")).Click();
                }

                int text_count_events = Int16.Parse(driver.FindElement(By.Id("add-sum-course")).Text);
                if (text_sum == text_count_events)
                {
                    Console.WriteLine("success! filtering of " + filter_name);
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! can't filtering of " + filter_name);
                    failed++;
                }


                jse.ExecuteScript("arguments[0].click();", input);
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FilterAutomatic " + e.Message);
                failed++;
            }

        }

        private static void FilterByMastersInTheLivingRoom(IWebDriver driver)
        {
            try
            {
                var label = driver.FindElement(By.CssSelector("label[for='event_type_871'"));
                int text_sum = Int16.Parse(label.FindElement(By.ClassName("sum")).Text);
                var input = label.FindElement(By.TagName("input"));
                FilterAutomatic(driver, input, text_sum, "masters in the living room");
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FilterByMastersInTheLivingRoom " + e.Message);
                failed++;
            }


        }

        private static void GoToEventPage(IWebDriver driver)
        {
            try
            {
                IWebElement first_event = driver.FindElement(By.ClassName("course-item-details"));
                string course_item_title = first_event.FindElement(By.ClassName("course-item-title")).Text;
                first_event.Click();
                if (driver.Title.Contains(course_item_title))
                {
                    Console.WriteLine("success! go to event page");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! don't go to event page");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! GoToEventPage " + e.Message);
                failed++;
            }

        }

        private static void OrganizationLogo(IWebDriver driver)
        {
            try
            {
                string img = driver.FindElement(By.XPath("//div[@id='hybrid_inst_banner_img']/img")).GetAttribute("src");
                if (img != "")
                {
                    Console.WriteLine("success! image organization logo");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! don't hame image organization logo");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! OrganizationLogo " + e.Message);
                failed++;
            }

        }

        private static void OrganizationDescription(IWebDriver driver)
        {
            try
            {
                if (driver.FindElement(By.Id("h_inst_content")).Text != "")
                {
                    Console.WriteLine("success! have content organization");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! doesn't content organization");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! OrganizationDescription " + e.Message);
                failed++;
            }

        }

        private static void FoundCourse(IWebDriver driver)
        {
            try
            {
                string sum = driver.FindElement(By.ClassName("found-course-number")).Text;
                string count = driver.FindElements(By.ClassName("item_post_type_course")).Count.ToString();
                if (sum == count)
                {
                    Console.WriteLine("success! Introduces the courses");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! doesn't introduces the courses");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FoundCourse " + e.Message);
                failed++;
            }

        }

        private static void FoundLecturer(IWebDriver driver)
        {
            try
            {
                string sum = driver.FindElements(By.ClassName("found-lecturer-number"))[0].Text;
                if (sum != "")
                {
                    Console.WriteLine("success! Introduces the lecturers " + sum);
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! doesn't introduces the lecturers " + sum);
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FoundLecturer " + e.Message);
                failed++;
            }

        }

        private static void FoundTrainers(IWebDriver driver)
        {
            try
            {
                string sum = driver.FindElements(By.ClassName("found-lecturer-number"))[1].Text;
                if (sum != "")
                {
                    Console.WriteLine("success! Introduces the trainers " + sum);
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! doesn't introduces the trainers " + sum);
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FoundTrainers " + e.Message);
                failed++;
            }

        }

        private static void HybridMoreCoursesBtn(IWebDriver driver)
        {
            try
            {
                int count = driver.FindElements(By.ClassName("item_post_type_course")).Count;
                if (count > 8)
                {
                    Console.WriteLine("success! courses more 8");
                    if (driver.FindElement(By.ClassName("hybrid_more_courses_btn")).Displayed)
                    {
                        Console.WriteLine("success! more courses button is display");
                        success++;
                    }
                    else
                    {
                        Console.WriteLine("fail! more courses button isn't display");
                        failed++;
                    }

                }
                else
                {
                    if (count > 0)
                    {
                        Console.WriteLine("success! courses less 8");
                        success++;
                    }

                    else
                    {
                        Console.WriteLine("fail! doesn't introduces the courses");
                        failed++;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! HybridMoreCoursesBtn " + e.Message);
                failed++;
            }


        }

        private static void HybridCourseMoreEight(IWebDriver driver)
        {
            try
            {
                IWebElement hybrid_more_courses_btn = driver.FindElement(By.ClassName("hybrid_more_courses_btn"));
                if (hybrid_more_courses_btn.Displayed)
                {
                    hybrid_more_courses_btn.Click();
                    if (driver.FindElements(By.CssSelector(".item_post_type_course > [style=display:none]")).Count > 0)
                    {
                        Console.WriteLine("success! Introduces all courses");
                        success++;
                    }

                    else
                    {
                        Console.WriteLine("fail! doesn't introduces all courses");
                        failed++;
                    }

                }
                else
                {
                    Console.WriteLine("impossible! doesn't have courses more 8");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! HybridCourseMoreEight " + e.Message);
                failed++;
            }

        }

        private static void CourseTrackAHybridCourse(IWebDriver driver)
        {
            try
            {
                IWebElement target = driver.FindElement(By.ClassName("course-item-details"));
                ((IJavaScriptExecutor)driver).ExecuteScript(
                "arguments[0].scrollIntoView();", target);
                Actions actions = new Actions(driver);
                actions.MoveToElement(target).Perform();
                IWebElement course_item_link = driver.FindElement(By.ClassName("course-item-link"));
                if (course_item_link.Displayed && course_item_link.Text == "לעמוד הקורס")
                {
                    Console.WriteLine("success! show course_item_link on mouse over");
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! doesn't show course_item_link on mouse over");
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! CourseTrackAHybridCourse " + e.Message);
                failed++;
            }

        }

        private static void ProjectPage(IWebDriver driver)
        {
            driver.Url = URL + "project/testpage";
            if (ChangeLanguageEn(driver, "project")) success++; else failed++;
            if (ChangeLanguageAr(driver, "project")) success++; else failed++;
            if (ChangeLanguageHe(driver, "project")) success++; else failed++;
            ImageInBanner(driver);
            TitleInBanner(driver, "//h1[@id='muni_page_banner']/span", "902.2");
            LogoInBannerMuni(driver, "902.3");
            AcademicsSlider(driver);
            driver.Url = URL + "project/testpage";
            PersonalitiesProject(driver);
            BannerCourses(driver);
            CountCubesOfCourses(driver, "912");
            FilterByInstitutionProject(driver, "institution_1128", "913.1", "913.2", "913.3");
            FilterProject(driver, driver.FindElement(By.Id("language_111")), "914.1", "914.2", "914.3");
            VideosHowtoLearn(driver);
            FloorQuestionsAndAnswers(driver);

        }

        private static void LogoInBannerMuni(IWebDriver driver, string IdScript)
        {
            try
            {
                string img = driver.FindElement(By.XPath("//a[@class='img_wrap']/img")).GetAttribute("src");
                if (img != "")
                {
                    Console.WriteLine("success! " + IdScript);
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! " + IdScript);
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! LogoInBannerMuni " + IdScript + " " + e.Message);
                failed++;
            }
        }

        private static void AcademicsSlider(IWebDriver driver)
        {
            try
            {
                // var first_academic = driver.FindElement(By.XPath("//div[@class='carousel-item slick-slide slick-active'/a"));
                var first_academic = driver.FindElement(By.CssSelector("div[class='carousel-item slick-slide slick-active']")).FindElement(By.TagName("a"));
                Console.WriteLine("success! 903");
                success++;
                string aria_label_first_academic = first_academic.GetAttribute("aria-label");
                first_academic.Click();
                if (driver.Title.Contains(aria_label_first_academic))
                {
                    Console.WriteLine("success! 905");
                    success++;
                }
                else
                {
                    Console.WriteLine("fail! 905");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! 903 " + e.Message);
                failed++;
            }

        }

        private static void PersonalitiesProject(IWebDriver driver)
        {
            try
            {
                var two_image = driver.FindElements(By.ClassName("single_project_quote"));
                CompareCounts(two_image.Count, 2, "906.1");
                int index = 0;
                foreach (var item in two_image)
                {
                    index += 1;
                    ImageSrc(item.FindElement(By.TagName("img")), index + "/2 906.2");
                    TextString(item.FindElement(By.ClassName("single_project_quote_name_and_role")), index + "/2 906.3");
                    TextString(item.FindElement(By.ClassName("single_project_quote_short_text")), index + "/2 906.4");
                    TextString(item.FindElement(By.ClassName("single_project_quote_long_text")), index + "/2 906.5");
                }
                BackgroundColor(two_image[0], "rgba(210, 229, 248, 0.61)", "1/2 906.6");
                BackgroundColor(two_image[1], "rgb(241, 246, 251)", "2/2 906.6");
                ReadMoreBtnPersonalities(two_image[0]);
                ReadMoreBtnPersonalities(two_image[1]);

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! 906 " + e.Message);
                failed++;
            }
        }

        private static void CompareCounts(int count_elements, int count, string IdScript)
        {
            if (count_elements == count)
            {
                Console.WriteLine("success! " + IdScript);
                success++;
            }
            else
            {
                Console.WriteLine("fail! " + IdScript);
                failed++;
            }

        }

        private static void BackgroundColor(IWebElement element, string color, string IdScript)
        {
            if (element.GetCssValue("background-color") == color)
            {
                Console.WriteLine("success! " + IdScript);
            }
            else
            {
                Console.WriteLine("fail! " + IdScript);
            }
        }

        private static void ReadMoreBtnPersonalities(IWebElement element)
        {
            try
            {
                IWebElement element_btn = element.FindElement(By.XPath("//a[@class='new_design_btn single_project_quote_btn']"));
                if (element_btn.Displayed)
                {
                    Console.WriteLine("success! 907.1 ");
                    success++;
                    element_btn.Click();
                    if (IsElementPresentByElement(element, By.XPath("//div[@class='single_project_quote_long_text expanded']")))
                    {
                        Console.WriteLine("success! 907.2 ");
                        success++;
                    }
                    else
                    {
                        Console.WriteLine("fail! 907.2");
                        failed++;
                    }

                    if (element_btn.Text == "סגור")
                    {
                        Console.WriteLine("success! 907.3 ");
                        success++;
                    }
                    else
                    {
                        Console.WriteLine("fail! 907.3");
                        failed++;
                    }
                }
                else
                {
                    Console.WriteLine("fail! 907.1");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! 907 " + e.Message);
                failed++;
            }

        }

        private static bool IsElementPresentByElement(IWebElement element, By by)
        {
            try
            {
                element.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private static void BannerCourses(IWebDriver driver)
        {
            BackgroundImage(driver.FindElement(By.XPath("//div[@id='cta_floor']")), "909.1");
            TextString(driver.FindElement(By.XPath("//h3[@id='cta_title']")), "909.2");
            TextString(driver.FindElement(By.XPath("//div[@id='cta_subtitle']")), "909.3");
            CatalogCoursesBtn(driver);
        }

        private static void CatalogCoursesBtn(IWebDriver driver)
        {
            try
            {
                IWebElement catalog_courses_btn = driver.FindElement(By.Id("cta_btn"));
                if (catalog_courses_btn.GetAttribute("href") == URL + "course/")
                {
                    Console.WriteLine("success! 909.4");
                    success++;
                }
                else
                {
                    Console.WriteLine("fail! 909.4");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! 909.4 " + e.Message);
            }
        }

        private static void CountCubesOfCourses(IWebDriver driver, string IdScript)
        {
            try
            {
                CourseLoadMoreBtn(driver, "912.1");
                TitleAndCountCourses(driver, "912.2");
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! CountCubesOfCourses " + IdScript + " " + e.Message);
            }

        }

        private static void CourseLoadMoreBtn(IWebDriver driver, string Idscript)
        {
            try
            {
                while (driver.FindElement(By.Id("course_load_more")).Displayed)
                {
                    driver.FindElement(By.Id("course_load_more")).Click();
                }
                Console.WriteLine("success! " + Idscript);
                success++;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! click on load more button 912.1" + e.Message);
                failed++;
            }
        }

        private static void FilterByInstitutionProject(IWebDriver driver, string input_checkbox, string IdScriptFilter, string IdScriptBtn, string IdScriptCount)
        {
            try
            {
                IWebElement institution = driver.FindElement(By.CssSelector("button[class='filter_main_button dropdown_open']"));
                institution?.Click();

                IWebElement input = driver.FindElement(By.Id(input_checkbox));
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                jse.ExecuteScript("arguments[0].click();", input);
                Console.WriteLine("success! " + IdScriptFilter);
                success++;

                if (driver.FindElement(By.CssSelector("div[class='row wrap-top-bar-search']")).FindElement(By.CssSelector("[class='filter_dynamic_tag']")) != null)
                {
                    CourseLoadMoreBtn(driver, IdScriptBtn);
                    TitleAndCountCourses(driver, IdScriptCount);
                }
                else
                {
                    Console.WriteLine("fail!  " + IdScriptFilter);
                    failed++;
                }


                jse.ExecuteScript("arguments[0].click();", input);
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! FilterByInstitutionProject " + IdScriptFilter + " " + e.Message);
                failed++;
            }
        }

        private static void FilterProject(IWebDriver driver, IWebElement input, string IdScriptFilter, string IdScriptBtn, string IdScriptCount)
        {

            try
            {
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                jse.ExecuteScript("arguments[0].click();", input);

                Console.WriteLine("success! " + IdScriptFilter);
                success++;

                CourseLoadMoreBtn(driver, IdScriptBtn);
                TitleAndCountCoursesAndParenthesis(driver, input, IdScriptCount);

                jse.ExecuteScript("arguments[0].click();", input);
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! " + IdScriptFilter + " " + e.Message);
                failed++;
            }
        }

        private static void TitleAndCountCoursesAndParenthesis(IWebDriver driver, IWebElement input, string IdScript)
        {
            try
            {
                int sum_course_text = Int16.Parse(driver.FindElement(By.Id("add-sum-course")).Text);
                int sum_course_list = driver.FindElements(By.CssSelector("div[class='item_post_type_course course-item col-xs-12 col-md-6 col-xl-4 course-item-with-border']")).Count;
                int sum_of_courses_in_parentheses = Int16.Parse(input.FindElement(By.XPath("following-sibling::*[1]")).FindElement(By.ClassName("sum")).Text.Replace("(", "").Replace(")", ""));
                if (sum_course_text == sum_course_list && sum_course_text == sum_of_courses_in_parentheses)
                {
                    Console.WriteLine("success! " + IdScript);
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! " + IdScript);
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! " + IdScript + " " + e.Message);
            }

        }

        private static void VideosHowtoLearn(IWebDriver driver)
        {
            var videos = driver.FindElements(By.CssSelector("[class='open_yt_lightbox open-popup-button']"));

            CompareCounts(videos.Count, 3, "916.1");

            int index = 0;
            foreach (var item in videos)
            {
                TextString(item.FindElement(By.ClassName("title-video-boxes")), (index + 1) + "/3 916.2");
                item.Click();

                OpacityPopopVideo(driver.FindElement(By.Id("popup_overlay")), (index + 1) + "/3 916.3");
                var close_video = driver.FindElements(By.CssSelector("[class='last-popup-element first-popup-element close-popup-button close-popup-iframe']"))[1];
                close_video.Click();

                index++;
            }
        }

        private static void OpacityPopopVideo(IWebElement element, string IdScript)
        {
            string opacity = element.GetCssValue("opacity");
            if (opacity == "1")
            {
                Console.WriteLine("success! " + IdScript);
                success++;
            }

            else
            {
                Console.WriteLine("fail! " + IdScript);
                failed++;
            }
        }

        private static void FloorQuestionsAndAnswers(IWebDriver driver)
        {
            TextString(driver.FindElement(By.ClassName("faq-title")), "917.1");
            var collection = driver.FindElements(By.ClassName("faq-item"));
            int length_collection = collection.Count;
            int index = 1;
            foreach (var item in collection)
            {
                item.FindElement(By.ClassName("faq-title-inner")).Click();
                DisplayBlock(item, index + "/" + length_collection + " 917.2");
                item.FindElement(By.ClassName("faq-title-inner")).Click();
                DisplayNone(item, index + "/" + length_collection + " 917.3");
                index++;
            }

        }

        private static void DisplayBlock(IWebElement element, string IdScript)
        {
            if (element.FindElement(By.ClassName("faq-answer")).Displayed)
            {
                Console.WriteLine("success! " + IdScript);
                success++;
            }
            else
            {
                Console.WriteLine("fail! " + IdScript);
                failed++;
            }
        }

        private static void DisplayNone(IWebElement element, string IdScript)
        {
            if (element.FindElement(By.ClassName("faq-answer")).Displayed)
            {
                Console.WriteLine("fail! " + IdScript);
                failed++;
            }
            else
            {
                Console.WriteLine("success! " + IdScript);
                success++;
            }
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
                failed++;
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
                failed++;
                return false;
            }
        }

        private static void IsAmountOfCoursesEqual(IWebDriver driver)
        {
            try
            {
                int sum = Int16.Parse(driver.FindElement(By.ClassName("found-course-number")).Text);
                int sum_list_course = driver.FindElements(By.ClassName("item_post_type_course")).Count;
                if (sum == sum_list_course)
                {
                    Console.WriteLine("success! courses equal");
                    success++;
                }
                else
                {
                    Console.WriteLine("fail! courses not equal");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! IsAmountOfCoursesEqual " + e.Message);
                failed++;
            }

        }

        private static void IsAmountOfLecturersEqual(IWebDriver driver)
        {
            try
            {
                int sum = Int16.Parse(driver.FindElement(By.ClassName("found-lecturer-number")).Text);
                int sum_list_lecturers = driver.FindElements(By.ClassName("single-lecturer")).Count;
                if (sum == sum_list_lecturers)
                {
                    Console.WriteLine("success! lecturers equal");
                    success++;
                }
                else
                {
                    Console.WriteLine("fail! lecturers not equal");
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! IsAmountOfLecturersEqual " + e.Message);
                failed++;
            }

        }

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

        private static void ImageSrc(IWebElement element, string IdScript)
        {
            try
            {
                string img = element.GetAttribute("src");
                if (img != "")
                {
                    Console.WriteLine("success! " + IdScript);
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! " + IdScript);
                    failed++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("fail! ImageSrc " + IdScript + " " + e.Message);
                failed++;
            }

        }

        private static void TextString(IWebElement element, string IdScript = "")
        {
            try
            {
                if (element.Text != "")
                {
                    Console.WriteLine("success! " + IdScript);
                    success++;
                }

                else
                {
                    Console.WriteLine("fail! " + IdScript);
                    failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! TextString " + e.Message);
                failed++;
            }

        }

        private static void BackgroundImage(IWebElement element, string IdScript)
        {
            try
            {
                String url_bgImage = element.GetCssValue("background-image");
                string bgImage = url_bgImage.Replace("url(", "").Replace(")", "");
                if (bgImage != "about:invalid")
                {
                    Console.WriteLine("success! " + IdScript);
                }
                else
                {
                    Console.WriteLine("fail! " + IdScript);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("fail! " + IdScript + " " + e.Message);
            }

        }

        private static void TitleAndCountCourses(IWebDriver driver, string IdScript)
        {
            int sum_course_text = Int16.Parse(driver.FindElement(By.Id("add-sum-course")).Text);
            int sum_course_list = driver.FindElements(By.CssSelector("div[class='item_post_type_course course-item col-xs-12 col-md-6 col-xl-4 course-item-with-border']")).Count;
            if (sum_course_text == sum_course_list)
            {
                Console.WriteLine("success! " + IdScript);
                success++;
            }

            else
            {
                Console.WriteLine("fail! " + IdScript);
                failed++;
            }
        }

    }
}