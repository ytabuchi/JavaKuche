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
			// 未選択の場合はjaにします。
			var lang = picker.SelectedItem == null ? "ja" : picker.SelectedItem.ToString();

			#region Get Photo Url

			var photoUrl = string.Empty;
            var imageChoiceResult = await DisplayAlert("画像の元を選択してください", "", "カメラ", "ギャラリー");

            try
            {
                if (imageChoiceResult)
                {
                    // 写真を撮影し、保存したURLを取得
                    photoUrl = await PhotoClient.TakePhotoAsync();
                }
                else
                {
                    // Galleryに保存したURLを取得
                    photoUrl = await PhotoClient.PickPhotoAsync();

                }
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
                indicatorLabel.Text = "Now doing OCR...";
                indicatorFrame.IsVisible = true;
                ocrResult = await OcrClient.DoOcrAsync(photoUrl, lang);
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

            #endregion

            #region Translate OCRed Text

            var translateResult = string.Empty;

            #endregion

        }


    }
}
