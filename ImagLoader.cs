using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml;

namespace ScreenshotViewer
{
    public class ImagLoader
    {
        public string CharLetters { get; private set; }
        public string CurrentImageAlphanumberValue { get; private set; }
        public string DefoultUrl { get; private set; }
        public string BaseUrl { get; private set; }
        public string CurrentImageUrl { get; private set; }
        public int CurrentImageNumber { get; private set; }

        const string downloadFolder = "img/";

        public ImagLoader(int curentImageNumber)
        {
            CurrentImageNumber = curentImageNumber;
            CharLetters = "abcdefghijklmnopqrstuvwxyz0123456789";
            CurrentImageAlphanumberValue = NumberToLetter(CurrentImageNumber);
            DefoultUrl = @"pack://application:,,,/Images/NotFound.png";
            BaseUrl = "https://prnt.sc/";
            CurrentImageUrl = DefoultUrl;
        }
        public ImagLoader(
            int curentImageNumber,
            string charLetters)
        {
            CurrentImageNumber = curentImageNumber;
            CharLetters = charLetters;
            CurrentImageAlphanumberValue = NumberToLetter(CurrentImageNumber);
            DefoultUrl = @"pack://application:,,,/Images/NotFound.png";
            BaseUrl = "https://prnt.sc/";
            CurrentImageUrl = DefoultUrl;
        }
        public ImagLoader(
            int curentImageNumber,
            string charLetters,
            string defoultUrl = @"pack://application:,,,/Images/NotFound.png")
        {
            CurrentImageNumber = curentImageNumber;
            CharLetters = charLetters;
            CurrentImageAlphanumberValue = NumberToLetter(CurrentImageNumber);
            DefoultUrl = defoultUrl;
            BaseUrl = "https://prnt.sc/";
            CurrentImageUrl = defoultUrl;
        }
        public ImagLoader(
            int curentImageNumber, 
            string charLetters = "abcdefghijklmnopqrstuvwxyz0123456789",
            string defoultUrl = @"pack://application:,,,/Images/NotFound.png",
            string baseUrl = "https://prnt.sc/")
        {
            CurrentImageNumber = curentImageNumber;
            CharLetters = charLetters;
            CurrentImageAlphanumberValue = NumberToLetter(CurrentImageNumber);
            DefoultUrl = defoultUrl;
            BaseUrl = baseUrl;
            CurrentImageUrl = defoultUrl;
        }

        /// <summary>
        /// Increaseing the property CurentImageNumber
        /// </summary>
        /// <param name="amount"></param>
        private void IncreaseCurentImageNumber(int amount = 1)
        {
            if (amount > 0)
            {
                CurrentImageNumber += amount;
            }
        }

        /// <summary>
        /// Decreaseing the property CurentImageNumber
        /// </summary>
        /// <param name="amount"></param>
        private void DecreaseCurentImageNumber(int amount = 1)
        {
            if (amount < 0)
            {
                amount *= -1;
            }
            if ((CurrentImageNumber - amount) >= 0)
            {
                CurrentImageNumber -= amount;
            }
        }

        /// <summary>
        /// Set the property of CurentImageNumber
        /// </summary>
        /// <param name="number"></param>
        private void SetCurentImageNumber(int number)
        {
            if (number < 0)
            {
                number *= -1;
            }
            CurrentImageNumber = number;
        }

        /// <summary>
        /// The input number will be converted into an alphanumeric value.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="alphanumberCharLenght"></param>
        /// <returns></returns>
        public string NumberToLetter(int number, int alphanumberCharLenght = 6)
        {
            char[] output = new char[alphanumberCharLenght];
            int index;

            for (int i = 0; i < alphanumberCharLenght; i++)
            {
                index = number / (int)Math.Pow(CharLetters.Length, i);
                index = index % CharLetters.Length;
                output[i] = CharLetters[index];
                number -= index;
            }

            Array.Reverse(output);
            return new string(output);
        }

        /// <summary>
        /// The input alphanumeric value will be converted into an number.
        /// </summary>
        /// <param name="alphanumericValue"></param>
        /// <returns></returns>
        public int LetterToNumber(string alphanumericValue)
        {
            int output = 0;
            int index = 0;
            char[] letterToChars = alphanumericValue.ToCharArray();
            Array.Reverse(letterToChars);

            for (int i = 0; i < letterToChars.Length; i++)
            {
                index = CharLetters.IndexOf(letterToChars[i]);
                output += index * (int)Math.Pow(CharLetters.Length, i);
            }

            return output;
        }

        /// <summary>
        /// Merge base Url with Alphanumber Value
        /// </summary>
        /// <returns></returns>
        public string GetSiteUrl()
        {
            CurrentImageAlphanumberValue = NumberToLetter(CurrentImageNumber);
            string url = BaseUrl + CurrentImageAlphanumberValue;
            return url;
        }

        private async Task<bool> SetCurrentImageUrl()
        {
            string siteUrl = GetSiteUrl();
            CurrentImageUrl = await GetImageUrl(siteUrl);
            return true;
        }

        public async Task<bool> IncreaseImageUrl(int amount = 1)
        {
            IncreaseCurentImageNumber(amount);
            await SetCurrentImageUrl();
            return true;
        }

        public async Task<bool> DecreaseImageUrl(int amount = 1)
        {
            DecreaseCurentImageNumber(amount);
            await SetCurrentImageUrl();
            return true;
        }

        public async Task<bool> SetImageUrl(int number)
        {
            SetCurentImageNumber(number);
            await SetCurrentImageUrl();
            return true;
        }

        public async Task<string> GetImageUrl(string url)
        {
            string imageUrl = await GetImageUrlFromUrl(url);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            return imageUrl != null ? imageUrl : DefoultUrl;
        }

        /// <summary>
        /// Find image URL.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="xpath"></param>
        /// <returns>Image URL</returns>
        static async Task<string> GetImageUrlFromUrl(string url, string xpath = "//img[@id='screenshot-image']")
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

                    string html = await client.GetStringAsync(url).ConfigureAwait(false);

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    HtmlNode imageNode = doc.DocumentNode.SelectSingleNode(xpath);

                    if (imageNode != null)
                    {
                        // Hier gehen wir davon aus, dass der Bild-Link im 'src'-Attribut steht.
                        string imageUrl = imageNode.GetAttributeValue("src", "");
                        return imageUrl;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private string GetImageName(string url)
        {
            if (CurrentImageUrl == DefoultUrl)
            {
                return null;
            }
            string[] segments = url.Split('/');
            string name = CurrentImageNumber + "_" + segments[segments.Length - 1];
            return name;
        }

        public async Task<bool> DownloadAndSaveImageAsync()
        {
            if (CurrentImageUrl == DefoultUrl)
            {
                return false;
            }
            string fileDownload = downloadFolder + GetImageName(CurrentImageUrl);
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(CurrentImageUrl);
                    response.EnsureSuccessStatusCode();
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDownload);
                    File.WriteAllBytes(filePath, imageBytes);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
