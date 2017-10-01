using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace JavaKucheSample
{
    public partial class JavaKucheSamplePage : ContentPage
    {
        public JavaKucheSamplePage()
        {
            InitializeComponent();

        }

        private async void PhotoButtonClicked(object sender, EventArgs e)
        {
            // デフォルトのOCRの言語（＝翻訳のソース言語）はjaにします。
            var sourceLang = picker.SelectedItem == null ? "ja" : picker.SelectedItem.ToString();

            var targetLang = sourceLang == "ja" ? "en" : "ja";

            #region Get Photo Url

            var photoUrl = string.Empty;
            var imageChoiceResult = await DisplayAlert("画像の元を選択してください", "", "カメラ", "ギャラリー");

            try
            {
                if (imageChoiceResult)
                    // 写真を撮影し、保存したURLを取得
                    photoUrl = await PhotoClient.TakePhotoAsync();
                else
                    // Galleryに保存したURLを取得
                    photoUrl = await PhotoClient.PickPhotoAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }

            if (string.IsNullOrEmpty(photoUrl))
                return;

            image.Source = photoUrl;

            #endregion

            #region OCR photo

            var ocrResult = string.Empty;

            try
            {
                indicatorLabel.Text = "Doing OCR now...";
                indicatorFrame.IsVisible = true;

                // Computer Vision APIからOcr結果をもらいLabelを更新します。
                ocrResult = await OcrClient.DoOcrAsync(photoUrl, sourceLang);
                ocredLabel.Text = $"OCR Result: {ocrResult}";

                indicatorFrame.IsVisible = false;
                indicatorLabel.Text = string.Empty;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");

                indicatorFrame.IsVisible = false;
                indicatorLabel.Text = string.Empty;
            }

            if (string.IsNullOrEmpty(ocrResult))
                return;

            #endregion

            #region Translate OCRed Text

            var translateResult = string.Empty;

            try
            {
                indicatorLabel.Text = "Translating now...";
                indicatorFrame.IsVisible = true;

                // Translater Text APIから翻訳結果をもらいLabelを更新します。
                translateResult = await TranslateClient.TranslateTextAsync(ocrResult, sourceLang, targetLang);
                translatedLabel.Text = $"Translate Result: {translateResult}";

                indicatorFrame.IsVisible = false;
                indicatorLabel.Text = string.Empty;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");

                indicatorFrame.IsVisible = false;
                indicatorLabel.Text = string.Empty;
            }

            #endregion

        }


    }
}
